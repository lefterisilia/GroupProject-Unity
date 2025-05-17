using UnityEngine;
using TMPro;

public class KeyListUI : MonoBehaviour
{
    public GameObject panel; // The UI container (can be empty GameObject)
    public TextMeshProUGUI keyListText;
    public KeyCode showKey = KeyCode.Q;
    private PlayerKeyInventory playerKeys;

    void Start()
    {
        panel.SetActive(false);
        playerKeys = FindObjectOfType<PlayerKeyInventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(showKey))
        {
            UpdateKeyList();
            panel.SetActive(true);
        }

        if (Input.GetKeyUp(showKey))
        {
            panel.SetActive(false);
        }
    }

    void UpdateKeyList()
    {
        var keys = playerKeys.GetAllKeys();
        keyListText.text = "Keys:\n";
        foreach (int key in keys)
        {
            keyListText.text += "- Key " + key + "\n";
        }

        if (keys.Count == 0)
            keyListText.text += "(none)";
    }
}
