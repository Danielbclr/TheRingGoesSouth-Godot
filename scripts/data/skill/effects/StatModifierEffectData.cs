namespace TheRingGoesSouth.scripts.data.skill.effects
{
    public class StatModifierEffectData : SkillEffectData
    {
        /// <summary>
        /// The target stat to modify (e.g., "Defense", "Attack").
        /// Your game logic will map this to your StatType enum.
        /// </summary>
        public string TargetStat { get; set; }
        public float Amount { get; set; }
        public bool IsPercentage { get; set; }
        public int DurationTurns { get; set; }

        public StatModifierEffectData() { Type = "StatModifier"; }
    }
}