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

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching;

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
            return crouchSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            return sprintSpeed;
        return walkSpeed;
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
}
