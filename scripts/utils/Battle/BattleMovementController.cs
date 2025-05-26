using Godot;
using Godot.Collections;
using System.Threading.Tasks;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.utils;

public partial class BattleMovementController : Node
{
    [Export] public float MoveDelayPerTile { get; set; } = 0.25f;
    [Export] public int PlayerMoveRange { get; set; } = 3;

    private BattleGridHelper _BattleGridHelper;
    private HighlightTileHelper _highlightTileHelper;
    private BattleTurnManager _turnManager;

    private Array<Vector2I> _validMoveLocations = new();
    private Array<Vector2I> _currentMovePath = new();

    public void Initialize(BattleGridHelper BattleGridHelper, HighlightTileHelper highlightTileHelper, BattleTurnManager turnManager)
    {
        _BattleGridHelper = BattleGridHelper;
        _highlightTileHelper = highlightTileHelper;
        _turnManager = turnManager;
    }

    public void InitiateMoveSelection(PlayerUnit activeUnit)
    {
        GD.Print("Selecting move target...");
        _validMoveLocations = _BattleGridHelper.GetTilesInRange(activeUnit.GridPosition, PlayerMoveRange, activeUnit);
        _highlightTileHelper.HighlightCollection(_validMoveLocations);
        GD.Print($"Found {_validMoveLocations.Count} valid move locations.");
    }

    public bool IsValidMoveTarget(Vector2I targetTile)
    {
        return _validMoveLocations.Contains(targetTile);
    }

    public async Task ExecuteMoveAsync(PlayerUnit unit, Vector2I targetMapPos)
    {
        _currentMovePath = _BattleGridHelper.GetMovementPath(unit.GlobalPosition, _BattleGridHelper.MapToWorld(targetMapPos));
        
        if (_currentMovePath.Count > 0 || targetMapPos == unit.GridPosition)
        {
            _highlightTileHelper.ClearHighlights();
            _turnManager.SetState(BattleTurnManager.BattleTurnState.PlayerMoving);
            await PerformMovementAnimation(unit, _currentMovePath, targetMapPos);
            _turnManager.EndTurn();
        }
        else
        {
            GD.Print("Path to target not found or invalid.");
        }
    }

    private async Task PerformMovementAnimation(PlayerUnit unit, Array<Vector2I> pathDeltas, Vector2I finalTargetMapPos)
    {
        Vector2I currentLogicalMapPos = unit.GridPosition;

        foreach (Vector2I moveDelta in pathDeltas)
        {
            currentLogicalMapPos += moveDelta;
            Vector2 targetWorldPos = _BattleGridHelper.MapToWorld(currentLogicalMapPos);

            Tween tween = CreateTween();
            tween.TweenProperty(unit, "global_position", targetWorldPos, MoveDelayPerTile);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Sine);
            await ToSignal(tween, Tween.SignalName.Finished);
            unit.SetGridPosition(currentLogicalMapPos, true);
        }

        unit.SetGridPosition(finalTargetMapPos, true);
    }

    public void ClearMoveSelection()
    {
        GD.Print("Move selection cancelled.");
        _highlightTileHelper.ClearHighlights();
        _validMoveLocations.Clear();
    }
}