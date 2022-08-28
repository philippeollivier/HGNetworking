using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoveManager
{
    public static bool isServer = false;
    /*Client
     *  List of Control Objects
     *  Send packets for each control objects
     *  Get info from ghostManager to make sure that server has received movement info?
     */

    public static bool HasMoreDataToWrite(int connectionId)
    {
        if (isServer == false && ECS.Components.InputComponent.readThisFrame)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public static int WriteToPacket(int connectionId, int remainingBytes, Packet packet)
    {
        if(isServer)
        {
            return 0;
            //return WriteToPacketServer();
        } else
        {
            return WriteToPacketClient(connectionId, remainingBytes, packet);
        }
    }

    public static void ReadFromPacket(int connectionId, Packet packet)
    {
        if (isServer)
        {
            ReadFromPacketServer(connectionId, packet);
        }
        else
        {
            //ReadFromPacketClient(connectionId, packet);
        }
    }

    private static void ReadFromPacketServer(int connectionId, Packet packet)
    {
        int startFrame = packet.ReadInt();
        int frameCount = packet.ReadInt();
        for(int i = startFrame; i < frameCount; i++)
        {
            Byte[] flags = packet.ReadBytes(ECS.Components.InputComponent.sizeOfFlags);
            ECS.Components.InputHistoryServerComponent.flagHistory[connectionId][i] = new BitArray(flags);
        }
    }


    private static void ReadFromPacketClient(int connectionId, Packet packet)
    {
        //No Implementation for client side
    }


    public static int WriteToPacketServer()
    {
        //No implementation for client side
        return 0;
    }

    public static int WriteToPacketClient(int connectionId, int remainingBytes, Packet packet)
    {
        ECS.Components.InputComponent.readThisFrame = true;

        int size = sizeof(int) + sizeof(int);
        packet.Write(ECS.Components.InputComponent.mostRecentNonAckedMoveFrame);
        packet.Write(ECS.Components.InputComponent.flagHistory.Count);
        foreach(BitArray flags in ECS.Components.InputComponent.flagHistory.Values)
        {
            ECS.Components.InputComponent.packetToMoveHistory[packet.PacketHeader.packetId] = ECS.Components.InputComponent.mostRecentNonAckedMoveFrame + ECS.Components.InputComponent.flagHistory.Count - 1;
            byte[] byteFlags = new byte[ECS.Components.InputComponent.sizeOfFlags];
            flags.CopyTo(byteFlags, 0);
            size += sizeof(byte) * ECS.Components.InputComponent.sizeOfFlags;
            packet.Write(byteFlags);
        }
        return size;
    }

    public static void Initialize(bool isServer)
    {
        MoveManager.isServer = isServer;
    }

    public static void GetControlOfGhost(int ghostId, int moveId)
    {
       
    //    MoveObject controller = GhostManager.localGhosts[ghostId].gameObject.AddComponent<MoveObject>();
    //    GhostManager.localGhosts[ghostId].isControlled = true;
    //    controller.Initialize(ghostId, moveId, true);
    //    moveConnections[1].moveObjects[moveId] = controller;
    //    //This should pass by reference
    }

    public static void ProcessNotification(bool success, int packetId, int connectionid)
    {
        if(success && !isServer)
        {
            for(int i = ECS.Components.InputComponent.mostRecentNonAckedMoveFrame; i < ECS.Components.InputComponent.packetToMoveHistory[packetId]; i++)
            {
                ECS.Components.InputComponent.flagHistory.Remove(i);
            }
        }
    }


    public static void GiveControlOfGhost(int connectionId, int ghostId)
    {
    //    int moveId = moveConnections[connectionId].objectId;
    //    moveConnections[connectionId].objectId++;
    //    MoveObject controller = GhostManager.ghosts[ghostId].gameObject.AddComponent<MoveObject>();
    //    controller.Initialize(ghostId, moveId, false);
    //    moveConnections[connectionId].moveObjects[moveId] = controller;
    //    Events.Event_GIVE_CONTROL giveControl = new Events.Event_GIVE_CONTROL();
    //    giveControl.ghostId = ghostId;
    //    giveControl.moveId = moveId;
    //    EventManager.QueueOutgoingEvent(giveControl, connectionId);
    }

    public static void UpdateTick()
    {
        ECS.Components.InputComponent.readThisFrame = false;
    }

    //Startframe
    //Num of frames
    //All of the frames
    //Start frame -> num of Frame-1
    // Lots of empty inputs if afk. Bad? Mad?


    //Num Of Moves
    //Move frame ID
    //Frame
    //Move Frame ID
    //Frame

}
