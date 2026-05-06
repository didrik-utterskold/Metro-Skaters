using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.CodeAnalysis;
using UnityEditorInternal;

public class PlayerMovementTemp : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float airDrag;
    [SerializeField] private float groundDrag = 4;
    [SerializeField] private Transform orientation;
    [SerializeField] private CapsuleCollider capsuleCollider;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
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
        airborne
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        normalHeight = capsuleCollider.height;
        normalCenter = capsuleCollider.center;

        currentState = MovementState.walking;
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
        Debug.DrawRay(Vector3.zero, Vector3.up * 5f, Color.red, 1f);
        HandleInput();

        if (slideCooldown > 0f)
            slideCooldown -= Time.deltaTime;

        if (stateLockTimer > 0f)
            stateLockTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        GroundDetection();
        HandleJump();
        GravityAmplifier();

        UpdateState();

        HandleMovement();
    }

    // ================= INPUT =================

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

    private void GroundDetection()
    {
        Vector3 center = groundCheck.position;

        float height = standingHeight; // update this if you crouch/slide

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

    private void UpdateState()
    {

        if (!isGrounded)
        {
            if (currentState != MovementState.airborne)
                SwitchState(MovementState.airborne);

            return;
        }

        if (slidingPressed && isGrounded && slideCooldown <= 0)
        {
            SwitchState(MovementState.sliding);
            return;
        }

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

    private void SwitchState(MovementState state)
    {
        if (currentState == state || (stateLockTimer > 0f))
        return;
        
        ExitState(currentState);

        previousState = currentState;
        currentState = state;

        EnterState(state);

    }


    private void EnterState(MovementState state)
    {
        switch (state)
        {
            case MovementState.sliding:
                EnterSlide();
                break;
        }
    }
    private void ExitState(MovementState state)
    {
        switch (state)
        {
            case MovementState.sliding:
                ExitSlide();
                break;
        }
    }

    // ================= MOVEMENT =================

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
        }
    }

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

    private void HandleSliding()
    {
        rb.linearDamping = 0f;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(-flatVel.normalized * slideFriction, ForceMode.Force);

        if (!slidingPressed || flatVel.magnitude < minSlideSpeed)
        {
            SwitchState(previousState);
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

    // ================= SLIDE =================

    private void EnterSlide()
    {
        capsuleCollider.height = slideHeight;
        capsuleCollider.center = new Vector3(0f, slideHeight / 2f, 0f);

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Impulse);

        stateLockTimer = stateLockTime;
    }

    private void ExitSlide()
    {
        capsuleCollider.height = normalHeight;
        capsuleCollider.center = normalCenter;

        slideCooldown = slideCooldownTime;
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
        jumpForce = 5f;
    }

    //=====================Speed control=================================
    private void GravityAmplifier()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Force);
        }
    }
}