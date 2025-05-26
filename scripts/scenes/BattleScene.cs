using Godot;
using Godot.Collections;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.utils;

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

		// Add components to scene tree
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
	}

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
