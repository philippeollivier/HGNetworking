using System;
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
        if(currEvent.GetType().Equals(Type.GetType("Events.Event_GIVE_CONTROL")))
        {
            int ghostId = ((Events.Event_GIVE_CONTROL)currEvent).ghostId;

            GhostManager.localGhosts[ghostId].GetComponent<FirstPersonCameraController>().enabled = true;
            GhostManager.localGhosts[ghostId].GetComponent<FPController>().enabled = true;
            GhostManager.localGhosts[ghostId].GetComponent<Rigidbody>().isKinematic = false;
            GhostManager.localGhosts[ghostId].GetComponentInChildren<Camera>().enabled = true;
            GhostManager.localGhosts[ghostId].GetComponentInChildren<AudioListener>().enabled = true;

            MoveManager.GetControlOfGhost(ghostId, ((Events.Event_GIVE_CONTROL)currEvent).moveId);
        }
        else if (currEvent.GetType().Equals(Type.GetType("Events.Event_KICK_BALL")))
        {
            Debug.Log("received event");
            int ghostId = ((Events.Event_KICK_BALL)currEvent).ghostId;
            Vector3 force = ((Events.Event_KICK_BALL)currEvent).kickVector;

            GhostManager.ghosts[ghostId].GetComponent<Rigidbody>().AddForce(force);
        }
    }
}