using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSComponent
{
    public static class ConnectionComponent
    {
        public static int MaxPlayers { get; set; }
        public static Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
        public static string[] connectionAddresses;
        public static int connectionIndex = 1;
        public static Dictionary<int, List<int>> acks = new Dictionary<int, List<int>>();
    }
}

