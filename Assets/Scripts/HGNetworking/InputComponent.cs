using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Components
{
    public static class InputComponent
    {
        public static List<Keybinds.KeybindingCheck> keybindingChecks = new List<Keybinds.KeybindingCheck>();
        public static Dictionary<int, BitArray> flagHistory = new Dictionary<int, BitArray>();
        public static Dictionary<int, int> packetToMoveHistory = new Dictionary<int, int>();
        public static int mostRecentNonAckedMoveFrame = 0;
        public static bool readThisFrame = false;
        public static int sizeOfFlags = 0;
    }

    public static class InputHistoryServerComponent
    {
        public static Dictionary<int, Dictionary<int, BitArray>> flagHistory = new Dictionary<int, Dictionary<int, BitArray>>();
    }
}

