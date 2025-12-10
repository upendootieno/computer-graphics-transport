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
    bool listenerStarted = false;

    TcpClient client;
    NetworkStream netStream;
    StreamReader reader;

    void Awake()
    {
        Debug.Log("[ServerExternalWorld] Awake() called.");
    }

    void Start()
    {
        Debug.Log("[ServerExternalWorld] Start() called.");

        TryStartListener();
    }

    void TryStartListener()
    {
        if (listenerStarted)
        {
            Debug.Log("[ServerExternalWorld] Listener already active.");
            return;
        }

        try
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            listenerStarted = true;

            Debug.Log($"[ServerExternalWorld] Listener started on 0.0.0.0:{Port}");
        }
        catch (Exception ex)
        {
            Debug.LogError("[ServerExternalWorld] Listener start FAILED: " + ex);
            listenerStarted = false;
        }
    }

    void Update()
    {
        // If listener isn't alive, try to restart it
        if (!listenerStarted)
        {
            TryStartListener();
            return;
        }

        if (listener == null || !listener.Server.IsBound)
        {
            Debug.LogWarning("[ServerExternalWorld] Listener exists but not bound. Restarting...");
            listenerStarted = false;
            TryStartListener();
            return;
        }

        // 1. Accept client
        if (client == null)
        {
            bool pending;
            try { pending = listener.Pending(); }
            catch (Exception ex)
            {
                Debug.LogError("[ServerExternalWorld] Pending() error: " + ex);
                listenerStarted = false;
                return;
            }

            if (pending)
            {
                Debug.Log("[ServerExternalWorld] Pending connection.");

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
                    Debug.LogError("[ServerExternalWorld] Accept error: " + ex);
                    CleanupClient();
                }
            }
        }

        // 2. Read data
        if (client != null && netStream != null && netStream.DataAvailable)
        {
            try
            {
                string msg = reader.ReadLine();
                Debug.Log("[ServerExternalWorld] Received: " + msg);

                if (!string.IsNullOrEmpty(msg) &&
                    float.TryParse(msg.Trim(), out float x))
                {
                    Vector3 p = transform.position;
                    p.x = x;
                    transform.position = p;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[ServerExternalWorld] Read error: " + ex);
                CleanupClient();
            }
        }

        // 3. Cleanup if client disconnected
        if (client != null && !client.Connected)
        {
            CleanupClient();
        }
    }

    void CleanupClient()
    {
        Debug.Log("[ServerExternalWorld] Cleaning client.");
        try { reader?.Close(); } catch { }
        try { netStream?.Close(); } catch { }
        try { client?.Close(); } catch { }

        reader = null;
        netStream = null;
        client = null;
    }

    void OnApplicationQuit()
    {
        Debug.Log("[ServerExternalWorld] Quitting, cleaning up.");
        CleanupClient();
        try { listener?.Stop(); } catch { }
        listenerStarted = false;
    }
}
