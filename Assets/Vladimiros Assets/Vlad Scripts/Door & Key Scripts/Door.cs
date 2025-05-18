using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public int requiredKey = -1; // -1 = no key required
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Interaction Offset")]
    public Vector3 offset = Vector3.zero;

    [Header("Sound Effects")]
    public AudioClip[] openSounds;
    public AudioClip[] closeSounds;
    public AudioClip lockedSound; // 🔐 NEW

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private AudioSource audioSource;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * openAngle);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + offset, interactRange);
        foreach (var hit in hits)
        {
            PlayerKeyInventory inv = hit.GetComponent<PlayerKeyInventory>();
            if (inv != null && Input.GetKeyDown(interactKey))
            {
                if (requiredKey == -1 || inv.HasKey(requiredKey))
                {
                    if (!isMoving)
                        StartCoroutine(ToggleDoor());
                }
                else
                {
                    // 🔊 Play locked door sound
                    if (lockedSound != null && audioSource != null)
                        audioSource.PlayOneShot(lockedSound);

                    // Optional UI message
                    UIManager.Instance.ShowMessage($"You don't have the key for Door {requiredKey}!");
                }

                break;
            }
        }
    }

    System.Collections.IEnumerator ToggleDoor()
    {
        isMoving = true;
        Quaternion target = isOpen ? closedRotation : openRotation;

        // 🔊 Play random open or close sound
        if (audioSource != null)
        {
            AudioClip[] soundPool = isOpen ? closeSounds : openSounds;

            if (soundPool != null && soundPool.Length > 0)
            {
                int randomIndex = Random.Range(0, soundPool.Length);
                audioSource.PlayOneShot(soundPool[randomIndex]);
            }
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, target, t);
            yield return null;
        }

        isOpen = !isOpen;
        isMoving = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + offset, interactRange);
    }
}
