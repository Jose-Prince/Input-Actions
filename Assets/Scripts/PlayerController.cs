using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Vector2 lookInput;
    private float xRotation = 0f;

    [SerializeField] float speed = 5f;
    [SerializeField] Transform cameraPivot;
    [SerializeField] float sensitivity = 0.5f;

    void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
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
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // Movement logic
        Vector3 localMove = transform.right * moveInput.x + transform.forward * moveInput.y;

        rb.linearVelocity = new Vector3(
            localMove.x * speed,
            rb.linearVelocity.y,
            localMove.z * speed
        );

        // Camera logic
        transform.Rotate(Vector3.up * lookInput.x * sensitivity);

        xRotation -= lookInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
