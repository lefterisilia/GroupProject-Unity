using UnityEngine;
using System.Collections;

public class ElevatorController : MonoBehaviour
{
    [Header("Floor Settings")]
    public Transform[] floorPositions;
    public float moveSpeed = 2f;

    [Header("Door Animation Parts")]
    public Animation elevatorAnim;
    public Animation hallFrameAnim;
    public Animation elevatorRedAnim;
    public Animation hallFrameRedAnim;

    [Header("Fix Requirements")]
    public int requiredFixes = 2;
    private int currentFixes = 0;
    private bool isPowered = false;

    [Header("Player Detection")]
    public LayerMask playerLayer;
    private GameObject player;

    private bool isMoving = false;
    private ElevatorLightController lightController;

    void Start()
    {
        lightController = GetComponentInChildren<ElevatorLightController>();

        // TEMP: auto-enable power if no fix is required
        if (requiredFixes == 0)
        {
            isPowered = true;
            if (lightController != null) lightController.PowerOn();
        }
    }

    void Update()
    {
        if (isMoving || !isPowered) return;

        if (IsPlayerInside())
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) { Debug.Log("[Elevator] 0"); MoveToFloor(0); }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { Debug.Log("[Elevator] 1"); MoveToFloor(1); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { Debug.Log("[Elevator] 2"); MoveToFloor(2); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { Debug.Log("[Elevator] 3"); MoveToFloor(3); }
        }
    }

    public void RegisterFix()
    {
        currentFixes++;

        if (currentFixes >= requiredFixes && !isPowered)
        {
            isPowered = true;
            if (lightController != null) lightController.PowerOn();
        }
    }

    bool IsPlayerInside()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player = hit.gameObject;
                return true;
            }
        }
        return false;
    }

    void MoveToFloor(int index)
    {
        if (index < 0 || index >= floorPositions.Length) return;
        StartCoroutine(ElevatorSequence(floorPositions[index].position));
    }

    IEnumerator ElevatorSequence(Vector3 targetPosition)
    {
        isMoving = true;

        // Close doors
        PlayDoorAnimation("CloseDoors");
        yield return new WaitForSeconds(1.5f);

        if (player != null)
            player.transform.SetParent(this.transform);

        // Move elevator
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (player != null)
            player.transform.SetParent(null);

        // Open doors
        PlayDoorAnimation("OpenDoors");
        yield return new WaitForSeconds(1.5f);

        isMoving = false;
    }

    void PlayDoorAnimation(string animName)
    {
        TryPlayAnim(elevatorAnim, animName);
        TryPlayAnim(hallFrameAnim, animName);
        TryPlayAnim(elevatorRedAnim, animName);
        TryPlayAnim(hallFrameRedAnim, animName);
    }

    void TryPlayAnim(Animation anim, string name)
    {
        if (anim == null)
        {
            Debug.LogWarning("[Elevator] Animation component is missing.");
            return;
        }

        AnimationClip clip = anim.GetClip(name);

        if (clip == null)
        {
            Debug.LogWarning($"[Elevator] Clip '{name}' not found on {anim.gameObject.name}.");
            return;
        }

        Debug.Log($"[Elevator] Playing: {name} on {anim.gameObject.name}");
        anim.clip = clip;
        anim.Play();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
