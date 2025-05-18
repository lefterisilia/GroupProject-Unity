using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FixMinigame : MonoBehaviour
{
    public RectTransform marker;
    public RectTransform successZone;
    public TextMeshProUGUI progressText;
    public float baseSpeed = 400f;
    public int requiredHits = 10;
    public float delayBetweenAttempts = 3f;

    private float speed;
    private float direction = 1f;
    private int currentHits = 0;
    private bool inputLocked = false;
    private bool active = false;

    private Transform player;
    private Transform pcObject;
    private float allowedRange = 3f;

    private System.Action<bool> onResult;

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!active || inputLocked) return;

        // Cancel if player walked away
        if (player != null && pcObject != null)
        {
            float dist = Vector3.Distance(player.position, pcObject.position);
            if (dist > allowedRange)
            {
                Debug.Log("[Minigame] Player walked away — cancelling");
                EndMinigame(false);
                return;
            }
        }

        // Move marker
        marker.anchoredPosition += Vector2.right * speed * direction * Time.deltaTime;

        // Bounce
        if (marker.anchoredPosition.x >= 200 || marker.anchoredPosition.x <= -200)
        {
            direction *= -1f;
        }

        // Space press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool success = IsMarkerInZone();

            if (success)
            {
                currentHits++;
                Debug.Log($"[Minigame] Hit {currentHits}/{requiredHits}");
                UpdateProgress();
            }
            else
            {
                Debug.Log("[Minigame] Missed!");
            }

            if (currentHits >= requiredHits)
            {
                EndMinigame(true);
            }
            else
            {
                StartCoroutine(DelayBeforeNextAttempt());
            }
        }
    }

    public void StartMinigame(System.Action<bool> callback, Transform playerRef, Transform pcRef, float range)
    {
        onResult = callback;
        player = playerRef;
        pcObject = pcRef;
        allowedRange = range;

        currentHits = 0;
        UpdateProgress();
        speed = baseSpeed;
        direction = 1f;
        ResetMarker();
        gameObject.SetActive(true);
        active = true;
        inputLocked = false;
    }

    IEnumerator DelayBeforeNextAttempt()
    {
        inputLocked = true;
        marker.gameObject.SetActive(false);
        yield return new WaitForSeconds(delayBetweenAttempts);
        ResetMarker();
        marker.gameObject.SetActive(true);
        inputLocked = false;
    }

    void ResetMarker()
    {
        marker.anchoredPosition = new Vector2(-200, 0);
        direction = 1f;
    }

    void EndMinigame(bool success)
    {
        active = false;
        gameObject.SetActive(false);
        onResult?.Invoke(success);
    }

    bool IsMarkerInZone()
    {
        Vector3 markerWorld = marker.position;
        Vector3 zoneWorld = successZone.position;
        float zoneWidth = successZone.rect.width * successZone.lossyScale.x / 2;

        return Mathf.Abs(markerWorld.x - zoneWorld.x) <= zoneWidth;
    }

    void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text = $"{currentHits} / {requiredHits}";
        }
    }
}
