using UnityEngine;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour
{
    public float Speed = 10f;

    private List<Vector3> Path = new List<Vector3>();
    private int currentIndex = 0;

    void Update()
    {
        if (Path == null || Path.Count == 0 || currentIndex >= Path.Count)
            return;

        Vector3 target = Path[currentIndex];
        Vector3 dir = (target - transform.position).normalized;

        transform.position += dir * Speed * Time.deltaTime;

        // Switch to next waypoint when close enough
        if (Vector3.Distance(transform.position, target) < 0.2f)
            currentIndex++;
    }

    // Called by the TCP script
    public void AddWaypointX(float xValue)
    {
        Path.Add(new Vector3(
            xValue,
            transform.position.y,
            transform.position.z
        ));
    }
}

