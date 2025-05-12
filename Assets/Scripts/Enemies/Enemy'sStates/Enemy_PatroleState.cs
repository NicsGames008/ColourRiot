using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Enemy patrol state that uses a custom graph-based waypoint system for patrolling,
// and Unity's NavMeshAgent to return to the patrol route after chasing or being off the graph.
public class Enemy_PatroleState : AStateBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrollingSpeed = 3f;
    [SerializeField] private float waypointReachThreshold = 0.2f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Waypoint Graph Reference")]
    [SerializeField] private WPManager graphManager = null;

    [Header("Audio")]
    [SerializeField] private AudioClip foundPlayerSound;
    [SerializeField] private AudioClip suspitionSound;

    // Component references
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private LineOfSight enemyLineOfSight;
    private NoiseDetection enemyNoiseDetection;

    // Noise system
    private GameObject[] noiseSources;
    private Transform susTagLocation;

    // Graph-based patrol tracking
    private int patrolIndex = 0;
    private List<Node> path = null;
    private int pathIndex = 0;
    private bool usingGraph = false;

    // Coroutine handle for managing patrol logic
    private Coroutine patrolCoroutine;

    // Called once when the state is initialized by the state machine.
    public override bool InitializeState()
    {
        // Cache necessary components
        agent = GetComponent<NavMeshAgent>();
        enemyLineOfSight = GetComponent<LineOfSight>();
        audioSource = GetComponent<AudioSource>();

        // Get all noise-detectable walls
        noiseSources = GameObject.FindGameObjectsWithTag("TaggableWall");

        // Return true only if core components are valid
        return agent != null && enemyLineOfSight != null;
    }

    // Called once when this state becomes active.
    public override void OnStateStart()
    {
        agent.isStopped = false;

        // Restart patrol coroutine if needed
        if (patrolCoroutine != null)
            StopCoroutine(patrolCoroutine);

        patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    // Called every frame while this state is active.
    public override void OnStateUpdate()
    {
        // If not on the graph and finished using NavMeshAgent, try to resume patrol
        if (!usingGraph && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (patrolCoroutine != null)
                StopCoroutine(patrolCoroutine);
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }

        // Check for noise and investigate
        foreach (var noiseGO in noiseSources)
        {
            enemyNoiseDetection = noiseGO.GetComponentInChildren<NoiseDetection>();

            foreach (Transform child in noiseGO.transform)
                if (child.name == "NoiseDetection")
                    susTagLocation = child;

            if (enemyNoiseDetection != null && enemyNoiseDetection.HasPoliceHeardTag(gameObject))
            {
                Debug.Log("Enemy heard a tag");
                agent.ResetPath();
                audioSource.PlayOneShot(suspitionSound);
                agent.SetDestination(susTagLocation.position);
            }
        }

        // Ensure the agent has the correct patrol speed
        SetAgentSpeed(patrollingSpeed);
    }

    // Called when this state ends.
    public override void OnStateEnd()
    {
        if (patrolCoroutine != null)
            StopCoroutine(patrolCoroutine);

        // Only stop if agent is active to avoid Unity error
        if (agent.enabled)
            agent.isStopped = true;

        // Ensure agent is enabled for future states that need it
        agent.enabled = true;
    }

    // Handles state transitions.
    public override int StateTransitionCondition()
    {
        if (enemyLineOfSight.HasSeenPlayerThisFrame())
        {
            audioSource.PlayOneShot(foundPlayerSound);
            return (int)EEnemyState.Chasing;
        }

        return (int)EEnemyState.Invalid;
    }

    // Patrol behavior using graph navigation and NavMeshAgent fallback.
    IEnumerator PatrolRoutine()
    {
        usingGraph = false;

        // Find the nearest waypoint from current position
        GameObject nearest = GetNearestWaypoint();
        float distanceToNearest = Vector3.Distance(transform.position, nearest.transform.position);

        // Use NavMeshAgent to return to patrol graph if far away
        if (distanceToNearest > 1.5f)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.SetDestination(nearest.transform.position);

            while (Vector3.Distance(transform.position, nearest.transform.position) > waypointReachThreshold)
                yield return null;
        }

        // Update patrol index to resume from nearest node
        patrolIndex = GetWaypointIndex(nearest);

        // Switch to manual movement using graph
        agent.enabled = false;
        usingGraph = true;

        while (true)
        {
            GameObject current = graphManager.waypoints[patrolIndex];
            GameObject next = graphManager.waypoints[(patrolIndex + 1) % graphManager.waypoints.Length];

            if (graphManager.graph.AStar(current, next))
            {
                path = graphManager.graph.pathList;
                pathIndex = 1;

                while (pathIndex < path.Count)
                {
                    Vector3 target = path[pathIndex].GetId().transform.position;

                    while (Vector3.Distance(transform.position, target) > waypointReachThreshold)
                    {
                        // Move toward the next node manually
                        transform.position = Vector3.MoveTowards(transform.position, target, patrollingSpeed * Time.deltaTime);

                        // Smoothly rotate toward movement direction
                        Vector3 dir = (target - transform.position).normalized;
                        if (dir.magnitude > 0.1f)
                        {
                            Quaternion lookRot = Quaternion.LookRotation(dir);
                            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
                        }

                        yield return null;
                    }

                    pathIndex++;
                }

                // Snap to final waypoint in path
                transform.position = next.transform.position;

                // Loop to next patrol target
                patrolIndex = (patrolIndex + 1) % graphManager.waypoints.Length;
            }
            else
            {
                Debug.LogWarning("Failed to find path between nodes.");
                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }
    }

    // Finds the index of a given waypoint GameObject.
    int GetWaypointIndex(GameObject node)
    {
        for (int i = 0; i < graphManager.waypoints.Length; i++)
        {
            if (graphManager.waypoints[i] == node)
                return i;
        }

        return 0;
    }

    // Returns the closest waypoint to the enemy's current position.
    GameObject GetNearestWaypoint()
    {
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject wp in graphManager.waypoints)
        {
            float dist = Vector3.Distance(transform.position, wp.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = wp;
            }
        }

        return nearest;
    }

    // Updates the NavMeshAgent's speed and adjusts rotation/acceleration accordingly.
    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;

        // Defaults for Unity NavMesh
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;

        float speedRatio = newSpeed / baseSpeed;
        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}
