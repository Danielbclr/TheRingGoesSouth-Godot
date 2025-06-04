using System;
using Godot;
using Godot.Collections;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.data.skill;
using TheRingGoesSouth.scripts.utils;
using TheRingGoesSouth.scripts.utils.Battle;

public partial class BattleScene : Node2D, ILoggable
{
	[Export] public bool DEBUG_TAG { get; set; } = false;
	[Export] public PackedScene PlayerUnitScene { get; set; }
	[Export] public float MoveDelayPerTile { get; set; } = 0.25f;
	[Export] public int PlayerMoveRange { get; set; } = 3;
	[Export] public TileMapLayer _battleGrid;

	// Component references
	private BattleTurnManager _turnManager;
	private BattleGridHelper _BattleGridHelper;
	private UnitSpawner _unitSpawner;
	private BattleInputHandler _inputHandler;
	private BattleMovementController _movementController;
	private BattleCameraController _cameraController;
	private HighlightTileHelper _highlightTileHelper;
	private BattleMenuHelper _battleMenuHelper;

	// Data
	private Array<PlayerUnit> _playerUnits = [];
	private Array<EnemyUnit> _enemyUnits = [];
	private Camera2D camera2D;

	public override void _Ready()
	{
		if (!ValidateComponents()) return;

		InitializeComponents();
		SetupEventHandlers();
		SpawnUnits();
		StartBattle();
	}

	private bool ValidateComponents()
	{
		camera2D = GetNode<Camera2D>("Camera2D");
		
		if (_battleGrid == null)
		{
			GD.PrintErr("BattleScene: TileMap node '_battleGrid' not found or not assigned!");
			GetTree().Quit(); return false;
		}
		if (_battleGrid.TileSet == null)
		{
			GD.PrintErr("BattleScene: _battleGrid does not have a TileSet assigned.");
			GetTree().Quit(); return false;
		}
		if (PlayerUnitScene == null)
		{
			GD.PrintErr("BattleScene: PlayerUnitScene PackedScene is not set in the editor!");
			GetTree().Quit(); return false;
		}
		return true;
	}

	private void InitializeComponents()
	{
		// Create and initialize all components
		_turnManager = new BattleTurnManager();
		_BattleGridHelper = new BattleGridHelper();
		_unitSpawner = new UnitSpawner { PlayerUnitScene = PlayerUnitScene };
		_inputHandler = new BattleInputHandler();
		_movementController = new BattleMovementController { MoveDelayPerTile = MoveDelayPerTile, PlayerMoveRange = PlayerMoveRange };
		_cameraController = new BattleCameraController();
		_highlightTileHelper = new HighlightTileHelper(this, _battleGrid);
		_battleMenuHelper = new BattleMenuHelper(this);

		// Add components to scene tree
		AddChild(_battleMenuHelper);
		AddChild(_turnManager);
		AddChild(_BattleGridHelper);
		AddChild(_unitSpawner);
		AddChild(_inputHandler);
		AddChild(_movementController);
		AddChild(_cameraController);

		// Initialize components with dependencies
		_unitSpawner.Initialize(_battleGrid, this);
		_cameraController.Initialize(camera2D);
	}

	private void SetupEventHandlers()
	{
		_turnManager.OnTurnStarted += OnTurnStarted;
		_inputHandler.OnMoveTargetSelected += OnMoveTargetSelected;
		_inputHandler.OnSkipTurnRequested += OnSkipTurnRequested;
		_battleMenuHelper.SkillSelected += OnSkillSelectedByPlayer;
		_battleMenuHelper.MenuEntered += OnBattleMenuEntered;
	}

	private void OnBattleMenuEntered(bool entered)
	{
		_inputHandler.blockInput(entered);
	}

	private void OnSkillSelectedByPlayer(string skillId)
	{
		GD.Print($"BattleScene: Skill '{skillId}' selected by player via BattleMenuHelper.");

		SkillData selectedSkillData = SkillDataLoader.GetSkillData(skillId);
		if (selectedSkillData == null)
		{
			GD.PrintErr($"BattleScene: Could not retrieve data for selected skill ID: {skillId}");
			return;
		}

		// TODO:
		// 1. Check character resources (ManaCost, CooldownTurns)
		// 2. Initiate targeting phase based on selectedSkillData.Targeting
		// 3. Once target(s) are confirmed, apply skill effects.
		// 4. Update UI, character states, and potentially end the turn or allow further actions.

		GD.Print($"Executing skill: {selectedSkillData.Name}. Targeting: {selectedSkillData.Targeting.Type}");
		// The BattleSkillMenu hides itself after selection.
	}

	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	// Example: Press 'K' to open the skill menu for the active player
	// 	if (_turnManager.CurrentBattleState == BattleTurnManager.BattleTurnState.PlayerTurnReady && 
	// 	    @event.IsActionPressed("ui_skill_menu")) // Define "ui_skill_menu" in Input Map (e.g., map to K key)
	// 	{
	// 		if (_battleMenuHelper != null && _turnManager.ActivePlayerUnit?.CharacterData != null)
	// 		{
	// 			_battleMenuHelper.DisplaySkillsForCharacter(_turnManager.ActivePlayerUnit);
	// 			GetViewport().SetInputAsHandled();
	// 		}
	// 	}
	// }

	private void SpawnUnits()
	{
		_playerUnits = _unitSpawner.SpawnPartyMembers();
		
		// Spawn enemies
		EnemyUnit goblin = _unitSpawner.SpawnEnemy("goblin_scout", new Vector2I(8, 5));
		if (goblin != null)
		{
			_enemyUnits.Add(goblin);
		}

		// Initialize hex grid utility with unit references
		_BattleGridHelper.Initialize(_battleGrid, _playerUnits, _enemyUnits);
		_movementController.Initialize(_BattleGridHelper, _highlightTileHelper, _turnManager);
		_inputHandler.Initialize(_turnManager, _BattleGridHelper, _movementController, _highlightTileHelper);
	}

	private void StartBattle()
	{
		if (_playerUnits.Count > 0)
		{
			_turnManager.Initialize(_playerUnits);
			_turnManager.StartBattle();
			GD.Print("BattleScene: Battle started.");
		}
		else
		{
			GD.Print("BattleScene: No player units to start battle with.");
		}
	}

	private void OnTurnStarted(PlayerUnit activeUnit)
	{
		_cameraController.FocusOnUnit(activeUnit, this);
		_movementController.InitiateMoveSelection(activeUnit);
		_battleMenuHelper.DisplaySkillsForCharacter(activeUnit);
		GD.Print("Choose action: (S)kip Turn");
	}

	private async void OnMoveTargetSelected(Vector2I targetTile)
	{
		if (_movementController.IsValidMoveTarget(targetTile))
		{
			GD.Print($"Move confirmed to: {targetTile}");
			await _movementController.ExecuteMoveAsync(_turnManager.ActivePlayerUnit, targetTile);
		}
		else
		{
			GD.Print($"Invalid move target: {targetTile}");
		}
	}

	private void OnSkipTurnRequested()
	{
		_movementController.ClearMoveSelection();
		_turnManager.SkipTurn();
	}
}
