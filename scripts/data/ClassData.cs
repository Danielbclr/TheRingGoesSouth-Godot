using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TheRingGoesSouth.scripts.data.skill;

namespace TheRingGoesSouth.scripts.data
{
    public partial class ClassData 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Collections.Generic.Dictionary<StatType, int> BaseStats { get; set; } = [];
        public string ScenePath { get; set; }
        public Array<string> Skills { get; set; } = [];
        public Array<string> ClassPerks { get; set; } = [];

        public ClassData() {}
    }
}