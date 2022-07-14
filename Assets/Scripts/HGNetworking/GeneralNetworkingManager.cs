using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GeneralNetworkingManager : MonoBehaviour
{
    [Header("Testing Values")]
    [SerializeField]
    public string ipAddress = "25.15.133.160";
    [SerializeField]
    private bool selfConnection = true;

    private const int SERVER_PORT = 6942;
    private const int CLIENT_PORT = 6943;

    private void FixedUpdate()
    {
        ConnectionManager.UpdateTick();
        StreamManager.UpdateTick();
        MoveManager.tickAdvance();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
        }
    }
    
    public void StartServer()
    {
        ConnectionManager.OpenServer(5, SERVER_PORT, false);
    }

    public void StartClient()
    {
        ConnectionManager.OpenServer(5, CLIENT_PORT, true);
    }

    public void ConnectClientToServer()
    {
        ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), SERVER_PORT));
    }
    public void ConnectLocally()
    {
        ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), SERVER_PORT));
    }

    public void GiveControlOfGhost()
    {
        MoveManager.GiveControlOfGhost(1, 1);
    }
}
