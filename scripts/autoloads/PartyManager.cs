
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PartyManager : Node
{
    private static PartyManager _instance;
    public static PartyManager Instance => _instance;

    public List<CharacterData> PartyMembers { get; private set; } = [];

    private int maxPartySize { get; set; } = 6;
    private int battlePartySize = 4;

    public override void _EnterTree()
    {
        if (_instance != null && _instance != this)
        {
            QueueFree(); // Another instance exists
            return;
        }
        _instance = this;
        LoadPartyMembers();
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void LoadPartyMembers()
    {
        LoadInitialParty();
    }

    private void LoadInitialParty()
    {
        GD.Print("GameSetup: Creating initial party...");
        CreateAndAddCharacter(0, "Aragorn", "dun_ranger");
        CreateAndAddCharacter(1, "Legolas", "wood_fighter");
        // PartyManager.Instance.CreateAndAddCharacter("Gimli", "warrior");
        // PartyManager.Instance.CreateAndAddCharacter("Gandalf", "mage"); // Add a "mage" class

        // You can now access the party:
        foreach (var member in PartyMembers)
        {
            GD.Print($"Party Member: {member.CharacterName}, Class: {member.CharacterClasses[0].Name}, HP: {member.CurrentHealth}/{member.getMaxHealth()}");
            GD.Print($"  Stats: Str={member.GetStat(StatType.PHY)}, Dex={member.GetStat(StatType.AGI)}");
            GD.Print($"  Skills: {string.Join(", ", member.CharacterSkills)}");
        }
    }

    public void AddPartyMember(CharacterData character)
    {
        if (PartyMembers.Count < maxPartySize && !PartyMembers.Contains(character))
        {
            PartyMembers.Add(character);
            GD.Print($"Added {character.CharacterName} to the party.");
        }
        else
        {
            GD.Print("Party is full. Cannot add more members.");
        }
    }

    public void RemovePartyMember(CharacterData character)
    {
        PartyMembers.Remove(character);
    }
    
    public CharacterData GetPartyMemberByName(string name)
    {
        return PartyMembers.FirstOrDefault(member => member.CharacterName == name);
    }

    // Example of how you might create an initial party
    public void CreateAndAddCharacter(int id, string name, string classId)
    {
        if (CharacterDataManager.Instance == null)
        {
            GD.PrintErr("PartyManager: CharacterDataManager is not ready!");
            return;
        }

        CharacterData newChar = CharacterDataManager.Instance.LoadCharacterFromClass(name, classId);
        AddPartyMember(newChar);
    }
}