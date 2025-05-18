using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeartbeatController : MonoBehaviour
{
    public Transform monster;
    public AudioClip heartbeatClip;

    [Header("Heartbeat Settings")]
    public float maxVolume = 1f;
    public float minDistance = 5f;
    public float maxDistance = 25f;

    private AudioSource heartbeatSource;
    private bool isPlaying = false;

    void Start()
    {
        heartbeatSource = GetComponent<AudioSource>();
        heartbeatSource.clip = heartbeatClip;
        heartbeatSource.loop = true;
        heartbeatSource.playOnAwake = false;
        heartbeatSource.spatialBlend = 0f;
        heartbeatSource.volume = 0f;
        heartbeatSource.panStereo = 0f;
    }

    void Update()
    {
        if (monster == null)
        {
            Debug.LogWarning("💥 No monster assigned to HeartbeatController!");
            return;
        }

        float distance = Vector3.Distance(transform.position, monster.position);
        Debug.Log("Distance to Monster: " + distance);

        if (distance <= maxDistance)
        {
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            float newVolume = Mathf.Lerp(0f, maxVolume, t);
            heartbeatSource.volume = newVolume;

            if (!isPlaying)
            {
                heartbeatSource.Play();
                isPlaying = true;
                Debug.Log("🔊 Heartbeat started");
            }
        }
        else
        {
            if (isPlaying)
            {
                heartbeatSource.Stop();
                isPlaying = false;
                Debug.Log("🔇 Heartbeat stopped");
            }
        }

        Debug.Log($"Heartbeat Playing: {heartbeatSource.isPlaying}, Volume: {heartbeatSource.volume}");
    }
}
