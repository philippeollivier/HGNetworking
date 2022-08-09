using UnityEngine;

namespace ECS.Components
{
    public class RigidbodyComponent : BaseComponent
    {
        public Rigidbody rb;

        public RigidbodyComponent(int entityId)
        {
            this.entityId = entityId;
        }

        public RigidbodyComponent()
        {

        }
    }
}