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
    private static Dictionary<int, List<int>> acks = new Dictionary<int, List<int>>();
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
                connections[connectionId].ResetTimeout(packetId);
                //Write packet header and sliding window information
                packet.PacketHeader = new PacketHeader(PacketType.Regular, packetId);
                packet.WritePacketHeaderToPacket();
                return true;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {type}");
        }
    }

    public static void SendPacket(int connectionId, Packet packet)
    {
        MetricsManager.AddDatapointToMetric($"Sent Packet Count [{connectionId}]", 1f, true);
        PlatformPacketManager.SendPacket(connections[connectionId].udp.endPoint, packet);
    }

    public static void ReadPacket(IPEndPoint connectionEndpoint, Packet packet)
    {
        int connectionId = Array.IndexOf(connectionAddresses, connectionEndpoint.ToString());
        MetricsManager.AddDatapointToMetric($"Read Packet Count [{connectionId}]", 1f, true);

        switch (packet.PacketHeader.packetType)
        {
            case PacketType.NoACK:
                StreamManager.ReadFromPacket(connectionId, packet);
                break;
            case PacketType.Regular:
                StreamManager.ReadFromPacket(connectionId, packet);
                // Ensures that the client is not being impersonated by another by sending a false clientID
                acks[connectionId].Add(packet.PacketHeader.packetId);
                break;
            case PacketType.ACK:
                ReadACK(connectionId, packet);
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
        switch (state)
        {
            case ConnectState.Connect:
                Debug.Log($"Received Connection from: {endpoint}");
                // If this is a new connection
                connections[connectionIndex].udp.Connect(endpoint);
                GhostManager.Connect(connectionIndex);
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
                Debug.Log($"Connection {connectionIndex} acknowledged from: {endpoint}");
                connections[connectionIndex].udp.Connect(endpoint);
                connectionAddresses[connectionIndex] = endpoint.ToString();
                connectionIndex++;
                break;
        }
    }

    public static void RespondToPacketWithACK(int connectionId)
    {

        using (Packet packet = new Packet())
        {
            packet.Write(Convert.ToByte(PacketType.ACK));
            packet.Write(0);
            packet.Write(acks[connectionId].Count);
            foreach(int ack in acks[connectionId])
            {
                packet.Write(ack);
            }

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

    private static void ReadACK(int connectionId, Packet packet)
    {
        int ackCount = packet.ReadInt();
        for(int i = 0; i < ackCount; i++)
        {
            int ack = packet.ReadInt();
            SlidingWindow.WindowStatus status = connections[connectionId].window.FillFrame(ack);
            switch (status)
            {
                case SlidingWindow.WindowStatus.Success:
                    MetricsManager.AddDatapointToMetric("ACK Successful", 1f, true);
                    StreamManager.ProcessNotification(true, ack, connectionId);
                    break;
                case SlidingWindow.WindowStatus.OutOfOrder:
                    MetricsManager.AddDatapointToMetric("ACK Out of Order", 1f, true);
                    StreamManager.ProcessNotification(false, ack, connectionId);
                    break;
                case SlidingWindow.WindowStatus.Duplicate:
                    MetricsManager.AddDatapointToMetric("ACK Duplicate", 1f, true);
                    Debug.Log("Duplicate ACK received");
                    break;
                case SlidingWindow.WindowStatus.OutofBounds:
                    MetricsManager.AddDatapointToMetric("ACK Out of Bounds", 1f, true);
                    Debug.Log("ACK Returned out of bounds");
                    break;

            }
        }
    }
        
    public static void InitializeServerData(int maxPlayers)
    {
        connectionAddresses = new string[maxPlayers + 1];
        for (int i = 1; i <= maxPlayers; i++)
        {
            connections.Add(i, new Connection(i));
            acks.Add(i, new List<int>());
            GhostManager.Initialize();
            ObjectManager.Initialize();
            GhostManager.ghostConnections.Add(i, new GhostManager.GhostConnection());
            EventManager.eventConnections.Add(i, new EventConnection());
            MoveManager.moveConnections.Add(i, new MoveConnection());
        }
    }

    public static void UpdateTick()
    {
        //Write Packets to all Outgoing Connections
        foreach (Connection connection in connections.Values)
        {
            connection.UpdateTick();
            if(acks[connection.id].Count > 0)
            {
                RespondToPacketWithACK(connection.id);
                acks[connection.id].Clear();
            }
        }
    }
}
