using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Connection
{
    public static int dataBufferSize = 4096;
    public int id;
    public UDP udp;
    public SlidingWindow window = new SlidingWindow(64, false);
    public long[] timeouts = new long[64];
    public long timeoutTime;
    public Connection(int connectionId, long timeoutTime)
    {
        id = connectionId;
        udp = new UDP(id);
        this.timeoutTime = timeoutTime;
    }
    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        /// <summary>Initializes the newly connected client's UDP-related info.</summary>
        /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        /// <summary>Cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            endPoint = null;
        }
    }

    public void UpdateTick()
    {
        for(int i = 0; i < timeouts.Length; i++)
        {
            if(window.ActiveFrames(i))
            {
                timeouts[i]++;
            }
        }
    }

    private void Disconnect()
    {
        udp.Disconnect();
    }
}
