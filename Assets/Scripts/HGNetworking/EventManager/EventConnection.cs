using System;
using System.Collections.Generic;
using UnityEngine;

public class EventConnection
{
    //Receiving Member Variables
    private Dictionary<int, Event> receivedEvents = new Dictionary<int, Event>();
    private int nextReadEventId = 0;

    //Sending Member Variables
    private Dictionary<int, Event> sentEvents = new Dictionary<int,Event>();
    private Dictionary<int, List<int>> packetEventMap = new Dictionary<int, List<int>>();
    private Queue<Event> outgoingEventsQueue = new Queue<Event>();

    //Constant Values
    private const int EVENT_WINDOW_SIZE = 64;
    private const int SLIDING_WINDOW_SIZE = EVENT_WINDOW_SIZE * 2;
    private SlidingWindow outgoingWindow = new SlidingWindow(EVENT_WINDOW_SIZE, true);

    public void ReceiveEvents(List<Event> packetEvents)
    {
        //Add received packets to the received events map
        foreach (Event e in packetEvents)
        {
            //If the Event's Id is within Dead Window Area, do not reprocess event. 
            if (!IsEventDuplicate(e.EventId))
            {
                receivedEvents[e.EventId] = e;
            }
        }


        //Process received events in an ordered fashion
        while (receivedEvents.ContainsKey(nextReadEventId))
        {
            ProcessEvents(nextReadEventId);

            //Increment the event id we are waiting on
            //Size of Received Window is larger than Sent Window to avoid processing same event twice
            nextReadEventId = (nextReadEventId + 1) % SLIDING_WINDOW_SIZE;
        }
    }

    private void ProcessEvents(int eventId)
    {
        Event eventToProcess = receivedEvents[eventId];
        receivedEvents.Remove(eventId);

        //Add processed event to events component for other systems to access
        EventManager.QueueIncomingEvent(eventToProcess);
    }

    public int WriteEvents(Packet packet, int remainingPacketSize)
    {
        //Exit early if there are no events to write or we have too many outgoing events that have not been processed
        if(outgoingEventsQueue.Count == 0 || sentEvents.Count >= EVENT_WINDOW_SIZE)
        {
            packet.Write(0);
            return sizeof(int);
        }

        //Figure out what events we will be writing to packet
        int totalEventsSize = 1; //Store one byte used for number of events
        Queue<Event> eventsToSendInPacket = new Queue<Event>();
        int currEventSize = outgoingEventsQueue.Peek().GetSize();
        while(remainingPacketSize > totalEventsSize + currEventSize)
        {
            //First see if Outgoing Sliding Window is full
            int nextValidId = outgoingWindow.AdvancePointer();
            if(nextValidId == -1)
            {
                break;
            }

            Event currEvent = outgoingEventsQueue.Dequeue();
            eventsToSendInPacket.Enqueue(currEvent);
            currEvent.EventId = nextValidId;

            //Add Events to Sent Events
            totalEventsSize += currEventSize;
            sentEvents[nextValidId] = currEvent;
            AddEventToPacketEventMap(packet.PacketHeader.packetId, currEvent);

            //Continue Iteration
            if (outgoingEventsQueue.Count == 0)  {  break; }
            currEventSize = outgoingEventsQueue.Peek().GetSize();
        }

        //Write to packet
        packet.Write(eventsToSendInPacket.Count);
        foreach(Event outgoingEvent in eventsToSendInPacket){
            outgoingEvent.WriteEventToPacket(packet);
        }

        return totalEventsSize;
    }
    public void ProcessNotification(bool success, int packetId)
    {
        if (success)
        {
            ACKEventPacket(packetId);
        }
        else
        {
            NOACKEventPacket(packetId);
        }
    }

    public bool HasMoreToWrite()
    {
        return outgoingEventsQueue.Count > 0;
    }

    public void QueueOutgoingEvent(Event e)
    {
        outgoingEventsQueue.Enqueue(e);
    }

    #region Helper Functions
    private bool IsEventDuplicate(int eventId)
    {
        if (eventId < nextReadEventId && eventId > nextReadEventId - EVENT_WINDOW_SIZE)
        {
            return true;
        }
        if ((eventId < nextReadEventId && eventId >= 0) || (eventId > nextReadEventId + EVENT_WINDOW_SIZE && eventId < SLIDING_WINDOW_SIZE))
        {
            return true;
        }
        return false;
    }

    private void AddEventToPacketEventMap(int packetId, Event currEvent)
    {
        if (!packetEventMap.ContainsKey(packetId))
        {
            packetEventMap[packetId] = new List<int>();
        }

        packetEventMap[packetId].Add(currEvent.EventId);
    }

    public void ACKEventPacket(int packetId)
    {
        try
        {
            if (packetEventMap.ContainsKey(packetId))
            {
                //Remove ACK'd Event Packet from Sent Events
                foreach (int eventId in packetEventMap[packetId])
                {
                    //Increment sliding window
                    outgoingWindow.FillFrame(eventId);

                    sentEvents.Remove(eventId);
                }
            }
        } catch(Exception e)
        {
            Debug.LogError($"Failed to ACK Event packet {packetId}\nException: {e}\n");
        }
    }

    public void NOACKEventPacket(int packetId)
    {
        //Add the failed events to the outgoing event queue
        if (packetEventMap.ContainsKey(packetId)){
            foreach (int eventId in packetEventMap[packetId])
            {
                outgoingEventsQueue.Enqueue(sentEvents[eventId]);
            }
        }

    }
    #endregion
}