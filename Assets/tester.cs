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
            ComponentLists.entities.Add(1);
            Methods.AddGameObjectComponent(1, go);
            Methods.AddRigidbodyComponent(1, go.GetComponent<Rigidbody>());
            Methods.AddColliderComponent(1, go.GetComponent<BoxCollider>());
            
        }
    }
}
