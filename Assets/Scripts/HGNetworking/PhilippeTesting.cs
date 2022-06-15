using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhilippeTesting : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(testPolling());
    }

    IEnumerator testPolling()
    {
        while (true)
        {
            MetricsManager.AddDatapointToMetric("testing", UnityEngine.Random.Range(-1.0f, 1.0f), true);

            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Events.Event_TEST_EVENT e = new Events.Event_TEST_EVENT();
        //    e.Username = "animbot";
        //    e.Number = 14;
        //    e.Vec = new Vector3(1, 2, 3);
        //    e.Quat = new Quaternion();
        //    e.Test = "TADA";

        //    Events.Event_TEST_EVENT e2 = new Events.Event_TEST_EVENT();
        //    e2.Username = "animb23123123123123ot";
        //    e2.Number = 14;
        //    e2.Vec = new Vector3(1, 2, 3);
        //    e2.Quat = new Quaternion();
        //    e2.Test = "TADA";


        //    Debug.Log($"Size of e is {e.GetSize()}");
        //    Debug.Log($"Size of e2 is {e2.GetSize()}");
        //}
    }
}
