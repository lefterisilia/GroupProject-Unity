using UnityEngine;
using TMPro;

public class FixManager : MonoBehaviour
{
    public static FixManager Instance;

    public int totalToFix = 5;
    private int fixedCount = 0;

    public TextMeshProUGUI statusText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void RegisterFix()
    {
        fixedCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (statusText != null)
        {
            statusText.text = $"{fixedCount} / {totalToFix} PCs Fixed";
        }
    }

    public bool AllFixed()
    {
        return fixedCount >= totalToFix;
    }
}
