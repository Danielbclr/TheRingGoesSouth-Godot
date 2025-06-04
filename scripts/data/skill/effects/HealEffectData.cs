namespace TheRingGoesSouth.scripts.data.skill.effects
{
    public class HealEffectData : SkillEffectData
    {
        public int BaseAmount { get; set; }

        /// <summary>
        /// The character stat that scales this healing (e.g., "Wisdom").
        /// Your game logic will map this to your StatType enum.
        /// </summary>
        public string ScalingStat { get; set; }
        public float ScalingFactor { get; set; }

        public HealEffectData() { Type = "Heal"; }
    }
}