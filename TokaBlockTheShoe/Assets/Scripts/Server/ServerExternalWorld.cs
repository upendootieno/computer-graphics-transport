using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerExternalWorld : MonoBehaviour
{
    const int Port = 6700;
    TcpListener listener;
    Thread listenerThread;

    // latest value written by server thread; consumed on main thread
    volatile float latestX = float.NaN;

    private VehicleController vehicleController;

    void Start()
    {
        vehicleController = GetComponent<VehicleController>();
        if (vehicleController == null)
        {
            Debug.LogError("[TCP] VehicleController component not found on this GameObject. Attach VehicleController.");
        }

        listenerThread = new Thread(ServerThread);
        listenerThread.IsBackground = true;
        listenerThread.Start();

        Debug.Log("[TCP] Server thread started");
    }

    void ServerThread()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Debug.Log("[TCP] Listening on ANY:" + Port);

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    Debug.Log("[TCP] Client connected");
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string line = reader.ReadLine();
                        Debug.Log("[TCP] Received: " + line);

                        if (float.TryParse(line, out float x))
                            latestX = x;
                    }
                }
            }
        }
        catch (ThreadAbortException)
        {
            // expected on quit/abort
        }
        catch (Exception e)
        {
            Debug.LogError("[TCP] Server error: " + e);
        }
    }

    void Update()
    {
        if (!float.IsNaN(latestX))
        {
            if (vehicleController != null)
            {
                // Use AddWaypointX or SetTargetX depending on behavior you want:
                vehicleController.AddWaypointX(latestX);
                // vehicleController.SetTargetX(latestX); // uncomment if you want single-target behavior
            }
            else
            {
                Debug.LogWarning("[TCP] Received X but no VehicleController to forward to.");
            }

            latestX = float.NaN; // consume
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            listener?.Stop();
            listenerThread?.Abort();
        }
        catch (Exception e)
        {
            Debug.LogWarning("[TCP] Error stopping thread: " + e);
        }
    }
}
