using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Using headers to organise the variables in the inspector
    [Header("Movement")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float airDrag;
    [SerializeField] private float groundDrag = 4;
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
    [SerializeField] private float slideForce;
    [SerializeField] private float slideFriction;
    [SerializeField] private float minSlideSpeed;
    [SerializeField] private float slideHeight = 0.5f;
    [SerializeField] private float slideCooldownTime = 0.2f;

    [Header("Input")]
    [SerializeField] InputAction jump;
    [SerializeField] InputAction movementInput;
    [SerializeField] InputAction sprint;
    [SerializeField] InputAction slide;


    [Header("Input State")]
    private float horizontalInput;
    private float verticalInput;
    private bool slidingPressed;
    private bool jumpingPressed;
    
    [Header("Movement State")]
    //temp while setting up statesystem
    private bool isSprinting;
    private bool isSliding;
    private bool isGrounded;
    
    public enum MovementState {
        walking,
        sprinting,
        sliding,
        airborne
    }
    private MovementState currentState;
    
    [Header("Slide State")]
    private float slideCooldown;
    private float normalHeight;
    private Vector3 normalCenter;

    [Header("Runtime References")]
    private Vector3 moveDirection;
    private Rigidbody rb;
    
    [Header("Abilities")]
    private Coroutine jumpBoostCoroutine;


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
        if (slideCooldown > 0f)
        slideCooldown -= Time.deltaTime;
    }

    // Run every physics update
    private void FixedUpdate()
    {
        GroundDetection();
        HandleSlideRequest();
        HandleState();
        MovePlayer();
        HandleSliding();
        HandleDrag();
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
        slidingPressed = slide.IsPressed();
        jumpingPressed = jump.IsPressed();
        if (sprint.WasPressedThisFrame()) {isSprinting = !isSprinting;}
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        switch (currentState)
        {
            case MovementState.walking:
                moveSpeed = walkingSpeed;
                break;

            case MovementState.sprinting:
                moveSpeed = sprintingSpeed;
                break;

            case MovementState.sliding:
                return;
        }

        rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
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
        if (isSliding) 
        {
            return;
        }
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
    private void SpeedControl() {           
       if (isSliding) return;

        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    // Method to set the jump boost effect, allowing for temporary increases in jump height
    public void SetJumpBoost(float jumpBoost, float duration)
    {
        if (jumpBoostCoroutine != null)
        {
            StopCoroutine(jumpBoostCoroutine);
        }

        jumpForce = jumpBoost;
        jumpBoostCoroutine = StartCoroutine(JumpBoostCoroutine(duration));
    }

    // Coroutine to reset the jump force after the duration of the jump boost effect has expired
    private IEnumerator JumpBoostCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpForce = 5f;
    }
    
    private void StartSlide()
    {
        if (slideCooldown > 0f) return;
        rb.linearDamping = 0f;
        isSliding = true;
        capsuleCollider.height = slideHeight;
        capsuleCollider.center = new Vector3(0f, slideHeight / 2f, 0f);

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
         rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Impulse);

    }

    private void StopSlide()
    {
        isSliding = false;
        capsuleCollider.height = normalHeight;
        capsuleCollider.center = normalCenter;
        slideCooldown = slideCooldownTime;

    }

    private void HandleSliding()
    {

        if (!isSliding) return;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (!slidingPressed || flatVel.magnitude < minSlideSpeed)
        {
            StopSlide();
            return;
        }

        rb.AddForce(-flatVel.normalized * slideFriction, ForceMode.Force);
    }

    private void HandleState()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (!isGrounded)
        {
            currentState = MovementState.airborne;
            return;
        }

        if (isSliding)
        {
            currentState = MovementState.sliding;
            return;
        }

        if (isSprinting && flatVel.magnitude > 0.1f)
        {
            currentState = MovementState.sprinting;
            return;
        }

        currentState = MovementState.walking;
    }

    private void HandleSlideRequest()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        bool canSlide =
            slidingPressed &&
            isGrounded &&
            flatVel.magnitude >= minSlideSpeed &&
            slideCooldown <= 0f;

        if (canSlide && !isSliding)
        {
            StartSlide();
        }
    }

}
