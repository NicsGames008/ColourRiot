using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_ChassingState : AStateBehaviour
{
    [SerializeField] private float chassingSpeed = 7f;
    [SerializeField] private Transform playerTransform = null;
    private NavMeshAgent agent = null;
    private LineOfSight monsterLineOfSight = null;

    [SerializeField] private float timeToLooseInterest = 3.0f;
    private float timer = 0.0f;

    public override bool InitializeState()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterLineOfSight = GetComponent<LineOfSight>();

        if (agent == null || playerTransform == null || monsterLineOfSight == null)
            return false;

        return true;
    }

    public override void OnStateStart()
    {
        timer = timeToLooseInterest;
    }

    public override void OnStateUpdate()
    {
        // Can We See The Player?
        if (monsterLineOfSight.HasSeenPlayerThisFrame())
        {
            // Reset Timer
            timer = timeToLooseInterest;

            agent.ResetPath();
            agent.SetDestination(playerTransform.position);
            SetAgentSpeed(chassingSpeed);

        }
        else
        {
            // TIck The Timer
            timer -= Time.deltaTime;
        }
    }

    public override void OnStateEnd()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public override int StateTransitionCondition()
    {
        if (timer < 0)
            return (int)EMonsterState.Patrolling;

        return (int)EMonsterState.Invalid;
    }

    void SetAgentSpeed(float newSpeed)
    {
        agent.speed = newSpeed;

        // Base reference values
        float baseSpeed = 3f;
        float baseAngularSpeed = 120f;
        float baseAcceleration = 8f;

        float speedRatio = newSpeed / baseSpeed;

        agent.angularSpeed = baseAngularSpeed * speedRatio;
        agent.acceleration = baseAcceleration * speedRatio;
    }
}
