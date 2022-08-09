using ECSComponent;
using ECSSkeleton;
using System.Collections.Generic;
using UnityEngine;

namespace ECSSystem
{
    public static class PhysicsSystem
    {
        #region Generic Systems Code

        static bool oneTime = true, asdTime = true;

        public static void Awake()
        {
            Physics.autoSimulation = false;
        }

        public static void FixedUpdate()
        {
            Physics.Simulate(Time.fixedDeltaTime);
            UpdateAllPhysicsGhostFrames();

            if(Input.GetKey(KeyCode.Space) && oneTime)
            {
                Debug.Log("Rewinding 10 frames");
                ReplayNFrames(10);
                oneTime = false;
            }
            if (Input.GetKey(KeyCode.E) && asdTime)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = Vector3.up;
                asdTime = false;
            }
        }

        public static void UpdateAllPhysicsGhostFrames()
        {
            foreach (KeyValuePair<int, PhysicsGhostComponent> entry in ComponentLists.componentDictionary.GetDict<PhysicsGhostComponent>())
            {
                Rigidbody rigidbody = ComponentLists.componentDictionary.GetValueAtIndex<Rigidbody>(entry.Key);

                entry.Value.historicalState.Add(SynchronizedClock.CommandFrame, new PhysicsState(rigidbody));
                entry.Value.historicalState.Remove(SynchronizedClock.CommandFrame - SynchronizedClock.PhysicsGhostFrames);
            }
        }

        public static void ReplayNFrames(int replayFrames)
        {
            for(int i = SynchronizedClock.CommandFrame - replayFrames; i <= SynchronizedClock.CommandFrame; i++)
            {
                foreach(KeyValuePair<int, PhysicsGhostComponent> entry in ComponentLists.componentDictionary.GetDict<PhysicsGhostComponent>())
                {
                    Rigidbody rigidbody = ComponentLists.componentDictionary.GetValueAtIndex<Rigidbody>(entry.Key);
                    GameObject gameObject = ComponentLists.componentDictionary.GetValueAtIndex<GameObject>(entry.Key);
                    //If player movement controller
                    //If other move controller do the stuff here as well

                    if (entry.Value.historicalState.ContainsKey(entry.Key) && gameObject.activeInHierarchy == false)
                    {
                        rigidbody.rotation = entry.Value.historicalState[i].rotation;
                        rigidbody.position = entry.Value.historicalState[i].position;
                        rigidbody.velocity = entry.Value.historicalState[i].velocity;
                        rigidbody.angularVelocity = entry.Value.historicalState[i].velocity;
                        gameObject.SetActive(true);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }

                Physics.Simulate(Time.fixedDeltaTime);
            }
        }

        #endregion
    }
}
