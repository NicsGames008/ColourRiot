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

    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool grounded;
    private bool wasGrounded;

    public Transform orientation;

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
    public Color fullStaminaColor = Color.green;
    public Color midStaminaColor = Color.yellow;
    public Color lowStaminaColor = Color.red;

    [Header("UI Fade")]
    private float staminaFadeTimer;
    public float staminaFadeDelay = 1.5f;
    public float staminaFadeSpeed = 2f;

    [Header("Camera Bobbing")]
    public Transform headBobTarget;
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    public float sprintBobMultiplier = 1.5f;
    //private float bobTimer = 0f;
    private Vector3 headStartPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentStamina = maxStamina;

        StaminaBar.maxValue = maxStamina;
        StaminaBar.value = currentStamina;
        staminaGroup.alpha = 0f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;






        //if (headBobTarget != null)
            //headStartPos = headBobTarget.localPosition;
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        SpeedControl();
        HandleStamina();
        HandleFallStaminaDrain();
        JumpPhysics();

        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;

        wasGrounded = grounded;
        lastYVelocity = rb.velocity.y;

      

        StaminaBar.value = currentStamina;

        
        if (isSprinting || currentStamina < maxStamina)
        {
            staminaFadeTimer = staminaFadeDelay;
        }

        if (staminaFadeTimer > 0)
        {
            staminaFadeTimer -= Time.deltaTime;
            staminaGroup.alpha = Mathf.Lerp(staminaGroup.alpha, 1f, Time.deltaTime * staminaFadeSpeed);
        }
        else
        {
            staminaGroup.alpha = Mathf.Lerp(staminaGroup.alpha, 0f, Time.deltaTime * staminaFadeSpeed);
        }

      

        float percent = currentStamina / maxStamina;

        if (percent > 0.5f)
        {
            float t = (percent - 0.5f) * 2f;
            staminaFill.color = Color.Lerp(midStaminaColor, fullStaminaColor, t);
        }
        else
        {
            float t = percent * 2f;
            staminaFill.color = Color.Lerp(lowStaminaColor, midStaminaColor, t);
        }

      
        //ApplyHeadBob();
    }

    void FixedUpdate()
    {
        MovePlayer();
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
        if (isSprinting)
            currentSpeed *= sprintMultiplier;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized;

        float speedMultiplier = grounded ? 1f : airMultiplier;
        rb.AddForce(moveDirection * currentSpeed * 10f * speedMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float maxSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
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

    private void JumpPhysics()
    {
        if (!grounded)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(jumpKey))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
            }
        }
    }

    //private void ApplyHeadBob()
    //{
       // if (headBobTarget == null) return;

       // bool isMoving = horizontalInput != 0 || verticalInput != 0;
      //  float speedFactor = isSprinting ? sprintBobMultiplier : 1f;

       // if (grounded && isMoving)
       // {
          //  bobTimer += Time.deltaTime * bobFrequency * speedFactor;

           // float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude;
          //  float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmplitude * 0.5f;

        //    headBobTarget.localPosition = headStartPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
      //  }
     //   else
      //  {
            // Smooth return to idle position
      //      headBobTarget.localPosition = Vector3.Lerp(headBobTarget.localPosition, headStartPos, Time.deltaTime * 5f);
      //      bobTimer = 0f;
      //  }
   // }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
