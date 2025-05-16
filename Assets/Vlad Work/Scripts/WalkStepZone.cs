using UnityEngine;

public class WalkStepZone : MonoBehaviour
{
    public LayerMask obstacleLayers;         // Assign "Wall" layer in Inspector
    public float raycastHeightOffset = 1.0f; // Used to cast ray from chest height

    private MonsterController controller;
    private Vector3? lastSeenWalkPosition;
    private bool playerWasInZone = false;

    void Start()
    {
        controller = GetComponentInParent<MonsterController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || player.IsCrouching) return;

        Vector3 playerPos = other.transform.position + Vector3.up * raycastHeightOffset;

        if (HasLineOfSight(playerPos))
        {
            lastSeenWalkPosition = playerPos;

            float speed = player.IsSprinting ? controller.runSpeed : controller.walkSpeed;
            controller.RequestFollowPosition(lastSeenWalkPosition.Value, MonsterController.FollowPriority.WalkStep, speed);

            playerWasInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (lastSeenWalkPosition.HasValue && HasLineOfSight(lastSeenWalkPosition.Value))
        {
            controller.RequestFollowPosition(lastSeenWalkPosition.Value, MonsterController.FollowPriority.WalkStep);
        }
        else
        {
            lastSeenWalkPosition = null; // Don't follow through walls
        }

        playerWasInZone = false;
    }

    void Update()
    {
        if (!playerWasInZone && lastSeenWalkPosition.HasValue)
        {
            float distance = Vector3.Distance(controller.transform.position, lastSeenWalkPosition.Value);
            if (distance <= 0.1f)
            {
                lastSeenWalkPosition = null;
                controller.ReleaseControl(MonsterController.FollowPriority.WalkStep);
            }
        }
    }

    bool HasLineOfSight(Vector3 targetPoint)
    {
        Vector3 origin = controller.transform.position + Vector3.up * raycastHeightOffset;
        Vector3 direction = targetPoint - origin;
        float distance = direction.magnitude;

        Debug.DrawLine(origin, targetPoint, Color.red);

        // Raycast against only the obstacle layers (e.g., Wall)
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, obstacleLayers))
        {
            Debug.Log("Blocked by: " + hit.collider.name + " | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            return false; // Line of sight is blocked by a wall
        }

        return true; // Clear path to player
    }
}
