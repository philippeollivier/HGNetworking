namespace ECS.Archetypes
{
    public class PhysicsEntityArchetype : Archetype
    {
        public PhysicsEntityArchetype()
        {
            pattern.Add(typeof(ECS.Components.GameObjectComponent));
            pattern.Add(typeof(ECS.Components.RigidbodyComponent));
            pattern.Add(typeof(ECS.Components.ColliderComponent));
        }
    }
}
