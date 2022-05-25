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


    public enum PacketType : byte
    {
        Regular,
        ACK,
        Connect,
        NoACK
    }

    private enum ConnectState : byte
    {
        Connect,
        Acknowledge
    }
    public struct PacketHeader
    {
        public PacketType packetType;
        public int packetId;

        public PacketHeader(Packet packet)
        {
            packetType = (PacketType)packet.ReadByte();
            packetId = packet.ReadInt();
        }
        public PacketHeader(int packetId, PacketType packetType)
        {
            this.packetType = packetType;
            this.packetId = packetId;
        }
        public override string ToString()
        {
            return $"{{PacketId: {packetId}, PacketType: {packetType} }}";
        }
    }

    public static Packet GetPacket(PacketType type, int connectionId)
    {
        switch (type)
        {
            case PacketType.NoACK:
                using (Packet packet = new Packet())
                {
                    //Write packet header information
                    packet.Write(connectionId);
                    //Get this from the sliding window
                    packet.Write(0);
                    packet.Write(Convert.ToByte(PacketType.NoACK));
                    return packet;
                }
            case PacketType.Regular:
                int packetId = connections[connectionId].window.AdvancePointer();
                if (packetId == -1) {
                    return null;
                }
                using (Packet packet = new Packet())
                {
                    //Write packet header information
                    packet.Write(connectionId);
                    //Get this from the sliding window
                    packet.Write(packetId);
                    packet.Write(Convert.ToByte(PacketType.Regular));
                    return packet;
                }
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

        //Read Packet Header
        PacketHeader packetHeader = new PacketHeader(packet);
        Debug.Log($"Packet recieved from {connectionId} {packetHeader}");
        switch (packetHeader.packetType)
        {
            case PacketType.NoACK:
                StreamManager.ReadFromPacket(connectionId, packetHeader.packetId, packet);
                break;
            case PacketType.Regular:
                StreamManager.ReadFromPacket(connectionId, packetHeader.packetId, packet);
                // Ensures that the client is not being impersonated by another by sending a false clientID
                RespondToPacketWithACK(connectionId, packetHeader);
                break;
            case PacketType.ACK:
                ReadACK(connectionId, packetHeader);
                //TODO ACK Business
                break;
            case PacketType.Connect:
                ReadConnect(packet, connectionEndpoint);
                break;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {packetHeader}");
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





    //Sent a packet, ACK header is set to last free value in sliding window (sent).

    //Client receives packet with that ACK header
    //Adds it to local sliding window (received)
    //Send packet back to server with ACK number

    //How do we differentiate ACK packets, from they want us to ACK

    //Solution: Sequence Header

    //Operation-ACK
    //00000000 00000000
    //Server to => Client 00000001 00000011 [Rest of Data] (Sent packet)           Operation = 00000001 means this packet must be ACKed ACK = 00000011 means it is locally sliding window 3
    //Client receives this packet, see it needs to be ACKed

    //Option 2)
    //Client add to local sliding window for received
    //Sends packet back to server as 00000010 00000011          Operation = 00000010 means this packet is ACKiong    ACK = 00000011 means it is servers sliding window 3

    //Server receives packet 00000010 00000011
    //Whenever we receive a packet ACK, then call stream manager callback
    //If we consider a packet dropped, then call stream manager dropped callback

    //Whenever we send a packet set its sentPackets[id] to 1
    //We store a byte array for each packet sent, (PacketId % SlidingWindowSize)
    //Whenever we receive a packet, send ACK for that packet number.
    //If SentPackets[(PacketId-timeout) wrapped] = 0 then dead callback

    //TODO: also add a proper timeout for ACKs

    //00000000 non ack
    //00000001 ack
    //00000001 please ack

    /*
        For every connection, store two sliding windows of sent and received.
        When you send an event, it goes in sent

        When an event is received, add it to the sliding window locally
        Send an ACK with n in packet, where n is latest value in sliding window in order (1, 2, 3, 4, 5)

        When an ACK is returned, if it is offset by 3 then resend n-3
        If you send back 3 get ack 6, then you know everything is good

        Packet timeout as well if the ACK wasn't receieved within 2 seconds, resend it

        If you receive two packets with same n, don't process twice.
    */




    //Map of Pairs of sliding window
    //Outgoing
    //Incoming


    //When packet is received 

    //When ACK is received then remove it from outgoing.
    //Send ACK to stream manager


}
