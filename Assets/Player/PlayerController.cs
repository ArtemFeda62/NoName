using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _gravity = -25f;
    [SerializeField] private float _jumpFallMultiplier = 4f;
    [SerializeField] private float _lowJumpMultiplier = 3f;

    [Header("Jump")]
    [SerializeField] private float _jumpCooldown = 0.3f;

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 10f;
    [SerializeField] private float _staminaDrainRate = 1f;
    [SerializeField] private float _staminaRegenRate = 2f;

    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    [Header("GroundCheck")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;

    private Vector2 _moveInput;
    private Vector3 _velocity;
    private float _currentSpeed;
    private bool _isGrounded;
    private bool _isRunning;
    private float _stamina;
    private float _staminaTimer;
    private bool _isJumping;
    private bool _isJumpButtonPressed;
    private float _jumpTimer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _currentSpeed = _walkSpeed;
        _stamina = _maxStamina;

        if (_groundCheck == null)
            _groundCheck = transform;
    }

    private void Update()
    {
        if (_jumpTimer > 0)
            _jumpTimer -= Time.deltaTime;

        UpdateStamina();
        HandleInput();
        ApplyJumpPhysics();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyGravity();
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = (GetForwardDirection() * _moveInput.y + GetRightDirection() * _moveInput.x).normalized;
        Vector3 finalMovement = moveDirection * _currentSpeed * Time.fixedDeltaTime;
        finalMovement += _velocity * Time.fixedDeltaTime;
        _characterController.Move(finalMovement);
    }

    private void CheckGrounded()
    {
        float checkDistance = _groundDistance + 0.1f;
        _isGrounded = Physics.CheckSphere(_groundCheck.position, checkDistance, _groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
            _isJumping = false;
        }
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
            return;
        }

        _velocity.y += _gravity * Time.fixedDeltaTime;

        if (_velocity.y < -30f)
            _velocity.y = -30f;
    }

    private void ApplyJumpPhysics()
    {
        if (!_isJumping) return;

        if (_velocity.y < 0)
        {
            _velocity.y += _gravity * _jumpFallMultiplier * Time.deltaTime;
        }
        else if (_velocity.y > 0 && !_isJumpButtonPressed)
        {
            _velocity.y += _gravity * _lowJumpMultiplier * Time.deltaTime;
        }
    }

    private Vector3 GetForwardDirection()
    {
        Vector3 forward = _cinemachineCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetRightDirection()
    {
        Vector3 right = _cinemachineCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void UpdateStamina()
    {
        if (_isRunning && _stamina >= 1)
        {
            _staminaTimer += Time.deltaTime;
            if (_staminaTimer >= 0.5f)
            {
                _stamina -= _staminaDrainRate;
                _stamina = Mathf.Clamp(_stamina, 0, _maxStamina);
                _staminaTimer = 0f;
            }
        }
        else if (_stamina < _maxStamina && !_isRunning)
        {
            _staminaTimer += Time.deltaTime;
            if (_staminaTimer >= 1f)
            {
                _stamina += _staminaRegenRate;
                _stamina = Mathf.Clamp(_stamina, 0, _maxStamina);
                _staminaTimer = 0f;
            }
        }
    }

    private void HandleInput()
    {
        if (_isRunning && _stamina < 1)
        {
            _isRunning = false;
            _currentSpeed = _walkSpeed;
        }
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        _isJumpButtonPressed = value.isPressed;

        if (_isGrounded && value.isPressed && _jumpTimer <= 0)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _isJumping = true;
            _jumpTimer = _jumpCooldown;
        }
    }

    public void OnSprint(InputValue value)
    {
        if (value.Get<float>() > 0.5f && _stamina >= 1)
        {
            _isRunning = true;
            _currentSpeed = _sprintSpeed;
        }
        else if (value.Get<float>() == 0)
        {
            _isRunning = false;
            _currentSpeed = _walkSpeed;
        }
    }
}