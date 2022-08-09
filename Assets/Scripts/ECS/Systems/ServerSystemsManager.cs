using UnityEngine;

public class ServerSystemsManager : MonoBehaviour
{
    public void Awake()
    {
		ECS.Methods.InitializeComponentArchetypeLists();
		ECS.Systems.PhysicsSystem.Awake();
	}

	public void FixedUpdate()
    {
        FixedUpdateServerSystems();
    }

    private void FixedUpdateServerSystems()
    {
		ECS.Systems.SynchronizedClockSystem.FixedUpdate();
		ECS.Systems.TestingSystem.FixedUpdate();
		
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
		ECS.Systems.PhysicsSystem.FixedUpdate();

		//Network Write
		//	EventManager Write(Read from outgoing queues singleton)
		//	GhostManager Write(For each ghost that changed, write its component information into packet)
	}
}
