using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public enum objectType
{
    TestGhost
}
public class ObjectManager : MonoBehaviour
{
    public static Dictionary<objectType, GameObject> objectPrefabs = new Dictionary<objectType, GameObject>();
    public GameObject[] prefabs;

    public static GameObject CreateObject(objectType objectType)
    {
        return Instantiate(objectPrefabs[objectType]);
    }

    public static void Initialize()
    {
    }

    private void FixedUpdate()
    {
        ConnectionManager.UpdateTick();
        StreamManager.UpdateTick();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConnectionManager.Connect(new IPEndPoint(IPAddress.Parse("25.15.133.160"), 6942));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectionManager.OpenServer(1, 6942);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GhostManager.NewGhost(GhostManager.ghostType.TestGhost);
        }
    }

    private void Start()
    {
        objectPrefabs[objectType.TestGhost] = prefabs[0];
    }
}