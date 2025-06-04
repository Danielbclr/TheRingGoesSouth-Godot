// File: c:\Users\nihan\Documents\Projetos\the-ring-goes-south\scripts\data\skill\effects\DamageEffectData.cs
namespace TheRingGoesSouth.scripts.data.skill.effects
{
    public class DamageEffectData : SkillEffectData
    {
        /// <summary>
        /// Type of damage (e.g., "Physical", "Fire").
        /// Your game logic will map this to an enum or damage calculation rules.
        /// </summary>
        public string DamageType { get; set; }

        public int BaseAmount { get; set; }

        /// <summary>
        /// The character stat that scales this damage (e.g., "Dexterity").
        /// Your game logic will map this to your StatType enum.
        /// </summary>
        public string ScalingStat { get; set; }
        public float ScalingFactor { get; set; }
        public bool CanCrit { get; set; }
        public float BonusCritChance { get; set; }

        public DamageEffectData() { Type = "Damage"; }
    }
}