using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public static class PlatformPacketManager
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
    public static String[] connectionAddresses;
    private static UdpClient udpListener;
    //This class handles establishing connection from player to server AND sending packet between server and player.

    //Client connect to server
    public static void Connect(string ip, int port)
    {

    }

    //Server receives incoming UDP
    public static void ReceiveConnection()
    {

    }

    //This function opens a UDP socket to receive incoming UDP data
    public static void OpenUDPSocket(int port)
    {
        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Listening started on port {Port}.");
    }

    public static void SendPacket(IPEndPoint endpoint, Packet packet)
    {
        try
        {
            if (endpoint != null)
            {
                udpListener.BeginSend(packet.ToArray(), packet.Length(), endpoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {endpoint} via UDP: {_ex}");
        }
    }
    public static void ReadPacket(int id, Packet packet)
    {
        ConnectionManager.ReadPacket(id, packet);
        //Send to connection manager
    }

    public static void OpenServer(int maxPlayers, int port )
    {
        MaxPlayers = maxPlayers;
        Port = port;
        InitializeServerData(maxPlayers);
        Debug.Log("Starting server...");

        OpenUDPSocket(port);
    }

    /// <summary>Receives incoming UDP data.</summary>
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _connectionEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _connectionEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);
            int connectionId = Array.IndexOf(connectionAddresses, _connectionEndPoint.ToString());

            using (Packet _packet = new Packet(_data))
            {

                if (connectionId == -1)
                {
                    connections[connectionId].udp.Connect(_connectionEndPoint);
                    return;
                }

                if (connections[connectionId].udp.endPoint.ToString() == _connectionEndPoint.ToString())
                {
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    connections[connectionId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    public static void InitializeServerData(int maxPlayers)
    {
        connectionAddresses = new string[maxPlayers+1];
        for(int i = 1; i <= maxPlayers; i++)
        {
            connections.Add(i, new Connection(i));
        }
    }
}
