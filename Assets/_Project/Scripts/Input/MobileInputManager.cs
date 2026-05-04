using UnityEngine;

/// <summary>
/// Mobile input manager.
/// Handles touch input for camera rotation and connects UI to game systems.
/// </summary>
public class MobileInputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VirtualJoystick _joystick;
    [SerializeField] private MobileButton _jumpButton;
    [SerializeField] private MobileButton _crouchButton;
    [SerializeField] private MobileButton _sprintButton;
    [SerializeField] private MobileButton _fireButton;
    
    [Header("Target Systems")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private FirstPersonCamera _firstPersonCamera;
    [SerializeField] private SimpleWeapon _weapon;
    
    [Header("Touch Camera Settings")]
    [SerializeField] private float _touchSensitivity = 1f;
    [SerializeField] private bool _invertY = false;
    
    [Header("Platform Detection")]
    [SerializeField] private bool _autoDetectPlatform = true;
    [SerializeField] private GameObject _mobileUI;
    
    // Touch state
    private Vector2 _lastTouchPosition;
    private bool _isTouching;
    private int _cameraFingerID = -1;
    
    // Properties
    public bool IsMobilePlatform { get; private set; }
    
    private void Start()
    {
        // Detect platform
        DetectPlatform();
        
        // Setup button events
        SetupButtons();
        
        // Enable/disable mobile UI based on platform
        if (_mobileUI != null)
        {
            _mobileUI.SetActive(IsMobilePlatform);
        }
    }
    
    private void Update()
    {
        if (!IsMobilePlatform)
            return;
        
        // Update movement from joystick
        UpdateMovement();
        
        // Update camera from touch
        UpdateTouchCamera();
    }
    
    private void DetectPlatform()
    {
        if (_autoDetectPlatform)
        {
            #if UNITY_ANDROID || UNITY_IOS
                IsMobilePlatform = true;
            #else
                IsMobilePlatform = Application.isMobilePlatform;
            #endif
        }
        else
        {
            IsMobilePlatform = true; // Force mobile mode
        }
        
        Debug.Log($"Platform detected: {(IsMobilePlatform ? "Mobile" : "PC")}");
    }
    
    private void SetupButtons()
    {
        // Jump button
        if (_jumpButton != null && _playerMovement != null)
        {
            _jumpButton.OnPressed += () => _playerMovement.Jump();
        }
        
        // Crouch button
        if (_crouchButton != null && _playerMovement != null)
        {
            _crouchButton.OnPressed += () => _playerMovement.ToggleCrouch();
        }
        
        // Sprint button
        if (_sprintButton != null && _playerMovement != null)
        {
            _sprintButton.OnPressed += () => _playerMovement.SetSprint(true);
            _sprintButton.OnReleased += () => _playerMovement.SetSprint(false);
        }
        
        // Fire button (hold mode)
        if (_fireButton != null && _weapon != null)
        {
            _fireButton.OnHold += () => _weapon.TryFire();
        }
    }
    
    private void UpdateMovement()
    {
        if (_joystick == null || _playerMovement == null)
            return;
        
        // Get joystick input
        Vector2 moveInput = _joystick.Input;
        
        // Send to player movement
        _playerMovement.SetMoveInput(moveInput);
    }
    
    private void UpdateTouchCamera()
    {
        if (_firstPersonCamera == null)
            return;
        
        // Handle touch input for camera
        if (UnityEngine.Input.touchCount > 0)
        {
            foreach (Touch touch in UnityEngine.Input.touches)
            {
                // Skip touches over UI
                if (IsTouchOverUI(touch.position))
                    continue;
                
                // Assign finger ID if not set
                if (_cameraFingerID == -1)
                {
                    _cameraFingerID = touch.fingerId;
                    _lastTouchPosition = touch.position;
                    continue;
                }
                
                // Only process our assigned finger
                if (touch.fingerId != _cameraFingerID)
                    continue;
                
                // Process touch phases
                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                        Vector2 delta = touch.position - _lastTouchPosition;
                        delta *= _touchSensitivity;
                        
                        if (_invertY)
                            delta.y = -delta.y;
                        
                        _firstPersonCamera.SetLookInput(delta);
                        _lastTouchPosition = touch.position;
                        break;
                    
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == _cameraFingerID)
                        {
                            _cameraFingerID = -1;
                            _firstPersonCamera.SetLookInput(Vector2.zero);
                        }
                        break;
                }
            }
        }
        else
        {
            // No touches - reset
            _cameraFingerID = -1;
            _firstPersonCamera.SetLookInput(Vector2.zero);
        }
    }
    
    private bool IsTouchOverUI(Vector2 touchPosition)
    {
        // Check if touch is over UI element (joystick or buttons)
        if (_joystick != null)
        {
            RectTransform joystickRect = _joystick.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(joystickRect, touchPosition))
                return true;
        }
        
        // Check buttons
        MobileButton[] buttons = { _jumpButton, _crouchButton, _sprintButton, _fireButton };
        foreach (var button in buttons)
        {
            if (button != null)
            {
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, touchPosition))
                    return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Force enable/disable mobile controls.
    /// </summary>
    public void SetMobileMode(bool enabled)
    {
        IsMobilePlatform = enabled;
        if (_mobileUI != null)
        {
            _mobileUI.SetActive(enabled);
        }
    }
}
