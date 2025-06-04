using Godot;
using System;
using System.Collections.Generic;
using TheRingGoesSouth.scripts.data;
using TheRingGoesSouth.scripts.data.skill;

public partial class CharacterData : Resource
{
	public int CharacterId { get; set; }
	public string CharacterName { get; set; }

	public List<ClassData> CharacterClasses { get; set; } = new List<ClassData>();
	public int CharacterLevel { get; set; } = 1;
	public Dictionary<StatType, int> CharacterStats { get; set; } = [];
	public int CurrentHealth { get; set; }
	public List<SkillData> CharacterSkills { get; set; } = [];
	public List<string> CharacterPerks { get; set; } = [];
	public List<SkillData> CurrentSkills { get; set; } = [];


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

		CurrentHealth = CharacterStats[StatType.PHY] + 8;

		GD.Print($"Initializing character {CharacterName} skills: {classData.Skills}");
		foreach (var skill in classData.Skills)
		{
			LearnSkill(skill);
		}
		CurrentSkills = CharacterSkills;
		foreach (var perk in classData.ClassPerks)
		{
			LearnPerk(perk);
		}
		GD.Print($"{CharacterName} ({classData.Name}) initialized. ");
	}

	private void LearnPerk(string perk)
	{
		if (!CharacterPerks.Contains(perk))
		{
			CharacterPerks.Add(perk);
		}
	}

	public void LearnSkill(String skillId)
	{
		GD.Print($"Learning skill: {skillId} for character: {CharacterName}");
		SkillData skillData = SkillDataLoader.GetSkillData(skillId);
		GD.Print($"SkillData: {skillData}");
		if (skillData != null && !CharacterSkills.Contains(skillData))
		{
			CharacterSkills.Add(skillData);
			GD.Print($"Skill {skillData.Name} learned by {CharacterName}.");
			GD.Print($"Skill {skillData.Name} {skillData.Description}");
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
		if (CurrentHealth > CharacterStats[StatType.PHY] + 8)
		{
			CurrentHealth = CharacterStats[StatType.PHY] + 8;
		}
	}

	internal object getMaxHealth()
	{
		return CharacterStats[StatType.PHY] + 8;
	}
}
