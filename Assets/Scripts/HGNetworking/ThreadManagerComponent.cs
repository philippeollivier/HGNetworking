using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ECSComponent
{
    public static class ThreadManagerComponent
    {
        public static readonly List<Action> executeOnMainThread = new List<Action>();
        public static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        public static bool actionToExecuteOnMainThread = false;

    }
}

