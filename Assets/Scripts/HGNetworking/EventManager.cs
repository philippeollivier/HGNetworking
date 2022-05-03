using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    //todo
    //enum rpc_guarantee_type
    //{
    //    rpc_guaranteed_ordered = 0, ///< Event delivery is guaranteed and will be processed in the order it was sent relative to other ordered events.
    //    rpc_guaranteed = 1, ///< Event delivery is guaranteed and will be processed in the order it was received.
    //    rpc_unguaranteed = 2 ///< Event delivery is not guaranteed - however, the event will remain ordered relative to other unguaranteed events.
    //};

    //How many Events
    //EVENT INT, -> Tells you how much to read.

    //Maintain a sliding window of events sent and events received
    //Whenever we send event, add to list of events sent.

    //Server receives event, sends ACK back
    //Store ACK

    //Whenever we 'update' the event manager

    //If first event is not dropped. Dropped means it has not been ACKed in 3 packets or TIMEOUT of n seconds.
    //If first event is ACKd, increment sliding window. Do all events that have been ACKd that are pending on first event being ACKd
    //If we receive ACK for second event, and first event has not been ACKed then
    //Add second event to pending queue of events

    //

    //Event 1 -> dies
    //Event 2 -> dies
    //Event 3 -> we get 3

    //Add 3 to pending

    //Resend event 1 and 2

    //Event 2 gets ACKd

    //Add event 2 to pending (ordered somehow)

    //Event 1 gets resent

    //Event 1 gets ACK'd

    //Events 1, 2, 3

    //Sliding window 

    //Whenever we send an Event, update its ACK'd bool to false
    //Whenever we receive an Event, set ACK'd to true


    //Datastructure of Events that we WANT to send this frame.

    //Q of Objects Sent
    //[Event1 sent, Event ID, Timeout something, ACK'd = true]
    //[Event2 sent, Event ID, Timeout something, ACK'd = true]
    //[Event3 sent, Event ID, Timeout something, ACK'd = true]


    //Map of Objects Received as well
    //Whenever we get an event, add it to event map of received. Event ID should be in order unless its wrapping around, so we just check if we have already done this event, don't do again. 
    //N - sliding window packet gets set to null or something. 

    //EarliestEvent = 1
    //LatestEvent = 3

    //Whenever we send an event do LatestEvent++

    //If we receive ACK callback:
    //Set its ACK to true
    //We check earliest event can be processed, can we process EarliestEvent++ until Latest Event? Continue

    //If we receive ACK failed callback for an event:
    //Resend the event


    //When writing to packet
    //Look at all events that need to be resent, send them
    //Look at all events that need to be sent, send them
    //Update Q of Objects accordingly


    //When receive events
    //Store event locally, see if its the next one sequentially, if its not, save it, if it is 
    //If we get an event, we ACK it, ACK gets lost, connection manager sends it again, do we want to like redo it 


    //public bool moreDataToWrite()
    //{
    //    return false;
    //}

    //public void writeToPacket()
    //{
    //    //Based on how much space is left in OVERALL packet, figure out how many EVENTS we can write, and write them
    //}

    //public void readPacket()
    //{
    //    /*
    //        Sliding window that events were sent + received in correct order.
    //        If correct order do event business
    //    */
    //}

    //public void ackCallback()
    //{
    //    //Get the event number from the ack packet also gets whether its a dropped callback or successful
    //    //Update sliding window for event manager
    //}




    public static bool HasMoreDataToWrite(int connectionId)
    {
        return false;
    }

    public static int WriteToPacket(int connectionId, int remainingBytes, int packetId, ref Packet packet)
    {
        return 1;
    }

    public static void ReadFromPacket(int connectionId, int packetId, ref Packet packet)
    {

    }
}
