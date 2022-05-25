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
    private int nextSendEventId = 0;

    //Constant Values
    private const int EVENT_WINDOW_SIZE = 64;
    private const int RECEIVED_WINDOW_SIZE = EVENT_WINDOW_SIZE * 2 + 1;

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
            nextReadEventId = (nextReadEventId + 1) % RECEIVED_WINDOW_SIZE;
        }
    }

    private void ProcessEvents(int eventId)
    {
        Event eventToProcess = receivedEvents[eventId];
        receivedEvents.Remove(eventId);

        //Send Events to Handlers
        EventManager.NotifyEventHandlers(eventToProcess);
    }

    public int WriteEvents(Packet packet, int packetId, int remainingPacketSize)
    {
        //Exit early if there are no events to write or we have too many outgoing events that have not been processed
        if(outgoingEventsQueue.Count == 0 || sentEvents.Count >= EVENT_WINDOW_SIZE)
        {
            return remainingPacketSize;
        }

        //TODO: maybe make this not just look at front, see if any event can fit? Sorted queue by size/event.id? 
        int currEventSize = outgoingEventsQueue.Peek().GetSize();
        while(remainingPacketSize > currEventSize)
        {
            Event currEvent = outgoingEventsQueue.Dequeue();

            //Increment the event packet id we are sending on
            nextSendEventId = (nextSendEventId + 1) % EVENT_WINDOW_SIZE;
            currEvent.EventId = nextSendEventId;

            currEvent.WriteEventToPacket(packet);
            remainingPacketSize -= currEventSize;

            //Add Events to Sent Events
            sentEvents[nextSendEventId] = currEvent;
            AddEventToPacketEventMap(packetId, currEvent);

            //Continue Iteration
            if (outgoingEventsQueue.Count == 0)  {  break; }
            currEventSize = outgoingEventsQueue.Peek().GetSize();
        }

        return remainingPacketSize;
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


    #region Helper Functions
    private bool IsEventDuplicate(int eventId)
    {
        if (eventId < nextReadEventId && eventId > nextReadEventId - EVENT_WINDOW_SIZE)
        {
            return true;
        }
        if ((eventId < nextReadEventId && eventId >= 0) || (eventId > (nextReadEventId - EVENT_WINDOW_SIZE) % (RECEIVED_WINDOW_SIZE) && eventId <= RECEIVED_WINDOW_SIZE))
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
        //Remove ACK'd Event Packet from Sent Events
        foreach (int eventId in packetEventMap[packetId])
        {
            sentEvents.Remove(eventId);
        }
    }

    public void NOACKEventPacket(int packetId)
    {
        //Add the failed events to the outgoing event queue
        foreach (int eventId in packetEventMap[packetId])
        {
            outgoingEventsQueue.Enqueue(sentEvents[eventId]);
        }
    }
    #endregion
}