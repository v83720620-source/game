using UnityEngine;

/// <summary>
/// Handles weapon sounds (fire, reload, empty).
/// Attach to weapon GameObjects.
/// </summary>
public class WeaponAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioManager _audioManager;
    
    [Header("Weapon Sounds (Optional)")]
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _emptySound;
    
    [Header("Settings")]
    [SerializeField] private float _fireVolume = 0.8f;
    [SerializeField] private float _reloadVolume = 0.6f;
    [SerializeField] private float _emptyVolume = 0.4f;
    
    private AdvancedWeapon _weapon;
    private Magazine _magazine;
    
    private void Start()
    {
        // Auto-find AudioManager
        if (_audioManager == null)
        {
            _audioManager = AudioManager.Instance;
        }
        
        // Get weapon components
        _weapon = GetComponent<AdvancedWeapon>();
        _magazine = GetComponent<Magazine>();
        
        // Subscribe to events
        if (_weapon != null)
        {
            _weapon.OnWeaponFired += OnWeaponFired;
        }
        
        if (_magazine != null)
        {
            _magazine.OnReloadStarted += OnReloadStarted;
            _magazine.OnReloadCompleted += OnReloadCompleted;
        }
    }
    
    private void OnDestroy()
    {
        if (_weapon != null)
        {
            _weapon.OnWeaponFired -= OnWeaponFired;
        }
        
        if (_magazine != null)
        {
            _magazine.OnReloadStarted -= OnReloadStarted;
            _magazine.OnReloadCompleted -= OnReloadCompleted;
        }
    }
    
    private void OnWeaponFired(Vector3 origin, Vector3 direction)
    {
        PlayFireSound();
    }
    
    private void OnReloadStarted()
    {
        PlayReloadSound();
    }
    
    private void OnReloadCompleted()
    {
        // Optional: play a different sound for reload complete
    }
    
    /// <summary>
    /// Play fire sound.
    /// </summary>
    public void PlayFireSound()
    {
        if (_audioManager == null) return;
        
        if (_fireSound != null)
        {
            _audioManager.PlaySound(_fireSound, transform.position, _fireVolume, Random.Range(0.95f, 1.05f));
        }
        else
        {
            // Use default fire sound from AudioManager
            _audioManager.PlayFireSound(transform.position);
        }
    }
    
    /// <summary>
    /// Play reload sound.
    /// </summary>
    public void PlayReloadSound()
    {
        if (_audioManager == null) return;
        
        if (_reloadSound != null)
        {
            _audioManager.PlaySound(_reloadSound, transform.position, _reloadVolume);
        }
        else
        {
            // Use default reload sound from AudioManager
            _audioManager.PlayReloadSound(transform.position);
        }
    }
    
    /// <summary>
    /// Play empty sound (click when out of ammo).
    /// </summary>
    public void PlayEmptySound()
    {
        if (_audioManager == null || _emptySound == null) return;
        
        _audioManager.PlaySound(_emptySound, transform.position, _emptyVolume);
    }
}
