using Godot;
using Godot.Collections;
using System;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.utils;

public partial class BattleInputHandler : Node2D
{
    private BattleTurnManager _turnManager;
    private BattleGridHelper _hexGridUtility;
    private BattleMovementController _movementController;
    private HighlightTileHelper _highlightTileHelper;

    public event Action<Vector2I> OnMoveTargetSelected;
    public event Action OnSkipTurnRequested;

    public void Initialize(BattleTurnManager turnManager, BattleGridHelper hexGridUtility, 
                          BattleMovementController movementController, HighlightTileHelper highlightTileHelper)
    {
        _turnManager = turnManager;
        _hexGridUtility = hexGridUtility;
        _movementController = movementController;
        _highlightTileHelper = highlightTileHelper;
    }

    public override void _Input(InputEvent @event)
    {
        if (_turnManager.ActivePlayerUnit == null) return;

        if (_turnManager.CurrentBattleState == BattleTurnManager.BattleTurnState.PlayerTurnReady)
        {
            if (Input.IsActionJustPressed("battle_skip_turn"))
            {
                OnSkipTurnRequested?.Invoke();
            }
            
            if (@event.IsActionPressed("left_click"))
            {
                Vector2I clickedTile = _hexGridUtility.WorldToMap(GetGlobalMousePosition());
                OnMoveTargetSelected?.Invoke(clickedTile);
            }
        }
    }
}
