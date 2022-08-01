using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;

namespace ECSComponent
{
    public static class PacketQueueComponent
    {
        public struct PacketQueueTuple
        {
            public IPEndPoint connectionEndpoint;
            public Packet packet;
        }
        public struct ProcessedPacketQueueTuple
        {
            public int connectionId;
            public Packet packet;
        }
        public static Queue<PacketQueueTuple> receivedPacketQueue = new Queue<PacketQueueTuple>();
        public static Dictionary<int, Queue<Packet>> processedPacketQueue = new Dictionary<int, Queue<Packet>>();
    }
}

