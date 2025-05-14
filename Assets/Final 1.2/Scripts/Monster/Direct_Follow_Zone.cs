using UnityEngine;

public class Direct_Follow_Zone : MonoBehaviour
{
    private Monster_Controller controller;

    void Start()
    {
        // Get the Monster_Controller on the parent object
        controller = GetComponentInParent<Monster_Controller>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start chasing the player
            controller.RequestDirectFollow(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop direct chasing
            controller.CancelDirectFollow();
        }
    }
}
