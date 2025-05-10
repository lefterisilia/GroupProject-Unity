using UnityEngine;

public class SprintFollowTrigger : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    private bool shouldFollow = false;
    private PlayerController playerController;

    void Update()
    {
        if (shouldFollow && player != null && playerController != null && playerController.IsSprinting)
        {
            // Move the parent (enemy) only if player is sprinting
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, player.position, followSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                shouldFollow = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldFollow = false;
            playerController = null;
        }
    }
}
