using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EngineMinigame : MonoBehaviour
{
    public RectTransform movingBar;
    public RectTransform greenZone;
    public RectTransform barParent;
    public TMP_Text failText;
    public TMP_Text progressText;

    public EngineFixer engineFixer;
    public Transform player;
    public float requiredDistance = 3f;

    public float moveSpeed = 250f;
    private int direction = 1;
    private bool isPlaying = false;

    private int successCount = 0;
    public int requiredHits = 3;

    private bool inputCooldown = false;

    void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        isPlaying = true;
        direction = 1;
        successCount = 0;
        UpdateProgressText();
        failText.gameObject.SetActive(false);
        progressText.gameObject.SetActive(true);
        ResetBar();
    }

    void Update()
    {
        if (!isPlaying) return;

        // Player walked away
        if (Vector3.Distance(player.position, engineFixer.transform.position) > requiredDistance)
        {
            Debug.Log("[Minigame] Cancelled - player too far");
            gameObject.SetActive(false);
            return;
        }

        // Move red bar unless in cooldown
        if (!inputCooldown)
        {
            Vector2 pos = movingBar.anchoredPosition;
            pos.x += direction * moveSpeed * Time.deltaTime;

            float limit = barParent.rect.width / 2 - movingBar.rect.width / 2;
            if (pos.x > limit)
            {
                pos.x = limit;
                direction = -1;
            }
            else if (pos.x < -limit)
            {
                pos.x = -limit;
                direction = 1;
            }

            movingBar.anchoredPosition = pos;
        }

        // Press space
        if (!inputCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HandleInputCooldown());
            CheckHit();
        }
    }

    void CheckHit()
    {
        float barX = movingBar.localPosition.x;
        float barHalf = movingBar.rect.width / 2f;

        float greenX = greenZone.localPosition.x;
        float greenHalf = greenZone.rect.width / 2f;

        float barLeft = barX - barHalf;
        float barRight = barX + barHalf;

        float greenLeft = greenX - greenHalf;
        float greenRight = greenX + greenHalf;

        if (barLeft >= greenLeft && barRight <= greenRight)
        {
            successCount++;
            UpdateProgressText();

            if (successCount >= requiredHits)
            {
                engineFixer.CompleteEngineFix();
                isPlaying = false;
                gameObject.SetActive(false);
            }
            else
            {
                ResetBar();
            }
        }
        else
        {
            StartCoroutine(ShowFail());
        }
    }

    IEnumerator HandleInputCooldown()
    {
        inputCooldown = true;
        movingBar.gameObject.SetActive(false); // Hide red bar
        yield return new WaitForSeconds(0.5f);
        movingBar.gameObject.SetActive(true);  // Show again
        inputCooldown = false;
    }

    IEnumerator ShowFail()
    {
        failText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        failText.gameObject.SetActive(false);

        successCount = 0;
        UpdateProgressText();
        ResetBar();
    }

    void ResetBar()
    {
        movingBar.anchoredPosition = new Vector2(-barParent.rect.width / 2 + 10f, movingBar.anchoredPosition.y);
        direction = 1;
    }

    void UpdateProgressText()
    {
        if (progressText != null)
            progressText.text = $"Progress: {successCount}/{requiredHits}";
    }
}
