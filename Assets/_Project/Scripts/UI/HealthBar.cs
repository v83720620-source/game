using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health bar UI component.
/// Displays current health with smooth animation.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _backgroundImage;
    
    [Header("Colors")]
    [SerializeField] private Color _healthyColor = Color.green;
    [SerializeField] private Color _damagedColor = Color.yellow;
    [SerializeField] private Color _criticalColor = Color.red;
    
    [Header("Animation")]
    [SerializeField] private float _animationSpeed = 5f;
    [SerializeField] private bool _smoothTransition = true;
    
    // State
    private float _currentFill;
    private float _targetFill;
    
    private void Awake()
    {
        if (_fillImage == null)
        {
            _fillImage = GetComponent<Image>();
        }
        
        _currentFill = 1f;
        _targetFill = 1f;
    }
    
    private void Update()
    {
        if (_smoothTransition)
        {
            // Smooth transition
            _currentFill = Mathf.Lerp(_currentFill, _targetFill, _animationSpeed * Time.deltaTime);
            UpdateFill(_currentFill);
        }
    }
    
    /// <summary>
    /// Set health value (0-1).
    /// </summary>
    public void SetHealth(float normalizedHealth)
    {
        normalizedHealth = Mathf.Clamp01(normalizedHealth);
        _targetFill = normalizedHealth;
        
        if (!_smoothTransition)
        {
            _currentFill = normalizedHealth;
            UpdateFill(_currentFill);
        }
    }
    
    /// <summary>
    /// Set health from current/max values.
    /// </summary>
    public void SetHealth(float currentHealth, float maxHealth)
    {
        float normalized = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        SetHealth(normalized);
    }
    
    private void UpdateFill(float value)
    {
        if (_fillImage == null)
            return;
        
        _fillImage.fillAmount = value;
        
        // Update color based on health
        if (value > 0.6f)
        {
            _fillImage.color = _healthyColor;
        }
        else if (value > 0.3f)
        {
            _fillImage.color = _damagedColor;
        }
        else
        {
            _fillImage.color = _criticalColor;
        }
    }
    
    /// <summary>
    /// Immediately set to full health.
    /// </summary>
    public void ResetToFull()
    {
        _currentFill = 1f;
        _targetFill = 1f;
        UpdateFill(1f);
    }
}
