using System;
using UnityEngine;

/// <summary>
/// Magazine and ammo management system.
/// Handles ammo count, reloading, and ammo events.
/// </summary>
public class Magazine : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] private int _magazineSize = 30;
    [SerializeField] private int _currentAmmo = 30;
    [SerializeField] private int _reserveAmmo = 90;
    [SerializeField] private bool _infiniteAmmo = false;
    
    [Header("Reload Settings")]
    [SerializeField] private float _reloadTime = 2f;
    
    // State
    private bool _isReloading;
    private float _reloadStartTime;
    
    // Events
    public event Action OnAmmoChanged;
    public event Action OnReloadStarted;
    public event Action OnReloadCompleted;
    public event Action OnAmmoEmpty;
    
    // Properties
    public int CurrentAmmo => _currentAmmo;
    public int MagazineSize => _magazineSize;
    public int ReserveAmmo => _reserveAmmo;
    public bool IsReloading => _isReloading;
    public bool HasAmmo => _currentAmmo > 0;
    public bool CanReload => !_isReloading && _currentAmmo < _magazineSize && (_reserveAmmo > 0 || _infiniteAmmo);
    public float ReloadProgress => _isReloading ? Mathf.Clamp01((Time.time - _reloadStartTime) / _reloadTime) : 0f;
    
    private void Update()
    {
        // Check reload completion
        if (_isReloading)
        {
            if (Time.time >= _reloadStartTime + _reloadTime)
            {
                CompleteReload();
            }
        }
    }
    
    /// <summary>
    /// Try to consume one bullet.
    /// </summary>
    public bool TryConsumeAmmo()
    {
        if (_isReloading)
            return false;
        
        if (_currentAmmo <= 0)
        {
            OnAmmoEmpty?.Invoke();
            return false;
        }
        
        _currentAmmo--;
        OnAmmoChanged?.Invoke();
        
        // Auto-reload when empty
        if (_currentAmmo <= 0 && CanReload)
        {
            StartReload();
        }
        
        return true;
    }
    
    /// <summary>
    /// Start reload process.
    /// </summary>
    public void StartReload()
    {
        if (!CanReload)
            return;
        
        _isReloading = true;
        _reloadStartTime = Time.time;
        
        OnReloadStarted?.Invoke();
        
        Debug.Log($"Reloading... ({_reloadTime}s)");
    }
    
    private void CompleteReload()
    {
        if (!_isReloading)
            return;
        
        _isReloading = false;
        
        // Calculate ammo to reload
        int ammoNeeded = _magazineSize - _currentAmmo;
        
        if (_infiniteAmmo)
        {
            _currentAmmo = _magazineSize;
        }
        else
        {
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);
            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;
        }
        
        OnReloadCompleted?.Invoke();
        OnAmmoChanged?.Invoke();
        
        Debug.Log($"Reload complete! {_currentAmmo}/{_magazineSize}");
    }
    
    /// <summary>
    /// Cancel reload (if interrupted).
    /// </summary>
    public void CancelReload()
    {
        if (_isReloading)
        {
            _isReloading = false;
            Debug.Log("Reload cancelled!");
        }
    }
    
    /// <summary>
    /// Add ammo to reserve.
    /// </summary>
    public void AddAmmo(int amount)
    {
        _reserveAmmo += amount;
        OnAmmoChanged?.Invoke();
    }
    
    /// <summary>
    /// Set ammo counts.
    /// </summary>
    public void SetAmmo(int current, int reserve)
    {
        _currentAmmo = Mathf.Clamp(current, 0, _magazineSize);
        _reserveAmmo = Mathf.Max(reserve, 0);
        OnAmmoChanged?.Invoke();
    }
    
    /// <summary>
    /// Initialize from weapon data.
    /// </summary>
    public void Initialize(int magazineSize, int startingAmmo, int reserveAmmo)
    {
        _magazineSize = magazineSize;
        _currentAmmo = startingAmmo;
        _reserveAmmo = reserveAmmo;
        _isReloading = false;
        
        OnAmmoChanged?.Invoke();
    }
}
