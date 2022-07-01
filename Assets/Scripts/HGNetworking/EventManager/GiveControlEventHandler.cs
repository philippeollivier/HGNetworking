using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveControlEventHandler : EventHandler
{
    void Start()
    {
        //Subscribe to EventManager
        EventManager.SubscribeHandler(this);
    }

    public override void HandleEvent(Event currEvent)
    {
        MoveManager.moveController.GiveControlOfGhost(((Events.Event_GIVE_CONTROL)currEvent).ghostId, ((Events.Event_GIVE_CONTROL)currEvent).moveId);
    }
}