using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StreamManager
{
    //TODO don't magic number this
    public static int MAX_PACKET_SIZE = 2048;
    public static int MIN_HEADER_SIZE = 4 + 4 + 4 + 4 + 4; //4 Bytes for ConnectionId, PacketId, MoveManager Header, EventManager Header, GhostManager Header

    public static Dictionary<int, int> latestPacket = new Dictionary<int, int>();

    public enum PacketType : byte { 
        Regular,
        ACK
    }

    #region General Information For Development

    /* PACKET STRUCTURE
    General Packet Header
        int connectionId
        int packetId
        byte packetFlags
    MoveManager info
        int number of moves
        Moves
    EventManager
        int number of events
        Events
    GhostManager
        int number of ghosts
        Ghosts
    */

    #endregion

    public static void WriteToPacket(int connectionId)
    {
        bool hasInfo = MoreInfoToWrite(connectionId);
        int remainingBytes = MAX_PACKET_SIZE - MIN_HEADER_SIZE;
        while (hasInfo)
        {
            //Create new packet
            Packet packet = new Packet();
            int packetId = GetLatestPacketId(connectionId);

            //Write packet header information
            packet.Write(connectionId);
            packet.Write(packetId);
            packet.Write(Convert.ToByte(PacketType.Regular));

            //Write info from each manager into packet in priority order (Move, Event, Ghost)
            remainingBytes -= MoveManager.WriteToPacket(connectionId, remainingBytes, packetId, ref packet);
            remainingBytes -= EventManager.WriteToPacket(connectionId, remainingBytes, packetId, ref packet);
            remainingBytes -= GhostManager.WriteToPacket(connectionId, remainingBytes, packetId, ref packet);

            //Send packet through connection manager
            //ConnectionManager.SendPacket(connectionId, packet);

            //Check if there is more info that needs to be sent
            hasInfo = MoreInfoToWrite(connectionId);
        }
    }

    public static void ReadFromPacket(Packet packet)
    {
        //Read Packet Header
        int connectionId = packet.ReadInt();
        int packetId = packet.ReadInt();

        PacketType packetType = (PacketType) packet.ReadByte();

        //Read info and send to appropriate manager (Event, Move, Ghost)
        if(PacketType.Regular == packetType)
        {
            MoveManager.ReadFromPacket(connectionId, packetId, ref packet);
            EventManager.ReadFromPacket(connectionId, packetId, ref packet);
            GhostManager.ReadFromPacket(connectionId, packetId, ref packet);
        }
        else if (PacketType.ACK == packetType)
        {
            //TODO
        }
    }

    public static void ProcessNotification(Packet packet)
    {
        //Write to each manager that needs ACK
        //How are we ACK
        
    }

    //Fixed Tick Update.
    //TODO: Should be called in another class in its late update
    public static void UpdateTick()
    {
        //Write Packets to all Outgoing Connections
        foreach(int connectionId in PlatformPacketManager.connections.Keys)
        {
            WriteToPacket(connectionId);
        }
    }


    #region Helper Functions
    private static bool MoreInfoToWrite(int connectionId)
    {
        return MoveManager.HasMoreDataToWrite(connectionId) || EventManager.HasMoreDataToWrite(connectionId) || GhostManager.HasMoreDataToWrite(connectionId);
    }

    private static int GetLatestPacketId(int connectionId)
    {
        if (latestPacket.ContainsKey(connectionId))
        {
            return ++latestPacket[connectionId];
        }
        else
        {
            latestPacket[connectionId] = 0;
            return 0;
        }
    }
    #endregion
}
