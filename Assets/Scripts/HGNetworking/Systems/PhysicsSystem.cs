using UnityEngine;

namespace ECSSystem
{
    public static class PhysicsSystem
    {
        #region Generic Systems Code

        public static void Awake()
        {
            Physics.autoSimulation = false;
        }

        public static void FixedUpdate()
        {
            Physics.Simulate(Time.fixedDeltaTime);
        }

        #endregion
    }
}
