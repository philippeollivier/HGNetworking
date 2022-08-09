using System.Collections.Generic;

namespace ECS.Archetypes
{
    //When adding an archetype. Add any component types to its pattern and nothing else. If an archetype cannot have a component, add it to the antiPattern
    public abstract class Archetype
    {
        public List<int> entities = new List<int>();
        public List<System.Type> pattern = new List<System.Type>();
        public List<System.Type> antiPattern = new List<System.Type>();
    }
}