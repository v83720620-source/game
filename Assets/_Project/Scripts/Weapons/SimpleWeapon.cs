using System;
using UnityEngine;

/// <summary>
/// Simple weapon with raycast shooting.
/// Network-ready: Uses events for shot registration.
/// </summary>
public class SimpleWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private float _damage = 20f;
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private float _range = 100f;
    
    [Header("Recoil")]
    [SerializeField] private float _recoilAmount = 1f;
    [SerializeField] private Vector2 _recoilPattern = new Vector2(0.3f, 0.8f);
    
    [Header("References")]
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private LayerMask _hitMask;
    
    [Header("Effects")]
    [SerializeField] private GameObject _muzzleFlashPrefab;
    
    // State
    private float _lastFireTime;
    private FirstPersonCamera _cameraController;
    
    // Events (for network)
    public event Action<Vector3, Vector3> OnWeaponFired; // origin, direction
    public event Action<Vector3, float> OnHitRegistered; // hit point, damage
    
    private void Start()
    {
        // Auto-find camera if not set
        if (_playerCamera == null)
        {
            _playerCamera = Camera.main;
        }
        
        // Find camera controller for recoil
        if (_playerCamera != null)
        {
            _cameraController = _playerCamera.GetComponent<FirstPersonCamera>();
        }
        
        // Auto-find muzzle point if not set
        if (_muzzlePoint == null)
        {
            _muzzlePoint = transform;
        }
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // PC/Editor Input (Mouse)
        // Left mouse button
        if (Input.GetMouseButton(0))
        {
            TryFire();
        }
#else
        // Mobile Input handled by MobileInputManager
#endif
    }
    
    /// <summary>
    /// Attempt to fire weapon.
    /// </summary>
    public void TryFire()
    {
        // Check fire rate
        if (Time.time - _lastFireTime < _fireRate)
            return;
        
        Fire();
    }
    
    private void Fire()
    {
        _lastFireTime = Time.time;
        
        // Get shoot direction from camera
        Vector3 shootDirection = _playerCamera.transform.forward;
        Vector3 shootOrigin = _playerCamera.transform.position;
        
        // Fire event (for network)
        OnWeaponFired?.Invoke(shootOrigin, shootDirection);
        
        // Apply recoil
        ApplyRecoil();
        
        // Spawn muzzle flash
        if (_muzzleFlashPrefab != null && _muzzlePoint != null)
        {
            GameObject flash = Instantiate(_muzzleFlashPrefab, _muzzlePoint.position, _muzzlePoint.rotation);
            Destroy(flash, 0.1f);
        }
        
        // Perform raycast
        PerformRaycast(shootOrigin, shootDirection);
    }
    
    private void PerformRaycast(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _range, _hitMask))
        {
            // Hit something
            OnHitRegistered?.Invoke(hit.point, _damage);
            
            // Debug visualization - color based on hit zone
            Color debugColor = Color.red;
            
            // Check for HitBox component
            HitBox hitBox = hit.collider.GetComponent<HitBox>();
            if (hitBox != null)
            {
                // Create damage info with hit zone
                DamageInfo damageInfo = new DamageInfo(
                    _damage,
                    hitBox.HitZone,
                    hit.point,
                    direction,
                    gameObject,
                    hit.collider.gameObject
                );
                
                // Apply damage through hit box
                hitBox.TakeDamage(damageInfo);
                
                // Color debug line by zone
                debugColor = GetZoneDebugColor(hitBox.HitZone);
            }
            else
            {
                // No hit box - try direct PlayerHealth
                PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(_damage, gameObject);
                }
            }
            
            // Debug visualization
            Debug.DrawLine(origin, hit.point, debugColor, 1f);
        }
        else
        {
            // Missed
            Debug.DrawRay(origin, direction * _range, Color.yellow, 1f);
        }
    }
    
    private Color GetZoneDebugColor(HitZoneType zone)
    {
        switch (zone)
        {
            case HitZoneType.Head:
                return Color.red;      // Head - red
            case HitZoneType.Body:
                return Color.yellow;   // Body - yellow
            case HitZoneType.Limbs:
                return Color.cyan;     // Limbs - cyan
            default:
                return Color.white;
        }
    }
    
    private void ApplyRecoil()
    {
        if (_cameraController == null)
            return;
        
        // Random horizontal recoil
        float horizontal = UnityEngine.Random.Range(-_recoilPattern.x, _recoilPattern.x) * _recoilAmount;
        
        // Vertical recoil (always up)
        float vertical = -_recoilPattern.y * _recoilAmount;
        
        _cameraController.ApplyRecoil(horizontal, vertical);
    }
    
    /// <summary>
    /// Set fire input (for mobile or network).
    /// </summary>
    public void SetFireInput(bool fire)
    {
        if (fire)
        {
            TryFire();
        }
    }
}
