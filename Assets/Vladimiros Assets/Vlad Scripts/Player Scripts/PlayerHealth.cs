using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 2;
    public int currentHP;

    [Header("Respawn Settings")]
    public Transform respawnPoint;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    private bool stunned = false;

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    public void TakeHit()
    {
        if (stunned) return;

        currentHP--;
        UpdateUI();

        if (currentHP <= 0)
        {
            stunned = true;
            Debug.Log("[Player] Knocked out!");
            Respawn();
        }
    }

    void Respawn()
    {
        // Move player to respawn point
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        currentHP = maxHP;
        UpdateUI();
        stunned = false;

        // Optional: delay control restore, play wake-up effect etc.
    }

    void UpdateUI()
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP}/{maxHP}";
        }
    }

    public bool IsStunned() => stunned;
}
