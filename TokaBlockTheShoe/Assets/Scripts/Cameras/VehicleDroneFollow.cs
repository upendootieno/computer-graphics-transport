using UnityEngine;

public class VehicleDroneFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float height = 30f;                 // how high above the car
    public float tiltAngle = 45f;              // angle to tilt downward
    public float orthographicSize = 20f;       // zoom level

    [Header("Smoothing")]
    public float followSmoothness = 5f;
    public float rotationSmoothness = 5f;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographic = true;               // ensure orthographic mode
        cam.orthographicSize = orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Build a rotated offset based on tilt
        Quaternion tilt = Quaternion.Euler(tiltAngle, 0f, 0f);
        Vector3 offset = tilt * Vector3.back * height;

        // Desired position = target + offset
        Vector3 desiredPosition = target.position + offset;

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

        // Smooth rotation to always look at target
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }
}
