using UnityEngine;

public class ClientSystemsManager : MonoBehaviour
{
    public void Awake()
    {
		HG.Networking.StartNetworkingClient(HG.NetworkingConstants.CLIENT_PORT, false);
		ECS.Utils.InitializeComponentArchetypeLists();
		ECS.Systems.PhysicsSystem.Awake();
	}

	public void FixedUpdate()
    {
		FixedUpdateClientSystems();
	}

	private void FixedUpdateClientSystems()
    {
		ECS.Systems.SynchronizedClockSystem.FixedUpdate();
		//Read/Process all incoming UDP packets for this frame on main thread
		HG.Networking.NetworkingThreadManager.ReadAsyncPackets();

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

		ECS.Systems.PhysicsSystem.FixedUpdate();

		HG.Networking.ConnectionManager.FixedUpdate();
	}
}
