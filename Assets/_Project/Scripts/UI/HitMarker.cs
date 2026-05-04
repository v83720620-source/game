using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hit marker visual feedback when hitting an enemy.
/// Attach to Crosshair or create separate UI element.
/// </summary>
public class HitMarker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _showDuration = 0.2f;
    [SerializeField] private Color _hitColor = Color.white;
    [SerializeField] private Color _killColor = Color.red;
    [SerializeField] private float _scaleMultiplier = 1.5f;
    
    [Header("References (Optional)")]
    [SerializeField] private Image _hitMarkerImage;
    [SerializeField] private Crosshair _crosshair;
    
    private float _showTimer;
    private Vector3 _originalScale;
    private Color _originalColor;
    
    private void Start()
    {
        // Auto-find crosshair
        if (_crosshair == null)
        {
            _crosshair = GetComponent<Crosshair>();
        }
        
        // Setup hitmarker image
        if (_hitMarkerImage != null)
        {
            _originalScale = _hitMarkerImage.transform.localScale;
            _originalColor = _hitMarkerImage.color;
            _hitMarkerImage.gameObject.SetActive(false);
        }
        
        // Subscribe to weapon events
        SubscribeToWeaponEvents();
    }
    
    private void Update()
    {
        if (_showTimer > 0f)
        {
            _showTimer -= Time.deltaTime;
            
            if (_showTimer <= 0f)
            {
                HideMarker();
            }
        }
    }
    
    private void SubscribeToWeaponEvents()
    {
        // Find player's weapon
        AdvancedWeapon weapon = FindAnyObjectByType<AdvancedWeapon>();
        if (weapon != null)
        {
            weapon.OnHitRegistered += OnHitRegistered;
        }
    }
    
    private void OnHitRegistered(DamageInfo damageInfo)
    {
        // Check if hit was on enemy (different team)
        if (damageInfo.Victim != null)
        {
            PlayerHealth health = damageInfo.Victim.GetComponent<PlayerHealth>();
            if (health != null)
            {
                bool isKill = health.IsDead;
                ShowMarker(isKill);
            }
        }
    }
    
    /// <summary>
    /// Show hit marker.
    /// </summary>
    public void ShowMarker(bool isKill = false)
    {
        _showTimer = _showDuration;
        
        Color targetColor = isKill ? _killColor : _hitColor;
        
        // Show hitmarker image
        if (_hitMarkerImage != null)
        {
            _hitMarkerImage.gameObject.SetActive(true);
            _hitMarkerImage.color = targetColor;
            _hitMarkerImage.transform.localScale = _originalScale * _scaleMultiplier;
        }
        
        // Flash crosshair
        if (_crosshair != null)
        {
            _crosshair.ShowHitFeedback(isKill);
        }
    }
    
    private void HideMarker()
    {
        if (_hitMarkerImage != null)
        {
            _hitMarkerImage.gameObject.SetActive(false);
            _hitMarkerImage.transform.localScale = _originalScale;
            _hitMarkerImage.color = _originalColor;
        }
    }
}
