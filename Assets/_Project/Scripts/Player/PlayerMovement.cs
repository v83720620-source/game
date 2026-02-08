using UnityEngine;

/// <summary>
/// FPS Player movement controller.
/// Handles walking, sprinting, jumping, and crouching.
/// Network-ready: Uses events for state changes.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _crouchSpeed = 2.5f;
    [SerializeField] private float _acceleration = 10f;
    
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = -15f;
    
    [Header("Crouch")]
    [SerializeField] private float _standHeight = 2f;
    [SerializeField] private float _crouchHeight = 1f;
    [SerializeField] private float _crouchTransitionSpeed = 10f;
    
    [Header("Ground Check")]
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundMask;
    
    // Components
    private CharacterController _controller;
    
    // Movement state
    private Vector3 _velocity;
    private Vector3 _currentVelocity;
    private bool _isGrounded;
    private bool _isSprinting;
    private bool _isCrouching;
    private float _targetHeight;
    
    // Input
    private Vector2 _moveInput;
    private bool _jumpInput;
    private bool _sprintInput;
    private bool _crouchInput;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _targetHeight = _standHeight;
    }
    
    private void Update()
    {
        HandleInput();
        CheckGround();
        HandleMovement();
        HandleCrouch();
        ApplyGravity();
    }
    
    private void HandleInput()
    {
        // OLD INPUT DISABLED - Using New Input System (Mobile UI) only
        // Mobile input is handled by MobileInputManager
        
        /* KEYBOARD INPUT DISABLED FOR ANDROID
        // WASD movement
        _moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpInput = true;
        }
        
        // Sprint
        _sprintInput = Input.GetKey(KeyCode.LeftShift);
        
        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _crouchInput = !_crouchInput;
        }
        */
    }
    
    private void CheckGround()
    {
        // Check if grounded - use controller center for accurate position
        Vector3 spherePos = transform.position + _controller.center - new Vector3(0, _controller.height / 2f, 0);
        _isGrounded = Physics.CheckSphere(spherePos, _groundCheckDistance, _groundMask);
        
        // Reset vertical velocity when grounded
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    }
    
    private void HandleMovement()
    {
        // Determine speed
        float targetSpeed = _walkSpeed;
        
        if (_isCrouching)
        {
            targetSpeed = _crouchSpeed;
            _isSprinting = false;
        }
        else if (_sprintInput && _moveInput.y > 0)
        {
            targetSpeed = _sprintSpeed;
            _isSprinting = true;
        }
        else
        {
            _isSprinting = false;
        }
        
        // Calculate move direction
        Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        moveDirection = moveDirection.normalized;
        
        // Target velocity
        Vector3 targetVelocity = moveDirection * targetSpeed;
        
        // Smooth acceleration
        _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, _acceleration * Time.deltaTime);
        
        // Handle jump
        if (_jumpInput && _isGrounded && !_isCrouching)
        {
            _velocity.y = Mathf.Sqrt(_jumpForce * -2f * _gravity);
            _jumpInput = false;
        }
        
        // Move
        Vector3 move = _currentVelocity * Time.deltaTime;
        _controller.Move(move);
    }
    
    private void HandleCrouch()
    {
        // Update crouch state
        _isCrouching = _crouchInput;
        
        // Set target height
        _targetHeight = _isCrouching ? _crouchHeight : _standHeight;
        
        // Smooth height transition
        float newHeight = Mathf.Lerp(_controller.height, _targetHeight, _crouchTransitionSpeed * Time.deltaTime);
        
        // Update controller height
        float heightDifference = newHeight - _controller.height;
        _controller.height = newHeight;
        _controller.center = new Vector3(0, newHeight / 2f, 0);
        
        // Adjust position to prevent floating
        if (heightDifference < 0)
        {
            transform.position += Vector3.up * (heightDifference / 2f);
        }
    }
    
    private void ApplyGravity()
    {
        // Apply gravity
        _velocity.y += _gravity * Time.deltaTime;
        
        // Apply vertical velocity
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Set move input (for mobile or network).
    /// </summary>
    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }
    
    /// <summary>
    /// Trigger jump (for mobile or network).
    /// </summary>
    public void Jump()
    {
        if (_isGrounded && !_isCrouching)
        {
            _velocity.y = Mathf.Sqrt(_jumpForce * -2f * _gravity);
        }
    }
    
    /// <summary>
    /// Set sprint state (for mobile or network).
    /// </summary>
    public void SetSprint(bool sprint)
    {
        _sprintInput = sprint;
    }
    
    /// <summary>
    /// Toggle crouch (for mobile or network).
    /// </summary>
    public void ToggleCrouch()
    {
        _crouchInput = !_crouchInput;
    }
    
    // Properties for network sync
    public bool IsGrounded => _isGrounded;
    public bool IsSprinting => _isSprinting;
    public bool IsCrouching => _isCrouching;
    public Vector3 Velocity => _currentVelocity;
}
