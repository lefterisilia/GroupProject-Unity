using UnityEngine;

public class SprintEchoZone : MonoBehaviour
{
    public float sprintDetectionSpeed = 10f;
    private MonsterController controller;

    void Start()
    {
        controller = GetComponentInParent<MonsterController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.IsSprinting)
        {
            // Update monster target position every frame while sprinting
            controller.RequestFollowPosition(
                other.transform.position,
                MonsterController.FollowPriority.SprintEcho,
                sprintDetectionSpeed
            );
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.ReleaseControl(MonsterController.FollowPriority.SprintEcho);
        }
    }
}
