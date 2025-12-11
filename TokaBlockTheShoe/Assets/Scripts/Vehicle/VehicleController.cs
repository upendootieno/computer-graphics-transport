using UnityEngine;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour
{
    public float Speed = 10f;

    private List<Vector3> Path = new List<Vector3>();
    private int currentIndex = 0;

    void Update()
    {
        if (Path.Count == 0 || currentIndex >= Path.Count)
            return;

        Vector3 target = Path[currentIndex];
        Vector3 dir = (target - transform.position).normalized;

        transform.position += dir * Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 0.2f)
            currentIndex++;
    }

    // Add a new waypoint for continuous motion
    public void AddWaypointX(float xValue)
    {
        Path.Add(new Vector3(
            xValue,
            transform.position.y,
            transform.position.z
        ));
    }

    // OPTIONAL: If you want only a single active target at a time
    public void SetTargetX(float xValue)
    {
        Path.Clear();
        currentIndex = 0;

        Path.Add(new Vector3(
            xValue,
            transform.position.y,
            transform.position.z
        ));
    }
}
