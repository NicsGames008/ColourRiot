using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float groundDrag;
    public float jumpForce = 8f;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;

    [Header("Jump Tuning")]
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 1.5f;
    public float maxFallSpeed = -20f; // Optional clamp for falling speed

    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool grounded;
    private bool wasGrounded;

    public Transform orientation; // CAMERA orientation

    private float horizontalInput;
    private float verticalInput;
    private bool isSprinting;

    private Vector3 moveDirection;
    private Rigidbody rb;

    private float lastYVelocity;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float staminaSprintDrain = 20f;
    public float fallStaminaDrainMultiplier = 2f;
    public float sprintMultiplier = 1.5f;

    [Header("UI")]
    public Slider StaminaBar;
    public CanvasGroup staminaGroup;
    public Image staminaFill;

    [Header("UI Fade")]
    private float staminaFadeTimer;
    public float staminaFadeDelay = 1.5f;
    public float staminaFadeSpeed = 2f;

    [Header("Camera Bobbing")]
    public Transform headBobTarget;
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    public float sprintBobMultiplier = 1.5f;
    private float bobTimer = 0f;
    private Vector3 headStartPos;

    private PlayerState playerState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerState = GetComponent<PlayerState>();
        rb.freezeRotation = true;

        currentStamina = maxStamina;

        StaminaBar.maxValue = maxStamina;
        StaminaBar.value = currentStamina;
        staminaGroup.alpha = 0f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (headBobTarget != null)
            headStartPos = headBobTarget.localPosition;
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        RotatePlayerToCamera(); // <-- Added rotation to match camera

        MyInput();
        SpeedControl();
        HandleStamina();
        HandleFallStaminaDrain();

        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;

        wasGrounded = grounded;
        lastYVelocity = rb.velocity.y;

        StaminaBar.value = currentStamina;

        if (isSprinting || currentStamina < maxStamina)
            staminaFadeTimer = staminaFadeDelay;

        if (staminaFadeTimer > 0)
        {
            staminaFadeTimer -= Time.deltaTime;
            staminaGroup.alpha = Mathf.Lerp(staminaGroup.alpha, 1f, Time.deltaTime * staminaFadeSpeed);
        }
        else
        {
            staminaGroup.alpha = Mathf.Lerp(staminaGroup.alpha, 0f, Time.deltaTime * staminaFadeSpeed);
        }

        // 🌀 Stamina bar color: dark blue to vibrant blue
        float percent = currentStamina / maxStamina;
        Color lowBlue = new Color(0.1f, 0.1f, 0.3f);
        Color fullBlue = new Color(0.2f, 0.4f, 0.8f);
        staminaFill.color = Color.Lerp(lowBlue, fullBlue, percent);

        // ApplyHeadBob(); // Optional toggle
    }

    void FixedUpdate()
    {
        if (playerState.GetPlayerstate() == EPlayerState.Moving)
        {
            MovePlayer();
        }

        ApplyJumpGravity(); // Frame-rate independent jump gravity
    }

    private void RotatePlayerToCamera()
    {
        Vector3 viewDirection = new Vector3(orientation.forward.x, 0f, orientation.forward.z).normalized;
        if (viewDirection.magnitude >= 0.1f)
        {
            transform.forward = viewDirection;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isSprinting = Input.GetKey(sprintKey) && grounded && currentStamina > 0;

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        float currentSpeed = moveSpeed;
        if (isSprinting) currentSpeed *= sprintMultiplier;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized;

        if (grounded)
        {
            rb.AddForce(moveDirection * currentSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection * currentSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float maxSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, rb.velocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset vertical velocity
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void HandleStamina()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        if (isSprinting && isMoving)
        {
            currentStamina -= staminaSprintDrain * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    private void HandleFallStaminaDrain()
    {
        if (!wasGrounded && grounded)
        {
            float fallImpact = Mathf.Abs(lastYVelocity);
            fallImpact = Mathf.Min(fallImpact, 20f);

            if (fallImpact > 10f)
            {
                float staminaDamage = fallImpact * fallStaminaDrainMultiplier;
                currentStamina -= staminaDamage;
                currentStamina = Mathf.Max(currentStamina, 0f);
            }
        }
    }

    private void ApplyJumpGravity()
    {
        if (!grounded)
        {
            Vector3 gravityAdjustment = Vector3.zero;

            if (rb.velocity.y < 0)
            {
                gravityAdjustment = Vector3.up * Physics.gravity.y * (fallMultiplier - 1f);
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(jumpKey))
            {
                gravityAdjustment = Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f);
            }

            rb.velocity += gravityAdjustment * Time.fixedDeltaTime;

            // Clamp falling speed
            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, maxFallSpeed, rb.velocity.z);
            }
        }
    }

    private void ApplyHeadBob()
    {
        if (headBobTarget == null) return;

        bool isMoving = horizontalInput != 0 || verticalInput != 0;
        float speedFactor = isSprinting ? sprintBobMultiplier : 1f;

        if (grounded && isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency * speedFactor;

            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmplitude * 0.5f;

            headBobTarget.localPosition = headStartPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
        }
        else
        {
            headBobTarget.localPosition = Vector3.Lerp(headBobTarget.localPosition, headStartPos, Time.deltaTime * 5f);
            bobTimer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
