using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float currentSpeed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    private float _timer = 0f;
    public float stamina { get; set; } = 10f;
    private Rigidbody rb;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineCamera _cinCam;

    private Vector2 _move;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isRunning;

    // для проверки земли
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        rb.mass = 5;
        if (groundCheck == null)
            groundCheck = transform;
    }
    private void CalculationStamina()
    {
        if (_isRunning == true && stamina >= 1)
        {
            _timer += Time.deltaTime;
            if (_timer >= 0.5f)
            {
                stamina -= 1;
                stamina = Mathf.Clamp(stamina, 0, 10); //ограничение стамины
                _timer = 0f;
            }
        }
        else if (stamina < 10 && _isRunning == false)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                stamina += 2;
                stamina = Mathf.Clamp(stamina, 0, 10); 
                _timer = 0f; 
            }
        }
        if (stamina < 1 && _isRunning)
        {
            _isRunning = false;
            currentSpeed = walkSpeed;
        }
    }
    public void OnMove(InputValue val)
    {
        _move = val.Get<Vector2>();
    }

    public void OnJump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    public void OnSprint(InputValue val)
    {
        if (val.Get<float>() > 0.5f && stamina>=1)
        {
            _isRunning = true;
            currentSpeed = sprintSpeed;
        }
        else if (val.Get<float>() == 0)
        {
            _isRunning = false;
            currentSpeed = walkSpeed;
        }
    }

    private void Update()
    {
        CalculationStamina();
        CheckGrounded();
        ApplyGravity();
        MoveCharacter();
    }
    
    private void CheckGrounded()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    }

    private void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void MoveCharacter()
    {
        Vector3 movement = (GetForward() * _move.y + GetRight() * _move.x).normalized * Time.deltaTime * currentSpeed;
        _characterController.Move(movement);
    }

    private Vector3 GetForward()
    {
        Vector3 forward = _cinCam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = _cinCam.transform.right;
        right.y = 0;
        return right.normalized;
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}