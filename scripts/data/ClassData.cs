using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace TheRingGoesSouth.scripts.data
{
    public partial class ClassData 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Collections.Generic.Dictionary<StatType, int> BaseStats { get; set; } = [];
        public string ScenePath { get; set; }
        public Array<string> ClassSkills { get; set; } = [];
        public Array<string> ClassPerks { get; set; } = [];

        public ClassData() {}
    }
}