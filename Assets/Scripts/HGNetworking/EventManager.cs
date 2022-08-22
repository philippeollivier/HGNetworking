using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    //Constant Values
    private const int EVENT_WINDOW_SIZE = 64;
    private const int SLIDING_WINDOW_SIZE = EVENT_WINDOW_SIZE * 2;

    //Receiving Member Variables
    private Dictionary<int, Event> receivedEvents = new Dictionary<int, Event>();
    private int nextReadEventId = 0;

    //Sending Member Variables
    private Dictionary<int, Event> sentEvents = new Dictionary<int, Event>();
    private Dictionary<int, List<int>> packetEventMap = new Dictionary<int, List<int>>();
    private Queue<Event> outgoingEventsQueue = new Queue<Event>();
    private SlidingWindow outgoingWindow = new SlidingWindow(EVENT_WINDOW_SIZE, true);

    #region General Data Manager Functions
    public bool HasMoreDataToWrite()
    {
        return outgoingEventsQueue.Count > 0;
    }

    public int WriteToPacket(int remainingBytes, Packet packet)
    {
        //Exit early if there are no events to write or we have too many outgoing events that have not been processed
        if (outgoingEventsQueue.Count == 0 || sentEvents.Count >= EVENT_WINDOW_SIZE)
        {
            packet.Write(0);
            return sizeof(int);
        }

        //Precalculate packet information before writing
        int totalEventsSize = sizeof(int);
        Queue<Event> eventsToSendInPacket = new Queue<Event>();
        int currEventSize = outgoingEventsQueue.Peek().GetSize();
        while (remainingBytes > totalEventsSize + currEventSize)
        {
            //First see if Outgoing Sliding Window is full
            int nextValidId = outgoingWindow.AdvancePointer();
            if (nextValidId == -1)
            {
                break;
            }

            Event currEvent = outgoingEventsQueue.Dequeue();
            Debug.Log(currEvent);
            eventsToSendInPacket.Enqueue(currEvent);
            currEvent.EventId = nextValidId;

            //Add Events to Sent Events
            totalEventsSize += currEventSize;
            sentEvents[nextValidId] = currEvent;
            AddEventToPacketEventMap(packet.PacketHeader.packetId, currEvent);

            //Continue Iteration
            if (outgoingEventsQueue.Count == 0) { break; }
            currEventSize = outgoingEventsQueue.Peek().GetSize();
        }

        //Write to packet
        packet.Write(eventsToSendInPacket.Count);
        foreach (Event outgoingEvent in eventsToSendInPacket)
        {
            outgoingEvent.WriteEventToPacket(packet);
        }
    
        return totalEventsSize;
    }

    public void ReadFromPacket(Packet packet)
    {
        int numEvents = packet.ReadInt();
        List<Event> packetEvents = new List<Event>();

        for (int i = 0; i < numEvents; i++)
        {
            Event currentEvent = Event.GetEventClassFromId(packet.ReadInt());
            currentEvent.ReadEventFromPacket(packet);

            packetEvents.Add(currentEvent);
        }

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
        Debug.Log(eventToProcess);
        receivedEvents.Remove(eventId);
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

    #endregion

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
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to ACK Event packet {packetId}\nException: {e}\n");
        }
    }

    public void NOACKEventPacket(int packetId)
    {
        //Add the failed events to the outgoing event queue
        if (packetEventMap.ContainsKey(packetId))
        {
            foreach (int eventId in packetEventMap[packetId])
            {
                outgoingEventsQueue.Enqueue(sentEvents[eventId]);
            }
        }

    }
    #endregion
}

namespace HG.Utils
{
    public static class EventManagerUtils
    {
        public static void SendEventToConnection(Connection connection, Event e)
        {
            connection.streamManager.eventManager.QueueOutgoingEvent(e);
        }

        public static void SendEventToAllExcludingConnection(Connection connection, Event e)
        {
            foreach (var pair in Networking.ConnectionManager.connections)
            {
                if(pair.Value != connection)
                {
                    SendEventToConnection(pair.Value, e);
                }
            }
        }

        public static void SendEventToAllConnections(Event e)
        {
            foreach(var pair in Networking.ConnectionManager.connections)
            {
                SendEventToConnection(pair.Value, e);
            }
        }
    }
}