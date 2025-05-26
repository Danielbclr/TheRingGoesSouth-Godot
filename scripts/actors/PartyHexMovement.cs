using Godot;

using Godot.Collections;

using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using TheRingGoesSouth.scripts.utils;
public partial class PartyHexMovement : Node2D, ILoggable
{
    [Export]
    public bool DEBUG_TAG { get; set; } = false;
    private Logger Logger;
    
    [Export]
    public TileMapLayer MapLayer { get; set; }
    [Export]
    public float MoveDelay { get; set; } = 0.25f;
    [Export]
    public Vector2 offset = new(0, -6);
    [Export]
    public PackedScene HighlightScene { get; set; }
    private bool _isMoving;
    private Array<Vector2I> _movementArray = [];
    private Array<HighlightHexTile> _highlightedTiles = [];

    private Vector2I HEX_NE = new(0, -1);
    private Vector2I HEX_NW = new(-1, 0);
    private Vector2I HEX_SE = new(1, 0);
    private Vector2I HEX_SW = new(0, 1);
    private Vector2I HEX_W = new(-1, 1);
    private Vector2I HEX_E = new(1, -1);

    private Vector2I[] _hexDirections;
    private HighlightTileHelper highlightTileHelper;

    public override void _Ready()
    {
        if (MapLayer == null || MapLayer.TileSet == null)
        {
            GD.PrintErr("PartyHexMovement: MapLayer or its TileSet is not set. Disabling script.");
            SetProcessInput(false);
            return;
        }
        Logger = new Logger(this);
        highlightTileHelper = new HighlightTileHelper(GetParent(), MapLayer);
        highlightTileHelper.offset = offset;
        _hexDirections = [HEX_NE, HEX_E, HEX_SE, HEX_SW, HEX_W, HEX_NW];
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("highlight_adjacent"))
        {
            Logger.Log("Highlighting adjacent tiles.");
            highlightTileHelper.ClearHighlights();
            HighlightTilesInRange(3);
        }
        else if (Input.IsActionJustPressed("highlight_lines"))
        {
            Logger.Log("Highlighting lines.");
            highlightTileHelper.ClearHighlights();
            HighlightStraightLines(3);
        }
        else if (Input.IsActionJustPressed("clear_highlights_action"))
        {
            Logger.Log("Clearing highlights manually.");
            highlightTileHelper.ClearHighlights();
        }
        else if (Input.IsActionPressed("left_click"))
        {
            GetMovementArray();
        }
        else if (Input.IsActionJustReleased("left_click"))
        {
            Logger.Log("Left click released");
            _ = Move();
        }
    }

    private Vector2I GetCurrentTilePosition()
    {
        return MapLayer.LocalToMap((Vector2I)GlobalPosition);
    }
    private void GetMovementArray()
    {
        Vector2 mousePosition = GetGlobalMousePosition() - offset;
        Vector2I? validTilePosition = HexTileHelper.GetTileGlobalPosition(mousePosition, MapLayer);
        Logger.Log($"Mouse position: {mousePosition}");
        Logger.Log($"Target position: {validTilePosition}");
        if (validTilePosition == null)
        {
            Logger.Log("Invalid target");
            return;
        }
        highlightTileHelper.ClearHighlights();
        _movementArray = HexTileHelper.GetRoute(GlobalPosition, mousePosition, MapLayer);
        HighlightPath(_movementArray);
    }
    private async Task Move()
    {
        if (_isMoving)
        {
            return;
        }

        _isMoving = true;
        foreach (Vector2I move in _movementArray)
        {
            Vector2 playerTilePosition = GetCurrentTilePosition();
            Vector2 targetTilePosition = (Vector2I)(playerTilePosition + move);
            Vector2 targetPosition = MapLayer.MapToLocal((Vector2I)targetTilePosition) + offset;
            Logger.Log($"Player at {playerTilePosition}");
            Logger.Log($"Target at {targetTilePosition}");
            Logger.Log($"Moving to {targetPosition}");
            Tween tween = CreateTween();
            tween.TweenProperty(this, "global_position", targetPosition, MoveDelay);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Expo);
            await ToSignal(tween, "finished");
        }
        highlightTileHelper.ClearHighlights();
        _isMoving = false;
    }
    
    private void HighlightPath(Array<Vector2I> movementArray)
    {
        highlightTileHelper.ClearHighlights();
        Array<Vector2I> movementTiles = HexTileHelper.GetPathFromMovementArray(movementArray, MapLayer, GlobalPosition, offset);
        highlightTileHelper.HighlightCollection(movementTiles);
    }

    private void HighlightStraightLines(int depth)
    {
        HexTileHelper.GetStraightTiles(GetCurrentTilePosition(), depth);
        highlightTileHelper.HighlightCollection(HexTileHelper.GetStraightTiles(GetCurrentTilePosition(), depth));
    }

    private void HighlightTilesInRange(int rangeLimit)
    {
        Array<Vector2I> tiles = HexTileHelper.GetTilesInRange(GetCurrentTilePosition(), rangeLimit);
        highlightTileHelper.HighlightCollection(tiles); 
    }
    
}