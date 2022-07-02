using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoveManager
{
    public static bool isServer = false;
    public static Dictionary<int, MoveConnection> moveConnections = new Dictionary<int, MoveConnection>();
    public static ClientMoveController moveController;
    /*Client
     *  List of Control Objects
     *  Send packets for each control objects
     *  Get info from ghostManager to make sure that server has received movement info?
     */

    public static bool HasMoreDataToWrite(int connectionId)
    {
        return false;
    }

    public static int WriteToPacket(int connectionId, int remainingBytes, Packet packet)
    {
        if(isServer)
        {
            return WriteToPacketServer();
        } else
        {
            return 0;
            //return WriteToPacketClient(connectionid, remainingbytes, packet);
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
        int numMoves = packet.ReadInt();
        for (int i = 0; i < numMoves; i++)
        {

        }
    }

    public static int WriteToPacketServer()
    {
        return 0;
    }

    //public static int WriteToPacket(int connectionId, int remainingBytes, Packet packet)
    //{
    //    return moveController.WriteToPacket(remainingBytes, packet);
    //}


    public static void Initialize(bool isServer)
    {
        MoveManager.isServer = isServer;
        if(isServer)
        {
            moveConnections = new Dictionary<int, MoveConnection>();
        } else
        {
           moveController = ObjectManager.Instance.CreateClientMoveController();
        }

    }




    }
