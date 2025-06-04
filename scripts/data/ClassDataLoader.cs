using Godot;
using System.Collections.Generic;
// using System.IO; // For File.ReadAllText - Note: Godot's FileAccess is preferred for res://
using System.Text.Json;
using TheRingGoesSouth.scripts.data; // Assuming ClassData.cs is in this namespace

namespace TheRingGoesSouth.scripts.data
{
	public static class ClassDataLoader
	{
		private static Dictionary<string, ClassData> _classDatabase;
		private const string ClassDataPath = "res://data/classes.json"; // Path to your JSON file

		public static void LoadClassData()
		{
			if (_classDatabase != null) return; // Already loaded

			_classDatabase = new Dictionary<string, ClassData>();

			if (!FileAccess.FileExists(ClassDataPath))
			{
				GD.PrintErr($"ClassLoader: Class data file not found at {ClassDataPath}");
				return;
			}

			using var file = FileAccess.Open(ClassDataPath, FileAccess.ModeFlags.Read);
			string content = file.GetAsText();
			
			if (string.IsNullOrWhiteSpace(content))
			{
				GD.PrintErr($"ClassLoader: Class data file at {ClassDataPath} is empty.");
				return;
			}

			try
			{
				var classList = JsonSerializer.Deserialize<List<ClassData>>(content, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true // Handles "Id" vs "id", "MaxHealth" vs "maxHealth" etc.
				});

				if (classList != null)
				{
					foreach (var classData in classList)
					{
						if (!_classDatabase.ContainsKey(classData.Id))
						{
							_classDatabase.Add(classData.Id, classData);
						}
						else
						{
							GD.PrintErr($"ClassLoader: Duplicate class ID '{classData.Id}' found in {ClassDataPath}.");
						}
					}
					GD.Print($"ClassLoader: Successfully loaded {_classDatabase.Count} enemies.");
				}
			}
			catch (JsonException ex)
			{
				GD.PrintErr($"ClassLoader: Error parsing JSON from {ClassDataPath}. Details: {ex.Message}");
			}
		}

		public static ClassData GetClassData(string classId)
		{
			LoadClassData(); // Ensure data is loaded
			return _classDatabase.TryGetValue(classId, out ClassData data) ? data : null;
		}
	}
}
