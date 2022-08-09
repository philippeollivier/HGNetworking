namespace ECS.Systems
{
    public static class SynchronizedClockSystem
    {
        #region Generic Systems Code

        public static void Awake()
        {
            ECS.Components.SynchronizedClock.CommandFrame = 0;
        }

        public static void FixedUpdate()
        {
            ECS.Components.SynchronizedClock.CommandFrame++;
        }

        #endregion
    }
}