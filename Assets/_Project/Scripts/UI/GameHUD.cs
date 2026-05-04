using UnityEngine;

/// <summary>
/// Main game HUD manager.
/// Connects all UI elements with game systems.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private AmmoDisplay _ammoDisplay;
    [SerializeField] private Crosshair _crosshair;
    
    [Header("Game Systems")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private SimpleWeapon _simpleWeapon;
    [SerializeField] private AdvancedWeapon _advancedWeapon;
    [SerializeField] private PlayerMovement _playerMovement;
    
    // Active weapon reference
    private Magazine _activeMagazine;
    
    private void Start()
    {
        // Subscribe to events
        SubscribeToEvents();
        
        // Initial update
        UpdateHealthDisplay();
        UpdateAmmoDisplay();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        UnsubscribeFromEvents();
    }
    
    private void Update()
    {
        // Update crosshair based on movement
        UpdateCrosshairMovement();
    }
    
    private void SubscribeToEvents()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged += UpdateHealthDisplay;
            _playerHealth.OnDamageReceived += OnPlayerDamaged;
        }
        
        // Subscribe to simple weapon
        if (_simpleWeapon != null)
        {
            _simpleWeapon.OnWeaponFired += OnWeaponFired;
        }
        
        // Subscribe to advanced weapon
        if (_advancedWeapon != null)
        {
            _advancedWeapon.OnWeaponFired += OnWeaponFired;
            _advancedWeapon.OnHitRegistered += OnHitRegistered;
            
            // Subscribe to magazine
            _activeMagazine = _advancedWeapon.Magazine;
            if (_activeMagazine != null)
            {
                _activeMagazine.OnAmmoChanged += UpdateAmmoDisplay;
            }
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged -= UpdateHealthDisplay;
            _playerHealth.OnDamageReceived -= OnPlayerDamaged;
        }
        
        if (_simpleWeapon != null)
        {
            _simpleWeapon.OnWeaponFired -= OnWeaponFired;
        }
        
        if (_advancedWeapon != null)
        {
            _advancedWeapon.OnWeaponFired -= OnWeaponFired;
            _advancedWeapon.OnHitRegistered -= OnHitRegistered;
        }
        
        if (_activeMagazine != null)
        {
            _activeMagazine.OnAmmoChanged -= UpdateAmmoDisplay;
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if (_healthBar != null && _playerHealth != null)
        {
            _healthBar.SetHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        }
    }
    
    private void UpdateAmmoDisplay()
    {
        if (_ammoDisplay != null)
        {
            if (_activeMagazine != null)
            {
                // Display magazine ammo
                _ammoDisplay.SetAmmo(_activeMagazine.CurrentAmmo, _activeMagazine.MagazineSize, _activeMagazine.ReserveAmmo);
            }
            else
            {
                // Fallback to infinite ammo
                _ammoDisplay.SetAmmo(999, 999, -1);
            }
        }
    }
    
    private void OnPlayerDamaged(float damage, GameObject attacker)
    {
        // Visual feedback when damaged
        // TODO: Add screen shake or damage vignette
    }
    
    private void OnWeaponFired(Vector3 origin, Vector3 direction)
    {
        // Update crosshair on shoot
        if (_crosshair != null)
        {
            _crosshair.OnShoot();
        }
    }
    
    private void OnHitRegistered(DamageInfo damageInfo)
    {
        // Show hit marker
        ShowHitMarker();
    }
    
    private void UpdateCrosshairMovement()
    {
        if (_crosshair != null && _playerMovement != null)
        {
            float moveSpeed = _playerMovement.Velocity.magnitude;
            _crosshair.OnMove(moveSpeed);
        }
    }
    
    /// <summary>
    /// Show hit marker when player hits target.
    /// </summary>
    public void ShowHitMarker()
    {
        if (_crosshair != null)
        {
            _crosshair.OnHit();
        }
    }
}
