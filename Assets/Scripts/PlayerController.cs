using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    public bool IsSprinting { get; private set; }
    public bool IsCrouching => isCrouching;
    public Vector3? LastSprintPosition { get; private set; }

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching;

    // Sprint memory
    private float sprintMemoryTime = 0.5f;
    private float lastSprintTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;
    }

    void Update()
    {
        Crouch();
        Movement();
        ApplyGravity();
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = GetCurrentSpeed();
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    float GetCurrentSpeed()
    {
        if (isCrouching)
        {
            IsSprinting = false;
            return crouchSpeed;
        }

        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        if (IsSprinting)
        {
            LastSprintPosition = transform.position;
            lastSprintTime = Time.time;
        }

        return IsSprinting ? sprintSpeed : walkSpeed;
    }

    void Crouch()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        controller.height = isCrouching ? crouchingHeight : standingHeight;
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public bool HasRecentlySprinted()
    {
        return Time.time - lastSprintTime <= sprintMemoryTime;
    }
}
