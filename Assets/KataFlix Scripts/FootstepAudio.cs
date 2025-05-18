using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;           // Default interval
    public float movementThreshold = 0.1f;
    public float footstepVolume = 0.4f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float stepTimer = 0f;
    private int currentFootstepIndex = 0;

    private PlayerController playerController;      // Link to movement script

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;

        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            Debug.LogWarning("PlayerController not found on object!");

        lastPosition = transform.position;
    }

    void Update()
    {
        if (playerController != null && playerController.IsCrouching)
            return; // Skip footsteps if crouching

        float distanceMoved = (transform.position - lastPosition).magnitude;
        float speed = distanceMoved / Time.deltaTime;
        lastPosition = transform.position;

        if (speed > movementThreshold)
        {
            stepTimer += Time.deltaTime;

            // Change interval if sprinting
            float interval = (playerController != null && playerController.IsSprinting)
                ? footstepInterval * 0.6f  // Faster footsteps when sprinting
                : footstepInterval;

            if (stepTimer >= interval)
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
