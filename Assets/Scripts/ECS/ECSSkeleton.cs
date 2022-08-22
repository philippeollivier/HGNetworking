using ECS.Archetypes;
using ECS.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

/* NEW COMPONENT CHECKLIST
 * 1) Create new component class deriving from BaseComponent (Look at GameObjectComponent as example)
 * 2) Add new component class to ComponentDictionary contains function
 * 3) Add new component type in InitializeComponentArchetypeLists
*/

/* NEW ARCHETYPE CHECKLIST
 * 1) Create new archetype class deriving from Archetype (Look at PhysicsObjectArchetype as an example)
 * 2) Add new archetype in InitializeComponentArchetypeLists
 */

namespace ECS
{
    public static class ComponentLists
    {
        public static List<int> entities = new List<int>();
        public static Dictionary<Type, Archetype> archetypes = new Dictionary<Type, Archetype>();
        public static ComponentDictionary componentDictionary = new ComponentDictionary();
    }

    public static class Utils
    {
        public static int AddEntity()
        {
            //This logic will need to change if we add entity deletion
            int id = ComponentLists.entities.Count;
            ComponentLists.entities.Add(id);
            return id;
        }

        public static void InitializeComponentArchetypeLists()
        {
            //Components: When you add a component, add its type to the component dictionary.
            ComponentLists.componentDictionary.AddComponentType<GameObjectComponent>();
            ComponentLists.componentDictionary.AddComponentType<ColliderComponent>();
            ComponentLists.componentDictionary.AddComponentType<RigidbodyComponent>();
            ComponentLists.componentDictionary.AddComponentType<PhysicsGhostComponent>();

            //Archetypes: When you add an archetype, add it to the dictionary.
            ComponentLists.archetypes.Add(typeof(PhysicsEntityArchetype), new PhysicsEntityArchetype());
            ComponentLists.archetypes.Add(typeof(GhostedPhysicsEntityArchetype), new GhostedPhysicsEntityArchetype());
        }

        #region Component Related Functions
        // A function needs to add to the dictionary list and must call MatchArchetypes at the end.
        public static void AddGameObjectComponent(int entityId, GameObject gameObject = null)
        {
            if (ValidateComponentExists(entityId, typeof(GameObjectComponent), false))
            {
                GameObjectComponent g = new GameObjectComponent(entityId);
                g.gameObject = (gameObject == null) ? (new GameObject("DefaultGameObjectComponent")) : (gameObject);
                ComponentLists.componentDictionary.Add(entityId, g);
            }
            MatchArchetypes(entityId);
        }

        public static void AddRigidbodyComponent(int entityId, Rigidbody rb = null)
        {
            if (ValidateComponentExists(entityId, typeof(GameObjectComponent), true) && ValidateComponentExists(entityId, typeof(RigidbodyComponent), false))
            {
                RigidbodyComponent rbc = new RigidbodyComponent(entityId);
                rbc.rb = (rb == null) ? (ComponentLists.componentDictionary.GetValueAtIndex<GameObjectComponent>(entityId).gameObject.AddComponent<Rigidbody>()) : (rb);
                ComponentLists.componentDictionary.Add(entityId, rbc);
            }
            MatchArchetypes(entityId);
        }

        public static void AddColliderComponent(int entityId, Collider collider = null)
        {
            if (ValidateComponentExists(entityId, typeof(GameObjectComponent), true) && ValidateComponentExists(entityId, typeof(ColliderComponent), false))
            {
                ColliderComponent cc = new ColliderComponent(entityId);
                cc.col = (collider == null) ? (ComponentLists.componentDictionary.GetValueAtIndex<GameObjectComponent>(entityId).gameObject.AddComponent<BoxCollider>()) : (collider);
                ComponentLists.componentDictionary.Add(entityId, cc);
            }
            MatchArchetypes(entityId);
        }

        public static void AddPhysicsGhostComponent(int entityId, PhysicsGhostComponent physicsGhostComponent = null)
        {
            if (ValidateComponentExists(entityId, typeof(GameObjectComponent), true) && ValidateComponentExists(entityId, typeof(RigidbodyComponent), true) && ValidateComponentExists(entityId, typeof(PhysicsGhostComponent), false))
            {
                PhysicsGhostComponent pgc = (physicsGhostComponent == null)?(new PhysicsGhostComponent(entityId)):(physicsGhostComponent);
                pgc.entityId = entityId;
                ComponentLists.componentDictionary.Add(entityId, pgc);
            }
            MatchArchetypes(entityId);
        }
        #endregion

        #region Private Helper Functions
        private static void MatchArchetype(int entityId, Archetype a)
        {
            foreach (Type type in a.pattern)
            {
                if (!ComponentLists.componentDictionary.Contains(type, entityId))
                {
                    for (int i = 0; i < a.entities.Count; i++)
                    {
                        if (a.entities[i] == entityId)
                        {
                            a.entities.RemoveAt(i);
                        }
                    }
                    return;
                }
            }
            foreach (Type type in a.antiPattern)
            {
                if (ComponentLists.componentDictionary.Contains(type, entityId))
                {
                    for (int i = 0; i < a.entities.Count; i++)
                    {
                        if (a.entities[i] == entityId)
                        {
                            a.entities.RemoveAt(i);
                        }
                    }
                    return;
                }
            }
            a.entities.Add(entityId);
        }

        private static void MatchArchetypes(int entityId)
        {
            foreach (Archetype a in ComponentLists.archetypes.Values)
            {
                MatchArchetype(entityId, a);
            }
        }

        private static bool ValidateComponentExists(int entityId, Type type, bool existance = true)
        {
            if (existance == ComponentLists.componentDictionary.Contains(type, entityId))
            {
                return true;
            }
            Debug.LogError($"Was expecting entity {entityId} to {((existance) ? ("not ") : (""))} have an existing {type} associated, but it didn't");
            return false;
        }
        #endregion
    }
}