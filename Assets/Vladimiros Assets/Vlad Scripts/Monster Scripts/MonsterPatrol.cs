using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class MonsterPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 1f;
    public float resumeDelay = 5f;

    [Header("Sound Effects")]
    public AudioClip[] footstepClips;
    public AudioClip[] idleClips;
    public float footstepVolume = 1f;
    public float idleVolume = 0.6f;

    private int currentPoint = 0;
    private float waitTimer = 0f;
    private float lostPlayerTimer = 0f;
    private bool isWaiting = false;
    private bool waitingToResume = false;

    private NavMeshAgent agent;
    private MonsterAI ai;
    private MonsterStunHandler stun;

    private AudioSource audioSource;
    private bool isMovingSoundPlaying = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<MonsterAI>();
        stun = GetComponent<MonsterStunHandler>();
        audioSource = GetComponent<AudioSource>();

        // Proper 3D spatial sound
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 25f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        GoToNextPoint();
    }

    void Update()
    {
        //  If stunned, stop sound and do nothing
        if (stun.IsStunned())
        {
            StopFootsteps();
            return;
        }

        //  If chasing player, stop patrol and footstep sound
        if (ai.IsChasingPlayer())
        {
            lostPlayerTimer = 0f;
            isWaiting = false;
            waitingToResume = true;
            StopFootsteps();
            return;
        }

        //  Resume patrol after chasing
        if (waitingToResume)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= resumeDelay)
            {
                waitingToResume = false;
                GoToNextPoint();
            }
            return;
        }

        //  Monster is patrolling
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StopFootsteps(); // 🔇 Stop footstep loop

            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;

                // 🔊 Play random idle sound once
                PlayIdleSound();
            }

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                GoToNextPoint();
                isWaiting = false;
            }
        }
        else
        {
            PlayFootsteps(); // 🔊 Play footsteps while moving
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPoint].position);
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    void StopFootsteps()
    {
        if (audioSource.isPlaying && isMovingSoundPlaying)
        {
            audioSource.Stop();
            isMovingSoundPlaying = false;
        }
    }

    private float footstepTimer = 0f;
    public float footstepInterval = 0.7f;

    void PlayFootsteps()
    {
        if (footstepClips.Length == 0 || audioSource == null) return;

        Camera playerCam = Camera.main;
        if (playerCam == null)
        {
            Debug.LogWarning("❌ No Main Camera found.");
            return;
        }

        float distToCamera = Vector3.Distance(transform.position, playerCam.transform.position);
        Debug.Log($"📏 Distance to Camera: {distToCamera}");

        if (distToCamera > 50f) return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            int randomIndex = Random.Range(0, footstepClips.Length);
            audioSource.PlayOneShot(footstepClips[randomIndex], 1f); // Full volume for now
            footstepTimer = footstepInterval;

            Debug.Log($"👣 Footstep played near camera at distance: {distToCamera:F1}");
        }
    }

    void PlayIdleSound()
    {
        if (idleClips.Length == 0 || audioSource == null) return;

        int randomIndex = Random.Range(0, idleClips.Length);
        AudioSource.PlayClipAtPoint(idleClips[randomIndex], transform.position, idleVolume);

        Debug.Log("[Monster] Playing idle sound: " + idleClips[randomIndex].name);
    }
}
