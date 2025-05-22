using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EngineFixer : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;
    public LayerMask playerLayer;
    public int requiredPCFixes = 5;

    public GameObject engineMinigameUI;
    public GameObject infoMessageUI;

    private bool isFixed = false;

    void Update()
    {
        if (isFixed) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, playerLayer);
        foreach (var hit in hits)
        {
            if (Input.GetKeyDown(interactKey))
            {
                if (FixManager.Instance.FixesDone >= requiredPCFixes)
                {
                    // Enable the engine minigame
                    engineMinigameUI.SetActive(true);
                }
                else
                {
                    // Show a message: not enough PCs fixed
                    StartCoroutine(ShowInfoMessage());
                }
            }
        }
    }

    public void CompleteEngineFix()
    {
        isFixed = true;
        FixManager.Instance.EngineIsFixed = true;

        
        FixManager.Instance.UpdateFixUI();

        engineMinigameUI.SetActive(false);
        Debug.Log("[Engine] Fix completed, elevator unlocked.");
    }

    IEnumerator ShowInfoMessage()
    {
        infoMessageUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        infoMessageUI.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
