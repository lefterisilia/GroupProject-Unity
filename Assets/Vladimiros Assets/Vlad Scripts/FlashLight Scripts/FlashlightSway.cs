using UnityEngine;

public class FlashlightSway : MonoBehaviour
{
    public Transform targetCamera;
    public float rotationSmoothSpeed = 5f;
    public float positionSmoothSpeed = 10f;
    public Vector3 positionOffset = new Vector3(0, -0.2f, 0.5f);

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // Smooth rotation
        Quaternion targetRot = targetCamera.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothSpeed);

        // Smooth position
        Vector3 desiredPos = targetCamera.position + targetCamera.TransformDirection(positionOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * positionSmoothSpeed);
    }
}
