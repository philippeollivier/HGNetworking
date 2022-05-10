using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;

public class TestScript : MonoBehaviour
{
    public int frameID;
    public SlidingWindow sw = new SlidingWindow(10, false);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse("25.18.58.72"), 6942));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectionManager.OpenServer(1, 6942);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            using(Packet packet = new Packet())
            {
                packet.Write(Convert.ToByte(ConnectionManager.PacketType.Regular));
                packet.Write(0);
                Events.Event_SEND_USERNAME e3 = new Events.Event_SEND_USERNAME();

                e3.Username = "ikkacuslayer69";
                e3.WriteEventToPacket(packet);
                ConnectionManager.SendPacket(0, packet);
            }

        }
    }
}
