namespace TheRingGoesSouth.scripts.data.skill
{
    public class SkillTargetingData
    {
        /// <summary>
        /// Type of targeting (e.g., "SingleEnemy", "AreaOfEffect").
        /// Your game logic will map this string to an enum or specific targeting behavior.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Maximum range for targeting (in game units/tiles).
        /// </summary>
        public int Range { get; set; }

        /// <summary>
        /// Radius for AreaOfEffect (if applicable). Defaults to 0 if not an AoE.
        /// </summary>
        public int AreaRadius { get; set; }

        /// <summary>
        /// Whether the skill requires a clear line of sight to the target.
        /// Defaults to true if not specified in JSON, but good practice to include it.
        /// </summary>
        public bool RequiresLineOfSight { get; set; } = true;

        /// <summary>
        /// For AreaOfEffect, indicates if it affects allies. Defaults to false.
        /// </summary>
        public bool AffectsAllies { get; set; }

        /// <summary>
        /// For AreaOfEffect, indicates if it affects enemies. Defaults to false.
        /// </summary>
        public bool AffectsEnemies { get; set; }

        // Default constructor for deserialization
        public SkillTargetingData() {}
    }
}