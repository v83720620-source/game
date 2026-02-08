using System;
using UnityEngine;

/// <summary>
/// Advanced weapon controller with magazine system.
/// Supports auto/semi-auto fire, reload, ammo management.
/// </summary>
[RequireComponent(typeof(Magazine))]
public class AdvancedWeapon : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private WeaponData _weaponData;
    
    [Header("References")]
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private LayerMask _hitMask;
    
    [Header("Effects")]
    [SerializeField] private GameObject _muzzleFlashPrefab;
    [SerializeField] private AudioSource _audioSource;
    
    [Header("VFX & Audio Managers")]
    [SerializeField] private VFXManager _vfxManager;
    [SerializeField] private AudioManager _audioManager;
    
    // Components
    private Magazine _magazine;
    private FirstPersonCamera _cameraController;
    
    // State
    private float _lastFireTime;
    private float _currentSpread;
    
    // Events
    public event Action<Vector3, Vector3> OnWeaponFired;
    public event Action<DamageInfo> OnHitRegistered;
    public event Action OnReloadStarted;
    public event Action OnReloadCompleted;
    
    // Properties
    public WeaponData WeaponData => _weaponData;
    public Magazine Magazine => _magazine;
    public bool CanFire => !_magazine.IsReloading && _magazine.HasAmmo && Time.time >= _lastFireTime + _weaponData.fireRate;
    
    private void Awake()
    {
        _magazine = GetComponent<Magazine>();
    }
    
    private void Start()
    {
        // Auto-find references
        if (_playerCamera == null)
            _playerCamera = Camera.main;
        
        if (_playerCamera != null)
            _cameraController = _playerCamera.GetComponent<FirstPersonCamera>();
        
        if (_muzzlePoint == null)
            _muzzlePoint = transform;
        
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        
        // Auto-find managers
        if (_vfxManager == null)
            _vfxManager = VFXManager.Instance;
        
        if (_audioManager == null)
            _audioManager = AudioManager.Instance;
        
        // Initialize magazine
        if (_weaponData != null)
        {
            _magazine.Initialize(_weaponData.magazineSize, _weaponData.magazineSize, _weaponData.reserveAmmo);
        }
        
        // Subscribe to magazine events
        _magazine.OnReloadStarted += HandleReloadStarted;
        _magazine.OnReloadCompleted += HandleReloadCompleted;
        _magazine.OnAmmoEmpty += HandleAmmoEmpty;
        
        _currentSpread = _weaponData != null ? _weaponData.baseSpread : 0.01f;
    }
    
    private void OnDestroy()
    {
        if (_magazine != null)
        {
            _magazine.OnReloadStarted -= HandleReloadStarted;
            _magazine.OnReloadCompleted -= HandleReloadCompleted;
            _magazine.OnAmmoEmpty -= HandleAmmoEmpty;
        }
    }
    
    private void Update()
    {
        HandleInput();
        UpdateSpread();
    }
    
    private void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // PC/Editor Input (Mouse/Keyboard)
        // Fire input
        bool fireInput = _weaponData.isAutomatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        
        if (fireInput)
        {
            TryFire();
        }
        
        // Reload input
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }
#else
        // Mobile Input handled by MobileInputManager buttons
#endif
    }
    
    private void UpdateSpread()
    {
        // Decrease spread over time
        if (_weaponData != null)
        {
            _currentSpread = Mathf.Lerp(_currentSpread, _weaponData.baseSpread, 
                _weaponData.spreadDecreaseSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Try to fire weapon.
    /// </summary>
    public void TryFire()
    {
        if (!CanFire)
        {
            // Play empty click sound
            if (_magazine.CurrentAmmo <= 0 && _weaponData != null && _weaponData.emptySound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_weaponData.emptySound);
            }
            return;
        }
        
        Fire();
    }
    
    private void Fire()
    {
        // Consume ammo
        if (!_magazine.TryConsumeAmmo())
            return;
        
        _lastFireTime = Time.time;
        
        // Get shoot direction with spread
        Vector3 shootDirection = CalculateShootDirection();
        Vector3 shootOrigin = _playerCamera.transform.position;
        
        // Fire event
        OnWeaponFired?.Invoke(shootOrigin, shootDirection);
        
        // Visual/audio effects
        PlayFireEffects();
        
        // Apply recoil
        ApplyRecoil();
        
        // Increase spread
        if (_weaponData != null)
        {
            _currentSpread += _weaponData.spreadIncreasePerShot;
            _currentSpread = Mathf.Min(_currentSpread, _weaponData.maxSpread);
        }
        
        // Perform raycast
        PerformRaycast(shootOrigin, shootDirection);
    }
    
    private Vector3 CalculateShootDirection()
    {
        Vector3 baseDirection = _playerCamera.transform.forward;
        
        // Add random spread
        float spreadX = UnityEngine.Random.Range(-_currentSpread, _currentSpread);
        float spreadY = UnityEngine.Random.Range(-_currentSpread, _currentSpread);
        
        Vector3 spread = _playerCamera.transform.right * spreadX + _playerCamera.transform.up * spreadY;
        
        return (baseDirection + spread).normalized;
    }
    
    private void PerformRaycast(Vector3 origin, Vector3 direction)
    {
        float range = _weaponData != null ? _weaponData.range : 100f;
        
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, _hitMask))
        {
            // Spawn hit effects
            SpawnHitEffects(hit);
            
            // Check for HitBox
            HitBox hitBox = hit.collider.GetComponent<HitBox>();
            if (hitBox != null)
            {
                DamageInfo damageInfo = new DamageInfo(
                    _weaponData.baseDamage,
                    hitBox.HitZone,
                    hit.point,
                    direction,
                    gameObject,
                    hit.collider.gameObject
                );
                
                hitBox.TakeDamage(damageInfo);
                OnHitRegistered?.Invoke(damageInfo);
                
                // Debug visualization
                Color debugColor = GetZoneDebugColor(hitBox.HitZone);
                Debug.DrawLine(origin, hit.point, debugColor, 1f);
            }
            else
            {
                // No hit box - try direct health
                PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(_weaponData.baseDamage, gameObject);
                }
                
                Debug.DrawLine(origin, hit.point, Color.red, 1f);
            }
        }
        else
        {
            Debug.DrawRay(origin, direction * range, Color.yellow, 0.5f);
        }
    }
    
    private void ApplyRecoil()
    {
        if (_cameraController == null || _weaponData == null)
            return;
        
        float horizontal = UnityEngine.Random.Range(-_weaponData.recoilPattern.x, _weaponData.recoilPattern.x) * _weaponData.recoilAmount;
        float vertical = -_weaponData.recoilPattern.y * _weaponData.recoilAmount;
        
        _cameraController.ApplyRecoil(horizontal, vertical);
    }
    
    private void PlayFireEffects()
    {
        // Muzzle flash via VFXManager
        if (_vfxManager != null && _muzzlePoint != null)
        {
            _vfxManager.SpawnMuzzleFlash(_muzzlePoint.position, _muzzlePoint.rotation);
        }
        else if (_muzzleFlashPrefab != null && _muzzlePoint != null)
        {
            // Fallback to old method if VFXManager not found
            GameObject flash = Instantiate(_muzzleFlashPrefab, _muzzlePoint.position, _muzzlePoint.rotation);
            Destroy(flash, 0.1f);
        }
        
        // Fire sound via AudioManager (WeaponAudio component handles this now)
        // But keep fallback for backwards compatibility
        if (_audioManager == null && _weaponData != null && _weaponData.fireSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_weaponData.fireSound);
        }
    }
    
    /// <summary>
    /// Spawn hit effects at impact point.
    /// </summary>
    private void SpawnHitEffects(RaycastHit hit)
    {
        if (_vfxManager == null) return;
        
        // Determine surface type from tag or layer
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
    /// Determine surface type from collider.
    /// </summary>
    private string DetermineSurfaceType(Collider collider)
    {
        // Check tag first
        switch (collider.tag)
        {
            case "Metal": return "metal";
            case "Concrete": return "concrete";
            case "Wood": return "wood";
            case "Dirt": return "dirt";
            case "Water": return "water";
        }
        
        // Default to metal
        return "metal";
    }
    
    /// <summary>
    /// Try to reload weapon.
    /// </summary>
    public void TryReload()
    {
        _magazine.StartReload();
    }
    
    private void HandleReloadStarted()
    {
        OnReloadStarted?.Invoke();
        
        // Play reload sound
        if (_weaponData != null && _weaponData.reloadSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_weaponData.reloadSound);
        }
    }
    
    private void HandleReloadCompleted()
    {
        OnReloadCompleted?.Invoke();
    }
    
    private void HandleAmmoEmpty()
    {
        Debug.Log("Out of ammo! Reloading...");
    }
    
    private Color GetZoneDebugColor(HitZoneType zone)
    {
        switch (zone)
        {
            case HitZoneType.Head: return Color.red;
            case HitZoneType.Body: return Color.yellow;
            case HitZoneType.Limbs: return Color.cyan;
            default: return Color.white;
        }
    }
}
