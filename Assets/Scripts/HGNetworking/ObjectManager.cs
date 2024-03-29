using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public enum objectType
{
    CubeGhost,
    ClientPlayer,
    ServerPlayer,
    ServerBall,
    ClientBall
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

    private void Start()
    {
        objectPrefabs[objectType.CubeGhost] = prefabs[(int)objectType.CubeGhost];
        objectPrefabs[objectType.ClientPlayer] = prefabs[(int)objectType.ClientPlayer];
        objectPrefabs[objectType.ServerPlayer] = prefabs[(int)objectType.ServerPlayer];
        objectPrefabs[objectType.ServerBall] = prefabs[(int)objectType.ServerBall];
        objectPrefabs[objectType.ClientBall] = prefabs[(int)objectType.ClientBall];
    }

}