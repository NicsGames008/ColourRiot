using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_ChassingState : AStateBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chassingSpeed = 7f;

    [Header("Audio")]
    [SerializeField] private AudioClip runningSound;

    [Header("Animation")]
    [SerializeField] private AnimationClip lookingAroundAnimation;

    // Component references
    private Transform playerTransform;
    private NavMeshAgent agent;
    private LineOfSight enemyLineOfSight;
    private AudioSource audioSource;
    private PlayerState playerState;
    private AnimationStateController animationController;

    // State tracking
    private Vector3 playerLastKnownPosition;
    private float timer;
    private bool isLookingAround;
    private bool hasReachedLastPosition;

    public override bool InitializeState()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyLineOfSight = GetComponent<LineOfSight>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        playerState = playerTransform.GetComponent<PlayerState>();
        audioSource = GetComponent<AudioSource>();
        animationController = GetComponent<AnimationStateController>();

        if (agent == null || playerTransform == null || enemyLineOfSight == null)
            return false;

        return true;
    }

    public override void OnStateStart()
    {
        timer = lookingAroundAnimation.length;
        isLookingAround = false;
        hasReachedLastPosition = false;
        playerLastKnownPosition = playerTransform.position;

        // Initialize movement
        agent.enabled = true;
        agent.isStopped = false;
        SetAgentSpeed(chassingSpeed);
        agent.SetDestination(playerLastKnownPosition);

        // Start running animation and sound
        animationController.SetBool("isRunning", true);
        animationController.SetBool("isLookingAround", false);
        audioSource.clip = runningSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    public override void OnStateUpdate()
    {
        if (playerState.GetPlayerstate() != EPlayerState.Moving)
            audioSource.pitch = 0f;
        else
            audioSource.pitch = 1.0f;

        // Always update the last known position if we can see the player
        if (enemyLineOfSight.HasSeenPlayerThisFrame())
        {
            playerLastKnownPosition = playerTransform.position;
            timer = lookingAroundAnimation.length;

            if (isLookingAround)
            {
                // If we were looking around but found the player again
                ResumeChasing();
            }
            else
            {
                // Continue normal chasing
                agent.SetDestination(playerLastKnownPosition);
            }
        }

        // Check if we've reached the last known position
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !hasReachedLastPosition)
        {
            hasReachedLastPosition = true;
            StartLookingAround();
        }

        // Handle looking around state
        if (isLookingAround)
        {
            timer -= Time.deltaTime;
        }
    }

    private void ResumeChasing()
    {
        isLookingAround = false;
        hasReachedLastPosition = false;

        animationController.SetBool("isLookingAround", false);
        animationController.SetBool("isRunning", true);

        audioSource.pitch = 1.0f;
        agent.isStopped = false;
        agent.SetDestination(playerLastKnownPosition);
    }

    private void StartLookingAround()
    {
        isLookingAround = true;
        agent.isStopped = true;

        animationController.SetBool("isRunning", false);
        animationController.SetBool("isLookingAround", true);

        audioSource.pitch = 0f; // Lower pitch or stop sound
    }

    public override void OnStateEnd()
    {
        agent.isStopped = true;
        agent.ResetPath();

        audioSource.Stop();
        audioSource.loop = false;

        animationController.SetBool("isRunning", false);
        animationController.SetBool("isLookingAround", false);
    }

    public override int StateTransitionCondition()
    {
        // Only transition back to patrolling if we've been looking around long enough
        if (isLookingAround && timer <= 0)
        {
            return (int)EEnemyState.Patrolling;
        }

        return (int)EEnemyState.Invalid;
    }

    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;
        float speedRatio = newSpeed / baseSpeed;
        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}