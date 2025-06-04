using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using TheRingGoesSouth.scripts.data.skill.effects;

namespace TheRingGoesSouth.scripts.data.skill
{
    public static class SkillDataLoader
    {
        private static Dictionary<string, SkillData> _skillDatabase;
        private const string SkillDataPath = "res://data/skills.json"; // Path to your skills.json

        public static void LoadSkillData()
        {
            if (_skillDatabase != null) return; // Already loaded

            _skillDatabase = new Dictionary<string, SkillData>();

            if (!FileAccess.FileExists(SkillDataPath))
            {
                GD.PrintErr($"SkillDataLoader: Skill data file not found at {SkillDataPath}");
                return;
            }

            using var file = FileAccess.Open(SkillDataPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"SkillDataLoader: Could not open file {SkillDataPath}. Error: {FileAccess.GetOpenError()}");
                return;
            }
            
            string content = file.GetAsText();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                GD.PrintErr($"SkillDataLoader: Skill data file at {SkillDataPath} is empty.");
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                // Add our custom converter for SkillEffectData
                Converters = { new SkillEffectDataConverter() } 
            };

            try
            {
                var skillList = JsonSerializer.Deserialize<List<SkillData>>(content, options);

                if (skillList != null)
                {
                    foreach (var skillData in skillList)
                    {
                        if (skillData.Id == null)
                        {
                            GD.PrintErr($"SkillDataLoader: Found a skill with a null SkillId in {SkillDataPath}. Skipping this entry.");
                            continue;
                        }
                        if (!_skillDatabase.ContainsKey(skillData.Id))
                        {
                            _skillDatabase.Add(skillData.Id, skillData);
                        }
                        else
                        {
                            GD.PrintErr($"SkillDataLoader: Duplicate skill ID '{skillData.Id}' found in {SkillDataPath}.");
                        }
                    }
                    GD.Print($"SkillDataLoader: Successfully loaded {_skillDatabase.Count} skills.");
                }
                else
                {
                    GD.PrintErr($"SkillDataLoader: Failed to deserialize skill list from {SkillDataPath}. Result was null.");
                }
            }
            catch (JsonException ex)
            {
                GD.PrintErr($"SkillDataLoader: Error parsing JSON from {SkillDataPath}. Details: {ex.Message} {ex.InnerException?.Message}");
            }
            catch (Exception ex) // Catch other potential exceptions during loading
            {
                GD.PrintErr($"SkillDataLoader: An unexpected error occurred while loading skills from {SkillDataPath}. Details: {ex.Message}");
            }
        }

        public static SkillData GetSkillData(string skillId)
        {
            // Ensure data is loaded. Consider making LoadSkillData() public and calling it
            // from an autoload script's _Ready() function for explicit load timing.
            if (_skillDatabase == null)
            {
                LoadSkillData();
            }
            
            if (_skillDatabase == null) // Still null after attempting load (e.g., file not found)
            {
                GD.PrintErr($"SkillDataLoader: Skill database is not initialized. Cannot get skill '{skillId}'.");
                return null;
            }
            GD.Print($"SkillDataLoader: Attempting to get skill data for ID '{skillId}'.");
            return _skillDatabase.TryGetValue(skillId, out SkillData data) ? data : null;
        }
    }
}
