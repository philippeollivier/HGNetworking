using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Systems
{
    public static class ClientInputBufferSystem
    {
        public static void LoadKeybindsIntoComponent()
        {
            foreach(Keybinds.KeybindingCheck keybind in InputManager.Instance.keybinds.keybindingChecks)
            {
                Components.InputComponent.keybindingChecks.Add(keybind);
            }
            Components.InputComponent.sizeOfFlags = Mathf.CeilToInt(Components.InputComponent.keybindingChecks.Count / 8);
        }
        public static void RecordInputs()
        {
            Components.InputComponent.flagHistory[Components.SynchronizedClock.CommandFrame] = InputManager.Instance.GetKeys();
        }

    }
} 

