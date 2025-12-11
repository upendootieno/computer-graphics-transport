using UnityEngine;

public class VehicleDroneFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 12f, -18f);

    [Header("Smoothing")]
    public float followSmoothness = 5f;
    public float rotationSmoothness = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired camera position
        Vector3 desiredPosition = target.position + target.transform.TransformDirection(offset);

        // Smoothly interpolate to that position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

        // Look at target with smoothing
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }
}
