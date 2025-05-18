using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public bool isInfinite = false;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var hit in hits)
        {
            FlashlightController flashlight = hit.GetComponentInChildren<FlashlightController>();
            if (flashlight != null && Input.GetKeyDown(interactKey))
            {
                flashlight.AddBattery();
                Debug.Log("[BatteryPickup] +1 Battery");

                if (!isInfinite)
                {
                    Destroy(gameObject);
                }
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isInfinite ? Color.cyan : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
