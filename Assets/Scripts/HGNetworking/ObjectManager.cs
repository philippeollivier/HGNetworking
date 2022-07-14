using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public enum objectType
{
    TestGhost,
    ClientPlayer,
    ServerPlayer
}
public class ObjectManager : MonoBehaviour
{
    #region Singleton Design
    private static ObjectManager _instance;

    public static ObjectManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    public Dictionary<objectType, GameObject> objectPrefabs = new Dictionary<objectType, GameObject>();
    public GameObject[] prefabs = new GameObject[3];

    public GameObject CreateObject(objectType objectType)
    {
        return Instantiate(objectPrefabs[objectType]);
    }

    public static void Initialize()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Ghost g = GhostManager.NewGhost(GhostManager.ghostType.Player);
        }
    }

    private void Start()
    {
        objectPrefabs[objectType.TestGhost] = prefabs[0];
        objectPrefabs[objectType.ClientPlayer] = prefabs[(int)objectType.ClientPlayer];
        objectPrefabs[objectType.ServerPlayer] = prefabs[(int)objectType.ServerPlayer];
    }

}