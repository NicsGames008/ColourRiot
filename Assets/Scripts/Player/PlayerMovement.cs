using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float groundDrag = 5f;
    public float jumpForce = 8f;
    public float jumpCooldown = 0.2f;
    public float airMultiplier = 3f;

    private bool readyToJump = true;

    [Header("Jump Tuning")]
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 1.5f;
    public float maxFallSpeed = -20f;

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

    [Header("Animation")]
    public Animator animator;

    private PlayerState playerState;
    private float sceneSpeedMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerState = GetComponent<PlayerState>();
        rb.freezeRotation = true;

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Appartment")
        {
            sceneSpeedMultiplier = 0.5f;
        }

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

        MyInput();
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

        float percent = currentStamina / maxStamina;
        Color lowBlue = new Color(0.1f, 0.1f, 0.3f);
        Color fullBlue = new Color(0.2f, 0.4f, 0.8f);
        staminaFill.color = Color.Lerp(lowBlue, fullBlue, percent);
    }

    void FixedUpdate()
    {
        if (playerState.GetPlayerstate() == EPlayerState.Climbing)
        {
            ClimbLadder();
        }
        else if (playerState.GetPlayerstate() == EPlayerState.Moving)
        {
            MovePlayer();
            ApplyJumpGravity();
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isSprinting = Input.GetKey(sprintKey) && grounded && currentStamina > 0;

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded && Mathf.Abs(rb.velocity.y) < 0.2f && playerState.GetPlayerstate() == EPlayerState.Moving)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (verticalInput > 0)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);

        float currentSpeed = moveSpeed * sceneSpeedMultiplier;

        if (verticalInput < 0)
        {
            currentSpeed *= 2f / 3f;
        }

        if (isSprinting)
        {
            currentSpeed *= sprintMultiplier;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        transform.rotation = orientation.rotation;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized;

        Vector3 desiredMove = moveDirection * currentSpeed;

        if (grounded)
        {
            rb.velocity = new Vector3(desiredMove.x, rb.velocity.y, desiredMove.z);
        }
        else
        {
            rb.velocity = new Vector3(
                Mathf.Lerp(rb.velocity.x, desiredMove.x, airMultiplier * Time.fixedDeltaTime),
                rb.velocity.y,
                Mathf.Lerp(rb.velocity.z, desiredMove.z, airMultiplier * Time.fixedDeltaTime)
            );
        }
    }

    private void ClimbLadder()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector3(0f, vertical * 3f, 0f);

        if (vertical == 0)
            rb.velocity = Vector3.zero;

        if (vertical < 0)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Ladder"))
                {
                    Ladder ladder = hit.GetComponent<Ladder>();
                    if (ladder != null && ladder.IsAtBottom(transform.position))
                    {
                        rb.useGravity = true;
                        playerState.ChangePlayerState(EPlayerState.Moving);
                        Debug.Log("Dismounted at bottom");
                        break;
                    }
                }
            }
        }

        if (Input.GetKeyDown(jumpKey))
        {
            playerState.ChangePlayerState(EPlayerState.Moving);
            rb.useGravity = true;
        }
    }

    private void Jump()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = 0f;
        rb.velocity = velocity;
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
        else if (currentStamina < maxStamina && !Input.GetKey(sprintKey))
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
        if (FindObjectOfType<PlayerCheats>().GetUnlimitedStamina())
        {
            currentStamina = maxStamina;
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

            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, maxFallSpeed, rb.velocity.z);
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
