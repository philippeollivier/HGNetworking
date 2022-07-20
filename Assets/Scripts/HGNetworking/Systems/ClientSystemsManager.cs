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
		ECSSystem.TestingSystem.FixedUpdate();
		
		//EntityComponentSystem(Populate singleton component which has list of components and Map<EntityId, Entity>)
		//		Entity is { EntityId; Map<ComponentType, Component>;

		//Network Read
		//EventManager Read(Write to events to queues singleton components)
		//GhostManager Read(Ghost component, which has transform, ghost history etcs) (This has #frame number, #entity id, components values, conditionally write it to reconciler singleton)

		//InputManager(Writes to input singleton(just a buncha bools))

		//EventHandlers 1(reads from events list singleton)
		//EventHandlers 2
		//EventHandlers 3

		//PlatformSystem ?? TBD

		//ClientPhysicsReconciler(reads ghost information and compares to historical state component)
		//ControllerObjectsReconciller(same ^)

		//PlayerStateManager(sets state grounded)
		//PlayerMovementManager(reads from input manager, apply forces)

		ECSSystem.PhysicsSystem.FixedUpdate();

		//Network Write
		//EventManager Write(Read from outgoing queues singleton)
		//MoveManager Write(Writes from input singleton)
	}
}
