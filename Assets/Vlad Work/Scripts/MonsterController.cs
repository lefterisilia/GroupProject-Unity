using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    private NavMeshAgent agent;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    private Vector3? targetPosition = null;
    private Transform followTarget = null;
    private FollowPriority currentPriority = FollowPriority.None;

    public enum FollowPriority
    {
        None,
        SprintEcho,  // Lowest
        WalkStep,
        DirectFollow // Highest
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
    }

    void Update()
    {
        if (followTarget != null)
        {
            MoveTo(followTarget.position, runSpeed); // Chase directly
        }
        else if (targetPosition.HasValue)
        {
            MoveTo(targetPosition.Value, walkSpeed); // Follow last known
        }
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.speed = speed;
            agent.SetDestination(destination);
        }
    }

    public void RequestFollowTarget(Transform playerTransform, FollowPriority priority)
    {
        if (priority >= currentPriority)
        {
            followTarget = playerTransform;
            targetPosition = null;
            currentPriority = priority;
        }
    }

    public void RequestFollowPosition(Vector3 position, FollowPriority priority, float speedOverride = 0)
    {
        if (priority >= currentPriority)
        {
            followTarget = null;
            targetPosition = position;
            currentPriority = priority;

            if (priority == FollowPriority.SprintEcho && speedOverride > 0)
            {
                agent.speed = speedOverride;
            }
        }
    }

    public void ReleaseControl(FollowPriority priority)
    {
        if (priority == currentPriority)
        {
            currentPriority = FollowPriority.None;
            followTarget = null;
            targetPosition = null;

            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.ResetPath();
            }
        }
    }
}
