using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

using ConnectionId = System.Int32;

public class Connection
{
    public ConnectionId connectionId;
    public StreamManager streamManager;
    public List<int> packetsToAck;
    public UDP udp;

    private SlidingWindow window;
    private long[] timeouts;
    
    public Connection(ConnectionId connectionId, IPEndPoint endpoint)
    {
        //Initialize Internal Components
        this.connectionId = connectionId;
        window = new SlidingWindow(HG.NetworkingConstants.WINDOW_SIZE, true);
        timeouts = new long[HG.NetworkingConstants.WINDOW_SIZE * 2];
        packetsToAck = new List<int>();

        if (HG.Networking.isServer)
        {
            streamManager = new ServerStreamManager(this);
        }
        else
        {
            streamManager = new ClientStreamManager(this);
        }

        //Initialize UDP Connection
        udp = new UDP(this.connectionId);
        udp.Connect(endpoint);
    }

    public void FixedUpdate()
    {
        //Break out early connection is not properly set yet
        if(udp.endPoint == null)
        {
            return;
        }

        //Increment all timeouts that are currently active
        IncrementTimeouts();

        //Update to all incoming packets with an ACK
        AckReceivedPackets();

        //Write all outgoing packets
        if( !window.IsFull)
        {
            streamManager.WriteToPacket();
        }
    }

    #region Timeout functions
    public void IncrementTimeouts()
    {
        for (int i = 0; i < timeouts.Length; i++)
        {
            if (window.ActiveFrames(i))
            {
                timeouts[i]++;
                if (timeouts[i] > HG.NetworkingConstants.TIMEOUT_TICKS)
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
        for (int i = 0; i < timeouts.Length; i++)
        {
            retVal += $"{timeouts[i]}|";
        }

        return retVal;
    }

    public float GetAveragePing()
    {
        float ping = 0;
        for (int i = 0; i < timeouts.Length; i++)
        {
            ping += timeouts[i];
        }

        ping *= Time.fixedDeltaTime / timeouts.Length;

        return ping;
    }
    #endregion

    #region ACK Functionality
    public void AckReceivedPackets()
    {
        if (packetsToAck.Count == 0)
        {
            return;
        }

        using (Packet packet = new Packet())
        {
            packet.Write(Convert.ToByte(PacketType.ACK));
            packet.Write(0);
            packet.Write(ECS.Components.SynchronizedClock.CommandFrame);
            packet.Write(packetsToAck.Count);
            foreach (int ack in packetsToAck)
            {
                packet.Write(ack);
            }

            PlatformPacketManager.SendPacket(udp.endPoint, packet);
        }

        packetsToAck.Clear();
    }

    public void ReadACK(Packet packet)
    {
        int ackCount = packet.ReadInt();
        for (int i = 0; i < ackCount; i++)
        {
            int ack = packet.ReadInt();
            SlidingWindow.WindowStatus status = window.FillFrame(ack);

            //TODO: Here add ping/latency calculations

            switch (status)
            {
                case SlidingWindow.WindowStatus.Success:
                    MetricsManager.AddDatapointToMetric("ACK Successful", 1f, true);
                    streamManager.ProcessNotification(true, ack);
                    break;
                case SlidingWindow.WindowStatus.OutOfOrder:
                    MetricsManager.AddDatapointToMetric("ACK Out of Order", 1f, true);
                    streamManager.ProcessNotification(false, ack);
                    break;
                case SlidingWindow.WindowStatus.Duplicate:
                    MetricsManager.AddDatapointToMetric("ACK Duplicate", 1f, true);
                    Debug.Log("Duplicate ACK received");
                    break;
                case SlidingWindow.WindowStatus.OutofBounds:
                    MetricsManager.AddDatapointToMetric("ACK Out of Bounds", 1f, true);
                    Debug.Log("ACK Returned out of bounds");
                    break;
            }
        }
    }
    #endregion

    #region Incoming Packet Utils

    public void ReadPacket(Packet packet, bool needsAck = false)
    {
        streamManager.ReadFromPacket(packet);
        if (needsAck)
        {
            packetsToAck.Add(packet.PacketHeader.packetId);
        }
    }

    #endregion

    #region Outgoing Packets Utils
    public bool GetPacket(Packet packet, PacketType type)
    {
        switch (type)
        {
            case PacketType.NoACK:
                packet.PacketHeader = new PacketHeader(PacketType.NoACK, 0, ECS.Components.SynchronizedClock.CommandFrame);
                packet.WritePacketHeaderToPacket();
                return true;
            case PacketType.Regular:
                int packetId = window.AdvancePointer();
                if (packetId == -1)
                {
                    return false;
                }
                ResetTimeout(packetId);
                packet.PacketHeader = new PacketHeader(PacketType.Regular, packetId, ECS.Components.SynchronizedClock.CommandFrame);
                packet.WritePacketHeaderToPacket();
                return true;
            default:
                throw new ArgumentException($"PacketType not currently implemented. PacketType: {type}");
        }
    }

    public void SendPacket(Packet packet)
    {
        PlatformPacketManager.SendPacket(udp.endPoint, packet);
    }
 
    #endregion
}


public class UDP
{
    public IPEndPoint endPoint;

    private ConnectionId id;

    public UDP(ConnectionId id)
    {
        this.id = id;
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