using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSComponent
{
    public static class InputComponent
    {
        public static List<Keybinds.KeybindingCheck> keybindingChecks = new List<Keybinds.KeybindingCheck>();
        public static List<InputFrame> flagHistory = new List<InputFrame>();
    }

    public class InputFrame
    {
        public BitArray bitArray;
        public int frame;
        public InputFrame(BitArray ba, int f)
        {
            bitArray = ba;
            frame = f;
        }
    }
}

