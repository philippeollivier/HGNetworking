namespace ECS.Archetypes
{
    public class PhysicsEntityArchetype : Archetype
    {
        public PhysicsEntityArchetype()
        {
            pattern.Add(typeof(Components.GameObjectComponent));
            pattern.Add(typeof(Components.RigidbodyComponent));
            pattern.Add(typeof(Components.ColliderComponent));
        }
    }
}
