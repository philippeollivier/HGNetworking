using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class Connection
{
    public static int dataBufferSize = 4096;
    public static int WINDOW_SIZE = 64;
    public static int TIMEOUT_TICKS = 64;

    public int id;
    public UDP udp;
    public SlidingWindow window = new SlidingWindow(WINDOW_SIZE, true);
    public long[] timeouts = new long[WINDOW_SIZE * 2];

    public Connection(int connectionId)
    {
        id = connectionId;
        udp = new UDP(id);
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

    #region Timeout functions
    public void UpdateTick()
    {
        //Increment all timeouts that are currently active
        for(int i = 0; i < timeouts.Length; i++)
        {
            if(window.ActiveFrames(i))
            {
                timeouts[i]++;
                if (timeouts[i] > TIMEOUT_TICKS)
                {
                    window.FillFrame(i); 
                }
            }
        }
    }

    public void ResetTimeout(int packetId)
    {
        //Clear timeout for a given packet id, 
        timeouts[packetId] = 0;
    }

    public string RenderTimeout()
    {
        string retVal = "|";
        for (int i = 0; i < 2 * WINDOW_SIZE; i++)
        {
            //retVal += $"<color={(InWindow(i) ? (ActiveFrames(i) ? "blue" : "green") : "red")}>{i}</color>|";
        }

        return retVal;
    }
    #endregion

    private void Disconnect()
    {
        udp.Disconnect();
    }
}
