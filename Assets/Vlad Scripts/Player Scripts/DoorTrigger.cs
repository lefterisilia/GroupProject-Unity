using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("Open", true); // Triggers open
            Debug.Log("Player entered trigger - door opening.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("Open", false); // Triggers close
            Debug.Log("Player exited trigger - door closing.");
        }
    }
}
