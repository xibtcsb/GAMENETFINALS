using Mirror;
using UnityEngine;
using TMPro;

public class Player : NetworkBehaviour
{

    [Header("Camera")]
    public Transform playerCamera;
    public float sens;
    public float lookXLimit = 45f;
    public float xRotation;
    public bool canMoveCam;

    [Header("Movement")]
    public Rigidbody rb;
    public Transform orientation;
    public float rightInput;
    public float forwardInput;
    public float moveSpeed;
    public Vector3 moveDirection;

    public TMPro.TextMeshProUGUI m_Score;
    public TMPro.TextMeshProUGUI m_BombTimer;
    public GameObject _gameManager;
    public int score;
    bool m_isInit;

    public override void OnStartServer()
    {
        base.OnStartServer();
        InitiateField();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

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
        
        canMoveCam = true;
    }

    [Command]
    public void InitiateField()
    {
        if (!isLocalPlayer)
            return;

        if (!m_isInit)
        {
            NetworkServer.Spawn(Instantiate(_gameManager));
            GameManager.instance.PlayerHost = this;
        }

        if (!m_isInit)
            m_isInit = true;

    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        CursorHandler();
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
        if(!canMoveCam)
            return;

        xRotation += -Input.GetAxis("Mouse Y") * sens;
        xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sens, 0);
    }

    private void CursorHandler()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canMoveCam = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canMoveCam = true;
        }
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