using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using TheRingGoesSouth.scripts.data;

namespace TheRingGoesSouth.scripts.actors
{
    public partial class EnemyUnit : BattleUnit
    {
        public EnemyData Stats { get; private set; }

        // Assuming BattleUnit has these properties or provides a way to set them.
        // If not, you'll need to add them to BattleUnit or directly here.
        // public string CharacterName { get; set; } (from BattleUnit)
        // public int MaxHealth { get; set; } (from BattleUnit)
        // public int CurrentHealth { get; set; } (from BattleUnit)
        // public int Attack { get; set; } (from BattleUnit)
        // public int Defense { get; set; } (from BattleUnit)
        // public int Speed { get; set; } (from BattleUnit)
        // public int MoveRange { get; set; } (from BattleUnit)

        public void Setup(EnemyData enemyData, TileMapLayer battleGrid, Vector2I gridPosition)
        {
            Stats = enemyData;
            // BattleGrid = battleGrid; // Assuming BattleUnit has a BattleGrid property

            // CharacterName = Stats.Name; // Assuming BattleUnit.CharacterName
            // MaxHealth = Stats.MaxHealth;
            // CurrentHealth = Stats.MaxHealth; // Start with full health
            // Attack = Stats.Attack;
            // Defense = Stats.Defense;
            // Speed = Stats.Speed;
            // MoveRange = Stats.MoveRange;
            _combatMap = battleGrid;
            SetGridPosition(gridPosition, true); 
        }
    }
}