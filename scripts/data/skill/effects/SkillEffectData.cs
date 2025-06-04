namespace TheRingGoesSouth.scripts.data.skill.effects
{
    /// <summary>
    /// Base class for all skill effects.
    /// The "Type" property from JSON (e.g., "Damage", "Heal") will be used by a
    /// custom deserializer to instantiate the correct derived class.
    /// </summary>
    public abstract class SkillEffectData
    {
        /// <summary>
        /// The type of effect, used for deserialization and game logic.
        /// This should be set by the derived class constructors or the deserializer.
        /// </summary>
        public string Type { get; protected set; }

        /// <summary>
        /// Chance for this specific effect to be applied (0.0 to 1.0).
        /// Defaults to 1.0 (100%) if not specified in JSON.
        /// </summary>
        public float ApplicationChance { get; set; } = 1.0f;
    }
}