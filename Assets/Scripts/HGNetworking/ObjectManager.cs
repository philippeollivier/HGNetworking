using System.Collections;
using System.Collections.Generic;
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

    public void Initialize()
    {
        objectPrefabs[objectType.TestGhost] = prefabs[0];
    }
}
