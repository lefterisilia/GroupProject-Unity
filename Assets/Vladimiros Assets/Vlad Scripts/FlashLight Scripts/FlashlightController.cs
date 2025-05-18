using UnityEngine;
using TMPro;
using System.Collections;

public class FlashlightController : MonoBehaviour
{
    public Light flashlightLight;
    public KeyCode toggleKey = KeyCode.F;
    public KeyCode reloadKey = KeyCode.R;

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float batteryDrainRate = 5f;
    public int maxSpareBatteries = 5;

    [Header("UI References")]
    public TextMeshProUGUI batteryPercentText;
    public TextMeshProUGUI spareCountText;

    [Header("Flicker Settings")]
    public bool enableFlicker = true;
    public float flickerThreshold = 20f;
    public float flickerChance = 0.2f;   // 20% chance
    public float flickerInterval = 0.2f;

    [Header("Sound Effects")]
    public AudioClip toggleSound;
    public float toggleVolume = 0.7f;

    private float currentBattery;
    private int spareBatteries = 0;
    private bool isOn = false;
    private float flickerTimer = 0f;

    private AudioSource audioSource;

    void Start()
    {
        if (flashlightLight == null)
            flashlightLight = GetComponent<Light>();

        currentBattery = maxBattery;
        isOn = false;
        flashlightLight.enabled = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        HandleToggle();
        HandleBattery();
        HandleReload();
        HandleFlicker();
        UpdateUI();
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (currentBattery > 0f)
            {
                isOn = !isOn;
                flashlightLight.enabled = isOn;

                // 🔊 Play toggle sound
                if (toggleSound != null && audioSource != null)
                    audioSource.PlayOneShot(toggleSound, toggleVolume);
            }
        }
    }

    void HandleBattery()
    {
        if (isOn && currentBattery > 0f)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

            if (currentBattery <= 0f)
            {
                flashlightLight.enabled = false;
                isOn = false;
            }
        }
    }

    void HandleReload()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            if (spareBatteries > 0 && currentBattery < maxBattery)
            {
                currentBattery = maxBattery;
                spareBatteries--;
                flashlightLight.enabled = true;
                isOn = true;
                Debug.Log("[Flashlight] Reloaded. Spare batteries left: " + spareBatteries);
            }
        }
    }

    void HandleFlicker()
    {
        if (!enableFlicker || !isOn || currentBattery > flickerThreshold)
            return;

        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            flickerTimer = flickerInterval;

            if (Random.value < flickerChance)
            {
                StartCoroutine(FlickerLight());
            }
        }
    }

    IEnumerator FlickerLight()
    {
        flashlightLight.enabled = false;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        flashlightLight.enabled = true;
    }

    void UpdateUI()
    {
        if (batteryPercentText != null)
        {
            int percent = Mathf.CeilToInt(currentBattery);
            batteryPercentText.text = percent + "%";
        }

        if (spareCountText != null)
        {
            spareCountText.text = "x" + spareBatteries;
        }
    }

    public void AddBattery()
    {
        if (spareBatteries < maxSpareBatteries)
        {
            spareBatteries++;
            Debug.Log("[Flashlight] Picked up battery. Now carrying: " + spareBatteries);
        }
    }

    public void RechargeBatteryFull()
    {
        currentBattery = maxBattery;

        if (!isOn && currentBattery > 0)
        {
            flashlightLight.enabled = true;
            isOn = true;
        }
    }

    public float GetBatteryPercent() => currentBattery / maxBattery;
    public int GetSpareBatteryCount() => spareBatteries;
}
