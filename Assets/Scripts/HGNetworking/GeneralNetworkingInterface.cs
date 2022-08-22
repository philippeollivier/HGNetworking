using System.Net;
using UnityEngine;

public class GeneralNetworkingInterface : MonoBehaviour
{
    [SerializeField] public string ipAddress = "25.15.133.160";
    [SerializeField] public string username = "AnimbotHatesECS492";
    public void StartServer()
    {
        gameObject.AddComponent<ServerSystemsManager>();
    }

    public void StartClient()
    {
        gameObject.AddComponent<ClientSystemsManager>();
    }

    public void ConnectClientToServer()
    {
        HG.Networking.ConnectionManager.EstablishNewConnection(new IPEndPoint(IPAddress.Parse(ipAddress), HG.NetworkingConstants.SERVER_PORT));
    }
    public void ConnectLocally()
    {
        HG.Networking.ConnectionManager.EstablishNewConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), HG.NetworkingConstants.SERVER_PORT));
    }

    //public void CreateGhost()
    //{
    //    //GhostManager.NewGhost(GhostManager.GhostType.Player, new Vector3(0,10,0));
    //}

    //public void GiveGhost0To1()
    //{
    //    //MoveManager.GiveControlOfGhost(1, 0);
    //}

    //public void GiveGhost1To2()
    //{
    //    //MoveManager.GiveControlOfGhost(2, 1);
    //}
    //public void Ball()
    //{
    //    //GhostManager.NewGhost(GhostManager.GhostType.Ball, new Vector3(0, 10, 0));
    //}
}
