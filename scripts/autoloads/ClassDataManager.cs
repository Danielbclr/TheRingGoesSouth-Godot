using Godot;
using System.Collections.Generic;

public partial class ClassDataManager : Node // Autoloads are Nodes
{
    private static ClassDataManager _instance;
    public static ClassDataManager Instance => _instance;

    private Dictionary<string, ClassData> _classDefinitions = new Dictionary<string, ClassData>();

    public override void _EnterTree()
    {
        if (_instance != null && _instance != this)
        {
            QueueFree(); // Another instance exists
            return;
        }
        _instance = this;
        LoadClassDefinitions();
    }
    
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }


    private void LoadClassDefinitions()
    {
        // TODO: Load these from JSON files or .tres Resource files
        // For now, let's define a couple manually for testing

        var warrior = new ClassData("warrior", "Warrior", "A brave fighter skilled in melee combat.");
        warrior.BaseStats[StatType.Strength] = 14;
        warrior.BaseStats[StatType.Dexterity] = 10;
        warrior.BaseStats[StatType.Intelligence] = 8;
        warrior.BaseStats[StatType.Constitution] = 12;
        warrior.ClassSkills.Add("skill_slash"); // Assuming you have skill IDs
        warrior.ClassSkills.Add("skill_defend");
        _classDefinitions.Add(warrior.ClassId.ToUpper(), warrior);

        var rogue = new ClassData("rogue", "Rogue", "A cunning operative excelling in stealth and precision.");
        rogue.BaseStats[StatType.Strength] = 10;
        rogue.BaseStats[StatType.Dexterity] = 14;
        rogue.BaseStats[StatType.Intelligence] = 10;
        rogue.BaseStats[StatType.Constitution] = 10;
        rogue.ClassSkills.Add("skill_backstab");
        rogue.ClassSkills.Add("skill_stealth");
        _classDefinitions.Add(rogue.ClassId.ToUpper(), rogue);

        GD.Print($"ClassDataManager: Loaded {_classDefinitions.Count} class definitions.");
    }

    public ClassData GetClassDefinition(string classId)
    {
        if (string.IsNullOrEmpty(classId)) return null;
        return _classDefinitions.TryGetValue(classId.ToUpper(), out ClassData def) ? def : null;
    }
}