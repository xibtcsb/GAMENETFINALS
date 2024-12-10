using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    /*
    [SerializeField] Transform playerCamera;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lookSpeed = 2f;
    [SerializeField] float lookXLimit = 45f;

    private Rigidbody _rigidbody;
    private float rotationX;

    void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(playerCamera.gameObject);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody is missing on the player object!");
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMouseLook();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
    }

    private void HandleMouseLook()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 movement = (transform.right * moveX + transform.forward * moveZ) * moveSpeed;
        movement.y = _rigidbody.velocity.y;

        _rigidbody.velocity = movement;
    }
    */

    [Header("Camera")]
    public Transform playerCamera;
    public float sens;
    public float lookXLimit = 45f;
    float xRotation;

    [Header("Movement")]
    public Rigidbody rb;
    public Transform orientation;
    public float rightInput;
    public float forwardInput;
    public float moveSpeed;
    public Vector3 moveDirection;


    private void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(playerCamera.gameObject);
            return;
        }

        rb = this.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        CamInput();
        MoveInput();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        MovePlayer();
    }

    private void CamInput()
    {
        xRotation += -Input.GetAxis("Mouse Y") * sens;
        xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sens, 0);
    }

    private void MoveInput()
    {
        rightInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * forwardInput + orientation.right * rightInput;
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }
}