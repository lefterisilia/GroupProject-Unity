using UnityEngine;
using UnityEngine.Rendering;

public class MonsterFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    private bool shouldFollow = false;

    void Update()
    {
        if (shouldFollow && player != null)
        {
            // Move the PARENT of this object
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, player.position, followSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldFollow = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldFollow = false;
        }
    }
}