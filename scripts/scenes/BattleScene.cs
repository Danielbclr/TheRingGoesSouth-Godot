
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq; // For HashSet
using System.Threading.Tasks;
using TheRingGoesSouth.scripts.utils;

public partial class BattleScene : Node2D, ILoggable
{
	[Export] public bool DEBUG_TAG { get; set; } = false;

	[Export] public PackedScene PlayerUnitScene { get; set; }

	[Export] public float MoveDelayPerTile { get; set; } = 0.25f;
	[Export] public int PlayerMoveRange { get; set; } = 3;
	[Export] public TileMapLayer _battleGrid;

	private Camera2D camera2D;

	private Array<PlayerUnit> _playerUnits = [];
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
	private Array<Vector2I> _currentMovePath = new();

	private readonly Vector2I[] _hexDirections = {
		new Vector2I(1, -1), new Vector2I(0, -1), new Vector2I(-1, 0), // NE, N, NW
		new Vector2I(-1, 1), new Vector2I(0, 1), new Vector2I(1, 0)    // SW, S, SE
	};

	private readonly Vector2I[] _battleHexDirections = {
		new(0, -1), new(1, -1), new(1, 0), 
		new(0, 1), new(-1, 1), new(-1, 0)
	};


	public override void _Ready()
	{
		camera2D = GetNode<Camera2D>("Camera2D");
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

		_highlightTileHelper = new HighlightTileHelper(this, _battleGrid);
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
			playerUnit.Setup(character, _battleGrid, gridPosition); 
			
			AddChild(playerUnit);
			_playerUnits.Add(playerUnit);
			
			GD.Print($"BattleScene: Placed {character.CharacterName} at map {gridPosition}, world {playerUnit.GlobalPosition}.");
		}
		GD.Print($"BattleScene: Placed {_playerUnits.Count} party members on the grid.");
	}

	private void StartBattle()
	{
		_currentPlayerIndex = -1;
		_currentBattleState = BattleTurnState.TurnEnding;
		NextTurn();
	}

	private void NextTurn()
	{

		_highlightTileHelper.ClearHighlights();
		_validMoveLocations.Clear();
		_currentMovePath.Clear();

		_currentPlayerIndex = (_currentPlayerIndex + 1) % _playerUnits.Count;
		_activePlayerUnit = _playerUnits[_currentPlayerIndex];
		_currentBattleState = BattleTurnState.PlayerTurnStarting;

		camera2D.Reparent(this);
		camera2D.MakeCurrent();

		Tween tween = CreateTween();
		tween.TweenProperty(camera2D, "global_position", _activePlayerUnit.GlobalPosition, 0.5f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Sine);

		GD.Print($"--- {_activePlayerUnit.CharacterData.CharacterName}'s Turn ---");

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

				if (_validMoveLocations.Contains(clickedTile))
				{
					GD.Print($"Move confirmed to: {clickedTile}");

					_currentMovePath = HexTileHelper.GetRoute(_activePlayerUnit.GlobalPosition, GetGlobalMousePosition(), _battleGrid);
					if (_currentMovePath.Count > 0 || clickedTile == _activePlayerUnit.GridPosition) // Allow clicking self to "pass" move
					{
						_highlightTileHelper.ClearHighlights();
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
		if (_battleGrid.GetCellSourceId(tileMapPosition) == -1)
		{
			return false;
		}
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
		Array<Vector2I> reachableTiles = [];
		Queue<(Vector2I coord, int distance)> queue = new();
		HashSet<Vector2I> visitedCoords = [];

		if (!IsTileWalkable(startCoord)) return reachableTiles;

		queue.Enqueue((startCoord, 0));
		visitedCoords.Add(startCoord);

		while (queue.Count > 0)
		{
			var (currentCellCoord, currentDistance) = queue.Dequeue();

			if (currentCellCoord != startCoord)
			{
				reachableTiles.Add(currentCellCoord);
			}

			if (currentDistance < rangeLimit)
			{
				foreach (Vector2I dir in _battleHexDirections)
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

	private void HighlightPath(Vector2I startMapPos, Array<Vector2I> movementDeltas)
	{
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
