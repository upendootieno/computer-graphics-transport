using UnityEngine;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float Speed = 10f;

    // keep Path private to prevent external scripts from mutating it directly
    [SerializeField]
    private List<Vector3> Path = new List<Vector3>();
    private int currentIndex = 0;

    void Update()
    {
        if (Path == null || Path.Count == 0 || currentIndex >= Path.Count)
            return;

        Vector3 target = Path[currentIndex];
        Vector3 dir = (target - transform.position);
        float dist = dir.magnitude;

        if (dist > 0.001f)
        {
            dir /= dist; // normalize
            float move = Speed * Time.deltaTime;
            if (move >= dist)
            {
                // snap to target and advance
                transform.position = target;
                currentIndex++;
            }
            else
            {
                transform.position += dir * move;
            }
        }
        else
        {
            currentIndex++;
        }
    }

    // Public way to append a new X waypoint (keeps vehicle's current Y & Z)
    public void AddWaypointX(float xValue)
    {
        if (Path == null) Path = new List<Vector3>();
        Path.Add(new Vector3(xValue, transform.position.y, transform.position.z));
    }

    // Public way to clear existing path and set a single target
    public void SetTargetX(float xValue)
    {
        if (Path == null) Path = new List<Vector3>();
        Path.Clear();
        currentIndex = 0;
        Path.Add(new Vector3(xValue, transform.position.y, transform.position.z));
    }

    // Optional: public read-only accessor (safe to inspect but not modify)
    public IReadOnlyList<Vector3> GetPathSnapshot()
    {
        return Path == null ? new List<Vector3>().AsReadOnly() : Path.AsReadOnly();
    }
}
