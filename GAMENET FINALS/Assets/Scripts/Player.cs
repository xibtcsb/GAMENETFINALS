using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] float moveSpeed = 5f, jumpPower = 7f, gravity = 10f, lookSpeed = 2f, lookXLimit = 45f;

    private Rigidbody _rigidbody;
    private Vector3 moveDirection;
    private float rotationX;

    void Start()
    {
        if (!isLocalPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer("Target");
            Destroy(playerCamera.gameObject); // Avoid conflicting cameras
            playerCamera = null;
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
        if (!isLocalPlayer || playerCamera == null) return;

        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // Look up/down
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Rotate player left/right
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void HandleMovement()
    {
        // Input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement vector
        Vector3 movement = (transform.right * moveX + transform.forward * moveZ) * moveSpeed;

        // Apply force to Rigidbody for slippery movement
        _rigidbody.AddForce(movement, ForceMode.Acceleration);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }


    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
