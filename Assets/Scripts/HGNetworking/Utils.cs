using System;

public static class Utils
{
    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="_action">The action to be executed on the main thread.</param>
    public static void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
        {
            Console.WriteLine("No action to execute on main thread!");
            return;
        }

        lock (ECSComponent.ThreadManagerComponent.executeOnMainThread)
        {
            ECSComponent.ThreadManagerComponent.executeOnMainThread.Add(_action);
            ECSComponent.ThreadManagerComponent.actionToExecuteOnMainThread = true;
        }
    }
}
