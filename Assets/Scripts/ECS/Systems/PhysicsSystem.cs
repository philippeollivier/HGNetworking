using ECS.Components;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Systems
{
    public static class PhysicsSystem
    {
        #region Generic Systems Code
        public static void Awake()
        {
            Physics.autoSimulation = false;
        }

        public static void FixedUpdate()
        {
            PhysicsUpdateWithGhostFrames();
        }

        public static void PhysicsUpdateWithGhostFrames()
        {
            Physics.Simulate(Time.fixedDeltaTime);
            UpdateAllPhysicsGhostFrames();
        }

        public static void UpdateAllPhysicsGhostFrames()
        {
            foreach (int entityId in ComponentLists.archetypes[typeof(Archetypes.GhostedPhysicsEntityArchetype)].entities)
            {
                Rigidbody rigidbody = ComponentLists.componentDictionary.GetValueAtIndex<RigidbodyComponent>(entityId).rb;
                PhysicsGhostComponent physicsGhostComponent = ComponentLists.componentDictionary.GetValueAtIndex<PhysicsGhostComponent>(entityId);

                PhysicsState ps = new PhysicsState(rigidbody);
                physicsGhostComponent.historicalState[SynchronizedClock.CommandFrame] = ps;
                physicsGhostComponent.historicalState.Remove(SynchronizedClock.CommandFrame - SynchronizedClock.PhysicsGhostFrames);
            }
        }

        public static void ReplayNFrames(int replayFrames)
        {
            //Disable all GhostedGameObjects
            foreach (int entityId in ComponentLists.archetypes[typeof(Archetypes.GhostedPhysicsEntityArchetype)].entities)
            {
                ComponentLists.componentDictionary.GetValueAtIndex<GameObjectComponent>(entityId).gameObject.SetActive(false);
            }

            for (int i = SynchronizedClock.CommandFrame - replayFrames; i <= SynchronizedClock.CommandFrame; i++)
            {
                foreach (int entityId in ComponentLists.archetypes[typeof(Archetypes.GhostedPhysicsEntityArchetype)].entities)
                {
                    GameObject gameObject = ComponentLists.componentDictionary.GetValueAtIndex<GameObjectComponent>(entityId).gameObject;
                    Rigidbody rigidbody = ComponentLists.componentDictionary.GetValueAtIndex<RigidbodyComponent>(entityId).rb;
                    PhysicsGhostComponent physicsGhostComponent = ComponentLists.componentDictionary.GetValueAtIndex<PhysicsGhostComponent>(entityId);

                    //Seed the initial Physics position
                    if (physicsGhostComponent.historicalState.ContainsKey(i) && gameObject.activeInHierarchy == false)
                    {
                        gameObject.transform.rotation = physicsGhostComponent.historicalState[i].rotation;
                        gameObject.transform.position = physicsGhostComponent.historicalState[i].position;
                        rigidbody.velocity = physicsGhostComponent.historicalState[i].velocity;
                        rigidbody.angularVelocity = physicsGhostComponent.historicalState[i].angularVelocity;
                        gameObject.SetActive(true);
                    }
                }

                PhysicsUpdateWithGhostFrames();
            }
        }

        #endregion
    }
}
