using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public virtual void HandleEvent(Event currEvent)
    {
        Debug.LogError($"Base EventHandler HandleEvent is being called on event {currEvent}. You dumb as hell");
    }

    public void QueueOutgoingEvent(Event currEvent)
    {
        //EventManager.QueueOutgoingEvent(currEvent);
    }
}
