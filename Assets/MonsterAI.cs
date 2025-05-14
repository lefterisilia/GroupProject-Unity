using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
    }

    public void OnPlayerTriggerEnter(TriggerZone.TriggerType type, PlayerController playerController)
    {
        player = playerController.transform;

        switch (type)
        {
            case TriggerZone.TriggerType.Close:
                Debug.Log("[CloseTrigger] Chasing player");
                StartChasing(player);
                break;

            case TriggerZone.TriggerType.Mid:
                if (!playerController.IsCrouching && HasLineOfSight(player.position))
                {
                    Debug.Log($"[MidTrigger] Player detected (sprinted: {playerController.IsSprinting})");
                    MoveToLastPosition(player.position, playerController.IsSprinting);
                }
                else
                {
                    Debug.Log("[MidTrigger] Ignored - blocked or crouching");
                }
                break;

            case TriggerZone.TriggerType.Far:
                if (playerController.HasRecentlySprinted() && playerController.LastSprintPosition.HasValue)
                {
                    Debug.Log("[FarTrigger] Sprint detected recently - moving to last sprint position");
                    MoveToLastPosition(playerController.LastSprintPosition.Value, false);
                }
                else
                {
                    Debug.Log("[FarTrigger] Ignored - no recent sprint");
                }
                break;
        }
    }

    public void OnPlayerTriggerStay(TriggerZone.TriggerType type, PlayerController playerController)
    {
        switch (type)
        {
            case TriggerZone.TriggerType.Mid:
                if (!playerController.IsCrouching && !isChasing && HasLineOfSight(playerController.transform.position))
                {
                    MoveToLastPosition(playerController.transform.position, playerController.IsSprinting);
                }
                break;

            case TriggerZone.TriggerType.Far:
                if (playerController.HasRecentlySprinted() &&
                    playerController.LastSprintPosition.HasValue &&
                    !isChasing)
                {
                    MoveToLastPosition(playerController.LastSprintPosition.Value, false);
                }
                break;
        }
    }

    public void OnPlayerTriggerExit(TriggerZone.TriggerType type, Vector3 lastPosition)
    {
        if (type == TriggerZone.TriggerType.Mid && !isChasing)
        {
            Debug.Log("[MidTrigger Exit] Player exited - moving to last known position");
            MoveToLastPosition(lastPosition, false);
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
        if (isChasing && player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    bool HasLineOfSight(Vector3 targetPos)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (targetPos - origin).normalized;
        float dist = Vector3.Distance(origin, targetPos);

        return !Physics.Raycast(origin, dir, dist, LayerMask.GetMask("Wall"));
    }

    public bool IsChasingPlayer()
    {
        return isChasing;
    }

    public bool WasRecentlyChasing()
    {
        // Optional: improve with timestamp if needed
        return isChasing; // Simple way for now
    }

}