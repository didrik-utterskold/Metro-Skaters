using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour 
{
    // Using headers to organise the variables in the inspector
    [Header("Movement")]

    [SerializeField] private float walkingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float groundDrag;
    private float moveSpeed;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float jumpForce = 5f;
    bool grounded;

    [SerializeField] private Transform orientation;

    [SerializeField] InputAction jump;

    float horizontalInput;
    float verticalInput;
    bool isSprinting;

    

    Vector3 moveDirection;

    Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        jump.Enable();
    }

    private void Update()
    {
        HandleInput();
        
    }

    // Run every physics update
    private void FixedUpdate()
    {
        grounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, whatIsGround);

        if (grounded)
        { 
            rb.linearDamping = groundDrag; 
        } 
        else
        {
            rb.linearDamping = 0f;
        }

        if (jump.IsPressed() && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
            

        MovePlayer();
        SpeedControl();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void MovePlayer()
    {
        HandleSprinting();
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
    }

    // Check if the player is sprinting and adjust the move speed accordingly
    private void HandleSprinting() 
    {
        if (isSprinting) 
        {
            moveSpeed = sprintingSpeed;
        } 
        else 
        {
            moveSpeed = walkingSpeed;
        }
    }

    // Limit the speed within reasonable intervals
    private void SpeedControl() {           
        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed) 
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
