using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamManager
{
    public void writeToPacket()
    {
        //While we have info to write
            //Create new packet
            //Cycle through all managers in priority order (Event, Move, Ghost)
            //Write info from each manager into packet
            //Send packet through connection manager
            //Check if more info to write
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
    public void update()
    {
        //write to packet
    }
}
