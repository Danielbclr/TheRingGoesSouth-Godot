// C:/Users/nihan/Documents/Projetos/the-ring-goes-south/scripts/scenes/BattleScene.cs
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq; // For HashSet
using System.Threading.Tasks;
using TheRingGoesSouth.scripts.utils; // For HighlightTileHelper and Logger (if you have one)

public partial class BattleScene : Node2D
{
	[Export] public PackedScene PlayerUnitScene { get; set; }
	[Export] public TileMapLayer _battleGrid;
	[Export] public float MoveDelayPerTile { get; set; } = 0.25f; // Movement speed
	[Export] public int PlayerMoveRange { get; set; } = 3;

	private Array<PlayerUnit> _playerUnits = new();
	private int _currentPlayerIndex = -1;
	private PlayerUnit _activePlayerUnit;
	private HighlightTileHelper _highlightTileHelper;

	private enum BattleTurnState
	{
		None,
		PlayerTurnStarting,
		PlayerTurnReady,
		PlayerMoving,
		TurnEnding
	}
	private BattleTurnState _currentBattleState = BattleTurnState.None;

	private Array<Vector2I> _validMoveLocations = new();
	private Array<Vector2I> _currentMovePath = new(); // Stores deltas

	// Hex directions (ensure these match your battle grid's logic)
	// These are relative steps in map coordinates.
	// Example for "pointy top" hexes, offset coordinates:
	// Even-q (q is column, r is row)
	// private readonly Vector2I[] _hexDirections = {
	//     new Vector2I(1, 0), new Vector2I(0, -1), new Vector2I(-1, -1), // E, NE, NW
	//     new Vector2I(-1, 0), new Vector2I(-1, 1), new Vector2I(0, 1)   // W, SW, SE
	// };
	// Or for "flat top" hexes, offset coordinates:
	// Even-r (q is column, r is row)
	private readonly Vector2I[] _hexDirections = {
		new Vector2I(1, -1), new Vector2I(0, -1), new Vector2I(-1, 0), // NE, N, NW
		new Vector2I(-1, 1), new Vector2I(0, 1), new Vector2I(1, 0)    // SW, S, SE
	};
	// If your PartyHexMovement used different directions, ensure these are consistent
	// with your TileSet's "Tile Shape" and "Tile Layout" for neighbor calculations.
	// The ones from PartyHexMovement:
	// private readonly Vector2I HEX_NE = new(0, -1); // N-ish for pointy top, NE for flat top
	// private readonly Vector2I HEX_NW = new(-1, 0); // NW for pointy top, W for flat top
	// private readonly Vector2I HEX_SE = new(1, 0);  // SE for pointy top, E for flat top
	// private readonly Vector2I HEX_SW = new(0, 1);  // S-ish for pointy top, SW for flat top
	// private readonly Vector2I HEX_W = new(-1, 1); // SW for pointy top, NW for flat top
	// private readonly Vector2I HEX_E = new(1, -1);  // NE for pointy top, N for flat top
	// Let's use the PartyHexMovement ones for consistency with its GetRoute logic
	private readonly Vector2I[] _battleHexDirections = {
		new(0, -1), new(1, -1), new(1, 0), 
		new(0, 1), new(-1, 1), new(-1, 0)
	};


	public override void _Ready()
	{
		if (_battleGrid == null)
		{
			GD.PrintErr("BattleScene: TileMap node '_battleGrid' not found or not assigned!");
			GetTree().Quit(); return;
		}
		if (_battleGrid.TileSet == null)
		{
			GD.PrintErr("BattleScene: _battleGrid does not have a TileSet assigned.");
			GetTree().Quit(); return;
		}
		if (PlayerUnitScene == null)
		{
			GD.PrintErr("BattleScene: PlayerUnitScene PackedScene is not set in the editor!");
			GetTree().Quit(); return;
		}

		_highlightTileHelper = new HighlightTileHelper(this, _battleGrid); // Highlights are children of BattleScene

		GD.Print("BattleScene: Initializing battle scene.");
		PlacePartyMembers();

		if (_playerUnits.Count > 0)
		{
			StartBattle();
		}
		else
		{
			GD.Print("BattleScene: No player units to start battle with.");
		}
	}

	private void PlacePartyMembers()
	{
		if (PartyManager.Instance == null)
		{
			GD.PrintErr("BattleScene: PartyManager instance is not available!");
			return;
		}
		List<CharacterData> party = PartyManager.Instance.PartyMembers;
		Vector2I[] startingPositions = {
			new(2, 5), new(3, 6), new(2, 7), new(3, 8)
		};

		for (int i = 0; i < party.Count && i < startingPositions.Length; i++)
		{
			CharacterData character = party[i];
			Vector2I gridPosition = startingPositions[i];
			
			PlayerUnit playerUnit = PlayerUnitScene.Instantiate<PlayerUnit>();
			// Pass combat map and initial grid position for setup
			playerUnit.Setup(character, _battleGrid, gridPosition); 
			
			AddChild(playerUnit);
			_playerUnits.Add(playerUnit);
			
			// PlayerUnit.Setup now handles its own positioning via SetGridPosition
			GD.Print($"BattleScene: Placed {character.CharacterName} at map {gridPosition}, world {playerUnit.GlobalPosition}.");
		}
		GD.Print($"BattleScene: Placed {_playerUnits.Count} party members on the grid.");
	}

	private void StartBattle()
	{
		_currentPlayerIndex = -1; // Will be incremented to 0 by NextTurn
		_currentBattleState = BattleTurnState.TurnEnding; // To trigger NextTurn
		NextTurn();
	}

	private void NextTurn()
	{
		if (_activePlayerUnit != null)
		{
			// GD.Print($"{_activePlayerUnit.CharacterData.CharacterName}'s turn ended.");
		}

		_highlightTileHelper.ClearHighlights();
		_validMoveLocations.Clear();
		_currentMovePath.Clear();

		_currentPlayerIndex = (_currentPlayerIndex + 1) % _playerUnits.Count;
		_activePlayerUnit = _playerUnits[_currentPlayerIndex];
		_currentBattleState = BattleTurnState.PlayerTurnStarting;

		GD.Print($"--- {_activePlayerUnit.CharacterData.CharacterName}'s Turn ---");
		// TODO: Update UI to show active player, enable action buttons etc.
		// For now, directly go to awaiting action
		_currentBattleState = BattleTurnState.PlayerTurnReady;
		InitiateMoveSelection();
		GD.Print("Choose action: (S)kip Turn");
	}

	public override void _Input(InputEvent @event)
	{
		if (_activePlayerUnit == null) return;

		if (_currentBattleState == BattleTurnState.PlayerTurnReady)
		{
			if (Input.IsActionJustPressed("battle_skip_turn"))
			{
				GD.Print($"{_activePlayerUnit.CharacterData.CharacterName} skips turn.");
				_currentBattleState = BattleTurnState.TurnEnding;
				NextTurn();
			}
			if (@event.IsActionPressed("left_click"))
			{
				Vector2I clickedTile = _battleGrid.LocalToMap(GetGlobalMousePosition());
				// If BattleScene itself is offset, or _battleGrid is deeply nested, you might need:
				// Vector2 localClickPos = _battleGrid.ToLocal(GetGlobalMousePosition());
				// Vector2I clickedTile = _battleGrid.LocalToMap(localClickPos);

				if (_validMoveLocations.Contains(clickedTile))
				{
					GD.Print($"Move confirmed to: {clickedTile}");
					// _currentMovePath = GetRouteForUnit(_activePlayerUnit.GridPosition, clickedTile);

					_currentMovePath = TileGetter.GetRoute(_activePlayerUnit.GlobalPosition, GetGlobalMousePosition(), _battleGrid);
					if (_currentMovePath.Count > 0 || clickedTile == _activePlayerUnit.GridPosition) // Allow clicking self to "pass" move
					{
						_highlightTileHelper.ClearHighlights(); // Clear range highlights
																// Optionally, highlight the chosen path
																// HighlightPath(_activePlayerUnit.GridPosition, _currentMovePath); 
						_currentBattleState = BattleTurnState.PlayerMoving;
						_ = ExecuteMoveAsync(_activePlayerUnit, _currentMovePath, clickedTile);
					}
					else
					{
						GD.Print("Path to target not found or invalid.");
						// Potentially re-show move highlights or go back to action selection
					}
				}
				else
				{
					GD.Print($"Invalid move target: {clickedTile}. Valid: {_validMoveLocations.Count}");
				}
			}
		}
	}
	
	private void InitiateMoveSelection()
	{
		GD.Print("Selecting move target...");
		_validMoveLocations = GetTilesInRangeForUnit(_activePlayerUnit.GridPosition, PlayerMoveRange);
		
		// For highlighting, we want to show all reachable tiles.
		// The GetTilesInRangeForUnit already filters by walkability.
		_highlightTileHelper.HighlightCollection(_validMoveLocations); 
		GD.Print($"Found {_validMoveLocations.Count} valid move locations.");
	}

	private void CancelMoveSelection()
	{
		GD.Print("Move selection cancelled.");
		_highlightTileHelper.ClearHighlights();
		_validMoveLocations.Clear();
		_currentBattleState = BattleTurnState.PlayerTurnReady;
	}

	private async Task ExecuteMoveAsync(PlayerUnit unit, Array<Vector2I> pathDeltas, Vector2I finalTargetMapPos)
	{
		Vector2I currentLogicalMapPos = unit.GridPosition;

		foreach (Vector2I moveDelta in pathDeltas)
		{
			currentLogicalMapPos += moveDelta;
			Vector2 targetWorldPos = _battleGrid.MapToLocal(currentLogicalMapPos);

			Tween tween = CreateTween();
			tween.TweenProperty(unit, "global_position", targetWorldPos, MoveDelayPerTile);
			tween.SetEase(Tween.EaseType.Out);
			tween.SetTrans(Tween.TransitionType.Sine); // Or Expo as in PartyHexMovement
			await ToSignal(tween, Tween.SignalName.Finished);
			unit.SetGridPosition(currentLogicalMapPos, true); // Update logical pos after each step visually completes
		}
		
		// Ensure final position is accurate
		unit.SetGridPosition(finalTargetMapPos, true); 

		_currentBattleState = BattleTurnState.TurnEnding;
		NextTurn();
	}

	private bool IsTileWalkable(Vector2I tileMapPosition)
	{
		// Basic check: is there a tile source defined for this cell?
		if (_battleGrid.GetCellSourceId(tileMapPosition) == -1)
		{
			return false; // No tile here, not walkable
		}
		// TODO: Add more sophisticated checks if needed:
		// 1. TileData custom properties (e.g., "is_walkable")
		//    TileData cellData = _battleGrid.GetCellTileData(tileMapPosition);
		//    if (cellData != null && cellData.HasCustomData("walkable") && !cellData.GetCustomData("walkable").AsBool()) return false;
		// 2. Check if tile is occupied by another unit (if units block movement)
		foreach(var unit in _playerUnits)
		{
			if (unit.GridPosition == tileMapPosition && unit != _activePlayerUnit) return false; // Occupied
		}

		return true;
	}

	/// <summary>
	/// Gets all valid and reachable tiles within a certain range using BFS.
	/// </summary>
	private Array<Vector2I> GetTilesInRangeForUnit(Vector2I startCoord, int rangeLimit)
	{
		Array<Vector2I> reachableTiles = new();
		Queue<(Vector2I coord, int distance)> queue = new();
		HashSet<Vector2I> visitedCoords = new();

		if (!IsTileWalkable(startCoord)) return reachableTiles;

		queue.Enqueue((startCoord, 0));
		visitedCoords.Add(startCoord);
		// Don't add startCoord to reachableTiles if unit shouldn't "move" to its own spot,
		// but it should be highlightable as the origin.
		// For movement, we usually want to highlight tiles *other* than the start.
		// However, the BFS explores from start, so start is distance 0.

		while (queue.Count > 0)
		{
			var (currentCellCoord, currentDistance) = queue.Dequeue();

			if (currentCellCoord != startCoord) // Only add actual move locations
			{
				reachableTiles.Add(currentCellCoord);
			}

			if (currentDistance < rangeLimit)
			{
				foreach (Vector2I dir in _battleHexDirections) // Use battle-specific directions
				{
					Vector2I neighborCoord = currentCellCoord + dir;
					if (IsTileWalkable(neighborCoord) && !visitedCoords.Contains(neighborCoord))
					{
						visitedCoords.Add(neighborCoord);
						queue.Enqueue((neighborCoord, currentDistance + 1));
					}
				}
			}
		}
		return reachableTiles;
	}

	/// <summary>
	/// Normalizes vector components to -1, 0, or 1. From PartyHexMovement.
	/// </summary>
	private Vector2 NormalizeHexComponents(Vector2 vector)
	{
		float x = 0, y = 0;
		if (vector.X > 0) x = 1;
		else if (vector.X < 0) x = -1;
		if (vector.Y > 0) y = 1;
		else if (vector.Y < 0) y = -1;
		return new Vector2(x, y);
	}

	private void HighlightPath(Vector2I startMapPos, Array<Vector2I> movementDeltas)
	{
		// This method is optional if you only highlight the final target or full range
		// If used, it should clear previous highlights first.
		// _highlightTileHelper.ClearHighlights(); 
		Array<Vector2I> pathTilesToHighlight = new();
		Vector2I currentMapPos = startMapPos;
		foreach (Vector2I moveDelta in movementDeltas)
		{
			currentMapPos += moveDelta;
			pathTilesToHighlight.Add(currentMapPos);
		}
		_highlightTileHelper.HighlightCollection(pathTilesToHighlight);
	}
}
