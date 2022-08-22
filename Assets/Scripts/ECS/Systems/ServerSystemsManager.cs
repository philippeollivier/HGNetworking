using UnityEngine;

public class ServerSystemsManager : MonoBehaviour
{
    public void Awake()
	{
		ECS.Utils.InitializeComponentArchetypeLists();
		ECS.Systems.PhysicsSystem.Awake();

		HG.Networking.StartNetworkingClient(HG.NetworkingConstants.SERVER_PORT, true);
	}

	public void FixedUpdate()
    {
        FixedUpdateServerSystems();
    }

    private void FixedUpdateServerSystems()
    {
		//Updates the command frame
		ECS.Systems.SynchronizedClockSystem.FixedUpdate();

		//Read/Process all incoming UDP packets for this frame on main thread
		HG.Networking.NetworkingThreadManager.ReadAsyncPackets();

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


		ECS.Systems.PhysicsSystem.FixedUpdate();

		HG.Networking.ConnectionManager.FixedUpdate();
	}
}
