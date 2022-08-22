namespace ECS.Archetypes
{
    public class GhostedPhysicsEntityArchetype : Archetype
    {
        public GhostedPhysicsEntityArchetype()
        {
            pattern.Add(typeof(Components.GameObjectComponent));
            pattern.Add(typeof(Components.RigidbodyComponent));
            pattern.Add(typeof(Components.ColliderComponent));
            pattern.Add(typeof(Components.PhysicsGhostComponent));
        }
    }
}
