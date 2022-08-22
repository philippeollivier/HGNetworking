using UnityEngine;

public class tester : MonoBehaviour
{
    public GameObject physicsCube;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for(int i = 0; i < 20; i++)
            {
                int entityId = ECS.Utils.AddEntity();
                GameObject go = Instantiate(physicsCube);
                go.transform.position = Vector3.up * 5f + Vector3.forward * i * 1.5f;
                ECS.Utils.AddGameObjectComponent(entityId, go);
                ECS.Utils.AddRigidbodyComponent(entityId, go.GetComponent<Rigidbody>());
                ECS.Utils.AddColliderComponent(entityId, go.GetComponent<BoxCollider>());
                ECS.Utils.AddPhysicsGhostComponent(entityId);
            }
        }
    }
}
