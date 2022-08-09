using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour
{
    public GameObject physicsCube;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int entityId = 1;
            GameObject go = GameObject.Instantiate(physicsCube);
            ECSSkeleton.ComponentLists.entities.Add(1);
            ECSSkeleton.Methods.AddGameObjectComponent(1);

        }
    }
}
