using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoveManager
{
    public static bool isServer = false;
    public static Dictionary<int, MoveConnection> moveConnections = new Dictionary<int, MoveConnection>();
    private static bool nextTick = true;
    /*Client
     *  List of Control Objects
     *  Send packets for each control objects
     *  Get info from ghostManager to make sure that server has received movement info?
     */

    public static bool HasMoreDataToWrite(int connectionId)
    {
        if (isServer == false && moveConnections.ContainsKey(1) && moveConnections[1].moveObjects.Count > 0 && nextTick)
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
            nextTick = false;
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
        int numObjects = packet.ReadInt();
        for (int i = 0; i < numObjects; i++)
        {
            int moveId = packet.ReadInt();
            moveConnections[connectionId].moveObjects[moveId].gameObject.transform.position = packet.ReadVector3();
            moveConnections[connectionId].moveObjects[moveId].gameObject.transform.rotation = packet.ReadQuaternion();
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
        
        return moveConnections[connectionId].WriteToPacket(remainingBytes, packet);
    }


    public static void Initialize(bool isServer)
    {
        MoveManager.isServer = isServer;
    }

    public static void GetControlOfGhost(int ghostId, int moveId)
    {
       
        MoveObject controller = GhostManager.localGhosts[ghostId].gameObject.AddComponent<MoveObject>();
        GhostManager.localGhosts[ghostId].isControlled = true;
        controller.Initialize(ghostId, moveId, true);
        moveConnections[1].moveObjects[moveId] = controller;
        //This should pass by reference
    }


    public static void GiveControlOfGhost(int connectionId, int ghostId)
    {
        int moveId = moveConnections[connectionId].objectId;
        moveConnections[connectionId].objectId++;
        MoveObject controller = GhostManager.ghosts[ghostId].gameObject.AddComponent<MoveObject>();
        controller.Initialize(ghostId, moveId, false);
        moveConnections[connectionId].moveObjects[moveId] = controller;
        Events.Event_GIVE_CONTROL giveControl = new Events.Event_GIVE_CONTROL();
        giveControl.ghostId = ghostId;
        giveControl.moveId = moveId;
        EventManager.QueueOutgoingEvent(giveControl, connectionId);
    }

    public static void tickAdvance()
    {
        nextTick = true;
    }




}
