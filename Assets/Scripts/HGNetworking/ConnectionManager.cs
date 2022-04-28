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

    //send packet, ack guaranteed
    public void SendPacket(bool guaranteed)
    {
        //Send to Platform Packet Manager
    }

    public void ReceivePacket()
    {
        //If ACK do ACK business
        //Send to stream mananger

        //Send ACK packets here? 
    }

    //Sent a packet, ACK header is set to last free value in sliding window (sent).

    //Client receives packet with that ACK header
    //Adds it to local sliding window (received)
    //Send packet back to server with ACK number

    //How do we differentiate ACK packets, from they want us to ACK

    //Solution: Sequence Header

    //Operation-ACK
    //00000000 00000000
    //Server to => Client 00000001 00000011 [Rest of Data] (Sent packet)           Operation = 00000001 means this packet must be ACKed ACK = 00000011 means it is locally sliding window 3
    //Client receives this packet, see it needs to be ACKed

    //Option 2)
    //Client add to local sliding window for received
    //Sends packet back to server as 00000010 00000011          Operation = 00000010 means this packet is ACKiong    ACK = 00000011 means it is servers sliding window 3

    //Server receives packet 00000010 00000011
    //Whenever we receive a packet ACK, then call stream manager callback
    //If we consider a packet dropped, then call stream manager dropped callback

    //Whenever we send a packet set its sentPackets[id] to 1
    //We store a byte array for each packet sent, (PacketId % SlidingWindowSize)
    //Whenever we receive a packet, send ACK for that packet number.
    //If SentPackets[(PacketId-timeout) wrapped] = 0 then dead callback

    //TODO: also add a proper timeout for ACKs

    //00000000 non ack
    //00000001 ack
    //00000001 please ack

    /*
        For every connect, store two sliding windows of sent and received.
        When you send an event, it goes in n

        When an event is received, add it to the sliding window locally
        Send an ACK with n in packet, where n is latest value in sliding window in order (1, 2, 3, 4, 5)

        When an ACK is returned, if it is offset by 3 then resend n-3
        If you send back 3 get ack 6, then you know everything is good

        Packet timeout as well if the ACK wasn't receieved within 2 seconds, resend it

        If you receive two packets with same n, don't process twice.
    */
}
