using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using TheRingGoesSouth.scripts.utils;

namespace TheRingGoesSouth.scripts.actors
{
    public partial class BattleUnit : Node2D, ILoggable
    {

        public bool DEBUG_TAG { get; set; } = false;
        public CharacterData CharacterData { get; private set; }
	    public Vector2I GridPosition { get; private set; } // Current logical grid position
        private Sprite2D _sprite;
        protected TileMapLayer _combatMap; // Store reference to the map for updates

        public override void _Ready()
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");

            if (_sprite == null)
            {
                GD.PrintErr("BattleUnit: Sprite2D child node 'Sprite' not found!");
            }
        }

        public void Setup(CharacterData data, TileMapLayer combatMap, Vector2I initialGridPosition)
        {
            CharacterData = data;
            _combatMap = combatMap;

            SetGridPosition(initialGridPosition, true); // Set initial position without animation
            GD.Print($"BattleUnit: Setting up unit for {CharacterData.CharacterName} ({CharacterData.CharacterClasses[0].ClassName}) at {GridPosition}");
        }

        /// <summary>
        /// Sets the unit's logical grid position and updates its world position.
        /// </summary>
        public void SetGridPosition(Vector2I newGridPosition, bool snap = false)
        {
            GridPosition = newGridPosition;
            if (_combatMap != null)
            {
                Vector2 targetWorldPosition = _combatMap.MapToLocal(GridPosition);
                if (snap)
                {
                    GlobalPosition = targetWorldPosition;
                }
            }
        }
        
        public Vector2 GetGlobalPosition()
        {
            return GlobalPosition;
        }
    }
}