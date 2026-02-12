using UnityEngine;

/// <summary>
/// Bot weapon controller.
/// Handles aiming and shooting at targets.
/// </summary>
public class BotWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float _damage = 15f;
    [SerializeField] private float _fireRate = 0.3f;
    [SerializeField] private float _range = 50f;
    [SerializeField] private float _accuracy = 0.8f;
    
    [Header("References")]
    [SerializeField] private Transform _weaponMuzzle;
    [SerializeField] private LayerMask _hitMask;
    
    [Header("VFX & Audio Managers")]
    [SerializeField] private VFXManager _vfxManager;
    [SerializeField] private AudioManager _audioManager;
    
    // State
    private float _lastFireTime;
    
    // Properties
    public bool CanFire => Time.time >= _lastFireTime + _fireRate;
    
    private void Start()
    {
        if (_weaponMuzzle == null)
        {
            _weaponMuzzle = transform;
        }
        
        // Auto-find managers
        if (_vfxManager == null)
            _vfxManager = VFXManager.Instance;
        
        if (_audioManager == null)
            _audioManager = AudioManager.Instance;
    }
    
    /// <summary>
    /// Aim at target position.
    /// </summary>
    public void AimAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
    
    /// <summary>
    /// Try to shoot at target.
    /// </summary>
    public bool TryShootAt(Vector3 targetPosition)
    {
        if (!CanFire)
            return false;
        
        _lastFireTime = Time.time;
        
        // Calculate direction with accuracy
        Vector3 direction = (targetPosition - _weaponMuzzle.position).normalized;
        
        // Add inaccuracy
        float inaccuracy = (1f - _accuracy) * 0.1f;
        direction += new Vector3(
            Random.Range(-inaccuracy, inaccuracy),
            Random.Range(-inaccuracy, inaccuracy),
            Random.Range(-inaccuracy, inaccuracy)
        );
        direction.Normalize();
        
        // Fire effects
        PlayFireEffects();
        
        // Perform raycast
        if (Physics.Raycast(_weaponMuzzle.position, direction, out RaycastHit hit, _range, _hitMask))
        {
            // Spawn hit effects
            SpawnHitEffects(hit);
            
            // NETWORK: Try NetworkPlayerHealth first (multiplayer players)
            var networkPlayerHealth = hit.collider.GetComponentInParent<FlumpGame.Network.Player.NetworkPlayerHealth>();
            if (networkPlayerHealth != null)
            {
                float damage = CalculateNetworkDamage(hit);
                
                // Get bot ID for attacker tracking
                var botController = GetComponent<FlumpGame.Network.AI.NetworkBotController>();
                ulong botId = botController != null ? botController.BotId : 0;
                
                networkPlayerHealth.TakeDamageServerRpc(damage, botId);
                Debug.DrawLine(_weaponMuzzle.position, hit.point, Color.red, 0.5f);
                return true;
            }
            
            // NETWORK: Try NetworkBotHealth (multiplayer bots)
            var networkBotHealth = hit.collider.GetComponentInParent<FlumpGame.Network.AI.NetworkBotHealth>();
            if (networkBotHealth != null)
            {
                float damage = CalculateNetworkDamage(hit);
                
                var botController = GetComponent<FlumpGame.Network.AI.NetworkBotController>();
                ulong botId = botController != null ? botController.BotId : 0;
                
                networkBotHealth.TakeDamageServerRpc(damage, botId);
                Debug.DrawLine(_weaponMuzzle.position, hit.point, Color.red, 0.5f);
                return true;
            }
            
            // FALLBACK: Old HitBox system (single-player)
            HitBox hitBox = hit.collider.GetComponent<HitBox>();
            if (hitBox != null)
            {
                DamageInfo damageInfo = new DamageInfo(
                    _damage,
                    hitBox.HitZone,
                    hit.point,
                    direction,
                    gameObject,
                    hit.collider.gameObject
                );
                
                hitBox.TakeDamage(damageInfo);
                Debug.DrawLine(_weaponMuzzle.position, hit.point, Color.yellow, 0.5f);
                return true;
            }
            
            // FALLBACK: Try direct health (single-player)
            PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(_damage, gameObject);
                Debug.DrawLine(_weaponMuzzle.position, hit.point, Color.cyan, 0.5f);
                return true;
            }
        }
        
        Debug.DrawRay(_weaponMuzzle.position, direction * _range, Color.cyan, 0.5f);
        return false;
    }
    
    /// <summary>
    /// Play fire effects (muzzle flash, sound).
    /// </summary>
    private void PlayFireEffects()
    {
        // Muzzle flash
        if (_vfxManager != null && _weaponMuzzle != null)
        {
            _vfxManager.SpawnMuzzleFlash(_weaponMuzzle.position, _weaponMuzzle.rotation);
        }
        
        // Fire sound
        if (_audioManager != null && _weaponMuzzle != null)
        {
            _audioManager.PlayFireSound(_weaponMuzzle.position);
        }
    }
    
    /// <summary>
    /// Spawn hit effects at impact point.
    /// </summary>
    private void SpawnHitEffects(RaycastHit hit)
    {
        if (_vfxManager == null) return;
        
        // Determine surface type
        string surfaceType = DetermineSurfaceType(hit.collider);
        
        // Spawn hit effect (sparks, dust, etc.)
        _vfxManager.SpawnHitEffect(hit.point, hit.normal, surfaceType);
        
        // Spawn bullet hole decal
        _vfxManager.SpawnBulletHole(hit.point, hit.normal);
        
        // Play hit sound
        if (_audioManager != null)
        {
            _audioManager.PlayHitSound(hit.point);
        }
    }
    
    /// <summary>
    /// NETWORK SUPPORT: Calculate damage with headshot multiplier.
    /// </summary>
    private float CalculateNetworkDamage(RaycastHit hit)
    {
        // Check for HitBox to get zone multiplier
        var hitBox = hit.collider.GetComponent<HitBox>();
        if (hitBox != null)
        {
            float multiplier = DamageInfo.GetZoneMultiplier(hitBox.HitZone);
            return _damage * multiplier;
        }
        
        return _damage;
    }
    
    /// <summary>
    /// Determine surface type from collider.
    /// </summary>
    private string DetermineSurfaceType(Collider collider)
    {
        // Check tag
        switch (collider.tag)
        {
            case "Metal": return "metal";
            case "Concrete": return "concrete";
            case "Wood": return "wood";
            case "Dirt": return "dirt";
            case "Water": return "water";
        }
        
        return "metal"; // Default
    }
}
