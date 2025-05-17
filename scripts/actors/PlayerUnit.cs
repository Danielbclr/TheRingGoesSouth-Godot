using Godot;
using System;

public partial class PlayerUnit : Node2D
{
	public CharacterData CharacterData { get; private set; }

	public Vector2I GridPosition { get; private set; }

	private Sprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");

		if (_sprite == null)
		{
			GD.PrintErr("PlayerUnit: Sprite2D child node 'Sprite' not found!");
		}

	}

	public void Setup(CharacterData data)
	{
		CharacterData = data;
		GD.Print($"PlayerUnit: Setting up unit for {CharacterData.CharacterName} ({CharacterData.CharacterClasses[0].ClassName})");

	}

	public void Initialize(Vector2I initialGridPosition)
	{
		GridPosition = initialGridPosition;
	}

	public void UpdateWorldPosition(TileMap combatMap, Vector2 halfTileSize)
	{
		if (combatMap == null) return;
		GlobalPosition = combatMap.MapToLocal(GridPosition) + halfTileSize;
	}

}
