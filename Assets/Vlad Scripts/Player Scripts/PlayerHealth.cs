using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 2;
    public Transform respawnPoint;

    private int currentHits = 0;
    private CharacterController controller;
    private bool isStunned = false;

    void Start()
    {
        currentHits = 0;
        controller = GetComponent<CharacterController>();
    }

    public void TakeHit()
    {
        if (isStunned) return;

        currentHits++;

        if (currentHits < maxHits)
        {
            Debug.Log("[Player] Hit 1 - still alive");
            // Optional: feedback/effect
        }
        else
        {
            Debug.Log("[Player] Hit 2 - respawning");
            StartCoroutine(RespawnRoutine());
        }
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        isStunned = true;

        controller.enabled = false;
        transform.position = respawnPoint.position;
        yield return null; // one frame delay
        controller.enabled = true;

        yield return new WaitForSeconds(5f); // stunned time
        isStunned = false;
        currentHits = 0;

        Debug.Log("[Player] Respawn finished");
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
