// scripts/screens/BattleScene.cs
using Godot;
using System;
using System.Collections.Generic;

public partial class BattleScene : Node2D // Inherits from Node2D as per scene root
{
	[Export]
	public PackedScene PlayerUnitScene { get; set; } // Export to link PlayerUnit.tscn in editor

	private TileMapLayer _battleGrid;

	public override void _Ready()
	{
		// Get references to nodes
		_battleGrid = GetNode<TileMapLayer>("battle_map");

		if (_battleGrid == null)
		{
			GD.PrintErr("BattleScene: TileMap node 'BattleGrid' not found!");
			return;
		}

		if (PlayerUnitScene == null)
		{
			GD.PrintErr("BattleScene: PlayerUnitScene PackedScene is not set in the editor!");
			return;
		}

		GD.Print("BattleScene: Initializing battle scene.");

		// Load battle map data (if not already set up in the .tscn)
		// If your battle map is a separate TMX, you might load it here
		// For now, we assume the TileMap node in the scene is already painted.

		// Place party members on the grid
		PlacePartyMembers();

		// TODO: Initialize turn manager, enemy AI, battle UI, etc.
	}

	private void PlacePartyMembers()
	{
		if (PartyManager.Instance == null)
		{
			GD.PrintErr("BattleScene: PartyManager instance is not available!");
			return;
		}

		List<CharacterData> party = PartyManager.Instance.PartyMembers;
		
		foreach (var member in party)
		{
			GD.Print($"PartyManager: Party member {member.CharacterName} is ready for battle.");
		}

		// Define starting positions on the battle grid (map coordinates)
		// These are just examples; you'll need to decide on actual starting positions
		Vector2I[] startingPositions = new Vector2I[]
		{
			new Vector2I(2, 5), // Position for first party member
			new Vector2I(3, 6), // Position for second party member
			new Vector2I(2, 7), // Position for third party member (if any)
			new Vector2I(3, 8)  // Position for fourth party member (if any)
		};

		for (int i = 0; i < party.Count && i < startingPositions.Length; i++)
		{
			CharacterData character = party[i];
			Vector2I gridPosition = startingPositions[i];
			GD.Print($"PartyManager: Placing Party member {character.CharacterName} at {gridPosition[0]}, {gridPosition[1]}.");

			// Instantiate the PlayerUnit scene
			PlayerUnit playerUnit = PlayerUnitScene.Instantiate<PlayerUnit>();

			// Setup the unit with character data
			playerUnit.Setup(character);

			// Add the unit to the scene tree
			AddChild(playerUnit);

			// Position the unit on the grid
			// Convert grid coordinates to world coordinates (center of the tile)
			// We need the TileMap's tile size for this
			Vector2I tileSize = _battleGrid.TileSet.TileSize;
			Vector2 halfTileSize = new Vector2(tileSize.X / 2.0f, tileSize.Y / 2.0f);
			Vector2 worldPosition = _battleGrid.MapToLocal(gridPosition) + halfTileSize;

			playerUnit.GlobalPosition = worldPosition; // Set the global position
			// Or if PlayerUnit root is Node2D and BattleScene root is Node2D at (0,0):
			// playerUnit.Position = worldPosition; // Set local position relative to BattleScene
		}

		GD.Print($"BattleScene: Placed {party.Count} party members on the grid.");
	}

	// TODO: Add methods for turn management, handling input, processing actions, etc.
}
