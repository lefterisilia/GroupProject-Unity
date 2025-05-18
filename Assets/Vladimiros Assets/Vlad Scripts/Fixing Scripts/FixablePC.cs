using UnityEngine;

public class FixablePC : MonoBehaviour
{
    public string interactKey = "e";
    public float interactRange = 3f;
    public LayerMask playerLayer;
    public FixMinigame fixMinigame;

    private bool isFixed = false;
    private bool isFixing = false;

    void Update()
    {
        if (isFixed || isFixing) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, playerLayer);
        foreach (var hit in hits)
        {
            if (Input.GetKeyDown(interactKey))
            {
                PlayerFixer fixer = hit.GetComponent<PlayerFixer>();
                if (fixer != null && fixer.hasScrewdriver)
                {
                    isFixing = true;
                    LaunchFixMinigame(fixer.transform);
                }
            }
        }
    }

    void LaunchFixMinigame(Transform playerTransform)
    {
        if (fixMinigame != null)
        {
            fixMinigame.StartMinigame(OnFixMinigameResult, playerTransform, transform, interactRange);
        }
        else
        {
            Debug.LogWarning("[FixablePC] No FixMinigame assigned!");
            isFixing = false;
        }
    }

    void OnFixMinigameResult(bool success)
    {
        if (success)
        {
            isFixed = true;
            FixManager.Instance.RegisterFix();
            Debug.Log("[PC] Fix successful!");
        }
        else
        {
            Debug.Log("[PC] Fix failed or cancelled.");
        }

        isFixing = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
