using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager
{
    //Ghost Manager is always server -> many client info
    //For every gameobject we want to ghost, we store it in a map
    //In the map we store the most recent ghost info for the object.

    //Ghost info has flags then Ghost Object ID
    //00000000 - 00000000
    //first bit is transform
    //second bit is scale
    //third bit is rotation
    //fourth bit could be something like create object (with object ID)
    //fifth bit can be destroy object



    //We use the flags to see what we are writing/reading from the packet

    //We use Most Recent State always, so once we have been ACK'd about a certain update we don't need to send it anymore until it changes.




    //Packet id 1, 11100000 -> to all clients
    //client 1 returns ack

    //You know client 1 has MRS, so for his ghost packet, you don't need to send that info anymore
    //Client 2 doesn't return ack, so keep sending the entire ghost update until ack'd.

    //Packet id 1

    //Packet id 2, 11110000 -> to all clients
    //Packet id 3, 11101000 -> to all clients

    //whenever you are making the packet, you OR the state flags until they have been acknowledged.

    //Make sure they have the most recent updates guaranteed.

    //We store outgoing packet in sliding window.
    //Whenever we receive an ACK, remove that outgoing packet. 
    //Whenever we are sending the data, OR all packets in outgoing packet array. 

    //Create ghost
    //1001 <position info here>

    //Move ghost (later packet)
    //1000 <position info here>

    //Destroy ghost
    //00001




    //What happens if client receives the same thing twice though, 

    //Create ghost isn't an issue to receive twice, because you can just ignore it
    //Destroy ghost isn't an issue because you can just ignore it. 



    //How do we decide what to send
    //Server needs to have a map of ghost ID -> ghost object (what is a ghost object)
    //Server needs to have map of client -> map of ghost id -> outgoing state changes

    //For each client
        //Figure out what their MRS is. Write into packet

    //Client side
    //When you receive a ghost packet, update local ghost objects accordingly.

    //Server receives ACK
    //Update outgoing map

}
