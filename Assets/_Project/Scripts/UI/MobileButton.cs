using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mobile button with touch support.
/// Supports press, hold, and release events.
/// </summary>
[RequireComponent(typeof(Image))]
public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Settings")]
    [SerializeField] private bool _holdMode = false;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _pressedColor = new Color(0.8f, 0.8f, 0.8f);
    
    // Events
    public event Action OnPressed;
    public event Action OnReleased;
    public event Action OnHold; // Called every frame while held
    
    // State
    private Image _image;
    private bool _isPressed;
    
    // Properties
    public bool IsPressed => _isPressed;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = _normalColor;
    }
    
    private void Update()
    {
        if (_isPressed && _holdMode)
        {
            OnHold?.Invoke();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        _image.color = _pressedColor;
        OnPressed?.Invoke();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        _image.color = _normalColor;
        OnReleased?.Invoke();
    }
    
    /// <summary>
    /// Manually trigger button press (for testing).
    /// </summary>
    public void SimulatePress()
    {
        OnPressed?.Invoke();
    }
    
    /// <summary>
    /// Manually trigger button release (for testing).
    /// </summary>
    public void SimulateRelease()
    {
        OnReleased?.Invoke();
    }
}
