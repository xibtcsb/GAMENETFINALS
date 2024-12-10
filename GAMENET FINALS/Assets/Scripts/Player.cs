using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
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
}