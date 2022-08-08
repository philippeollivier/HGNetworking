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
        public static Dictionary<int, GameObjectComponent> gameObjectComponents = new Dictionary<int, GameObjectComponent>();
        public static Dictionary<int, RigidBodyComponent> rigidBodyComponents = new Dictionary<int, RigidBodyComponent>();
        public static Dictionary<int, ColliderComponent> colliderComponents = new Dictionary<int, ColliderComponent>();
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
            ComponentLists.archetypes.Add(new ConnectionEntityArchetype());

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
            a.entities.Add(entityId);
        }

        public static void MatchArchetypes(int entityId)
        {
            foreach(Archetype a in ComponentLists.archetypes)
            {
                MatchArchetype(entityId, a);
            }
        }
        //public void MatchPhysicsArchetype(int entityId)
        //{
        //     if(ComponentLists.gameObjectComponents.ContainsKey(entityId) &&
        //ComponentLists.rigidBodyComponents.ContainsKey(entityId) &&
        //ComponentLists.colliderComponents.ContainsKey(entityId))
        //    {
        //        ComponentLists.archetypes[0].entities.Add(entityId);
        //    } else
        //    {
        //        ComponentLists.archetypes[0].entities.Remove(entityId);

        //    }
        //}

        //public void MatchConnectionArchetype(int entityId)
        //{
        //}

        public static void AddGameObjectComponent(int entityId)
        {
            if(!ComponentLists.gameObjectComponents.ContainsKey(entityId))
            {
                GameObjectComponent g = new GameObjectComponent(entityId);
                g.gameObject = new GameObject("Test");
                ComponentLists.gameObjectComponents[entityId] = g;
            } else
            {
                Debug.Log("Entity already has gameObject associated");
            }
            MatchArchetypes(entityId);
        }

        public static void AddRigidbodyComponent(int entityId)
        {
            if (!ComponentLists.gameObjectComponents.ContainsKey(entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if(ComponentLists.rigidBodyComponents.ContainsKey(entityId))
            {
                Debug.Log("Entity already has rigidbody associated");
            }
            else
            {
                RigidBodyComponent rbc = new RigidBodyComponent(entityId);
                rbc.rb = ComponentLists.gameObjectComponents[entityId].gameObject.AddComponent<Rigidbody>();
                ComponentLists.rigidBodyComponents[entityId] = rbc;
            }
            MatchArchetypes(entityId);
        }

        public static void AddColliderComponent(int entityId)
        {
            if (!ComponentLists.gameObjectComponents.ContainsKey(entityId))
            {
                Debug.Log("Entity has no gameObject associated");
            }
            else if (ComponentLists.colliderComponents.ContainsKey(entityId))
            {
                Debug.Log("Entity already has rigidbody associated");
            }
            else
            {
                ColliderComponent cc = new ColliderComponent(entityId);
                cc.col = ComponentLists.gameObjectComponents[entityId].gameObject.AddComponent<BoxCollider>();
                ComponentLists.colliderComponents[entityId] = cc;
            }
            MatchArchetypes(entityId);
        }
    }



}
