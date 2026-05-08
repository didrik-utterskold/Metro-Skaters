using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float airDrag;
    [SerializeField] private float groundDrag = 4;
    [SerializeField] private Transform orientation;
    [SerializeField] private CapsuleCollider capsuleCollider;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float capsuleRadius = 0.3f;
    [SerializeField] private float standingHeight = 2f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private float extraGravity = 10f;

    [Header("Sliding")]
    [SerializeField] private float slideForce;
    [SerializeField] private float slideFriction;
    [SerializeField] private float minSlideSpeed;
    [SerializeField] private float slideHeight = 0.5f;
    [SerializeField] private float slideCooldownTime = 0.2f;

    [Header("Input")]
    [SerializeField] private InputAction jump;
    [SerializeField] private InputAction movementInput;
    [SerializeField] private InputAction sprint;
    [SerializeField] private InputAction slide;

    [Header("Input State")]
    private float horizontalInput;
    private float verticalInput;
    private bool slidingPressed;
    private bool jumpPressed;
    private bool sprintToggle;

    [Header("State")]
    private float stateLockTimer;
    [SerializeField] private float stateLockTime = 0.15f;
    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        airborne,
        crouching
    }

    private MovementState currentState;
    private MovementState previousState;

    [Header("Runtime References")]
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isGrounded;

    [Header("Slide State")]
    private float slideCooldown;
    private float normalHeight;
    private Vector3 normalCenter;

    [Header("Abilities")]
    private Coroutine jumpBoostCoroutine;


//establishes Rigid body and startup state
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        normalHeight = capsuleCollider.height;
        normalCenter = capsuleCollider.center;

        currentState = MovementState.walking;
    }
// Enables inputs
    private void OnEnable()
    {
        movementInput.Enable();
        jump.Enable();
        sprint.Enable();
        slide.Enable();
    }
// Kills inputs
    private void OnDisable()
    {
        movementInput.Disable();
        jump.Disable();
        sprint.Disable();
        slide.Disable();
    }
// Keeps track of timers and handles input, everything that needs to be done in real time.
    private void Update()
    {
        Debug.Log("IsGrounded: " + isGrounded);
        HandleInput();

        if (slideCooldown > 0f)
            slideCooldown -= Time.deltaTime;

        if (stateLockTimer > 0f)
            stateLockTimer -= Time.deltaTime;
    }
// Handles input-based physics, UpdateState() + HandleMovement() enables all movement
    private void FixedUpdate()
    {
        GroundDetection();
        HandleJump();
        GravityAmplifier();

        UpdateState();

        HandleMovement();
    }

    // ================= INPUT =================
// Tracks movement inputs and toggles
    private void HandleInput()
    {
        Vector2 input = movementInput.ReadValue<Vector2>();
        horizontalInput = input.x;
        verticalInput = input.y;

        slidingPressed = slide.IsPressed();
        jumpPressed = jump.IsPressed();

        if (sprint.WasPressedThisFrame())
            {
                sprintToggle = !sprintToggle;
            }
    }

    // ================= GROUND =================
// Capsule cast ground detection through a child object "GroundCheck" attached to player obj in unity
    private void GroundDetection()
    {
        Vector3 center = groundCheck.position;

        float height = standingHeight;

        Vector3 top = center + Vector3.up * (height / 2f - capsuleRadius);
        Vector3 bottom = center - Vector3.up * (height / 2f - capsuleRadius);

        isGrounded = Physics.CheckCapsule(
            bottom,
            top,
            capsuleRadius,
            whatIsGround,
            QueryTriggerInteraction.Ignore
        );
    }

    // ================= STATE =================
// Handles all (-crouch) state entries through priority based on which is more important
    private void UpdateState()
    {
        // if not grounded -> Airborne, the most important check
        if (!isGrounded)
        {
            if (currentState != MovementState.airborne)
                SwitchState(MovementState.airborne);

            return;
        }
        // If slide is valid change state to sliding
        if (slidingPressed && isGrounded && slideCooldown <= 0 && currentState != MovementState.crouching)
        {
            SwitchState(MovementState.sliding);
            return;
        } 
        // sprint toggle handler
        if (sprintToggle && currentState == MovementState.walking)
        {
            SwitchState(MovementState.sprinting);
            return;
        }

        if (!sprintToggle && currentState == MovementState.sprinting)
        {
            SwitchState(MovementState.walking);
            return;
        }

    }
// Handles all state switching, stores prev state in variable
    private void SwitchState(MovementState state)
    {
        if (currentState == state || (stateLockTimer > 0f))
        return;
        //Checks if the new state has any on-entry or the old one has any on-exit conditions
        ExitState(currentState);

        previousState = currentState;
        currentState = state;

        EnterState(state);

    }

// Just a switch triggering on-entry scripts
    private void EnterState(MovementState state)
    {
        switch (state)
        {
            case MovementState.sliding:
                EnterSlide();
                break;
            case MovementState.crouching:
                EnterCrouch();
                break;
        }
    }
// same as prev but for exits
    private void ExitState(MovementState state)
    {
        switch (state)
        {
            case MovementState.sliding:
                ExitSlide();
                break;
            case MovementState.crouching:
                ExitSlide();
                break;
        }
    }

    // ================= MOVEMENT =================
// Decides which movement script to use depending on current state
    private void HandleMovement()
    {
        switch (currentState)
        {
            case MovementState.walking:
                HandleWalking();
                break;

            case MovementState.sprinting:
                HandleSprinting();
                break;

            case MovementState.sliding:
                HandleSliding();
                break;

            case MovementState.airborne:
                HandleAirborne();
                break;
            
            case MovementState.crouching:
            HandleCrouching();
            break;
        }
    }
// Move character in direction set by WASD with a set speed that is the input
    private void Move(float speed)
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        rb.AddForce(moveDirection * speed * 10f, ForceMode.Force);

        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

    }
// As jump is basically just applying a force its when player presses jump we apply that force
    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleWalking()
    {
        rb.linearDamping = groundDrag;
        Move(walkingSpeed);
    }

    private void HandleSprinting()
    {
        rb.linearDamping = groundDrag;
        Move(sprintingSpeed);
    }
// Handles movement in slide, linear damping to zero and locks movement in the direction it was pre-slide
    private void HandleSliding()
    {
        rb.linearDamping = 0f;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(-flatVel.normalized * slideFriction, ForceMode.Force);

// continues until sliding isnt pressed or the speed is low enough to enter crouch
        if (!slidingPressed)
        {
            SwitchState(previousState);
        } else if (flatVel.magnitude < minSlideSpeed)
        {
            SwitchState(MovementState.crouching);
        }
    }

    private void HandleAirborne()
    {
        rb.linearDamping = airDrag;
        Move(walkingSpeed);

        if (isGrounded)
        {
            SwitchState(MovementState.walking);
        }
    }

    //Crouch is basically walk with its own speed and its own exit condition
    private void HandleCrouching()
    {
        rb.linearDamping = groundDrag;
        Move(crouchSpeed);
        if (!slidingPressed)
        {
            SwitchState(MovementState.walking);
        }
    }

    // ================= SLIDE/CROUCH =================

    private void EnterSlide()
    {
        //Adjusts capsule height for slide hitbox
        capsuleCollider.height = slideHeight;
        capsuleCollider.center = new Vector3(0f, slideHeight / 2f, 0f);
        //adds a small boost downwards as the deform will make player airborne
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        //Records old movespeed, applies a boost and off we go
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Impulse);

        //statelock so that the short period of airborne from the bean adjustment wont mess with states
        stateLockTimer = stateLockTime;
    }

    //use exit slide to exit crouch because we want to do the exact same thing anyway
    private void ExitSlide()
    {
        //readjusts capsule height
        capsuleCollider.height = normalHeight;
        capsuleCollider.center = normalCenter;

        //slide cooldown so we cant spam slide
        slideCooldown = slideCooldownTime;
    }

    private void EnterCrouch()
    {
        // as exit slide currently will run when crouch is enabled, readjust capusle height, a little scuffed but eh
        capsuleCollider.height = slideHeight;
        capsuleCollider.center = new Vector3(0f, slideHeight / 2f, 0f);
    }

    // ================= ABILITIES =================

    public void SetJumpBoost(float jumpBoost, float duration)
    {
        if (jumpBoostCoroutine != null)
        {
            StopCoroutine(jumpBoostCoroutine);
        }

        jumpForce = jumpBoost;
        jumpBoostCoroutine = StartCoroutine(JumpBoostCoroutine(duration));
    }

    private IEnumerator JumpBoostCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpForce = 10f;
    }

    //=====================Speed control=================================
    //Jump is a little floaty with lineardamping applying to all axes, so this grav amp helps reduce it
    private void GravityAmplifier()
    {
        //only add force when falling
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Force);
        }
    }
}