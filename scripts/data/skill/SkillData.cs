using System.Collections.Generic;
using Godot;
using TheRingGoesSouth.scripts.data.skill.effects;

namespace TheRingGoesSouth.scripts.data.skill
{
    public partial class SkillData : GodotObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SkillTargetingData Targeting { get; set; }

        /// <summary>
        /// List of effects this skill applies. Will contain instances of
        /// DamageEffectData, HealEffectData, etc., after deserialization.
        /// </summary>
        public List<SkillEffectData> Effects { get; set; }

        // Default constructor for deserialization
        public SkillData()
        {
            // Initialize collections and complex objects to avoid null references
            Targeting = new SkillTargetingData();
            Effects = new List<SkillEffectData>();
        }
    }
}