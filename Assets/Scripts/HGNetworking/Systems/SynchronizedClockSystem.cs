using UnityEngine;

namespace ECSSystem
{
    public static class SynchronizedClockSystem
    {
        #region Generic Systems Code

        public static void Awake()
        {
            ECSComponent.SynchronizedClock.CommandFrame = 0;
        }

        public static void FixedUpdate()
        {
            ECSComponent.SynchronizedClock.CommandFrame++;
        }

        #endregion
    }
}