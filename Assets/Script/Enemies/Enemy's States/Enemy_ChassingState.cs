using UnityEngine;
using UnityEngine.AI;

// Ensure the GameObject has a NavMeshAgent component
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_ChassingState : AStateBehaviour
{
    // Speed at which the enemy chases the player
    [SerializeField] private float chassingSpeed = 7f;

    // Reference to the player’s transform (target)
    [SerializeField] private Transform playerTransform = null;

    // Cached references
    private NavMeshAgent agent = null;
    private LineOfSight monsterLineOfSight = null;

    // Time before the enemy gives up chasing if it loses sight of the player
    [SerializeField] private float timeToLooseInterest = 3.0f;

    // Countdown timer used to track time without seeing the player
    private float timer = 0.0f;

    // Called once when the state is initialized by the state machine
    public override bool InitializeState()
    {
        // Grab required components
        agent = GetComponent<NavMeshAgent>();
        monsterLineOfSight = GetComponent<LineOfSight>();

        // Make sure all required references are valid
        if (agent == null || playerTransform == null || monsterLineOfSight == null)
            return false;

        return true;
    }

    // Called when this state becomes active
    public override void OnStateStart()
    {
        // Reset the timer every time we enter this state
        timer = timeToLooseInterest;

    }

    // Called every frame while this state is active
    public override void OnStateUpdate()
    {
        // Check if the player is currently visible
        if (monsterLineOfSight.HasSeenPlayerThisFrame())
        {
            // Reset the timer because we still see the player
            timer = timeToLooseInterest;

            // Update the destination to chase the player
            agent.ResetPath(); // Clear current path first
            agent.SetDestination(playerTransform.position);

            // Update speed and turning based on chase speed
            SetAgentSpeed(chassingSpeed);
        }
        else
        {
            // Decrease the timer if the player is no longer visible
            timer -= Time.deltaTime;
        }
    }

    // Called when exiting this state
    public override void OnStateEnd()
    {
        // Stop the agent and clear its path when leaving this state
        agent.isStopped = true;
        agent.ResetPath();
    }

    // Determines whether to switch to another state
    public override int StateTransitionCondition()
    {
        // If the player has not been seen for the entire timer duration, go back to patrolling
        if (timer < 0)
            return (int)EEnemyState.Patrolling;

        // Otherwise, stay in the current state
        return (int)EEnemyState.Invalid;
    }

    // Helper method to scale movement-related properties based on the agent's speed
    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;

        // Base speed values to scale from
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;

        // Determine ratio based on new speed and apply it
        float speedRatio = newSpeed / baseSpeed;

        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}
