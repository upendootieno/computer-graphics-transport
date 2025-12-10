using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServerExternalWorld : MonoBehaviour
{
    const int Port = 6700;

    TcpListener listener;
    TcpClient client;
    NetworkStream netStream;
    StreamReader reader;

    void Start()
    {
        try
        {
            // Listen on ALL interfaces (LAN + localhost)
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            Debug.Log($"[ServerExternalWorld] Listening on 0.0.0.0:{Port}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ServerExternalWorld] Failed to start listener: {ex}");
        }
    }

    void Update()
    {
        // 1. Accept new client if none connected
        if (listener != null && listener.Pending() && client == null)
        {
            try
            {
                client = listener.AcceptTcpClient();
                client.NoDelay = true;

                netStream = client.GetStream();
                reader = new StreamReader(netStream, Encoding.UTF8);

                Debug.Log("[ServerExternalWorld] Client connected.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServerExternalWorld] Accept error: {ex}");
            }
        }

        // 2. If connected and data is available, read it
        if (client != null && netStream != null && netStream.DataAvailable)
        {
            try
            {
                // Read one newline-terminated message
                string msg = reader.ReadLine();

                if (!string.IsNullOrEmpty(msg))
                {
                    Debug.Log($"[ServerExternalWorld] Received: {msg}");

                    // Example: parse number and move object
                    if (float.TryParse(msg.Trim(), out float x))
                    {
                        Vector3 pos = transform.position;
                        pos.x = x;
                        transform.position = pos;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServerExternalWorld] Read error: {ex}");
                CleanupClient();
            }
        }

        // 3. Clean up if client disconnected
        if (client != null && !client.Connected)
        {
            CleanupClient();
        }
    }

    void CleanupClient()
    {
        Debug.Log("[ServerExternalWorld] Cleaning up client...");

        try { reader?.Close(); } catch { }
        try { netStream?.Close(); } catch { }
        try { client?.Close(); } catch { }

        reader = null;
        netStream = null;
        client = null;
    }

    void OnApplicationQuit()
    {
        CleanupClient();
        try { listener?.Stop(); } catch { }
    }
}
