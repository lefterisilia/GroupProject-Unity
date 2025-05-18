using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public int keyNumber; // Set this in Inspector
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var hit in hits)
        {
            PlayerKeyInventory inventory = hit.GetComponent<PlayerKeyInventory>();
            if (inventory != null && Input.GetKeyDown(interactKey))
            {
                inventory.AddKey(keyNumber);
                Destroy(gameObject);
                Debug.Log($"[KeyPickup] Picked up Key {keyNumber}");
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
