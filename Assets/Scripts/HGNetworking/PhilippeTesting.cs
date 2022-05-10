using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhilippeTesting : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Events.Event_TEST_EVENT e = new Events.Event_TEST_EVENT();
            e.Username = "animbot";
            e.Number = 14;
            e.Vec = new Vector3(1, 2, 3);
            e.Quat = new Quaternion();
            e.Test = "TADA";


            Events.Event_SEND_USERNAME e3 = new Events.Event_SEND_USERNAME();
            e3.Username = "ikkacuslayer69";


            //TODO remove these debugs
            Debug.Log($"Writing All properties");
            DateTime before = DateTime.Now;
            Debug.Log($"Writing packet type properties for packet {GetType().Name}");


            Packet p = new Packet();
            e.WriteEventToPacket(ref p);
            e3.WriteEventToPacket(ref p);

            p.ToArray();

            Event e2 = Event.GetEventClassFromId(p.ReadInt());
            e2.ReadEventFromPacket(ref p);
            Debug.Log(e2);

            Event e4 = Event.GetEventClassFromId(p.ReadInt());
            e4.ReadEventFromPacket(ref p);
            Debug.Log(e4);

            DateTime after = DateTime.Now;
            Debug.Log($"Finished writing Property in Time: {after.Subtract(before).Milliseconds}");
        }
    }
}
