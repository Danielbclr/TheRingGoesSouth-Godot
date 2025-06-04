// File: c:\Users\nihan\Documents\Projetos\the-ring-goes-south\scripts\data\skill\effects\StatusEffectApplicationData.cs
namespace TheRingGoesSouth.scripts.data.skill.effects
{
    public class StatusApplyEffectData : SkillEffectData
    {
        /// <summary>
        /// Identifier for the status effect to be applied (e.g., "poison_weak").
        /// This would link to another data structure defining status effects.
        /// </summary>
        public string StatusEffectType { get; set; }
        public int DurationTurns { get; set; }

        public StatusApplyEffectData() { Type = "StatusApply"; }
    }
}