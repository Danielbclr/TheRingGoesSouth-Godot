using Godot;

using Godot.Collections;

using System;
using System.Collections.Generic;

using System.Threading.Tasks;
public partial class PartyHexMovement : Node2D

{
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

    public override void _Ready()
    {
        if (MapLayer == null || MapLayer.TileSet == null)
        {
            GD.PrintErr("PartyHexMovement: MapLayer or its TileSet is not set. Disabling script.");
            SetProcessInput(false);
            return;
        }

        // Initialize hex directions array
        _hexDirections = [HEX_NE, HEX_E, HEX_SE, HEX_SW, HEX_W, HEX_NW];
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("highlight_adjacent"))
        {
            GD.Print("Highlighting adjacent tiles.");
            ClearHighlights();
            HighlightTilesInRange(3);
        }
        else if (Input.IsActionJustPressed("highlight_lines"))
        {
            GD.Print("Highlighting lines.");
            ClearHighlights();
            HighlightStraightLines(3); // Highlight 3 tiles deep
        }
        else if (Input.IsActionJustPressed("clear_highlights_action")) // Optional: A key to manually clear
        {
            GD.Print("Clearing highlights manually.");
            ClearHighlights();
        }
        else if (Input.IsActionPressed("left_click"))
        {
            GetMovementArray();
        }
        else if (Input.IsActionJustReleased("left_click"))
        {
            GD.Print("Left click released");
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
        Vector2I? isTargetValid = GetTileGlobalPosition(mousePosition);
        GD.Print($"Mouse position: {mousePosition}");
        GD.Print($"Target position: {isTargetValid}");
        if (isTargetValid == null)
        {
            GD.Print("Invalid target");
            return;
        }
        ClearHighlights();
        _movementArray = GetRoute(mousePosition);
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
            //HighlightTile((Vector2I)targetPosition);
            GD.Print($"Player at {playerTilePosition}");
            GD.Print($"Target at {targetTilePosition}");
            GD.Print($"Moving to {targetPosition}");
            Tween tween = CreateTween();
            tween.TweenProperty(this, "global_position", targetPosition, MoveDelay);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Expo);
            await ToSignal(tween, "finished");
        }
        ClearHighlights();
        _isMoving = false;
    }
    private Vector2I? GetTileGlobalPosition(Vector2 mousePosition)
    {
        Vector2I mouseTilePosition = MapLayer.LocalToMap(mousePosition);
        Vector2 mouseTileData = MapLayer.GetCellAtlasCoords(mouseTilePosition);
        // if (mouseTileData == new Vector2(-1, -1))
        // {
        //     GD.Print("Invalid tile");
        //     return null;
        // }
        GD.Print($"Mouse tile position: {mouseTilePosition}");
        Vector2 tilePosition = MapLayer.MapToLocal(mouseTilePosition);
        GD.Print($"GetTileGlobalPosition: Tile position: {tilePosition}");
        return (Vector2I)tilePosition;
    }
    
    private Array<Vector2I> GetRoute(Vector2 targetPosition)
    {
        Array<Vector2I> route = [];
        Vector2 direction = MapLayer.LocalToMap(targetPosition) - MapLayer.LocalToMap(GlobalPosition);
        Vector2 currentTilePosition = MapLayer.LocalToMap(GlobalPosition);
        GD.Print($"Direction: {direction}");
        GD.Print($"Current tile position: {currentTilePosition}");
        while (direction != Vector2I.Zero)
        {
            Vector2 newPoint = Normalize(direction);
            if (newPoint[0] * newPoint[1] == 1)
            {
                Vector2 newDirection = new(0, newPoint[1]);
                if (!HasTile((Vector2I)newDirection))
                {
                    newPoint[1] = 0;
                }
                else
                {
                    newPoint[0] = 0;
                }
            }
            if (!HasTile((Vector2I)(currentTilePosition + newPoint)))
            {
                Array<Vector2I> points = [];
                points.Add((Vector2I)new Vector2(0, newPoint[1]));
                points.Add((Vector2I)new Vector2(newPoint[0], 0));
                foreach (Vector2I point in points)
                {
                    if (point == Vector2I.Zero)
                    {
                        GD.Print("Zero point");
                        continue;
                    }
                    if (HasTile((Vector2I)(currentTilePosition + point)))
                    {
                        GD.Print($"Found point: {point}");
                        newPoint = point;
                    }
                }
            }
            route.Add((Vector2I)newPoint);
            direction -= newPoint;
            currentTilePosition += newPoint;
        }
        return route;
    }
    private Vector2 Normalize(Vector2 vector)
    {
        GD.Print($"Normalizing vector: {vector}");
        if (vector[0] > 0)
        {
            vector[0] = 1;
        }
        else if (vector[0] < 0)
        {
            vector[0] = -1;
        }
        if (vector[1] > 0)
        {
            vector[1] = 1;
        }
        else if (vector[1] < 0)
        {
            vector[1] = -1;
        }
        GD.Print($"Normalized vector: {vector}");
        return vector;
    }
    private bool HasTile(Vector2I tilePosition)
    {
        GD.Print($"Checking tile: {tilePosition}");
        return MapLayer.GetCellAtlasCoords(tilePosition) != null;
    }

    private void HighlightPath(Array<Vector2I> movementArray)
    {
        Vector2 targetTilePosition = MapLayer.LocalToMap((Vector2I)GlobalPosition);
        foreach (Vector2I move in movementArray)
        {
            targetTilePosition = (Vector2I)(targetTilePosition + move);
            Vector2 targetPosition = MapLayer.MapToLocal((Vector2I)targetTilePosition) + offset;
            Vector2I targetTile = MapLayer.LocalToMap((Vector2I)targetPosition);
            CreateHighlightInstanceAtMapCoord(targetTile);
        }
    }

    private void HighlightCollection(Array<Vector2I> tileCollection)
    {
        Vector2 targetTilePosition = MapLayer.LocalToMap((Vector2I)GlobalPosition);
        foreach (Vector2I move in tileCollection)
        {
            targetTilePosition = (Vector2I)(targetTilePosition + move);
            Vector2 targetPosition = MapLayer.MapToLocal((Vector2I)targetTilePosition) + offset;
            CreateHighlightInstanceAtMapCoord((Vector2I)targetPosition);
        }
    }

    private void CreateHighlightInstanceAtMapCoord(Vector2I mapCoord)
    {
        if (HighlightScene == null)
        {
            GD.PrintErr("HighlightScene is not set in PartyHexMovement!");
            return;
        }

        HighlightHexTile instance = HighlightScene.Instantiate<HighlightHexTile>();
        if (instance != null)
        {

            Vector2 tileWorldPosition = MapLayer.MapToLocal(mapCoord) + offset;

            instance.GlobalPosition = tileWorldPosition;

            GetParent().AddChild(instance); // Add as sibling to PartyHexMovement
            _highlightedTiles.Add(instance);
        }
    }

    private void HighlightAdjacentTiles()
    {
        Vector2I playerMapCoords = GetCurrentTilePosition();
        foreach (Vector2I dir in _hexDirections)
        {
            Vector2I adjacentCell = playerMapCoords + dir;
            if (HasTile(adjacentCell))
            {
                CreateHighlightInstanceAtMapCoord(adjacentCell);
            }
        }
    }

    private void HighlightStraightLines(int depth)
    {
        Vector2I playerMapCoords = GetCurrentTilePosition();
        foreach (Vector2I dir in _hexDirections)
        {
            for (int i = 1; i <= depth; i++)
            {
                Vector2I currentCell = playerMapCoords + (dir * i);
                if (HasTile(currentCell))
                {
                    CreateHighlightInstanceAtMapCoord(currentCell);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void HighlightTilesInRange(int rangeLimit)
    {
        Vector2I startCoord = GetCurrentTilePosition();

        Queue<(Vector2I coord, int distance)> queue = new();
        HashSet<Vector2I> visitedCoords = new(); // To avoid processing/highlighting the same tile multiple times

        if (!HasTile(startCoord))
        {
            GD.PrintErr($"Player start tile {startCoord} is invalid for range highlight.");
            return;
        }

        queue.Enqueue((startCoord, 0));
        visitedCoords.Add(startCoord);

        while (queue.Count > 0)
        {
            var (currentCellCoord, currentDistance) = queue.Dequeue();

            // Highlight this cell as it's confirmed to be in range
            // (Could add a check here to not highlight the startCoord itself if desired)
            // if (currentCellCoord != startCoord || rangeLimit == 0) // Example: don't highlight start unless range is 0
            CreateHighlightInstanceAtMapCoord(currentCellCoord);

            if (currentDistance < rangeLimit) // Only expand if we haven't reached the range limit
            {
                foreach (Vector2I dir in _hexDirections)
                {
                    Vector2I neighborCoord = currentCellCoord + dir;

                    // Check if neighbor is valid and not visited yet
                    if (HasTile(neighborCoord) && !visitedCoords.Contains(neighborCoord))
                    {
                        visitedCoords.Add(neighborCoord); // Mark as visited
                        queue.Enqueue((neighborCoord, currentDistance + 1)); // Add to queue for processing
                    }
                }
            }
        }
    }
    
    private void ClearHighlights()
    {
        foreach (HighlightHexTile highlight in _highlightedTiles)
        {
            highlight.QueueFree();
        }
        _highlightedTiles.Clear();
    }
}