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

    volatile float latestX = float.NaN;

    void Start()
    {
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
                TcpClient client = listener.AcceptTcpClient();
                Debug.Log("[TCP] Client connected");

                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line = reader.ReadLine();
                    Debug.Log("[TCP] Received: " + line);

                    if (float.TryParse(line, out float x))
                        latestX = x;   // thread-safe write
                }
            }
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
            transform.position = new Vector3(
                latestX,
                transform.position.y,
                transform.position.z
            );

            latestX = float.NaN; // consume
        }
    }

    void OnApplicationQuit()
    {
        listener?.Stop();
        listenerThread?.Abort();
    }
}
