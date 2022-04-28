using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager
{
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
