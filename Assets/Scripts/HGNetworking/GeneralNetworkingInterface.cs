using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GeneralNetworkingInterface : MonoBehaviour
{
    [SerializeField]
    public string ipAddress = "25.15.133.160";
    private const int SERVER_PORT = 6942;
    private const int CLIENT_PORT = 6943;

    private void FixedUpdate()
    {
        ConnectionManager.UpdateTick();
        StreamManager.UpdateTick();
        MoveManager.tickAdvance();
    }
    
    public void StartServer()
    {
        ConnectionManager.OpenServer(5, SERVER_PORT, true);
    }

    public void StartClient()
    {
        ConnectionManager.OpenServer(5, CLIENT_PORT, false);
    }

    public void ConnectClientToServer()
    {
        ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), SERVER_PORT));
    }
    public void ConnectLocally()
    {
        ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), SERVER_PORT));
    }

    public void CreateGhost()
    {
        GhostManager.NewGhost(GhostManager.GhostType.Player, new Vector3(0,10,0));
    }

    public void GiveGhost0To1()
    {
        MoveManager.GiveControlOfGhost(1, 0);
    }

    public void GiveGhost1To2()
    {
        MoveManager.GiveControlOfGhost(2, 1);
    }
    public void Ball()
    {
        GhostManager.NewGhost(GhostManager.GhostType.Ball, new Vector3(0, 10, 0));
    }
}
