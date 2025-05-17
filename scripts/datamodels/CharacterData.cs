using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterData : Resource
{
    public int CharacterId { get; set; }
    public string CharacterName { get; set; }

    public List<ClassData> CharacterClasses { get; set; } = new List<ClassData>();
    public int CharacterLevel { get; set; } = 1;
    public Dictionary<StatType, int> CharacterStats { get; set; } = [];
    public int CurrentHealth { get; set; }
    public List<string> CharacterSkills { get; set; } = new List<string>();
    public List<string> CharacterPerks { get; set; } = new List<string>();
    public List<string> CurrentSkills { get; set; } = new List<string>();


    public CharacterData()
    {
    }

    public CharacterData(int characterId, string characterName, List<ClassData> characterClasses)
    {
        CharacterId = characterId;
        CharacterName = characterName;
        CharacterClasses = characterClasses;
    }

    public CharacterData(int characterId, string characterName, ClassData classData)
    {
        CharacterId = characterId;
        CharacterName = characterName;
        InitializeFromClassData(classData);
    }

    private void InitializeFromClassData(ClassData classData)
    {
        if (CharacterClasses.Count != 0)
        {
            return;
        }
        CharacterClasses.Add(classData);
        foreach (var statPair in classData.BaseStats)
        {
            CharacterStats[statPair.Key] = statPair.Value;
        }

        CurrentHealth = CharacterStats[StatType.Constitution] + 8;

        foreach (var skill in classData.ClassSkills)
        {
            LearnSkill(skill);
        }
        CurrentSkills = CharacterSkills;
        foreach (var perk in classData.ClassPerks)
        {
            LearnPerk(perk);
        }
        GD.Print($"{CharacterName} ({classData.ClassName}) initialized. ");
    }

    private void LearnPerk(string perk)
    {
        if (!CharacterPerks.Contains(perk))
        {
            CharacterPerks.Add(perk);
        }
    }

    public void LearnSkill(string skill)
    {
        if (!CharacterSkills.Contains(skill))
        {
            CharacterSkills.Add(skill);
        }
    }

    public int GetStat(StatType stat)
    {
        return CharacterStats.TryGetValue(stat, out int value) ? value : 0;
    }

    public void SetStat(StatType stat, int value)
    {
        CharacterStats[stat] = value;
        // Potentially re-calculate dependent stats like MaxHealth if base stats change
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
    
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > CharacterStats[StatType.Constitution] + 8)
        {
            CurrentHealth = CharacterStats[StatType.Constitution] + 8;
        }
    }

    internal object getMaxHealth()
    {
        return CharacterStats[StatType.Constitution] + 8;
    }
}