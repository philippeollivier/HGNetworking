using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager
{
    public bool isServer = false;
    public Dictionary<int, MoveConnection> moveConnections = new Dictionary<int, MoveConnection>();
    private bool nextTick = true;
    /*Client
     *  List of Control Objects
     *  Send packets for each control objects
     *  Get info from ghostManager to make sure that server has received movement info?
     */

    public bool HasMoreDataToWrite()
    {
        if (isServer == false && moveConnections.ContainsKey(1) && moveConnections[1].moveObjects.Count > 0 && nextTick)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public int WriteToPacket(int remainingBytes, Packet packet)
    {
        if(isServer)
        {
            return 0;
            //return WriteToPacketServer();
        } else
        {
            nextTick = false;
            return WriteToPacketClient(remainingBytes, packet);
        }
    }

    public void ReadFromPacket(int connectionId, Packet packet)
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

    private void ReadFromPacketServer(int connectionId, Packet packet)
    {
        int numObjects = packet.ReadInt();
        for (int i = 0; i < numObjects; i++)
        {
            int moveId = packet.ReadInt();
            moveConnections[connectionId].moveObjects[moveId].gameObject.transform.localPosition = packet.ReadVector3();
            moveConnections[connectionId].moveObjects[moveId].gameObject.transform.localRotation = packet.ReadQuaternion();
        }
    }


    private void ReadFromPacketClient(int connectionId, Packet packet)
    {
        //No Implementation for client side
    }


    public int WriteToPacketServer()
    {
        //No implementation for client side
        return 0;
    }

    public int WriteToPacketClient(int remainingBytes, Packet packet)
    {
        return 0;
        //return moveConnections[connectionId].WriteToPacket(remainingBytes, packet);
    }


    public void Initialize(bool isServer)
    {
        //HG.Networking.MoveManager.isServer = isServer;
    }

    public void GetControlOfGhost(int ghostId, int moveId)
    {
       
        //MoveObject controller = HG.Networking.GhostManager.localGhosts[ghostId].gameObject.AddComponent<MoveObject>();
        //HG.Networking.GhostManager.localGhosts[ghostId].isControlled = true;
        //controller.Initialize(ghostId, moveId, true);
        //moveConnections[1].moveObjects[moveId] = controller;
        ////This should pass by reference
    }


    public void GiveControlOfGhost(int connectionId, int ghostId)
    {
        //int moveId = moveConnections[connectionId].objectId;
        //moveConnections[connectionId].objectId++;
        //MoveObject controller = HG.Networking.GhostManager.ghosts[ghostId].gameObject.AddComponent<MoveObject>();
        //controller.Initialize(ghostId, moveId, false);
        //moveConnections[connectionId].moveObjects[moveId] = controller;
        //Events.Event_GIVE_CONTROL giveControl = new Events.Event_GIVE_CONTROL();
        //giveControl.ghostId = ghostId;
        //giveControl.moveId = moveId;
        //HG.Networking.EventManager.QueueOutgoingEvent(giveControl, connectionId);
    }

    public void tickAdvance()
    {
        nextTick = true;
    }




}
