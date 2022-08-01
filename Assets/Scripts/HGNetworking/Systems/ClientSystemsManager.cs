using UnityEngine;

public class ClientSystemsManager : MonoBehaviour
{
    public void Awake()
    {
		ECSSystem.PhysicsSystem.Awake();   
    }

    public void FixedUpdate()
    {
		FixedUpdateClientSystems();
	}

	private void FixedUpdateClientSystems()
    {
		ECSSystem.SynchronizedClockSystem.FixedUpdate();

		//Read/Process all incoming UDP packets for this frame on main thread
		NetworkingThreadManager.ReadAsyncPackets();

		

		//Network Read
		//EventManager Read(Write to events to queues singleton components)

		/*
		What does Event Manager Do?

		Reads component with event information and updates handlers accordingly. 

		How does server event manager differ?

		it is reading multiple events from multiple client Id
		
		For each connectionId it has a Queue of Incoming Events populated by Stream Manager

		Event Manager "Handles incoming event"
		*/

		//GhostManager Read(Ghost component, which has transform, ghost history etcs) (This has #frame number, #entity id, components values, conditionally write it to reconciler singleton)

		//InputManager(Writes to input singleton(just a buncha bools))

		//EventHandlers 1(reads from events list singleton)
		//EventHandlers 2+
		//EventHandlers 3

		//PlatformSystem ?? TBD

		//ClientPhysicsReconciler(reads ghost information and compares to historical state component)
		//ControllerObjectsReconciller(same ^)

		//PlayerStateManager(sets state grounded)
		//PlayerMovementManager(reads from input manager, apply forces)

		ECSSystem.PhysicsSystem.FixedUpdate();

		ConnectionManager.UpdateTick();
		StreamManager.WriteToAllConnections();
	}
}
