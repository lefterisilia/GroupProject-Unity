using UnityEngine;
using System.Collections;

public class ElevatorController : MonoBehaviour
{
    [Header("Floor Settings")]
    public Transform[] floorPositions;
    public float moveSpeed = 2f;

    [Header("Door Settings")]
    public Transform door;
    public Vector3 doorOpenOffset = new Vector3(1f, 0, 0);
    public float doorMoveSpeed = 2f;

    [Header("Fix Requirements")]
    public int requiredFixes = 2;
    private int currentFixes = 0;

    [Header("Player Detection")]
    public LayerMask playerLayer;

    private bool isMoving = false;
    private bool doorOpen = false;
    private Vector3 doorClosedPos;
    private Vector3 doorOpenedPos;
    private GameObject player;

    void Start()
    {
        if (door != null)
        {
            doorClosedPos = door.localPosition;
            doorOpenedPos = doorClosedPos + doorOpenOffset;
        }
    }

    void Update()
    {
        if (isMoving || currentFixes < requiredFixes) return;

        if (IsPlayerInside())
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) MoveToFloor(0);
            if (Input.GetKeyDown(KeyCode.Alpha1)) MoveToFloor(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) MoveToFloor(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) MoveToFloor(3);
        }
    }

    public void RegisterFix()
    {
        currentFixes++;
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

        if (doorOpen)
        {
            yield return StartCoroutine(MoveDoor(doorOpenedPos, doorClosedPos));
            doorOpen = false;
        }

        if (player != null)
            player.transform.SetParent(this.transform);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (player != null)
            player.transform.SetParent(null);

        yield return StartCoroutine(MoveDoor(doorClosedPos, doorOpenedPos));
        doorOpen = true;

        isMoving = false;
    }

    IEnumerator MoveDoor(Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * doorMoveSpeed;
            door.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }
    }
}
