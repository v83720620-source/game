using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dynamic crosshair UI component.
/// Can expand/contract based on movement and shooting.
/// </summary>
public class Crosshair : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _topLine;
    [SerializeField] private RectTransform _bottomLine;
    [SerializeField] private RectTransform _leftLine;
    [SerializeField] private RectTransform _rightLine;
    
    [Header("Crosshair Settings")]
    [SerializeField] private float _baseSpread = 10f;
    [SerializeField] private float _maxSpread = 50f;
    [SerializeField] private float _spreadSpeed = 5f;
    
    [Header("Dynamic Spread")]
    [SerializeField] private float _shootSpreadIncrease = 5f;
    [SerializeField] private float _moveSpreadIncrease = 10f;
    
    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hitColor = Color.red;
    [SerializeField] private float _hitFeedbackDuration = 0.1f;
    
    // State
    private float _currentSpread;
    private float _targetSpread;
    private float _hitFeedbackTimer;
    private Image[] _crosshairImages;
    
    private void Awake()
    {
        _currentSpread = _baseSpread;
        _targetSpread = _baseSpread;
        
        // Get all images
        _crosshairImages = GetComponentsInChildren<Image>();
        
        // Set initial color
        SetColor(_normalColor);
    }
    
    private void Update()
    {
        // Smooth spread transition
        _currentSpread = Mathf.Lerp(_currentSpread, _targetSpread, _spreadSpeed * Time.deltaTime);
        
        // Return to base spread when not moving/shooting
        _targetSpread = Mathf.Lerp(_targetSpread, _baseSpread, Time.deltaTime * 2f);
        
        // Update crosshair position
        UpdateCrosshairSpread();
        
        // Hit feedback timer
        if (_hitFeedbackTimer > 0)
        {
            _hitFeedbackTimer -= Time.deltaTime;
            if (_hitFeedbackTimer <= 0)
            {
                SetColor(_normalColor);
            }
        }
    }
    
    private void UpdateCrosshairSpread()
    {
        float spread = Mathf.Clamp(_currentSpread, _baseSpread, _maxSpread);
        
        if (_topLine != null)
            _topLine.anchoredPosition = new Vector2(0, spread);
        
        if (_bottomLine != null)
            _bottomLine.anchoredPosition = new Vector2(0, -spread);
        
        if (_leftLine != null)
            _leftLine.anchoredPosition = new Vector2(-spread, 0);
        
        if (_rightLine != null)
            _rightLine.anchoredPosition = new Vector2(spread, 0);
    }
    
    /// <summary>
    /// Increase spread when shooting.
    /// </summary>
    public void OnShoot()
    {
        _targetSpread += _shootSpreadIncrease;
        _targetSpread = Mathf.Clamp(_targetSpread, _baseSpread, _maxSpread);
    }
    
    /// <summary>
    /// Increase spread when moving.
    /// </summary>
    public void OnMove(float movementSpeed)
    {
        float moveInfluence = movementSpeed * _moveSpreadIncrease;
        _targetSpread = _baseSpread + moveInfluence;
        _targetSpread = Mathf.Clamp(_targetSpread, _baseSpread, _maxSpread);
    }
    
    /// <summary>
    /// Show hit feedback (red flash).
    /// </summary>
    public void OnHit()
    {
        SetColor(_hitColor);
        _hitFeedbackTimer = _hitFeedbackDuration;
    }
    
    /// <summary>
    /// Show hit feedback with kill indicator.
    /// </summary>
    public void ShowHitFeedback(bool isKill = false)
    {
        Color feedbackColor = isKill ? Color.red : _hitColor;
        SetColor(feedbackColor);
        _hitFeedbackTimer = _hitFeedbackDuration;
        
        // Extra spread on kill
        if (isKill)
        {
            _targetSpread += _shootSpreadIncrease * 2f;
        }
    }
    
    /// <summary>
    /// Set crosshair visibility.
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
    
    private void SetColor(Color color)
    {
        foreach (var image in _crosshairImages)
        {
            if (image != null)
                image.color = color;
        }
    }
}
