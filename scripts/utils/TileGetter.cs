using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace TheRingGoesSouth.scripts.utils
{
    public static class TileGetter
    {
        private static Vector2I[] _hexDirections =
        [
            new Vector2I(0, -1),  // N
            new Vector2I(-1, 0), // NW
            new Vector2I(1, 0),  // SE
            new Vector2I(0, 1),  // S
            new Vector2I(-1, 1), // SW
            new Vector2I(1, -1)  // NE
        ];
        public static Array<Vector2I> GetStraightTiles(Vector2I playerMapCoords, int depth)
        {
            Array<Vector2I> tiles = [];
            foreach (Vector2I dir in _hexDirections)
            {
                for (int i = 1; i <= depth; i++)
                {
                    Vector2I currentCell = playerMapCoords + (dir * i);
                    tiles.Add(currentCell);
                }
            }
            return tiles;
        }

        public static Array<Vector2I> GetPathFromMovementArray(Array<Vector2I> movementArray, TileMapLayer MapLayer, Vector2 GlobalPosition, Vector2? offset = null)
        {
            if (offset == null)
                offset = new Vector2(0, -6);
            Array<Vector2I> path = [];
            Vector2 targetTilePosition = MapLayer.LocalToMap((Vector2I)GlobalPosition);
            foreach (Vector2I move in movementArray)
            {
                targetTilePosition = (Vector2I)(targetTilePosition + move);
                Vector2 targetPosition = MapLayer.MapToLocal((Vector2I)targetTilePosition) + offset.Value;
                Vector2I targetTile = MapLayer.LocalToMap((Vector2I)targetPosition);
                path.Add(targetTile);
            }
            return path;
        }

        public static Array<Vector2I> GetTilesInRange(Vector2I startCoord, int rangeLimit, bool addCurrentTile = false)
        {
            Array<Vector2I> tiles = [];
            Queue<(Vector2I coord, int distance)> queue = new();
            HashSet<Vector2I> visitedCoords = new(); // To avoid processing/highlighting the same tile multiple times

            queue.Enqueue((startCoord, 0));
            visitedCoords.Add(startCoord);

            while (queue.Count > 0)
            {
                var (currentCellCoord, currentDistance) = queue.Dequeue();

                tiles.Add(currentCellCoord);

                if (currentDistance < rangeLimit) // Only expand if we haven't reached the range limit
                {
                    foreach (Vector2I dir in _hexDirections)
                    {
                        Vector2I neighborCoord = currentCellCoord + dir;

                        // Check if neighbor is valid and not visited yet
                        if (!visitedCoords.Contains(neighborCoord))
                        {
                            visitedCoords.Add(neighborCoord); // Mark as visited
                            queue.Enqueue((neighborCoord, currentDistance + 1)); // Add to queue for processing
                        }
                    }
                }
            }
            if (!addCurrentTile)
            {
                tiles.RemoveAt(0); // Remove the starting tile if not needed
            }
            return tiles;
        }

        public static Array<Vector2I> GetRoute(Vector2 startingPosition, Vector2 targetPosition, TileMapLayer MapLayer)
        {
            Array<Vector2I> route = [];
            Vector2 direction = MapLayer.LocalToMap(targetPosition) - MapLayer.LocalToMap(startingPosition);
            Vector2 currentTilePosition = MapLayer.LocalToMap(startingPosition);
            while (direction != Vector2I.Zero)
            {
                Vector2 newPoint = Normalize(direction);
                if (newPoint[0] * newPoint[1] == 1)
                {
                    Vector2 newDirection = new(0, newPoint[1]);
                    if (!HasTile((Vector2I)newDirection, MapLayer))
                    {
                        newPoint[1] = 0;
                    }
                    else
                    {
                        newPoint[0] = 0;
                    }
                }
                if (!HasTile((Vector2I)(currentTilePosition + newPoint), MapLayer))
                {
                    Array<Vector2I> points = [];
                    points.Add((Vector2I)new Vector2(0, newPoint[1]));
                    points.Add((Vector2I)new Vector2(newPoint[0], 0));
                    foreach (Vector2I point in points)
                    {
                        if (point == Vector2I.Zero)
                        {
                            continue;
                        }
                        if (HasTile((Vector2I)(currentTilePosition + point), MapLayer))
                        {
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

        private static Vector2 Normalize(Vector2 vector)
        {
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
            return vector;
        }
        
        private static bool HasTile(Vector2I tilePosition, TileMapLayer MapLayer)
        {
            return MapLayer.GetCellAtlasCoords(tilePosition) != null;
        }
    }
}