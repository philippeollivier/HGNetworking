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
            ReadFromPacketClient(connectionId, packet);
        }
    }

    private static void ReadFromPacketServer(int connectionId, Packet packet)
    {
        int numMoves = packet.ReadInt();
        for (int i = 0; i < numMoves; i++)
        {

        }
    }

    private static void ReadFromPacketClient(int connectionId, Packet packet)
    {
        int numMoves = packet.ReadInt();
        for(int i = 0; i < numMoves; i++)
        {
            
        }

    }

    public static int WriteToPacketServer()
    {
        return 0;
    }

    public static int WriteToPacketClient(int connectionId, int remainingBytes, Packet packet)
    {
        //Write all control inputs to packets for each control

        //foreach (MoveObject control in moveConnections)
        //{
        //    control.WriteToPacket(packet);
        //}
        return 0;
    }


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

        //}
        //Store packet id -> moves 
        //Whenever we ack a packet id, remove it from that store
        //Update the outgoing moves store

        //Store sliding window of outgoing moves

        //Player moves
        //Add it to sliding window of outgoing moves

        //Sending packet
        //Send all outgoing moves in order (sliding window should be ordered)

        //Server side
        //When receive move packet, apply input to local object











        //Not this
        //This sends client side authority state for the object to the server
        //Store this list of outgoing states locally in a sliding window
        //When we receive GHOST DATA ? Remove from list of outgoing states


        //Left
        //Right Left
        //Forward Left Right
        //Back Forward Left Right


        //Left input
        //Left and Forward input
        //Left and Forward input
        //Forward input

        //100 ms 3 frames, 3 packets sent


        //You move, gets sent to server 200 ms
        //Server receives move, sends ghost info back 200 ms
        //You see that ghost moved 400 ms later, (what now?)


        //We need to be able to figure





        //Client side authority
        //





    }
