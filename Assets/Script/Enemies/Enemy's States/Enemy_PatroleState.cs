using UnityEngine;
using UnityEngine.AI;

// This state handles patrolling behavior for an enemy using Unity's NavMesh system
public class Enemy_PatroleState : AStateBehaviour
{
    // Patrol movement speed
    [SerializeField] private float patrollingspeed = 3f;

    // Reference to the waypoint manager that provides patrol points
    [SerializeField] private WayPointManager waypointManager = null;

    // Tracks which WayPoint index the enemy was last assigned
    private int lastWayPointRequested = 0;

    // References to required components
    private NavMeshAgent agent = null;
    private LineOfSight enemyLineOfSight = null;
    private GameObject[] enemyNoiseDetectionGO = null;
    private NoiseDetection enemyNoiseDetection = null;

    private Transform susTagLocation = null;

    // Called once to initialize the state. Fails if required components are missing.
    public override bool InitializeState()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyLineOfSight = GetComponent<LineOfSight>();

        enemyNoiseDetectionGO = GameObject.FindGameObjectsWithTag("TaggableWall");

        // Make sure both components exist
        if (agent == null || enemyLineOfSight == null)
            return false;

        return true;
    }

    // Called when this state begins
    public override void OnStateStart()
    {
        // Reset the patrol index if it's out of bounds
        if (!waypointManager.IsIndexValid(lastWayPointRequested))
        {
            lastWayPointRequested = 0;
        }

        // Get the patrol target and set the NavMeshAgent destination
        Transform poiTransform = waypointManager.GetwaypointsAtIndex(lastWayPointRequested);
        if (poiTransform)
        {
            agent.isStopped = false;
            agent.SetDestination(poiTransform.position);


            // Increment index for next point
            lastWayPointRequested++;
        }
    }

    // Called once per frame while this state is active
    public override void OnStateUpdate()
    {
        // If the agent has reached its current destination, move to the next point
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!waypointManager.IsIndexValid(lastWayPointRequested))
            {
                lastWayPointRequested = 0;
            }

            Transform poiTransform = waypointManager.GetwaypointsAtIndex(lastWayPointRequested);
            if (poiTransform)
            {
                agent.isStopped = false;
                agent.SetDestination(poiTransform.position);

                lastWayPointRequested++;
            }
        }


        foreach (var noiseDetectionGO in enemyNoiseDetectionGO)
        {
            enemyNoiseDetection = noiseDetectionGO.GetComponentInChildren<NoiseDetection>();


            // Log all children
            foreach (Transform child in noiseDetectionGO.transform)
            {
                if (child != null && child.name == "NoiseDetection")
                {
                    susTagLocation = child;
                }
            }

            if (enemyNoiseDetection != null && enemyNoiseDetection.HasPoliceHeardTag(gameObject))
            {
                Debug.Log("Has heard a tag");
                agent.ResetPath();
                agent.SetDestination(susTagLocation.position);
            }
        }
        // Set the agent's movement characteristics based on patrol speed
        SetAgentSpeed(patrollingspeed);
    }

    // Called when this state ends
    public override void OnStateEnd()
    {
        // Stop movement when leaving this state
        agent.isStopped = true;
    }

    // Determines if this state should transition to another
    public override int StateTransitionCondition()
    {
        // If the player has been seen, transition to chasing
        if (enemyLineOfSight.HasSeenPlayerThisFrame())
        {
            return (int)EEnemyState.Chasing;
        }


        // Otherwise, remain in this state
        return (int)EEnemyState.Invalid;
    }

    // Helper method to apply speed-based adjustments to the NavMeshAgent
    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;

        // These values represent the default baseline behavior
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;

        // Scale angular speed and acceleration based on the speed ratio
        float speedRatio = newSpeed / baseSpeed;

        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}
