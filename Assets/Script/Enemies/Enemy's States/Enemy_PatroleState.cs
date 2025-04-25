using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_PatroleState : AStateBehaviour
{
    [SerializeField] private float patrollingspeed = 3f;
    [SerializeField] private WPManager graphManager = null;
    [SerializeField] private AudioClip foundPlayerSound;
    [SerializeField] private AudioClip suspitionSound;
    [SerializeField] private float waypointReachThreshold = 0.2f;
    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private LineOfSight enemyLineOfSight;
    private NoiseDetection enemyNoiseDetection;

    private GameObject[] noiseSources;
    private Transform susTagLocation;

    private int patrolIndex = 0;
    private List<Node> path = null;
    private int pathIndex = 0;
    private bool usingGraph = false;

    private Coroutine patrolCoroutine;

    public override bool InitializeState()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyLineOfSight = GetComponent<LineOfSight>();
        audioSource = GetComponent<AudioSource>();
        noiseSources = GameObject.FindGameObjectsWithTag("TaggableWall");

        return agent != null && enemyLineOfSight != null;
    }

    public override void OnStateStart()
    {
        agent.isStopped = false;

        if (patrolCoroutine != null)
            StopCoroutine(patrolCoroutine);

        patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    public override void OnStateUpdate()
    {
        // NavMesh movement fallback
        if (!usingGraph && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (patrolCoroutine != null)
                StopCoroutine(patrolCoroutine);
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }

        // Check for noise
        foreach (var noiseGO in noiseSources)
        {
            enemyNoiseDetection = noiseGO.GetComponentInChildren<NoiseDetection>();
            foreach (Transform child in noiseGO.transform)
                if (child.name == "NoiseDetection") susTagLocation = child;

            if (enemyNoiseDetection != null && enemyNoiseDetection.HasPoliceHeardTag(gameObject))
            {
                Debug.Log("Enemy heard a tag");
                agent.ResetPath();
                audioSource.PlayOneShot(suspitionSound);
                agent.SetDestination(susTagLocation.position);
            }
        }

        SetAgentSpeed(patrollingspeed);
    }
    public override void OnStateEnd()
    {
        if (patrolCoroutine != null)
            StopCoroutine(patrolCoroutine);

        if (agent.enabled)
        {
            agent.isStopped = true;
        }

        agent.enabled = true; // Always enable it for the next state (like chasing)
    }



    public override int StateTransitionCondition()
    {
        if (enemyLineOfSight.HasSeenPlayerThisFrame())
        {
            audioSource.PlayOneShot(foundPlayerSound);
            return (int)EEnemyState.Chasing;
        }
        return (int)EEnemyState.Invalid;
    }

    IEnumerator PatrolRoutine()
    {
        // Ensure state starts clean
        usingGraph = false;

        GameObject nearest = GetNearestWaypoint();
        float distanceToNearest = Vector3.Distance(transform.position, nearest.transform.position);

        // If far from graph, walk to it using NavMesh
        if (distanceToNearest > 1.5f)
        {
            agent.enabled = true; // ✅ Ensure NavMesh is active
            agent.isStopped = false;
            agent.SetDestination(nearest.transform.position);

            while (Vector3.Distance(transform.position, nearest.transform.position) > waypointReachThreshold)
                yield return null;
        }

        // Now on a waypoint — update patrolIndex to match the closest node
        patrolIndex = GetWaypointIndex(nearest);

        // Start patrol on graph
        agent.enabled = false; // ✅ Disable NavMeshAgent before manual movement
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
                        // Move manually
                        transform.position = Vector3.MoveTowards(transform.position, target, patrollingspeed * Time.deltaTime);

                        // Rotate smoothly toward movement direction
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

                transform.position = next.transform.position;
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


    int GetWaypointIndex(GameObject node)
    {
        for (int i = 0; i < graphManager.waypoints.Length; i++)
        {
            if (graphManager.waypoints[i] == node)
                return i;
        }
        return 0;
    }


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
