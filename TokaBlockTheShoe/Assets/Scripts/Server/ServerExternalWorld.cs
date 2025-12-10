using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServerExternalWorld : MonoBehaviour
{
    // port must match MATLAB sender
    const int Port = 6700;
    TcpListener listener;
    TcpClient client;
    NetworkStream netStream;
    StreamReader reader;

    // Called once when the GameObject (ServerExternalWorld) becomes active
    void Start()
    {
        try
        {
            listener = new TcpListener(IPAddress.Loopback, Port);
            listener.Start();
            Debug.Log($"[ServerExternalWorld] Listening on {IPAddress.Loopback}:{Port}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ServerExternalWorld] Failed to start TcpListener: {ex}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            // If there's a pending connection accept it
            if (listener != null && listener.Pending() && client == null)
            {
                client = listener.AcceptTcpClient();
                netStream = client.GetStream();
                reader = new StreamReader(netStream, Encoding.UTF8);
                Debug.Log("[ServerExternalWorld] Client connected");
            }

            // If connected and data available, read it (non-blocking)
            if (client != null && netStream != null && netStream.DataAvailable)
            {
                // Read up to available bytes as a string
                string msg = reader.ReadToEnd(); // small messages are fine; for streaming consider Read/ReadAsync
                if (!string.IsNullOrEmpty(msg))
                {
                    Debug.Log($"[ServerExternalWorld] Received: {msg}");
                    // Example: parse numeric value and move the object
                    // Try parse a number and move along x-axis
                    // UNMARSHALLING GOES HERE
                    if (float.TryParse(msg.Trim(), out float x))
                    {
                        transform.position = new Vector3(x, transform.position.y, transform.position.z);
                    }
                }

                // Close client after reading one message
                CleanupClient();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ServerExternalWorld] Update error: {ex}");
            CleanupClient();
        }
    }

    void OnApplicationQuit()
    {
        // clean up on exit
        try { CleanupClient(); } catch { }
        try { listener?.Stop(); } catch { }
    }

    void CleanupClient()
    {
        try { reader?.Close(); } catch { }
        try { netStream?.Close(); } catch { }
        try { client?.Close(); } catch { }
        client = null;
        netStream = null;
        reader = null;
    }
}
