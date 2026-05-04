using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Using headers to organise the variables in the inspector
    [Header("Movement")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private Transform orientation;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private float moveSpeed;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private float groundLength;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Sliding")]
    [SerializeField] private float slideDuration;

    [Header("Input")]
    [SerializeField] InputAction jump;
    [SerializeField] InputAction movementInput;
    [SerializeField] InputAction sprint;
    [SerializeField] InputAction slide;


    private float horizontalInput;
    private float verticalInput;
    private float normalHeight;
    private Vector3 normalCenter;
    private float slideTimer;
    private bool sprintingPressed;
    private bool isSliding;
    private bool slidingPressed;
    private bool jumpingPressed;
    private bool isGrounded;

    private Coroutine jumpBoostCoroutine;

    private Vector3 moveDirection;

    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        normalHeight = capsuleCollider.height;
        normalCenter = capsuleCollider.center;
    }

    private void OnEnable()
    {
        movementInput.Enable();
        jump.Enable();
        sprint.Enable();
        slide.Enable();
    }

    private void OnDisable()
    {
        movementInput.Disable();
        jump.Disable();
        sprint.Disable();
        slide.Disable();
    }

    private void Update()
    {
        HandleInput();

    }

    // Run every physics update
    private void FixedUpdate()
    {
        GroundDetection();
        HandleDrag();
        MovePlayer();
        HandleSliding();
        HandleJumping();
        SpeedControl();
    }

    private void GroundDetection()
    {
        bool isTouching = isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, whatIsGround);

        bool isBelow = isGrounded = Physics.SphereCast(
            groundCheck.position,
            groundRadius,
            Vector3.down,
            out RaycastHit hit,
            groundLength,
            whatIsGround,
            QueryTriggerInteraction.Ignore
        );

        isGrounded = isTouching || isBelow;
    }

    private void HandleInput()
    {
        Vector2 movementInput = this.movementInput.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        sprintingPressed = sprint.IsPressed();
        slidingPressed = slide.IsPressed();
        jumpingPressed = jump.IsPressed();
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
        if (sprintingPressed)
        {
            moveSpeed = sprintingSpeed;
        }
        else
        {
            moveSpeed = walkingSpeed;
        }
    }

    private void StartSlide()
    {
        if (slidingPressed && isGrounded && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;
            capsuleCollider.height = 0.5f;
            capsuleCollider.center = new Vector3(0f, 0.25f, 0f);
        }
    }

    private void HandleSliding()
    {
        StartSlide();

        if (!isSliding) return;

        slideTimer -= Time.fixedDeltaTime;

        if (slideTimer <= 0f)
        {
            capsuleCollider.height = normalHeight;
            capsuleCollider.center = normalCenter;
            isSliding = false;
        }
    }


    private void HandleJumping()
    {
        if (jumpingPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleDrag()
    {
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    // Limit the speed within reasonable intervals
    private void SpeedControl()
    {
        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    // Method to set the jump boost effect, allowing for temporary increases in jump height
    public void SetJumpBoost(float jumpBoost)
    {
        if (jumpBoostCoroutine != null)
        {
            StopCoroutine(jumpBoostCoroutine);
        }

        jumpForce = jumpBoost;
        jumpBoostCoroutine = StartCoroutine(JumpBoostCoroutine(10f));
    }

    // Coroutine to reset the jump force after the duration of the jump boost effect has expired
    private IEnumerator JumpBoostCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpForce = 5f;
    }
}