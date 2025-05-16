using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FixablePC : MonoBehaviour
{
    public string interactKey = "e";
    public float interactRange = 3f;
    public LayerMask playerLayer;
    public Slider progressBar;
    public float fixDuration = 3f;

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
                    StartCoroutine(FixProcess());
                }
            }
        }
    }

    IEnumerator FixProcess()
    {
        isFixing = true;
        progressBar.gameObject.SetActive(true);
        progressBar.value = 0;

        float timer = 0f;
        Transform player = FindObjectOfType<PlayerFixer>().transform;

        while (timer < fixDuration)
        {
            // Cancel if player walks away
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance > interactRange)
            {
                Debug.Log("[PC] Fix canceled - player walked away");
                progressBar.gameObject.SetActive(false);
                isFixing = false;
                yield break;
            }

            timer += Time.deltaTime;
            progressBar.value = timer / fixDuration;
            yield return null;
        }

        isFixed = true;
        isFixing = false;
        progressBar.gameObject.SetActive(false);
        FixManager.Instance.RegisterFix();
        Debug.Log("[PC] Fixed!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
