using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private Animator doorAnimator;

    private void Awake()
    {
        // Find the Animator component in children (like the Door object inside the prefab)
        doorAnimator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("Open", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("Open", false);
        }
    }
}