using ECSArchetype;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECSComponent
{
    public class EComponent
    {
        public int entityId;
    }
    public class GameObjectComponent : EComponent
    {
        public GameObject gameObject;
        public GameObjectComponent(int entityId)
        {
            this.entityId = entityId;
        }
        public GameObjectComponent()
        {
        }
    }

    public class RigidBodyComponent : EComponent
    {
        public Rigidbody rb;
        public RigidBodyComponent(int entityId)
        {
            this.entityId = entityId;
        }
    }
    public class ColliderComponent : EComponent
    {
        public Collider col;
        public ColliderComponent(int entityId)
        {
            this.entityId = entityId;
        }
    }

    public class ConnectionComponent : EComponent
    {
        public Collider col;
        public ConnectionComponent(int entityId)
        {
            this.entityId = entityId;
        }
    }
}

namespace ECSArchetype
{
    //When adding an archetype. Add any component types to its pattern and nothing else. If an archetype cannot have a component, add it to the antiPattern
    public abstract class Archetype
    {
        public List<int> entities = new List<int>();
        public List<Type> pattern = new List<Type>();
        public List<Type> antiPattern = new List<Type>();
    }

    public class PhysicsEntityArchetype : Archetype
    {
        public PhysicsEntityArchetype()
        {
            pattern.Add(typeof(ECSComponent.GameObjectComponent));
            pattern.Add(typeof(ECSComponent.RigidBodyComponent));
            pattern.Add(typeof(ECSComponent.ColliderComponent));
        }
    }

    public class ConnectionEntityArchetype : Archetype
    {

    }
}
    

    public static class ComponentLists
    {
        public static List<int> entities = new List<int>();
        public static List<ECSArchetype.Archetype> archetypes = new List<ECSArchetype.Archetype>();
        public static ComponentDictionary componentDictionary = new ComponentDictionary();

    }

    public static class Methods
    {
        public static void MatchArchetype(int entityId, ECSArchetype.Archetype a)
        {
            foreach(Type type in a.pattern)
            {
                if (!ComponentLists.componentDictionary.Contains(type, entityId))
                {
                    for(int i = 0; i < a.entities.Count; i++)
                    {
                        if(a.entities[i] == entityId)
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
            Debug.Log($"Added to ${a}");
            a.entities.Add(entityId);
        }

        public static void MatchArchetypes(int entityId)
        {
            foreach(Archetype a in ComponentLists.archetypes)
            {
                MatchArchetype(entityId, a);
            }
        }

        #region Component Adding Functions
        // A function needs to add to the dictionary list and must call MatchArchetypes at the end.
        public static void AddGameObjectComponent(int entityId)
        {
            if(!ComponentLists.componentDictionary.Contains(typeof(ECSComponent.GameObjectComponent), entityId))
            {
                ECSComponent.GameObjectComponent g = new ECSComponent.GameObjectComponent(entityId);
                g.gameObject = new GameObject("Test");
                ComponentLists.componentDictionary.Add(entityId, g);
            } else
            {
                Debug.Log("Entity already has gameObject associated");
            }
            MatchArchetypes(entityId);
        }
        public static void AddGameObjectComponent(int entityId, GameObject gameObject)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(ECSComponent.GameObjectComponent), entityId))
            {
            ECSComponent.GameObjectComponent g = new ECSComponent.GameObjectComponent(entityId);
                g.gameObject = gameObject;
                ComponentLists.componentDictionary.Add(entityId, g);
            }
            else
            {
                Debug.Log("Entity already has gameObject associated");
            }
            MatchArchetypes(entityId);
        }

        public static void AddRigidbodyComponent(int entityId)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(ECSComponent.GameObjectComponent), entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if(ComponentLists.componentDictionary.Contains(typeof(ECSComponent.RigidBodyComponent), entityId))
            {
                Debug.Log("Entity already has rigidbody associated");
            }
            else
            {
            ECSComponent.RigidBodyComponent rbc = new ECSComponent.RigidBodyComponent(entityId);
                rbc.rb = ComponentLists.componentDictionary.GetValueAtIndex<ECSComponent.GameObjectComponent>(entityId).gameObject.AddComponent<Rigidbody>();
                ComponentLists.componentDictionary.Add(entityId, rbc);
            }
            MatchArchetypes(entityId);
        }

        public static void AddRigidbodyComponent(int entityId, Rigidbody rb)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(ECSComponent.GameObjectComponent), entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if (ComponentLists.componentDictionary.Contains(typeof(ECSComponent.RigidBodyComponent), entityId))
            {
                Debug.Log("Entity already has rigidbody associated");
            }
            else
            {
            ECSComponent.RigidBodyComponent rbc = new ECSComponent.RigidBodyComponent(entityId);
                rbc.rb = rb;
                ComponentLists.componentDictionary.Add(entityId, rb);
            }
            MatchArchetypes(entityId);
        }

        public static void AddColliderComponent(int entityId)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(ECSComponent.GameObjectComponent), entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if (ComponentLists.componentDictionary.Contains(typeof(ECSComponent.ColliderComponent), entityId))
            {
                Debug.Log("Entity already has collider associated");
            }
            else
            {
            ECSComponent.ColliderComponent cc = new ECSComponent.ColliderComponent(entityId);
                cc.col = ComponentLists.componentDictionary.GetValueAtIndex<ECSComponent.GameObjectComponent>(entityId).gameObject.AddComponent<BoxCollider>();
                ComponentLists.componentDictionary.Add(entityId, cc);
            }
            MatchArchetypes(entityId);
        }

        public static void AddColliderComponent(int entityId, Collider collider)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(ECSComponent.GameObjectComponent), entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if (ComponentLists.componentDictionary.Contains(typeof(ECSComponent.ColliderComponent), entityId))
            {
                Debug.Log("Entity already has collider associated");
            }
            else
            {
            ECSComponent.ColliderComponent cc = new ECSComponent.ColliderComponent(entityId);
                cc.col = collider;
                ComponentLists.componentDictionary.Add(entityId, cc);
            }
            MatchArchetypes(entityId);
        }

    public static int AddEntity()
    {
        int id = ComponentLists.entities.Count;
        ComponentLists.entities.Add(id);
        return id;
    } 
        #endregion

    }
