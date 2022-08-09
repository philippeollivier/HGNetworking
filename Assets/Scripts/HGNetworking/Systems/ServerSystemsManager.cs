using UnityEngine;

public class ServerSystemsManager : MonoBehaviour
{
    public void Awake()
    {
		ECSSystem.PhysicsSystem.Awake();
		Init();
	}

	public void FixedUpdate()
    {
        FixedUpdateServerSystems();
    }

    private void FixedUpdateServerSystems()
    {
		ECSSystem.SynchronizedClockSystem.FixedUpdate();
		ECSSystem.TestingSystem.FixedUpdate();
		
		//	EntityComponentSystem(Populate singleton component which has list of components and Map<EntityId, Entity>)
		//For each component type, we have a Map<EntityId, Component>;

		//Network Read
		//	EventManager Read(Write to events to queues singleton components)
		//	MoveManager Read(Update player input component for EntityId and Frame)

		//EventHandlers 1 (reads from events list singleton)
		//EventHandlers 2
		//EventHandlers 3

		//InputBufferSystem(updates player move manager component figures out buffer + RTT stuff)
		//ServerPlayerMovementManager(reads from player move manager component figures, apply forces)

		//Physics Tick
		ECSSystem.PhysicsSystem.FixedUpdate();

		//Network Write
		//	EventManager Write(Read from outgoing queues singleton)
		//	GhostManager Write(For each ghost that changed, write its component information into packet)
	}

	public static void Init()
	{
		//Components: When you add a component, add its type to the component dictionary.
		ECSSkeleton.ComponentLists.componentDictionary.AddComponentType<ECSSkeleton.GameObjectComponent>();
		ECSSkeleton.ComponentLists.componentDictionary.AddComponentType<ECSSkeleton.ColliderComponent>();
		ECSSkeleton.ComponentLists.componentDictionary.AddComponentType<ECSSkeleton.RigidBodyComponent>();

		//Archetypes: When you add an archetype, add it to the dictionary.
		ECSSkeleton.ComponentLists.archetypes.Add(new ECSSkeleton.PhysicsEntityArchetype());
		//ComponentLists.archetypes.Add(new ConnectionEntityArchetype());
	}
}
