namespace ECSSystem
{

    public static class ThreadManager
    {
        public static void FixedUpdate()
        {
            UpdateMain();
        }


        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain()
        {
            if (ECSComponent.ThreadManagerComponent.actionToExecuteOnMainThread)
            {
                ECSComponent.ThreadManagerComponent.executeCopiedOnMainThread.Clear();
                lock (ECSComponent.ThreadManagerComponent.executeOnMainThread)
                {
                    ECSComponent.ThreadManagerComponent.executeCopiedOnMainThread.AddRange(ECSComponent.ThreadManagerComponent.executeOnMainThread);
                    ECSComponent.ThreadManagerComponent.executeOnMainThread.Clear();
                    ECSComponent.ThreadManagerComponent.actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < ECSComponent.ThreadManagerComponent.executeCopiedOnMainThread.Count; i++)
                {
                    ECSComponent.ThreadManagerComponent.executeCopiedOnMainThread[i]();
                }
            }
        }
    }
}

