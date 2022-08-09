using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECSSkeleton
{
    public enum ComponentId
    {
        gameObjectComponent,
        rigidBodyComponent,
        colliderComponent
    }
    public class Test
    {

    }
    public class Entity
    {

    }

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
            pattern.Add(typeof(GameObjectComponent));
            pattern.Add(typeof(RigidBodyComponent));
            pattern.Add(typeof(ColliderComponent));
        }
    }

    public class ConnectionEntityArchetype : Archetype
    {

    }

    public static class ComponentLists
    {
        public static List<int> entities = new List<int>();
        public static GameObjectComponent dummyGameObjectComponent = new GameObjectComponent();
        public static List<Archetype> archetypes = new List<Archetype>();
        public static ComponentDictionary componentDictionary = new ComponentDictionary();

    }

    public static class Methods
    {
        public static void Init()
        {
            ComponentLists.componentDictionary.AddComponentType<GameObjectComponent>();
            ComponentLists.componentDictionary.AddComponentType<ColliderComponent>();
            ComponentLists.componentDictionary.AddComponentType<RigidBodyComponent>();

            ComponentLists.archetypes.Add(new PhysicsEntityArchetype());
            //ComponentLists.archetypes.Add(new ConnectionEntityArchetype());
        }

        public static void MatchArchetype(int entityId, Archetype a)
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
        //To add a function 
        public static void AddGameObjectComponent(int entityId)
        {
            if(!ComponentLists.componentDictionary.Contains(typeof(GameObjectComponent), entityId))
            {
                GameObjectComponent g = new GameObjectComponent(entityId);
                g.gameObject = new GameObject("Test");
                ComponentLists.componentDictionary.Add(entityId, g);
            } else
            {
                Debug.Log("Entity already has gameObject associated");
            }
            MatchArchetypes(entityId);
        }

        public static void AddRigidbodyComponent(int entityId)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(GameObjectComponent), entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if(ComponentLists.componentDictionary.Contains(typeof(RigidBodyComponent), entityId))
            {
                Debug.Log("Entity already has rigidbody associated");
            }
            else
            {
                RigidBodyComponent rbc = new RigidBodyComponent(entityId);
                rbc.rb = ComponentLists.componentDictionary.GetValueAtIndex<GameObjectComponent>(entityId).gameObject.AddComponent<Rigidbody>();
                ComponentLists.componentDictionary.Add(entityId, rbc);
            }
            MatchArchetypes(entityId);
        }

        public static void AddColliderComponent(int entityId)
        {
            if (!ComponentLists.componentDictionary.Contains(typeof(GameObjectComponent), entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if (ComponentLists.componentDictionary.Contains(typeof(ColliderComponent), entityId))
            {
                Debug.Log("Entity already has collider associated");
            }
            else
            {
                ColliderComponent cc = new ColliderComponent(entityId);
                cc.col = ComponentLists.componentDictionary.GetValueAtIndex<GameObjectComponent>(entityId).gameObject.AddComponent<BoxCollider>();
                ComponentLists.componentDictionary.Add(entityId, cc);
            }
            MatchArchetypes(entityId);
        }
    }
    #endregion



}
