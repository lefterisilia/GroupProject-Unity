using UnityEngine;

public class Walk_Step_Zone : MonoBehaviour
{
    public LayerMask obstacleLayers;
    public float raycastHeightOffset = 0.5f;

    private Monster_Controller controller;
    private Vector3? lastSeenPosition;
    private bool playerInZone = false;

    void Start()
    {
        controller = GetComponentInParent<Monster_Controller>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || player.IsCrouching) return;

        Vector3 playerPos = other.transform.position + Vector3.up * raycastHeightOffset;
        Vector3 origin = controller.transform.position + Vector3.up * raycastHeightOffset;
        Vector3 dir = playerPos - origin;

        if (!Physics.Raycast(origin, dir.normalized, dir.magnitude, obstacleLayers))
        {
            lastSeenPosition = playerPos;
            controller.RequestWalkStepFollow(playerPos, player.IsSprinting);
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = false;

        if (lastSeenPosition.HasValue)
        {
            controller.RequestWalkStepFollow(lastSeenPosition.Value, false);
        }
        else
        {
            controller.CancelWalkStepFollow();
        }
    }

    void Update()
    {
        if (!playerInZone && lastSeenPosition.HasValue)
        {
            float distance = Vector3.Distance(controller.transform.position, lastSeenPosition.Value);
            if (distance <= 0.1f)
            {
                lastSeenPosition = null;
                controller.CancelWalkStepFollow();
            }
        }
    }
}
