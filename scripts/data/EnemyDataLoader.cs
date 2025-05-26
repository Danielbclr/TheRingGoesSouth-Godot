using Godot;
using System.Collections.Generic;
// using System.IO; // For File.ReadAllText - Note: Godot's FileAccess is preferred for res://
using System.Text.Json;
using TheRingGoesSouth.scripts.data; // Assuming EnemyData.cs is in this namespace

namespace TheRingGoesSouth.scripts.utils
{
    public static class EnemyDataLoader
    {
        private static Dictionary<string, EnemyData> _enemyDatabase;
        private const string EnemyDataPath = "res://data/enemies.json"; // Path to your JSON file

        public static void LoadEnemyData()
        {
            if (_enemyDatabase != null) return; // Already loaded

            _enemyDatabase = new Dictionary<string, EnemyData>();

            if (!FileAccess.FileExists(EnemyDataPath))
            {
                GD.PrintErr($"EnemyLoader: Enemy data file not found at {EnemyDataPath}");
                return;
            }

            using var file = FileAccess.Open(EnemyDataPath, FileAccess.ModeFlags.Read);
            string content = file.GetAsText();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                GD.PrintErr($"EnemyLoader: Enemy data file at {EnemyDataPath} is empty.");
                return;
            }

            try
            {
                var enemyList = JsonSerializer.Deserialize<List<EnemyData>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Handles "Id" vs "id", "MaxHealth" vs "maxHealth" etc.
                });

                if (enemyList != null)
                {
                    foreach (var enemyData in enemyList)
                    {
                        if (!_enemyDatabase.ContainsKey(enemyData.Id))
                        {
                            _enemyDatabase.Add(enemyData.Id, enemyData);
                        }
                        else
                        {
                            GD.PrintErr($"EnemyLoader: Duplicate enemy ID '{enemyData.Id}' found in {EnemyDataPath}.");
                        }
                    }
                    GD.Print($"EnemyLoader: Successfully loaded {_enemyDatabase.Count} enemies.");
                }
            }
            catch (JsonException ex)
            {
                GD.PrintErr($"EnemyLoader: Error parsing JSON from {EnemyDataPath}. Details: {ex.Message}");
            }
        }

        public static EnemyData GetEnemyData(string enemyId)
        {
            LoadEnemyData(); // Ensure data is loaded
            return _enemyDatabase.TryGetValue(enemyId, out EnemyData data) ? data : null;
        }
    }
}