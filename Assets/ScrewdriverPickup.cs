using UnityEngine;

public class ScrewdriverPickup : MonoBehaviour
{
    public string interactKey = "e";
    public float interactRange = 3f;
    public LayerMask playerLayer;

    private bool pickedUp = false;

    void Update()
    {
        if (pickedUp) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, playerLayer);
        foreach (var hit in hits)
        {
            if (Input.GetKeyDown(interactKey))
            {
                PlayerFixer fixer = hit.GetComponent<PlayerFixer>();
                if (fixer != null)
                {
                    fixer.GiveScrewdriver();
                    pickedUp = true;
                    Destroy(gameObject); // Remove screwdriver from scene
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
