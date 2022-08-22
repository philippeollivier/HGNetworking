namespace ECS.Components
{
    public static class SynchronizedClock
    {
        public static int CommandFrame;
        public static int AverageRTTFrames;
        public static int AverageRTT;
        public static int PhysicsGhostFrames = 120;
    }
}
