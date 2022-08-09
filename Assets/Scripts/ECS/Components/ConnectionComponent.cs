using UnityEngine;

namespace ECS.Components
{
    public class ConnectionComponent : BaseComponent
    {
        //TODO: Add connection info

        public ConnectionComponent(int entityId)
        {
            this.entityId = entityId;
        }

        public ConnectionComponent()
        {

        }
    }
}