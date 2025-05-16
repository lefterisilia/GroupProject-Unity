using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;
    public float movementThreshold = 0.1f;
    public float footstepVolume = 0.4f; // Lower volume here

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float stepTimer = 0f;
    private int currentFootstepIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;

        // Optional: tweak pitch/rolloff here if needed
    }

    void Update()
    {
        float distanceMoved = (transform.position - lastPosition).magnitude;
        float speed = distanceMoved / Time.deltaTime;
        lastPosition = transform.position;

        if (speed > movementThreshold)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= footstepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[currentFootstepIndex];
        audioSource.PlayOneShot(clip, footstepVolume);

        currentFootstepIndex = (currentFootstepIndex + 1) % footstepClips.Length;
    }
}
