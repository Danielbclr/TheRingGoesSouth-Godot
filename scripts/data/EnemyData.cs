using Godot; // For [GlobalClass] if you intend to use it as a Resource in editor

namespace TheRingGoesSouth.scripts.data
{
    // If you want to use this class as a custom resource type in the Godot editor,
    // you can uncomment the line below and make it inherit from Resource.
    // [GlobalClass] 
    // public partial class EnemyData : Resource 
    public partial class EnemyData // Or public class EnemyData if not a Godot Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public string ScenePath { get; set; }
        public int MoveRange { get; set; }

        // Default constructor is needed for JSON deserialization
        public EnemyData() {}
    }
}