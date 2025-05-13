using UnityEngine;

public class SprintFollowTrigger : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public LayerMask obstacleLayers;

    private bool isPlayerInTrigger = false;
    private PlayerController playerController;
    private Vector3? lastSeenPosition = null;

    void Update()
    {
        if (isPlayerInTrigger && player != null && playerController != null)
        {
            bool canSeePlayer = HasLineOfSight();

            if (playerController.IsSprinting && canSeePlayer)
            {
                lastSeenPosition = player.position;
                MoveToPosition(player.position);
            }
            else if (lastSeenPosition.HasValue)
            {
                float distance = Vector3.Distance(transform.parent.position, lastSeenPosition.Value);

                if (distance > 0.1f)
                {
                    MoveToPosition(lastSeenPosition.Value);
                }
                else
                {
                    // Stop chasing when reached last position and can't see player
                    if (!canSeePlayer)
                    {
                        lastSeenPosition = null; // Release control
                    }
                }
            }
        }
    }

    void MoveToPosition(Vector3 target)
    {
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, followSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                isPlayerInTrigger = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            playerController = null;
            lastSeenPosition = null; // Forget if player leaves zone
        }
    }

    bool HasLineOfSight()
    {
        Vector3 origin = transform.parent.position;
        Vector3 direction = player.position - origin;
        float distance = direction.magnitude;

        Debug.DrawLine(origin, player.position, Color.red);

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance))
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayers.value) != 0)
            {
                return false; // Something is in the way
            }
        }

        return true;
    }
}
