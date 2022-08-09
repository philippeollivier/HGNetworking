using UnityEngine;

public class ClientSystemsManager : MonoBehaviour
{
    public void Awake()
    {
		ECSSystem.PhysicsSystem.Awake();
//		ECSSystem.ClientInputBufferSystem.Initialize();
		Init();
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

	public static void Init()
	{
		//Components: When you add a component, add its type to the component dictionary.
		ComponentLists.componentDictionary.AddComponentType<ECSComponent.GameObjectComponent>();
		ComponentLists.componentDictionary.AddComponentType<ECSComponent.ColliderComponent>();
		ComponentLists.componentDictionary.AddComponentType<ECSComponent.RigidBodyComponent>();

		//Archetypes: When you add an archetype, add it to the dictionary.
		ComponentLists.archetypes.Add(new ECSArchetype.PhysicsEntityArchetype());
		//ComponentLists.archetypes.Add(new ConnectionEntityArchetype());
	}
}
