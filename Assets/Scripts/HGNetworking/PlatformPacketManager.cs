using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

//This class handles UDP Connection (Sending/Receiving packets) between player and server. 
public static class PlatformPacketManager
{
    private static UdpClient udpListener;

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
            else
            {
                Debug.LogError("PlatformPacketManager failed to SendPacket due to endpoint being null.");
            }
        }
        catch (Exception _ex)
        {
            Debug.LogError($"Error sending data to {endpoint} via UDP: {_ex}");
        }
    }

    /// <summary>Receives incoming UDP data asynchronously.</summary>
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _connectionEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _connectionEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);
            NetworkingThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data, true))
                {
                    ConnectionManager.ReadPacket(_connectionEndPoint, _packet);
                }
            });

        }
        catch (Exception _ex)
        {
            Debug.LogError($"Error receiving UDP data: {_ex}");
        }
    }
}