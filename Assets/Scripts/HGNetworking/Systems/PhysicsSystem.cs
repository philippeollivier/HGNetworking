using System.Collections.Generic;
using UnityEngine;

namespace ECSSystem
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
            Physics.Simulate(Time.fixedDeltaTime);
        }

        public static void FixedUpdateFrame()
        {

        }

        public static void UpdateAllPhysicsGhostFrames()
        {

        }

        public static void RewindStateNFrames(int rewindFrames)
        {
            //For every PhysicsGhostObject in the scene
            List<PhysicsGhostComponent> unloadedPhysicsObjects = new List<PhysicsGhostComponent>(); // = GEtcomponents<whatever>
            List<PhysicsGhostComponent> loadedPhysicsObjects = new List<PhysicsGhostComponent>();

            foreach (PhysicsGhostComponent physicsGhostComponent in unloadedPhysicsObjects)
            {
                //Get player controller if it has one
                //Get physics controller if it has one
                //Get rigidbody
                //Get gameobject associated with it
                GameObject gameObject;
                Rigidbody rb = null;
                

                //Some physics objects might not have been initialized here, so we need to do frame, load all that can be loaded.
                //Update all that have been loaded
                //
            }
        }

        #endregion
    }
}
