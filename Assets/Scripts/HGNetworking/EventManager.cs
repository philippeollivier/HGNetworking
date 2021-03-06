using System.Collections.Generic;

public static class EventManager
{
    public static Dictionary<int, EventConnection> eventConnections = new Dictionary<int, EventConnection>();

    #region General Data Manager Functions
    public static bool HasMoreDataToWrite(int connectionId)
    {

        return eventConnections[connectionId].HasMoreToWrite();
    }

    public static int WriteToPacket(int connectionId, int remainingBytes, Packet packet)
    {

        return eventConnections[connectionId].WriteEvents(packet, remainingBytes);
    }

    public static void ReadFromPacket(int connectionId, Packet packet)
    {
        int numEvents = packet.ReadInt();
        List<Event> allEvents = new List<Event>();

        for (int i = 0; i < numEvents; i++)
        {
            Event currentEvent = Event.GetEventClassFromId(packet.ReadInt());
            currentEvent.ReadEventFromPacket(packet);

            allEvents.Add(currentEvent);
        }

        eventConnections[connectionId].ReceiveEvents(allEvents);
    }

    public static void ProcessNotification(bool success, int packetId, int connectionId)
    {
        eventConnections[connectionId].ProcessNotification(success, packetId);
    }

    #endregion

    #region Event Handler Business
    public static void QueueIncomingEvent(Event e)
    {
        ECSComponent.EventComponent.incomingEvents.Enqueue(e);
    }

    public static void QueueOutgoingEvent(Event outgoingEvent)
    {
        foreach (EventConnection eventConnection in eventConnections.Values)
        {
            eventConnection.QueueOutgoingEvent(outgoingEvent);
        }
    }

    public static void QueueOutgoingEvent(Event outgoingEvent, int connectionId)
    {
        eventConnections[connectionId].QueueOutgoingEvent(outgoingEvent);

    }
    #endregion
}