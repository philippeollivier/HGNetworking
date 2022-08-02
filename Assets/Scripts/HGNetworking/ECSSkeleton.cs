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
        public int 
    }

    public class GameObjectComponent : EComponent
    {
        GameObject gameObject;
        public GameObjectComponent(int entityId)
        {
            this.entityId = entityId;
        }
    }

    public class RigidBodyComponent : EComponent
    {
        Rigidbody rb;
    }
    public class ColliderComponent : EComponent
    {
        Collider col;
    }

    public abstract class Archetype
    {
        public List<int> entities = new List<int>();
        public List<Type> pattern = new List<Type>();
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
        public static List<Archetype> archetypes = new List<Archetype>();
        public static Dictionary<int, GameObjectComponent> gameObjectComponents = new Dictionary<int, GameObjectComponent>();
        public static Dictionary<int, RigidBodyComponent> rigidBodyComponents = new Dictionary<int, RigidBodyComponent>();
        public static Dictionary<int, ColliderComponent> colliderComponents = new Dictionary<int, ColliderComponent>();
        public static ComponentDictionary componentDictionary = new ComponentDictionary();

    }

    public class Methods
    {
        public void Init()
        {
            ComponentLists.componentDictionary.AddComponentType<GameObjectComponent>();
            ComponentLists.componentDictionary.AddComponentType<ColliderComponent>();
            ComponentLists.componentDictionary.AddComponentType<RigidBodyComponent>();

            ComponentLists.archetypes.Add(new PhysicsEntityArchetype());
            ComponentLists.archetypes.Add(new ConnectionEntityArchetype());

        }

        public void MatchArchetype(int entityId, Archetype a)
        {
            bool doesArchetypeMatch = true;
            foreach(Type type in a.pattern)
            {
                switch (type)
                {
                    case Type RigidBodyComponent:
                        doesArchetypeMatch |= ComponentLists.componentDictionary.Contains<RigidBodyComponent>(entityId);
                        break;
                    default:
                        throw new ArgumentException($"Type is not currently handled by MatchArchetype: {type}");
                }
            }
        }
        public void MatchPhysicsArchetype(int entityId)
        {
             if(ComponentLists.gameObjectComponents.ContainsKey(entityId) &&
        ComponentLists.rigidBodyComponents.ContainsKey(entityId) &&
        ComponentLists.colliderComponents.ContainsKey(entityId))
            {
                ComponentLists.archetypes[0].entities.Add(entityId);
            } else
            {
                ComponentLists.archetypes[0].entities.Remove(entityId);

            }
        }

        public void MatchConnectionArchetype(int entityId)
        {
        }

        public void AddGameObjectComponent(int entityId)
        {
            if(ComponentLists.gameObjectComponents[entityId] == null)
            {
                GameObjectComponent g = new GameObjectComponent(entityId);
                ComponentLists.gameObjectComponents[entityId] = g;
            } else
            {
                Debug.Log("Entity already has gameObject associated");
            }
            MatchArchetypes(entityId);


        }
    }



}
