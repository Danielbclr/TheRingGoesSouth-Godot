using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.utils;

public partial class BattleGridHelper : Node
{
    private TileMapLayer _battleGrid;
    private Array<PlayerUnit> _playerUnits;
    private Array<EnemyUnit> _enemyUnits;

    private readonly Vector2I[] _battleHexDirections = {
        new(0, -1), new(1, -1), new(1, 0),
        new(0, 1), new(-1, 1), new(-1, 0)
    };

    public void Initialize(TileMapLayer battleGrid, Array<PlayerUnit> playerUnits, Array<EnemyUnit> enemyUnits)
    {
        _battleGrid = battleGrid;
        _playerUnits = playerUnits;
        _enemyUnits = enemyUnits;
    }

    public bool IsTileWalkable(Vector2I tileMapPosition, PlayerUnit excludeUnit = null)
    {
        if (_battleGrid.GetCellSourceId(tileMapPosition) == -1)
        {
            return false;
        }

        foreach (var unit in _playerUnits)
        {
            if (unit.GridPosition == tileMapPosition && unit != excludeUnit) 
                return false;
        }

        foreach (var unit in _enemyUnits)
        {
            if (unit.GridPosition == tileMapPosition) 
                return false;
        }

        return true;
    }

    public Array<Vector2I> GetTilesInRange(Vector2I startCoord, int rangeLimit, PlayerUnit excludeUnit = null)
    {
        Array<Vector2I> reachableTiles = [];
        Queue<(Vector2I coord, int distance)> queue = new();
        HashSet<Vector2I> visitedCoords = [];

        if (!IsTileWalkable(startCoord, excludeUnit)) return reachableTiles;

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
                    if (IsTileWalkable(neighborCoord, excludeUnit) && !visitedCoords.Contains(neighborCoord))
                    {
                        visitedCoords.Add(neighborCoord);
                        queue.Enqueue((neighborCoord, currentDistance + 1));
                    }
                }
            }
        }
        return reachableTiles;
    }

    public Array<Vector2I> GetMovementPath(Vector2 startWorldPos, Vector2 targetWorldPos)
    {
        return HexGridHelper.GetRoute(startWorldPos, targetWorldPos, _battleGrid);
    }

    public Vector2 MapToWorld(Vector2I mapPosition)
    {
        return _battleGrid.MapToLocal(mapPosition);
    }

    public Vector2I WorldToMap(Vector2 worldPosition)
    {
        return _battleGrid.LocalToMap(worldPosition);
    }
}
