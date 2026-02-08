using UnityEngine;

/// <summary>
/// First-person camera controller.
/// Handles mouse look with smooth rotation.
/// Network-ready: Can be controlled by input or network data.
/// </summary>
public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _smoothTime = 0.1f;
    
    [Header("Rotation Limits")]
    [SerializeField] private float _minVerticalAngle = -90f;
    [SerializeField] private float _maxVerticalAngle = 90f;
    
    [Header("References")]
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Transform _playerBody;
    
    // Rotation
    private float _verticalRotation;
    private float _horizontalRotation;
    private float _currentVerticalVelocity;
    private float _currentHorizontalVelocity;
    
    // Input
    private Vector2 _lookInput;
    
    private void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Auto-find references if not set
        if (_cameraHolder == null)
        {
            _cameraHolder = transform;
        }
        
        if (_playerBody == null && transform.parent != null)
        {
            _playerBody = transform.parent;
        }
    }
    
    private void Update()
    {
        HandleInput();
        ApplyRotation();
    }
    
    private void HandleInput()
    {
        // OLD INPUT DISABLED - Using Touch input for mobile
        // Camera rotation handled by touch drag on screen
        
        /* MOUSE INPUT DISABLED FOR ANDROID
        // Get mouse input
        _lookInput = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y")
        );
        
        // Unlock cursor with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
        */
    }
    
    private void ApplyRotation()
    {
        // Calculate target rotation
        float targetVertical = _verticalRotation - _lookInput.y * _mouseSensitivity;
        float targetHorizontal = _horizontalRotation + _lookInput.x * _mouseSensitivity;
        
        // Clamp vertical rotation
        targetVertical = Mathf.Clamp(targetVertical, _minVerticalAngle, _maxVerticalAngle);
        
        // Smooth rotation
        _verticalRotation = Mathf.SmoothDampAngle(_verticalRotation, targetVertical, 
            ref _currentVerticalVelocity, _smoothTime);
        _horizontalRotation = Mathf.SmoothDampAngle(_horizontalRotation, targetHorizontal, 
            ref _currentHorizontalVelocity, _smoothTime);
        
        // Apply rotation
        if (_cameraHolder != null)
        {
            _cameraHolder.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }
        
        if (_playerBody != null)
        {
            _playerBody.localRotation = Quaternion.Euler(0f, _horizontalRotation, 0f);
        }
    }
    
    /// <summary>
    /// Set look input (for mobile or network).
    /// </summary>
    public void SetLookInput(Vector2 input)
    {
        _lookInput = input;
    }
    
    /// <summary>
    /// Add recoil to camera (for weapon kick).
    /// </summary>
    public void ApplyRecoil(float horizontal, float vertical)
    {
        _verticalRotation += vertical;
        _horizontalRotation += horizontal;
    }
    
    /// <summary>
    /// Toggle cursor lock state.
    /// </summary>
    public void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    // Properties for network sync
    public float VerticalRotation => _verticalRotation;
    public float HorizontalRotation => _horizontalRotation;
}
