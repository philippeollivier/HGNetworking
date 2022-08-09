using UnityEngine;

namespace ECS.Systems
{
    public static class TestingSystem
    {
        #region Generic Systems Code

        public static void Awake()
        {

        }

        public static void FixedUpdate()
        {
            Debug.Log("frog");
        }

        #endregion
    }
}