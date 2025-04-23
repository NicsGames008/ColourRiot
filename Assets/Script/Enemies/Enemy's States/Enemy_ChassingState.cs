using UnityEngine;
using UnityEngine.AI;

// Requires a NavMeshAgent to be attached to this GameObject
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_ChassingState : AStateBehaviour
{
    // Speed at which the enemy chases the player
    [SerializeField] private float chassingSpeed = 7f;


    // Sound played while the enemy is chasing the player
    [SerializeField] private AudioClip runningSound;

    // Cached component references
    private Transform playerTransform = null;
    private NavMeshAgent agent = null;
    private LineOfSight enemyLineOfSight = null;
    private AudioSource audioSource;

    // How long the enemy will continue to chase after losing sight of the player
    [SerializeField] private float timeToLooseInterest = 3.0f;

    // Internal timer for tracking how long the player has been out of sight
    private float timer = 0.0f;

    // Called once when this state is initialized
    public override bool InitializeState()
    {
        // Cache required components
        agent = GetComponent<NavMeshAgent>();
        enemyLineOfSight = GetComponent<LineOfSight>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        audioSource = GameObject.FindWithTag("MainCamera")?.GetComponent<AudioSource>();

        // Initialization fails if any required reference is missing
        if (agent == null || playerTransform == null || enemyLineOfSight == null)
            return false;

        return true;
    }

    // Called once when entering this state
    public override void OnStateStart()
    {
        // Reset chase timer
        timer = timeToLooseInterest;

        // Play the chase sound on loop
            audioSource.clip = runningSound;
            audioSource.loop = true;
            audioSource.Play();
    }

    // Called every frame while in this state
    public override void OnStateUpdate()
    {
        if (PauseMenu.gameIsPause)
            audioSource.pitch = 0f;
        else
            audioSource.pitch = 1.0f;

        // If the enemy sees the player this frame, keep chasing
        if (enemyLineOfSight.HasSeenPlayerThisFrame())
        {
            // Reset timer since the player is still in sight
            timer = timeToLooseInterest;

            // Move towards the player's current position
            agent.ResetPath();
            agent.SetDestination(playerTransform.position);

            // Apply chase movement characteristics
            SetAgentSpeed(chassingSpeed);
        }
        else
        {
            // Reduce the timer since the player is no longer visible
            timer -= Time.deltaTime;
        }
    }

    // Called once when exiting this state
    public override void OnStateEnd()
    {
        // Stop movement and clean up path
        agent.isStopped = true;
        agent.ResetPath();

        // Stop chase sound
        audioSource.Stop();
        audioSource.loop = false;
    }

    // Determines if the state should transition to a new one
    public override int StateTransitionCondition()
    {
        // If the player has been out of sight too long, return to patrolling
        if (timer < 0)
            return (int)EEnemyState.Patrolling;

        // Stay in this state otherwise
        return (int)EEnemyState.Invalid;
    }

    // Helper to adjust agent speed, turn rate, and acceleration based on a speed value
    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;

        // Reference values used for scaling
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;

        float speedRatio = newSpeed / baseSpeed;

        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}
