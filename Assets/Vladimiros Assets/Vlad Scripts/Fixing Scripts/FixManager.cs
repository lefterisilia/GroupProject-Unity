using UnityEngine;
using TMPro;

public class FixManager : MonoBehaviour
{
    public static FixManager Instance;

    [Header("Fix Settings")]
    public int totalFixesRequired = 5;

    [Header("UI")]
    public TMP_Text fixStatusText;      // PCs fixed text
    public TMP_Text engineStatusText;   // Engine fixed text

    public int FixesDone { get; private set; } = 0;

    public bool EngineIsFixed { get; set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateFixUI();
    }

    public void RegisterFix()
    {
        FixesDone++;
        UpdateFixUI();
    }

    public void UpdateFixUI()
    {
        if (fixStatusText != null)
            fixStatusText.text = $"PCs Fixed: {FixesDone}/{totalFixesRequired}";

        if (engineStatusText != null)
            engineStatusText.text = $"Engines Fixed: {(EngineIsFixed ? 1 : 0)}/1";
    }
}
