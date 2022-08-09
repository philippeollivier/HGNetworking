using UnityEngine;

namespace ECS.Components
{
    public class GameObjectComponent : BaseComponent
    {
        public GameObject gameObject;

        public GameObjectComponent(int entityId)
        {
            this.entityId = entityId;
        }

        public GameObjectComponent()
        {

        }
    }

}