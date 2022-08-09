using UnityEngine;

namespace ECS.Components
{
    public class ColliderComponent : BaseComponent
    {
        public Collider col;

        public ColliderComponent(int entityId)
        {
            this.entityId = entityId;
        }

        public ColliderComponent()
        {

        }
    }
}