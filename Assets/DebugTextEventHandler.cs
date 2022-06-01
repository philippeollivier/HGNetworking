using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextEventHandler : EventHandler
{
    public Text text;

    void Start()
    {
        //Subscribe to EventManager
        EventManager.SubscribeHandler(this);

        StartCoroutine(testPolling());
    }

    IEnumerator testPolling()
    {
        while (true)
        {
            Event_TEST_EVENT e;
            e = new Event_TEST_EVENT();
            e.Username = "Animbot";
            e.Number = Random.Range(-1000, 1000);
            e.Vec = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
            e.Quat = new Quaternion();
            e.Test = Random.Range(0, 1000000).ToString();

            QueueOutgoingEvent(e);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void HandleEvent(Event currEvent)
    {
        text.text = $"{currEvent}\n{text.text}";
    }
}
