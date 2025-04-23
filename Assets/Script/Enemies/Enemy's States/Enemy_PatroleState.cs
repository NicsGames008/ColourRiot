using UnityEngine;
using UnityEngine.AI;

// Handles patrol behavior for an enemy using Unity's NavMesh system
public class Enemy_PatroleState : AStateBehaviour
{
    // Speed at which the enemy moves while patrolling
    [SerializeField] private float patrollingspeed = 3f;

    // Reference to a waypoint manager that provides patrol points
    [SerializeField] private WayPointManager waypointManager = null;

    // Sound clips for suspicion and spotting the player
    [SerializeField] private AudioClip foundPlayerSound;
    [SerializeField] private AudioClip suspitionSound;

    // Tracks the last patrol waypoint index requested
    private int lastWayPointRequested = 0;

    // Cached references
    private NavMeshAgent agent = null;
    private LineOfSight enemyLineOfSight = null;
    private GameObject[] enemyNoiseDetectionGO = null;
    private NoiseDetection enemyNoiseDetection = null;
    private Transform susTagLocation = null;

    // For playing audio clips
    private AudioSource audioSource;

    // Called once when the state is initialized by the state machine
    public override bool InitializeState()
    {
        // Cache required components
        agent = GetComponent<NavMeshAgent>();
        enemyLineOfSight = GetComponent<LineOfSight>();
        audioSource = GameObject.FindWithTag("MainCamera")?.GetComponent<AudioSource>();

        // Find all objects tagged as "TaggableWall" (used for noise detection)
        enemyNoiseDetectionGO = GameObject.FindGameObjectsWithTag("TaggableWall");

        // Initialization fails if required components are missing
        if (agent == null || enemyLineOfSight == null)
            return false;

        return true;
    }

    // Called once when this state becomes active
    public override void OnStateStart()
    {
        // Reset patrol index if it's invalid
        if (!waypointManager.IsIndexValid(lastWayPointRequested))
            lastWayPointRequested = 0;

        // Move to the first valid patrol point
        Transform poiTransform = waypointManager.GetwaypointsAtIndex(lastWayPointRequested);
        if (poiTransform)
        {
            agent.isStopped = false;
            agent.SetDestination(poiTransform.position);
            lastWayPointRequested++;
        }
    }

    // Called every frame while this state is active
    public override void OnStateUpdate()
    {
        // If the agent has reached its destination, move to the next patrol point
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!waypointManager.IsIndexValid(lastWayPointRequested))
                lastWayPointRequested = 0;

            Transform poiTransform = waypointManager.GetwaypointsAtIndex(lastWayPointRequested);
            if (poiTransform)
            {
                agent.isStopped = false;
                agent.SetDestination(poiTransform.position);
                lastWayPointRequested++;
            }
        }

        // Check all tagged noise sources for suspicion triggers
        foreach (var noiseDetectionGO in enemyNoiseDetectionGO)
        {
            enemyNoiseDetection = noiseDetectionGO.GetComponentInChildren<NoiseDetection>();

            // Find the "NoiseDetection" child transform
            foreach (Transform child in noiseDetectionGO.transform)
            {
                if (child != null && child.name == "NoiseDetection")
                    susTagLocation = child;
            }

            // If the enemy detects noise it cares about, investigate
            if (enemyNoiseDetection != null && enemyNoiseDetection.HasPoliceHeardTag(gameObject))
            {
                Debug.Log("Enemy heard a tag");
                agent.ResetPath();
                audioSource.PlayOneShot(suspitionSound);
                agent.SetDestination(susTagLocation.position);
            }
        }

        // Continuously apply patrol speed to the agent
        SetAgentSpeed(patrollingspeed);
    }

    // Called once when this state ends
    public override void OnStateEnd()
    {
        // Stop the agent when leaving this state
        agent.isStopped = true;
    }

    // Determines if the state should transition to a new one
    public override int StateTransitionCondition()
    {
        // If the enemy sees the player, play a sound and switch to chasing
        if (enemyLineOfSight.HasSeenPlayerThisFrame())
        {
            audioSource.PlayOneShot(foundPlayerSound);
            return (int)EEnemyState.Chasing;
        }

        // Stay in patrol state by default
        return (int)EEnemyState.Invalid;
    }

    // Helper method to update NavMeshAgent's movement characteristics
    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;

        // Reference speed values
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;

        // Adjust turn rate and acceleration relative to speed
        float speedRatio = newSpeed / baseSpeed;
        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}
