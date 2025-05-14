using UnityEngine;
using UnityEngine.AI;

public class Monster_Controller : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Movement Speeds")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 7f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    // Follow targets
    private Transform directTarget;
    private Vector3? walkStepTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
            GoToNextPoint();
    }

    void Update()
    {
        // 🔴 Direct follow takes priority
        if (directTarget != null)
        {
            agent.speed = runSpeed;
            agent.SetDestination(directTarget.position);
            return;
        }

        // 🟡 Walk step follow (if not directly chasing)
        if (walkStepTarget.HasValue)
        {
            agent.SetDestination(walkStepTarget.Value);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                walkStepTarget = null;
            }

            return;
        }

        // 🟢 Idle patrol
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.speed = walkSpeed;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    // 🔴 From Direct_Follow_Zone
    public void RequestDirectFollow(Transform player)
    {
        directTarget = player;
        walkStepTarget = null;
        agent.speed = runSpeed;
    }

    public void CancelDirectFollow()
    {
        directTarget = null;
    }

    // 🟡 From Walk_Step_Zone
    public void RequestWalkStepFollow(Vector3 position, bool isSprinting)
    {
        if (directTarget != null) return;

        walkStepTarget = position;
        agent.speed = isSprinting ? runSpeed : walkSpeed;
        agent.SetDestination(position);
    }

    public void CancelWalkStepFollow()
    {
        if (directTarget == null)
        {
            walkStepTarget = null;
        }
    }
}
