using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSComponent
{
    public static class PrefabMap
    {
        public enum PrefabType
        {
            Test
        }
        public static Dictionary<PrefabType, GameObject> prefabs;

        public static void Awake()
        {
            prefabs = new Dictionary<PrefabType, GameObject>();
        }

    }
}
