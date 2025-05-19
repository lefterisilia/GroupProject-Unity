using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    
    [Header("Stuck Detection")]
    public float stuckDistanceThreshold = 0.1f;
    public float stuckTimeThreshold = 2f;
    public float unstuckDistance = 1f;
    public float unstuckAngle = 45f;

    [Header("Chase Settings")]
    public float chaseResumeDelay = 2f;
    public float maxChaseDistance = 100f;
    private float lastChaseTime;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;
    private MonsterStunHandler stun;
    private Animator animator;
    
    // Stuck detection variables
    private Vector3 lastPosition;
    private float stuckTimer;
    private bool isStuck = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        stun = GetComponent<MonsterStunHandler>();
        animator = GetComponent<Animator>();
        
        // Configure NavMeshAgent for better navigation
        agent.acceleration = 15f;
        agent.angularSpeed = 240f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.radius = 0.6f;
        agent.height = 2.2f;
        agent.baseOffset = 0.1f;
        
        lastPosition = transform.position;
    }

    public void OnPlayerTriggerEnter(TriggerZone.TriggerType type, PlayerController playerController)
    {
        if (stun.IsStunned()) return;

        player = playerController.transform;

        switch (type)
        {
            case TriggerZone.TriggerType.Close:
                Debug.Log("[CloseTrigger] Player very close");

                PlayerHealth health = playerController.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    if (!health.IsStunned() && !stun.IsStunned())
                    {
                        health.TakeHit();
                        stun.TriggerStun();
                        isChasing = false;
                    }
                    else if (!health.IsStunned() && !isChasing && !stun.IsStunned())
                    {
                        Debug.Log("[CloseTrigger] Re-detected player — resuming chase");
                        StartChasing(player);
                    }
                }
                break;

            case TriggerZone.TriggerType.Mid:
                if (!playerController.IsCrouching && HasLineOfSight(player.position))
                {
                    StartChasing(player);
                }
                break;

            case TriggerZone.TriggerType.Far:
                if (playerController.HasRecentlySprinted() || HasLineOfSight(player.position))
                {
                    StartChasing(player);
                }
                break;
        }
    }

    public void OnPlayerTriggerStay(TriggerZone.TriggerType type, PlayerController playerController)
    {
        if (stun.IsStunned()) return;

        switch (type)
        {
            case TriggerZone.TriggerType.Mid:
                if (!IsChasingPlayer() &&
                    !playerController.IsCrouching &&
                    HasLineOfSight(playerController.transform.position))
                {
                    Debug.Log("[MidTrigger] Stay: detected player - starting chase");
                    StartChasing(playerController.transform);
                }
                break;

            case TriggerZone.TriggerType.Far:
                if (!IsChasingPlayer() &&
                    (playerController.HasRecentlySprinted() || HasLineOfSight(playerController.transform.position)))
                {
                    StartChasing(playerController.transform);
                }
                break;
        }
    }

    public void OnPlayerTriggerExit(TriggerZone.TriggerType type, Vector3 lastPosition)
    {
        if (type == TriggerZone.TriggerType.Mid && !isChasing && !stun.IsStunned())
        {
            if (HasLineOfSight(lastPosition))
            {
                Debug.Log("[MidTrigger Exit] Valid exit detection - starting chase");
                StartChasing(player);
            }
        }
    }

    void StartChasing(Transform target)
    {
        isChasing = true;
        lastChaseTime = Time.time;
        agent.speed = runSpeed;
        agent.SetDestination(target.position);
    }

    void MoveToLastPosition(Vector3 position, bool sprinted)
    {
        if (player != null && HasLineOfSight(player.position))
        {
            StartChasing(player);
            return;
        }

        isChasing = false;
        agent.speed = sprinted ? runSpeed : walkSpeed;
        agent.SetDestination(position);
    }

    void Update()
    {
        if (stun.IsStunned()) 
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            return;
        }

        // Check for stuck condition
        if (!isStuck && !agent.pathPending)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved < stuckDistanceThreshold)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= stuckTimeThreshold)
                {
                    isStuck = true;
                    HandleStuckSituation();
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }
        
        lastPosition = transform.position;

        // Update animator speed parameter based on actual movement
        if (animator != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }

        // Enhanced chase behavior
        if (isChasing && player != null && !stun.IsStunned())
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer <= maxChaseDistance || Time.time - lastChaseTime < chaseResumeDelay)
            {
                agent.SetDestination(player.position);
                
                if (isStuck && agent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    TryFindAlternativePath();
                }
            }
            else
            {
                isChasing = false;
            }
        }
    }

    void HandleStuckSituation()
    {
        Debug.Log("[Monster] Detected stuck situation - attempting recovery");
        
        // Try to move slightly back and to the side
        Vector3 randomDirection = Quaternion.Euler(0, Random.Range(-unstuckAngle, unstuckAngle), 0) * -transform.forward;
        Vector3 unstuckPosition = transform.position + randomDirection * unstuckDistance;
        
        // Sample a valid position on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(unstuckPosition, out hit, unstuckDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            agent.isStopped = false;
            isStuck = false;
            stuckTimer = 0f;
        }
        else
        {
            // If we can't find a valid position, try to teleport slightly up
            transform.position += Vector3.up * 0.1f;
            agent.ResetPath();
            isStuck = false;
            stuckTimer = 0f;
        }
    }

    void TryFindAlternativePath()
    {
        if (player == null) return;

        // Try to find a path with a larger radius
        NavMeshPath path = new NavMeshPath();
        float radius = 2f;
        Vector3 randomOffset = Random.insideUnitSphere * radius;
        randomOffset.y = 0;
        
        if (NavMesh.CalculatePath(transform.position, player.position + randomOffset, NavMesh.AllAreas, path))
        {
            agent.SetPath(path);
            isStuck = false;
            stuckTimer = 0f;
        }
    }

    public bool IsChasingPlayer() => isChasing;
    public bool WasRecentlyChasing() => isChasing;

    bool HasLineOfSight(Vector3 targetPos)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (targetPos - origin).normalized;
        float dist = Vector3.Distance(origin, targetPos);
        return !Physics.Raycast(origin, dir, dist, LayerMask.GetMask("Wall"));
    }
}
