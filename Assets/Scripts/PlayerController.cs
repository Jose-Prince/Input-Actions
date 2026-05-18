using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private Rigidbody rb;
    private CapsuleCollider cc;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private bool isRunning;
    private bool isCrouching;
    private bool jumpPressed;
    private bool isGrounded;


    [Header("Movement Speed")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float crouchSpeed = 2.5f;
    [SerializeField] float jumpForce = 7f;

    [Header("Height")]
    [SerializeField] float standingHeight = 2f;
    [SerializeField] float crouchHeight = 1f;

    [Header("Ground check")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;


    [SerializeField] Transform cameraPivot;
    [SerializeField] float sensitivity = 0.5f;

    void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += ctx =>
            moveInput = ctx.ReadValue<Vector2>();

        controls.Player.Move.canceled += ctx =>
            moveInput = Vector2.zero;

        controls.Player.Camera.performed += ctx =>
            lookInput = ctx.ReadValue<Vector2>();

        controls.Player.Camera.canceled += ctx =>
            lookInput = Vector2.zero;

        controls.Player.Run.performed += ctx => isRunning = true;
        controls.Player.Run.canceled += ctx => isRunning = false;

        controls.Player.Crouch.performed += ctx => isCrouching = true;
        controls.Player.Crouch.canceled += ctx => isCrouching = false;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        CheckGround();

        // Camera logic
        transform.Rotate(Vector3.up * lookInput.x * sensitivity);

        xRotation -= lookInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    
        UpdateCrouch();
    }

    void FixedUpdate()
    {
        // Movement logic
        float currentSpeed = walkSpeed;

        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isRunning)
            currentSpeed = runSpeed;

        Vector3 movement = transform.forward * moveInput.y + transform.right * moveInput.x;
        
        rb.linearVelocity = new Vector3(
            movement.x * currentSpeed,
            rb.linearVelocity.y,
            movement.z * currentSpeed
        );

        // Apply jump
        if (jumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
        }
    }

    void UpdateCrouch()
    {
        cc.height = isCrouching ? crouchHeight : standingHeight;

        Vector3 camPos = cameraPivot.localPosition;

        camPos.y = isCrouching ? 0.5f : 1f;

        cameraPivot.localPosition = camPos;
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}
