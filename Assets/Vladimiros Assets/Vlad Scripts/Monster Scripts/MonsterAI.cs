using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;
    private MonsterStunHandler stun;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        stun = GetComponent<MonsterStunHandler>();
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
                        isChasing = false; // Stop chasing after hit
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
                    MoveToLastPosition(player.position, playerController.IsSprinting);
                }
                break;

            case TriggerZone.TriggerType.Far:
                if (playerController.HasRecentlySprinted() && playerController.LastSprintPosition.HasValue)
                {
                    MoveToLastPosition(playerController.LastSprintPosition.Value, false);
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
                    Debug.Log("[MidTrigger] Stay: valid sound - going to last position");
                    MoveToLastPosition(playerController.transform.position, playerController.IsSprinting);
                }
                break;

            case TriggerZone.TriggerType.Far:
                if (!IsChasingPlayer() &&
                    playerController.HasRecentlySprinted() &&
                    playerController.LastSprintPosition.HasValue)
                {
                    MoveToLastPosition(playerController.LastSprintPosition.Value, false);
                }
                break;
        }
    }

    public void OnPlayerTriggerExit(TriggerZone.TriggerType type, Vector3 lastPosition)
    {
        if (type == TriggerZone.TriggerType.Mid && !isChasing && !stun.IsStunned())
        {
            // Double check if line of sight is valid before reacting
            if (HasLineOfSight(lastPosition))
            {
                Debug.Log("[MidTrigger Exit] Valid exit detection - moving to last known position");
                MoveToLastPosition(lastPosition, false);
            }
            else
            {
                Debug.Log("[MidTrigger Exit] Ignored - blocked by wall");
            }
        }
    }

    void StartChasing(Transform target)
    {
        isChasing = true;
        agent.speed = runSpeed;
        agent.SetDestination(target.position);
    }

    void MoveToLastPosition(Vector3 position, bool sprinted)
    {
        isChasing = false;
        agent.speed = sprinted ? runSpeed : walkSpeed;
        agent.SetDestination(position);
    }

    void Update()
    {
        if (isChasing && player != null && !stun.IsStunned())
        {
            agent.SetDestination(player.position);
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
