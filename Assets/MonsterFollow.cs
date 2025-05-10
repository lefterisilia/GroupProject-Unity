using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public LayerMask obstacleLayers;

    private bool isPlayerInTrigger = false;

    void Update()
    {
        if (isPlayerInTrigger && player != null && HasLineOfSight())
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, player.position, followSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    bool HasLineOfSight()
    {
        Vector3 origin = transform.parent.position;
        Vector3 direction = player.position - origin;
        float distance = direction.magnitude;

        Debug.DrawLine(origin, player.position, Color.green); // Optional debug line

        RaycastHit hit;
        if (Physics.Raycast(origin, direction.normalized, out hit, distance))
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayers.value) != 0)
            {
                return false; // Line of sight blocked by obstacle
            }
        }

        return true; // Clear path to player
    }
}
