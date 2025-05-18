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

    [Header("Player Detection")]
    public LayerMask playerLayer;
    private GameObject player;

    private bool isMoving = false;
    private bool isPowered = false;
    private ElevatorLightController lightController;

    void Start()
    {
        lightController = GetComponentInChildren<ElevatorLightController>();

        // If engine is already fixed (e.g., testing scene), power on immediately
        if (FixManager.Instance != null && FixManager.Instance.EngineIsFixed)
        {
            PowerOnElevator();
        }
    }

    void Update()
    {
        // Check if engine got fixed during runtime and hasn't powered elevator yet
        if (!isPowered && FixManager.Instance != null && FixManager.Instance.EngineIsFixed)
        {
            PowerOnElevator();
        }

        if (isMoving || !isPowered) return;

        if (IsPlayerInside())
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) { MoveToFloor(0); }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { MoveToFloor(1); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { MoveToFloor(2); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { MoveToFloor(3); }
        }
    }

    void PowerOnElevator()
    {
        isPowered = true;
        if (lightController != null) lightController.PowerOn();
        Debug.Log("[Elevator] Power ON (Engine fixed)");
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

        anim.clip = clip;
        anim.Play();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
