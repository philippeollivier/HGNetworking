using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamManager
{
    public ConnectionManager connectionManager;


    public void WriteToPacket(int connectionId)
    {
        bool hasInfo = moreInfoToWrite(connectionId);
        int remainingBytes;
        while (hasInfo)
        {
            //Create new packet
            //Packet packet = new Packet();

            //Write packet header information
            //TODO

            //Write info from each manager into packet in priority order (Move, Event, Ghost)
            remainingBytes -= MoveManager.WriteToPacket(connectionId, remainingBytes, packet);
            remainingBytes -= EventManager.WriteToPacket(connectionId, remainingBytes, packet);
            remainingBytes -= GhostManager.WriteToPacket(connectionId, remainingBytes, packet);

            //Send packet through connection manager
            //connectionManager.SendPacket(connectionId, packet);

            //Check if there is more info that needs to be sent
            hasInfo = moreInfoToWrite(connectionId);
        }

    }


    public void readFromPacket()
    {
        //Read info and send to appropriate manager (Event, Move, Ghost)
    }

    public void processNotification()
    {
        //Write to each manager that needs ACK
    }

    //Fixed tick update n times a second
    //Should also be a late update, like only after whole frame has been processed
    public void update()
    {
        //write to packet to all outgoing connects we can get that from platform packet manager
    }


    #region Helper Functions
    private bool MoreInfoToWrite(int connectionId)
    {
        return MoveManager.HasMoreDataToWrite(connectionId) || EventManager.HasMoreDataToWrite(connectionId) || GhostManager.HasMoreDataToWrite(connectionId);
    }
    #endregion
}
