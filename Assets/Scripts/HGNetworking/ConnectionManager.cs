using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
namespace ECSSystem
{
    public static class ConnectionManager
    {

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
                    int packetId = ECSComponent.ConnectionComponent.connections[connectionId].window.AdvancePointer();
                    if (packetId == -1)
                    {
                        return false;
                    }
                    ECSComponent.ConnectionComponent.connections[connectionId].ResetTimeout(packetId);
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
            PlatformPacketManager.SendPacket(ECSComponent.ConnectionComponent.connections[connectionId].udp.endPoint, packet);
        }

        public static void ReadPacket(IPEndPoint connectionEndpoint, Packet packet)
        {
            int connectionId = Array.IndexOf(ECSComponent.ConnectionComponent.connectionAddresses, connectionEndpoint.ToString());
            MetricsManager.AddDatapointToMetric($"Read Packet Count [{connectionId}]", 1f, true);

            switch (packet.PacketHeader.packetType)
            {
                case PacketType.NoACK:
                    StreamManager.ReadFromPacket(connectionId, packet);
                    break;
                case PacketType.Regular:
                    StreamManager.ReadFromPacket(connectionId, packet);
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    ECSComponent.ConnectionComponent.acks[connectionId].Add(packet.PacketHeader.packetId);
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
            ConnectState state = (ConnectState)packet.ReadByte();
            switch (state)
            {
                case ConnectState.Connect:
                    Debug.Log($"Received Connection from: {endpoint}");
                    // If this is a new connection
                    ECSComponent.ConnectionComponent.connections[ECSComponent.ConnectionComponent.connectionIndex].udp.Connect(endpoint);
                    GhostManager.Connect(ECSComponent.ConnectionComponent.connectionIndex);
                    ECSComponent.ConnectionComponent.connectionAddresses[ECSComponent.ConnectionComponent.connectionIndex] = endpoint.ToString();
                    ECSComponent.ConnectionComponent.connectionIndex++;
                    using (Packet responsePacket = new Packet())
                    {
                        responsePacket.Write(Convert.ToByte(PacketType.Connect));
                        responsePacket.Write(0);
                        responsePacket.Write(Convert.ToByte(ConnectState.Acknowledge));
                        PlatformPacketManager.SendPacket(endpoint, responsePacket);
                    }
                    break;
                case (ConnectState.Acknowledge):
                    Debug.Log($"Connection {ECSComponent.ConnectionComponent.connectionIndex} acknowledged from: {endpoint}");
                    ECSComponent.ConnectionComponent.connections[ECSComponent.ConnectionComponent.connectionIndex].udp.Connect(endpoint);
                    ECSComponent.ConnectionComponent.connectionAddresses[ECSComponent.ConnectionComponent.connectionIndex] = endpoint.ToString();
                    ECSComponent.ConnectionComponent.connectionIndex++;
                    break;
            }
        }

        public static void RespondToPacketWithACK(int connectionId)
        {

            using (Packet packet = new Packet())
            {
                packet.Write(Convert.ToByte(PacketType.ACK));
                packet.Write(0);
                packet.Write(ECSComponent.ConnectionComponent.acks[connectionId].Count);
                foreach (int ack in ECSComponent.ConnectionComponent.acks[connectionId])
                {
                    packet.Write(ack);
                }

                PlatformPacketManager.SendPacket(ECSComponent.ConnectionComponent.connections[connectionId].udp.endPoint, packet);
            }
        }

        public static void OpenServer(int maxPlayers, int port, bool isServer)
        {
            ECSComponent.ConnectionComponent.MaxPlayers = maxPlayers;
            InitializeServerData(maxPlayers, isServer);
            Debug.Log("Starting server...");
            if (!isServer)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("ServerPhysics"), LayerMask.NameToLayer("ServerPhysics"));
            }
            PlatformPacketManager.OpenUDPSocket(port);
        }

        private static void ReadACK(int connectionId, Packet packet)
        {
            int ackCount = packet.ReadInt();
            for (int i = 0; i < ackCount; i++)
            {
                int ack = packet.ReadInt();
                SlidingWindow.WindowStatus status = ECSComponent.ConnectionComponent.connections[connectionId].window.FillFrame(ack);
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

        public static void InitializeServerData(int maxPlayers, bool isServer)
        {
            ECSComponent.ConnectionComponent.connectionAddresses = new string[maxPlayers + 1];
            GhostManager.Initialize();
            ObjectManager.Initialize();
            MoveManager.Initialize(isServer);
            for (int i = 1; i <= maxPlayers; i++)
            {
                ECSComponent.ConnectionComponent.connections.Add(i, new Connection(i));
                ECSComponent.ConnectionComponent.acks.Add(i, new List<int>());
                GhostManager.ghostConnections.Add(i, new GhostManager.GhostConnection());
                EventManager.eventConnections.Add(i, new EventConnection());
                MoveManager.moveConnections.Add(i, new MoveConnection());
            }
        }

        public static void UpdateTick()
        {
                //Write Packets to all Outgoing Connections
                foreach (Connection connection in ECSComponent.ConnectionComponent.connections.Values)
                {
                    connection.UpdateTick();
                    if (ECSComponent.ConnectionComponent.acks[connection.id].Count > 0)
                    {
                        RespondToPacketWithACK(connection.id);
                        ECSComponent.ConnectionComponent.acks[connection.id].Clear();
                    }
                }
        }
    }
}

