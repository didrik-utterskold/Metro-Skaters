using UnityEngine;

public class PlayerMovement: MonoBehaviour 
{
    // Using headers to organise the variables in the inspector
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    [SerializeField] private Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleInput();
    }

    // Run every physics update
    private void FixedUpdate()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (grounded)
        { 
            rb.linearDamping = groundDrag; 
        } 
        else
        {
            rb.linearDamping = 0f;
        }
            

        MovePlayer();
        SpeedControl();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
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
