// C:/Users/nihan/Documents/Projetos/the-ring-goes-south/scripts/actors/PlayerUnit.cs
using Godot;
using System;

public partial class PlayerUnit : Node2D
{
	public CharacterData CharacterData { get; private set; }
	public Vector2I GridPosition { get; private set; } // Current logical grid position

	private Sprite2D _sprite;
	// Store half tile size for consistent positioning
	private Vector2 _halfTileSize;
	private TileMapLayer _combatMap; // Store reference to the map for updates

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");

		if (_sprite == null)
		{
			GD.PrintErr("PlayerUnit: Sprite2D child node 'Sprite' not found!");
		}
	}

	public void Setup(CharacterData data, TileMapLayer combatMap, Vector2I initialGridPosition)
	{
		CharacterData = data;
		_combatMap = combatMap;

		SetGridPosition(initialGridPosition, true); // Set initial position without animation
		GD.Print($"PlayerUnit: Setting up unit for {CharacterData.CharacterName} ({CharacterData.CharacterClasses[0].ClassName}) at {GridPosition}");
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
			// If not snapping, movement will be handled by BattleScene's tween
		}
	}
	
	public Vector2 GetGlobalPosition()
	{
		return GlobalPosition;
	}

	// This method was for a different context, BattleScene will handle movement.
	// public void UpdateWorldPosition(TileMap combatMap, Vector2 halfTileSize)
	// {
	//     if (combatMap == null) return;
	//     GlobalPosition = combatMap.MapToLocal(GridPosition) + halfTileSize;
	// }
}
