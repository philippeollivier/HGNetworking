using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Event
{
    private enum EventType{
        Event_ERROR,
        Event_TEST_EVENT,
        Event_SEND_USERNAME
    }

    public void WriteEventHeader(ref Packet packet)
    {
        packet.Write(GetType().Name);
    }

    public void WriteEventToPacket(ref Packet packet)
    {
        packet.Write(GetEventIdFromString(GetType().Name));

        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
        {
            packet.Write((dynamic) propertyInfo.GetValue(this, null));
        }
    }

    public void ReadEventFromPacket(ref Packet packet)
    {
        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
        {
            propertyInfo.SetValue(this, packet.ReadGeneric(propertyInfo.PropertyType));
        }
    }

    #region Helper Functions
    public static int GetEventIdFromString(string eventName)
    {
        try
        {
            return (int) Enum.Parse(typeof(EventType), eventName);
        }
        catch (ArgumentException)
        {
            Debug.LogError($"Event {eventName} cannot be found in the EventType enum");
            return -1;
        }
    }

    public static Event GetEventClassFromId(int id)
    {
        Type eventType = Type.GetType($"Events.{((EventType) id).ToString()}");
        return (Event) Activator.CreateInstance(eventType);
    }

    public override string ToString()
    {
        string retVal = "";
        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
        {
            retVal += $"{propertyInfo.Name}: {propertyInfo.GetValue(this, null)}\n";;
        }
        return retVal;
    }
    #endregion
}
