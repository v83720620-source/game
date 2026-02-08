using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Virtual joystick for mobile movement input.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    [SerializeField] private float _handleRange = 50f;
    [SerializeField] private float _deadZone = 0.1f;
    
    [Header("Visual Settings")]
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private bool _dynamicJoystick = true;
    
    // State
    private Vector2 _input;
    private Vector2 _joystickStartPosition;
    private Canvas _canvas;
    private Camera _camera;
    
    // Properties
    public Vector2 Input => _input.magnitude < _deadZone ? Vector2.zero : _input;
    public float Horizontal => Input.x;
    public float Vertical => Input.y;
    
    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        
        if (_canvas == null)
        {
            Debug.LogError("VirtualJoystick: Canvas not found!");
        }
        
        // Get camera for screen point conversion
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            _camera = _canvas.worldCamera;
        }
        
        // Store initial position
        _joystickStartPosition = _background.anchoredPosition;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_dynamicJoystick)
        {
            // Move joystick to touch position
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background.parent as RectTransform,
                eventData.position,
                _camera,
                out localPoint
            );
            _background.anchoredPosition = localPoint;
        }
        
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _background,
            eventData.position,
            _camera,
            out position
        );
        
        // Normalize
        position = position / _handleRange;
        
        // Clamp to circle
        _input = (position.magnitude > 1f) ? position.normalized : position;
        
        // Move handle
        _handle.anchoredPosition = _input * _handleRange;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset
        _input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
        
        if (_dynamicJoystick)
        {
            // Return to initial position
            _background.anchoredPosition = _joystickStartPosition;
        }
    }
    
    /// <summary>
    /// Reset joystick to center.
    /// </summary>
    public void ResetJoystick()
    {
        _input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
    }
}
