using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TheRingGoesSouth.scripts.data.skill.effects;

namespace TheRingGoesSouth.scripts.data.skill
{
    public class SkillEffectDataConverter : JsonConverter<SkillEffectData>
    {
        public override SkillEffectData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Create a copy of the reader to be able to read the "Type" property
            // without advancing the original reader.
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token for SkillEffectData.");
            }

            string effectType = null;

            // Find the "Type" property to determine which concrete class to deserialize into.
            while (readerClone.Read())
            {
                if (readerClone.TokenType == JsonTokenType.EndObject)
                {
                    // Reached the end of the current JSON object without finding "Type".
                    // This might happen if the "Type" property is missing or not the first one.
                    // We'll let the specific deserializer handle it or throw an error later.
                    break; 
                }

                if (readerClone.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = readerClone.GetString();
                    readerClone.Read(); // Move to the property value

                    if (string.Equals(propertyName, "Type", StringComparison.OrdinalIgnoreCase) || 
                        string.Equals(propertyName, "$type", StringComparison.OrdinalIgnoreCase)) // Common for type discriminators
                    {
                        effectType = readerClone.GetString();
                        break; // Found the type, no need to read further in the clone
                    }
                }
            }

            if (string.IsNullOrEmpty(effectType))
            {
                throw new JsonException("SkillEffectData 'Type' property not found or is null/empty.");
            }

            // Based on the effectType, deserialize into the concrete type.
            // The original 'reader' is passed, which is still at the StartObject token.
            return effectType switch
            {
                "Damage" => JsonSerializer.Deserialize<DamageEffectData>(ref reader, options),
                "Heal" => JsonSerializer.Deserialize<HealEffectData>(ref reader, options),
                "StatModifier" => JsonSerializer.Deserialize<StatModifierEffectData>(ref reader, options),
                "StatusApply" => JsonSerializer.Deserialize<StatusApplyEffectData>(ref reader, options),
                // Add other effect types here as needed
                _ => throw new JsonException($"Unknown SkillEffectData type: {effectType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, SkillEffectData value, JsonSerializerOptions options)
        {
            // This example focuses on deserialization. Serialization would require
            // casting to the concrete type and then serializing.
            // For simplicity, we can rely on the default serialization if we don't need custom write logic.
            // Or, more robustly:
            switch (value)
            {
                case DamageEffectData damageEffect:
                    JsonSerializer.Serialize(writer, damageEffect, options);
                    break;
                case HealEffectData healEffect:
                    JsonSerializer.Serialize(writer, healEffect, options);
                    break;
                case StatModifierEffectData statModifierEffect:
                    JsonSerializer.Serialize(writer, statModifierEffect, options);
                    break;
                case StatusApplyEffectData statusEffect:
                    JsonSerializer.Serialize(writer, statusEffect, options);
                    break;
                default:
                    throw new NotSupportedException($"Type {value.GetType()} is not supported by SkillEffectDataConverter for writing.");
            }
        }
    }
}
