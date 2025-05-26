using Godot;
using System.Collections.Generic;
using TheRingGoesSouth.scripts.data;
using TheRingGoesSouth.scripts.utils;

public partial class CharacterDataManager : Node, ILoggable
{
    public bool DEBUG_TAG { get; set; } = true;

    private static CharacterDataManager _instance;
    public static CharacterDataManager Instance => _instance;

    public override void _EnterTree()
    {
        if (_instance != null && _instance != this)
        {
            QueueFree(); // Another instance exists
            return;
        }
        _instance = this;
    }
    
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public CharacterData LoadCharacterFromClass(string characterName, string classId)
    {
        ClassData classData = GetClass(classId);
        if (classData == null)
        {
            GD.PrintErr($"CharacterDataManager: Class '{classId}' not found for character '{characterName}'.");
            return null;
        }

        CharacterData characterData = new CharacterData(0, characterName,classData);

        return characterData;
    }

    public ClassData GetClass(string classId)
    {
        return ClassDataLoader.GetClassData(classId);
    }
}