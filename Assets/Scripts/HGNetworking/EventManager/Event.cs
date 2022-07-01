using System;
using System.Reflection;
using UnityEngine;

public class Event
{
    public int EventId { get; set; }

    private enum EventType{
        Event_ERROR,
        Event_TEST_EVENT,
        Event_SEND_USERNAME,
        Event_GIVE_CONTROL
    }

    public void WriteEventHeader(ref Packet packet)
    {
        packet.Write(GetType().Name);
    }

    public void WriteEventToPacket(Packet packet)
    {
        packet.Write(GetEventTypeFromString(GetType().Name));

        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
        {
            packet.Write((dynamic) propertyInfo.GetValue(this, null));
        }
    }

    public void ReadEventFromPacket(Packet packet)
    {
        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
        {
            propertyInfo.SetValue(this, packet.ReadGeneric(propertyInfo.PropertyType));
        }
    }

    #region Helper? hardly knew 'er Functions
    public static int GetEventTypeFromString(string eventName)
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

    public int GetSize()
    {
        int size = 0;
        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
        {
            switch (propertyInfo.PropertyType)
            {
                case Type byteType when byteType == typeof(byte):
                case Type boolType when boolType == typeof(bool):
                    size += 1;
                    break;
                case Type shortType when shortType == typeof(short):
                    size += 2;
                    break;
                case Type floatType when floatType == typeof(float):
                case Type intType when intType == typeof(int):
                    size += 4;
                    break;
                case Type longType when longType == typeof(long):
                    size += 8;
                    break;
                case Type vector3Type when vector3Type == typeof(Vector3):
                    size += 12;
                    break;
                case Type quaternionType when quaternionType == typeof(Quaternion):
                    size += 16;
                    break;
                case Type stringType when stringType == typeof(string):
                    size += 4 + ((String)propertyInfo.GetValue(this, null)).Length;
                    break;
                case Type byteArrType when byteArrType == typeof(byte[]):
                    size += ((byte[])propertyInfo.GetValue(this, null)).Length;
                    break;
                default:
                    throw new ArgumentException($"Type is not currently handled by GetSize: {propertyInfo.PropertyType}");
            }
        }
        return size;
    }
    #endregion
}
