using UnityEngine;

public class DirectFollowZone : MonoBehaviour
{
    private MonsterController controller;

    void Start()
    {
        controller = GetComponentInParent<MonsterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.RequestFollowTarget(other.transform, MonsterController.FollowPriority.DirectFollow);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.ReleaseControl(MonsterController.FollowPriority.DirectFollow);
        }
    }
}
