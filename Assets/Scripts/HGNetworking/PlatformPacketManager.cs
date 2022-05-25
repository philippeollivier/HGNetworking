using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public static class PlatformPacketManager
{
    public static int Port { get; private set; }

    private static UdpClient udpListener;
    //This class handles establishing connection from player to server AND sending packet between server and player.


    //This function opens a UDP socket to receive incoming UDP data
    public static void OpenUDPSocket(int port)
    {
        udpListener = new UdpClient(port);
        udpListener.BeginReceive(UDPReceiveCallback, null);
        Debug.Log($"Listening started on port {port}.");
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

    /// <summary>Receives incoming UDP data.</summary>
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            Debug.Log("In UDP Receive Callback");
            IPEndPoint _connectionEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _connectionEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            using (Packet _packet = new Packet(_data))
            {
                ConnectionManager.ReadPacket(_connectionEndPoint, _packet);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }
}
