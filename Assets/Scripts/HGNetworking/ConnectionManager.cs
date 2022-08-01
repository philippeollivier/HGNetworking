using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public static class ConnectionManager
{
    public static int MaxPlayers { get; set; }
    public static Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
    public static string[] connectionAddresses;
    public static int connectionIndex = 1;
    public static Dictionary<int, List<int>> acks = new Dictionary<int, List<int>>();

    public enum ConnectState : byte
    {
        Connect,
        Acknowledge
    }

    public static void UpdateTick()
    {
        //Write Packets to all Outgoing Connections
        foreach (Connection connection in connections.Values)
        {
            connection.UpdateTick();
            if (acks[connection.id].Count > 0)
            {
                RespondToPacketWithACK(connection.id);
                acks[connection.id].Clear();
            }
        }
    }

    #region Outgoing Packets Utils
    public static bool GetPacket(Packet packet, PacketType type, int connectionId)
    {
        switch (type)
        {
            case PacketType.NoACK:
                packet.PacketHeader = new PacketHeader(PacketType.NoACK, 0);
                packet.WritePacketHeaderToPacket();
                return true;
            case PacketType.Regular:
                int packetId = connections[connectionId].window.AdvancePointer();
                if (packetId == -1)
                {
                    return false;
                }
                connections[connectionId].ResetTimeout(packetId);
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

    public static void SendConnectionPacket(IPEndPoint endpoint)
    {
        using (Packet packet = new Packet())
        {
            packet.Write(Convert.ToByte(PacketType.Connection));
            packet.Write(0);
            packet.Write(Convert.ToByte(ConnectState.Connect));
            PlatformPacketManager.SendPacket(endpoint, packet);
        }
    }
    #endregion

    #region Incoming Packets Utils
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
                break;
            case PacketType.Connection:
                ReadConnectionPacket(packet, connectionEndpoint);
                break;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {packet.PacketHeader}");
        }
    }

    public static void ReadConnectionPacket(Packet packet, IPEndPoint endpoint)
    {
        ConnectState state = (ConnectState)packet.ReadByte();
        switch (state)
        {
            case ConnectState.Connect:
                Debug.Log($"Received a new connection from: {endpoint}");

                connections[connectionIndex].udp.Connect(endpoint);
                GhostManager.Connect(connectionIndex);
                connectionAddresses[connectionIndex] = endpoint.ToString();
                connectionIndex++;
                using (Packet responsePacket = new Packet())
                {
                    responsePacket.Write(Convert.ToByte(PacketType.Connection));
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
    #endregion

    #region ACK
    public static void RespondToPacketWithACK(int connectionId)
    {

        using (Packet packet = new Packet())
        {
            packet.Write(Convert.ToByte(PacketType.ACK));
            packet.Write(0);
            packet.Write(acks[connectionId].Count);
            foreach (int ack in acks[connectionId])
            {
                packet.Write(ack);
            }

            PlatformPacketManager.SendPacket(connections[connectionId].udp.endPoint, packet);
        }
    }

    private static void ReadACK(int connectionId, Packet packet)
    {
        int ackCount = packet.ReadInt();
        for (int i = 0; i < ackCount; i++)
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
    #endregion

    #region Server Utils
    public static void OpenServer(int maxPlayers, int port, bool isServer)
    {
        MaxPlayers = maxPlayers;
        InitializeServerData(maxPlayers, isServer);
        Debug.Log("Starting server...");
        if (!isServer)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("ServerPhysics"), LayerMask.NameToLayer("ServerPhysics"));
        }
        PlatformPacketManager.OpenUDPSocket(port);
    }

   

    public static void InitializeServerData(int maxPlayers, bool isServer)
    {
        connectionAddresses = new string[maxPlayers + 1];
        GhostManager.Initialize();
        ObjectManager.Initialize();
        MoveManager.Initialize(isServer);
        for (int i = 1; i <= maxPlayers; i++)
        {
            connections.Add(i, new Connection(i));
            acks.Add(i, new List<int>());
            GhostManager.ghostConnections.Add(i, new GhostManager.GhostConnection());
            EventManager.eventConnections.Add(i, new EventConnection());
            MoveManager.moveConnections.Add(i, new MoveConnection());
        }
    }
    #endregion
}

