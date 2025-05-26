using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class ClassData : Resource
{
    public string ClassId { get; set; }
    public string ClassName { get; set; }
    public string ClassDescription { get; set; }
    public Dictionary<StatType, int> BaseStats { get; set; } = new Dictionary<StatType, int>();
    public List<string> ClassSkills { get; set; } = new List<string>();
    public List<string> ClassPerks { get; set; } = new List<string>();

    public ClassData()
    {
    }

    public ClassData(string classId, string className, string classDescription, Dictionary<StatType, int> classStats)
    {
        ClassId = classId;
        ClassName = className;
        ClassDescription = classDescription;
        BaseStats = classStats;
    }

    public ClassData(string classId, string className, string classDescription)
    {
        ClassId = classId;
        ClassName = className;
        ClassDescription = classDescription;
        BaseStats[StatType.Strength] = 4;
        BaseStats[StatType.Dexterity] = 4;
        BaseStats[StatType.Constitution] = 4;
        BaseStats[StatType.Intelligence] = 4;
        BaseStats[StatType.Wisdom] = 4;
        BaseStats[StatType.Charisma] = 2;
        GD.Print($"ClassData: Created class {className} with ID {classId}");
    }

    private void loadClassData()
    {
        // Load class data from a file or database
        // This is just a placeholder for the actual implementation
        ClassId = "warrior";
        ClassName = "Warrior";
        ClassDescription = "A strong and brave fighter.";
        BaseStats[StatType.Strength] = 10;
        BaseStats[StatType.Dexterity] = 5;
        BaseStats[StatType.Constitution] = 8;
        BaseStats[StatType.Intelligence] = 3;
        BaseStats[StatType.Wisdom] = 4;
        BaseStats[StatType.Charisma] = 2;
    }
}