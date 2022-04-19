using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager
{
    //normal send packet, drop to next layer
    public void SendPacket()
    {
        //Send to Platform Packet Manager
    }

    //send packet, guaranteed
    public void SendPacket(bool guaranteed)
    {
        //Send to Platform Packet Manager
    }


    public void ReceivePacket()
    {
        //If ACK do ACK business
        //Send to stream mananger
    }

    //When you Receive Packet, with ACK required then respond that you have

    /*
        For every connect, store two sliding windows of sent and received.
        When you send an event, it goes in n

        When an event is received, add it to the sliding window locally
        Send an ACK with n in packet, where n is latest value in sliding window in order (1, 2, 3, 4, 5)

        When an ACK is returned, if it is offset by 3 then resend n-3
        If you send back 3 get ack 6, then you know everything is good

        Packet timeout as well if the ACK wasn't receieved within m seconds, resend it

        If you receive two packets with same n, don't process twice.
    */
}
