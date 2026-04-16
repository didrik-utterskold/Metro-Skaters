using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour 
{
    // Using headers to organise the variables in the inspector
    [Header("Movement")]

    [SerializeField] private float walkingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private Transform orientation;
    private float moveSpeed;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    bool grounded;

    [Header("Input")]
    [SerializeField] InputAction jump;
    [SerializeField] InputAction movementInput;
    [SerializeField] InputAction sprint;


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
        movementInput.Enable();
        jump.Enable();
        sprint.Enable();
    }

    private void OnDisable()
    {
        movementInput.Disable();
        jump.Disable();
        sprint.Disable();
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
        Vector2 movementInput = this.movementInput.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        isSprinting = sprint.ReadValue<float>() > 0;
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
