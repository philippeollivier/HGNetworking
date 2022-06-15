using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public static class ConnectionManager
{
    public static int MaxPlayers { get; private set; }
    public static Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
    public static string[] connectionAddresses;
    private static int connectionIndex = 1;

    public enum ConnectState : byte
    {
        Connect,
        Acknowledge
    }

    public static bool GetPacket(Packet packet, PacketType type, int connectionId)
    {
        switch (type)
        {
            case PacketType.NoACK:
                //Write packet header and sliding window information
                packet.PacketHeader = new PacketHeader(PacketType.NoACK, 0);
                packet.WritePacketHeaderToPacket();
                return true;
            case PacketType.Regular:
                int packetId = connections[connectionId].window.AdvancePointer();
                if (packetId == -1) {
                    return false;
                }
                //Write packet header and sliding window information
                packet.PacketHeader = new PacketHeader(PacketType.Regular, packetId);
                packet.WritePacketHeaderToPacket();
                return true;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {type}");
        }
    }
    //send given packet to a connection with id
    public static void SendPacket(int connectionId, Packet packet)
    {
        PlatformPacketManager.SendPacket(connections[connectionId].udp.endPoint, packet);
    }

    public static void ReadPacket(IPEndPoint connectionEndpoint, Packet packet)
    {
        int connectionId = Array.IndexOf(connectionAddresses, connectionEndpoint.ToString());

        switch (packet.PacketHeader.packetType)
        {
            case PacketType.NoACK:
                StreamManager.ReadFromPacket(connectionId, packet);
                break;
            case PacketType.Regular:
                StreamManager.ReadFromPacket(connectionId, packet);
                // Ensures that the client is not being impersonated by another by sending a false clientID
                RespondToPacketWithACK(connectionId, packet.PacketHeader);
                break;
            case PacketType.ACK:
                ReadACK(connectionId, packet.PacketHeader);
                //TODO ACK Business
                break;
            case PacketType.Connect:
                ReadConnect(packet, connectionEndpoint);
                break;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {packet.PacketHeader}");
        }
    }

    public static void Connect(IPEndPoint endpoint)
    {
        using (Packet packet = new Packet())
        {
            packet.Write(Convert.ToByte(PacketType.Connect));
            packet.Write(0);
            packet.Write(Convert.ToByte(ConnectState.Connect));
            PlatformPacketManager.SendPacket(endpoint, packet);
        }
    }

    public static void ReadConnect(Packet packet, IPEndPoint endpoint)
    {
        ConnectState state = (ConnectState) packet.ReadByte();
        Debug.Log(state);
        switch (state)
        {
            case ConnectState.Connect:
                Debug.Log($"Received Connection from: {endpoint.ToString()}");
                // If this is a new connection
                connections[connectionIndex].udp.Connect(endpoint);
                GhostManager.ghostConnections[connectionIndex].Connect(connectionIndex);
                connectionAddresses[connectionIndex] = endpoint.ToString();
                connectionIndex++;
                using (Packet responsePacket = new Packet())
                {
                    responsePacket.Write(Convert.ToByte(PacketType.Connect));
                    responsePacket.Write(0);
                    responsePacket.Write(Convert.ToByte(ConnectState.Acknowledge));
                    PlatformPacketManager.SendPacket(endpoint, responsePacket);
                }
                break;
            case (ConnectState.Acknowledge):
                    Debug.Log(connectionIndex);
                    Debug.Log(connections[connectionIndex]);
                    Debug.Log($"Connection acknowledged from: {endpoint.ToString()}");
                    connections[connectionIndex].udp.Connect(endpoint);
                    connectionAddresses[connectionIndex] = endpoint.ToString();
                    connectionIndex++;
                break;
        }
    }

    public static void RespondToPacketWithACK(int connectionId, PacketHeader packetHeader)
    {

        using (Packet packet = new Packet())
        {
            packet.Write(Convert.ToByte(PacketType.ACK));
            packet.Write(packetHeader.packetId);
            PlatformPacketManager.SendPacket(connections[connectionId].udp.endPoint, packet);
        }
    }

    public static void OpenServer(int maxPlayers, int port)
    {
        MaxPlayers = maxPlayers;
        InitializeServerData(maxPlayers);
        Debug.Log("Starting server...");

        PlatformPacketManager.OpenUDPSocket(port);
    }

    private static void ReadACK(int connectionId, PacketHeader packet)
    {

        SlidingWindow.WindowStatus status = connections[connectionId].window.FillFrame(packet.packetId);
        switch (status)
        {
            case SlidingWindow.WindowStatus.Success:
                StreamManager.ProcessNotification(true, packet.packetId, connectionId);
                break;
            case SlidingWindow.WindowStatus.OutOfOrder:
                StreamManager.ProcessNotification(false, packet.packetId, connectionId);
                break;
            case SlidingWindow.WindowStatus.Duplicate:
                Debug.Log("Duplicate ACK received");
                break;
            case SlidingWindow.WindowStatus.OutofBounds:
                Debug.Log("ACK Returned out of bounds");
                break;
        }
    }

    public static void InitializeServerData(int maxPlayers)
    {
        connectionAddresses = new string[maxPlayers + 1];
        for (int i = 1; i <= maxPlayers; i++)
        {
            connections.Add(i, new Connection(i, 1000));
            GhostManager.Initialize();
            ObjectManager.Initialize();
            GhostManager.ghostConnections.Add(i, new GhostManager.GhostConnection());
            EventManager.eventConnections.Add(i, new EventConnection());
        }
    }

    public static void UpdateTick()
    {
        //Write Packets to all Outgoing Connections
        foreach (Connection connection in connections.Values)
        {
            connection.UpdateTick();
        }
    }
}
