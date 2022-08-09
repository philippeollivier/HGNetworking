using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickableBallable : MonoBehaviour
{
    public float kickForce = 50f, upKickForce = 50f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Events.Event_KICK_BALL e = new Events.Event_KICK_BALL();
            Vector3 kickVec = Vector3.zero;

            Vector3 difference = transform.position - collision.gameObject.transform.position;

            difference.y = 0;
            difference.Normalize();

            difference *= kickForce;
            difference += upKickForce * Vector3.up;

            e.kickVector = difference;

            e.ghostId = GetComponent<Ghost>().ghostId;

            EventManager.QueueOutgoingEvent(e);
        }
    }
}
