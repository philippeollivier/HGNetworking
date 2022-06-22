using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;


public class GeneralNetworkingManager : MonoBehaviour
{
    [Header("Testing Values")]
    [SerializeField]
    private string ipAddress = "25.15.133.160";
    [SerializeField]
    private bool selfConnection = true;

    private const int SERVER_PORT = 6942;
    private const int CLIENT_PORT = 6943;

    private void FixedUpdate()
    {
        ConnectionManager.UpdateTick();
        StreamManager.UpdateTick();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse((selfConnection)?("127.0.0.1"):(ipAddress)), SERVER_PORT));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectionManager.OpenServer(5, SERVER_PORT);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ConnectionManager.OpenServer(5, CLIENT_PORT);
        }
    }
}
