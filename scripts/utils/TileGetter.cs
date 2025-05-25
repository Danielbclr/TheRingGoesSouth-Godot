using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace TheRingGoesSouth.scripts.utils
{
    public class TileGetter
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
    }
}