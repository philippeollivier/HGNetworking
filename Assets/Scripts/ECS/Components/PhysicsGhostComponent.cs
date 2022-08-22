using System.Collections.Generic;

namespace ECS.Components
{
    public class PhysicsGhostComponent : BaseComponent
    {
        public Dictionary<int, PhysicsState> historicalState = new Dictionary<int, PhysicsState>();

        public PhysicsGhostComponent(int entityId)
        {
            this.entityId = entityId;
        }

        public PhysicsGhostComponent()
        {

        }
    }

}