using UnityEngine;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour
{
    public List<Vector3> Path;
    public float Speed = 10f;

    private int currentIndex = 0;

    void Update()
    {
        if (Path == null || currentIndex >= Path.Count)
            return;

        Vector3 target = Path[currentIndex];
        Vector3 dir = (target - transform.position).normalized;

        transform.position += dir * Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 0.5f)
            currentIndex++;
    }
}
