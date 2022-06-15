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

    #region General Information For Development

    /* PACKET STRUCTURE
    General Packet Header
        int connectionId
        int packetId
        byte packetType
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
            using (Packet packet = new Packet())
            {
                //Create new packet
                bool validPacket = ConnectionManager.GetPacket(packet, PacketType.Regular, connectionId);
                if (!validPacket) {  break; }

                //Write info from each manager into packet in priority order (Move, Event, Ghost)
                Debug.Log($"Looking for data to write to packet");
                remainingBytes -= MoveManager.WriteToPacket(connectionId, remainingBytes, packet);
                remainingBytes -= EventManager.WriteToPacket(connectionId, remainingBytes, packet);
                remainingBytes -= GhostManager.WriteToPacket(connectionId, remainingBytes, packet);

                //Send packet through connection manager
                ConnectionManager.SendPacket(connectionId, packet);

                //Check if there is more info that needs to be sent
                hasInfo = MoreInfoToWrite(connectionId);
            }
        }
    }

    public static void ReadFromPacket(int connectionId, Packet packet)
    {
        //Read info and send to appropriate manager (Event, Move, Ghost)
        MoveManager.ReadFromPacket(connectionId, packet);
        EventManager.ReadFromPacket(connectionId, packet);
        GhostManager.ReadFromPacket(connectionId, packet);
    }

    public static void ProcessNotification(bool success, int packetId, int connectionId)
    {
        EventManager.ProcessNotification(success, packetId, connectionId);
        GhostManager.ProcessNotification(success, packetId, connectionId);
    }

    //Fixed Tick Update.
    public static void UpdateTick()
    {
        //Write Packets to all Outgoing Connections
        foreach(int connectionId in ConnectionManager.connections.Keys)
        {
            if(!ConnectionManager.connections[connectionId].window.IsFull && ConnectionManager.connections[connectionId].udp.endPoint != null) //TODO: remove this conditional to a function in ConnectionManager
            {
                WriteToPacket(connectionId);
            }
        }
    }


    #region Helper Functions
    private static bool MoreInfoToWrite(int connectionId)
    {
        Debug.Log("Has data to write");
        return MoveManager.HasMoreDataToWrite(connectionId) || EventManager.HasMoreDataToWrite(connectionId) || GhostManager.HasMoreDataToWrite(connectionId);
    }
    #endregion
}
