using UnityEngine;

public class SprintFollowTrigger : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public LayerMask obstacleLayers;

    private bool isPlayerInTrigger = false;
    private PlayerController playerController;

    void Update()
    {
        if (isPlayerInTrigger && player != null && playerController != null)
        {
            if (playerController.IsSprinting && HasLineOfSight())
            {
                // Only move if sprinting AND visible
                transform.parent.position = Vector3.MoveTowards(transform.parent.position, player.position, followSpeed * Time.deltaTime);
            }
        }
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
        }
    }

    bool HasLineOfSight()
    {
        Vector3 origin = transform.parent.position;
        Vector3 direction = player.position - origin;
        float distance = direction.magnitude;

        Debug.DrawLine(origin, player.position, Color.red); // Optional debug

        RaycastHit hit;
        if (Physics.Raycast(origin, direction.normalized, out hit, distance))
        {
            // Check if hit object is on an obstacle layer
            if (((1 << hit.collider.gameObject.layer) & obstacleLayers.value) != 0)
            {
                return false; // Blocked by obstacle
            }
        }

        return true; // No obstacle
    }
}
