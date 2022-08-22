using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

using ConnectionId = System.Int32;
using ConnectionAddress = System.String;
public enum ConnectState : byte
{
    Connect,
    Acknowledge
}

public class ConnectionManager
{
    public Dictionary<ConnectionId, Connection> connections = new Dictionary<ConnectionId, Connection>();
    private Dictionary<ConnectionAddress, ConnectionId> connectionAddresses = new Dictionary<ConnectionAddress, ConnectionId>();
    private int latestValidConnectionIndex = 1;

    public void FixedUpdate()
    {
        foreach (Connection connection in connections.Values)
        {
            connection.FixedUpdate();
        }
    }

    #region Incoming Packets Utils
    public void ReadPacket(IPEndPoint connectionEndpoint, Packet packet)
    {
        ConnectionId connectionId;
        connectionAddresses.TryGetValue(connectionEndpoint.ToString(), out connectionId);

        switch (packet.PacketHeader.packetType)
        {
            case PacketType.NoACK:
                connections[connectionId].ReadPacket(packet);
                break;
            case PacketType.Regular:
                connections[connectionId].ReadPacket(packet, true);
                break;
            case PacketType.ACK:
                connections[connectionId].ReadACK(packet);
                break;
            case PacketType.Connection:
                ReadConnectionPacket(packet, connectionEndpoint);
                break;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {packet.PacketHeader}");
        }
    }

    #endregion

    #region Connection Functionality
    public void EstablishNewConnection(IPEndPoint endpoint)
    {
        SendConnectionPacket(endpoint, ConnectState.Connect);
    }

    public void SendConnectionPacket(IPEndPoint endpoint, ConnectState connectState)
    {
        using (Packet packet = new Packet())
        {
            packet.PacketHeader = new PacketHeader(PacketType.Connection, 0, ECS.Components.SynchronizedClock.CommandFrame);
            packet.WritePacketHeaderToPacket();
            packet.Write(Convert.ToByte(connectState));
            PlatformPacketManager.SendPacket(endpoint, packet);
        }
    }

    public void ReadConnectionPacket(Packet packet, IPEndPoint endpoint)
    {
        ConnectState state = (ConnectState)packet.ReadByte();
        switch (state)
        {
            case ConnectState.Connect:
                Debug.Log($"Received a new connection request from: {endpoint}. Attempting to connect via UDP...");
                connections[latestValidConnectionIndex] = new Connection(latestValidConnectionIndex, endpoint);
                connectionAddresses[endpoint.ToString()] = latestValidConnectionIndex++;
                SendConnectionPacket(endpoint, ConnectState.Acknowledge);
                break;
            case (ConnectState.Acknowledge):
                Debug.Log($"Connection {latestValidConnectionIndex} acknowledged from: {endpoint}. Establishing connection on local UDP.");
                connections[latestValidConnectionIndex] = new Connection(latestValidConnectionIndex, endpoint);
                connectionAddresses[endpoint.ToString()] = latestValidConnectionIndex++;
                break;
        }
    }

    #endregion
}