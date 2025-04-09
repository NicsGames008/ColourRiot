using UnityEngine;
using UnityEngine.UI;



public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool grounded;
    bool wasGrounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    float lastYVelocity;
    bool isSprinting;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float staminaSprintDrain = 20f;
    public float fallStaminaDrainMultiplier = 2f;
    public float sprintMultiplier = 1.5f;

    [Header("UI")]
    public Slider StaminaBar;

    
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentStamina = maxStamina;
        StaminaBar.maxValue = maxStamina;
        StaminaBar.value = currentStamina;

        
        
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        SpeedControl();
        HandleStamina();
        HandleFallStaminaDrain();

        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;

        wasGrounded = grounded;
        lastYVelocity = rb.velocity.y;
        
        StaminaBar.value = currentStamina;

        
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

        if (flatVel.magnitude > moveSpeed * (isSprinting ? sprintMultiplier : 1f))
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed * (isSprinting ? sprintMultiplier : 1f);
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
        if (isSprinting)
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

            if (fallImpact > 10f) 
            {
                float staminaDamage = fallImpact * fallStaminaDrainMultiplier;
                currentStamina -= staminaDamage;
                currentStamina = Mathf.Max(currentStamina, 0f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
