using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace TheRingGoesSouth.scripts.utils
{
    /// <summary>
    /// Provides helper methods for working with hexagonal tile maps, including tile selection,
    /// pathfinding, and range calculations for Godot TileMap layers.
    /// </summary>
    public static class HexTileHelper
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
        /// <summary>
        /// Gets all tiles in straight lines from a starting coordinate in all hex directions up to a given depth.
        /// </summary>
        /// <param name="playerMapCoords">The starting tile coordinates.</param>
        /// <param name="depth">How far to extend in each direction.</param>
        /// <returns>An array of tile coordinates in straight lines from the start.</returns>
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

        /// <summary>
        /// Converts a movement array into a path of tile coordinates, applying an optional offset.
        /// </summary>
        /// <param name="movementArray">Array of movement vectors.</param>
        /// <param name="MapLayer">The tile map layer to use for coordinate conversion.</param>
        /// <param name="GlobalPosition">The starting global position.</param>
        /// <param name="offset">Optional offset to apply to each step (default is (0, -6)).</param>
        /// <returns>An array of tile coordinates representing the path.</returns>
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

        /// <summary>
        /// Gets all tile coordinates within a given range of a starting coordinate.
        /// </summary>
        /// <param name="startCoord">The starting tile coordinate.</param>
        /// <param name="rangeLimit">The maximum range (distance) to include.</param>
        /// <param name="addCurrentTile">If true, includes the starting tile in the result; otherwise, excludes it.</param>
        /// <returns>An array of tile coordinates within the specified range.</returns>
        public static Array<Vector2I> GetTilesInRange(Vector2I startCoord, int rangeLimit, bool addCurrentTile = false)
        {
            Array<Vector2I> tiles = [];
            Queue<(Vector2I coord, int distance)> queue = new();
            HashSet<Vector2I> visitedCoords = new();

            queue.Enqueue((startCoord, 0));
            visitedCoords.Add(startCoord);

            while (queue.Count > 0)
            {
                var (currentCellCoord, currentDistance) = queue.Dequeue();

                tiles.Add(currentCellCoord);

                if (currentDistance < rangeLimit)
                {
                    foreach (Vector2I dir in _hexDirections)
                    {
                        Vector2I neighborCoord = currentCellCoord + dir;

                        if (!visitedCoords.Contains(neighborCoord))
                        {
                            visitedCoords.Add(neighborCoord);
                            queue.Enqueue((neighborCoord, currentDistance + 1));
                        }
                    }
                }
            }
            if (!addCurrentTile)
            {
                tiles.RemoveAt(0);
            }
            return tiles;
        }

        /// <summary>
        /// Calculates a route of tile steps from a starting position to a target position on a hex grid.
        /// </summary>
        /// <param name="startingPosition">The starting position in local coordinates.</param>
        /// <param name="targetPosition">The target position in local coordinates.</param>
        /// <param name="MapLayer">The tile map layer to use for coordinate conversion and tile checks.</param>
        /// <returns>An array of movement vectors representing the route.</returns>
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
                    if (!IsTileWalkable((Vector2I)newDirection, MapLayer))
                    {
                        newPoint[1] = 0;
                    }
                    else
                    {
                        newPoint[0] = 0;
                    }
                }
                if (!IsTileWalkable((Vector2I)(currentTilePosition + newPoint), MapLayer))
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
                        if (IsTileWalkable((Vector2I)(currentTilePosition + point), MapLayer))
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

        /// <summary>
        /// Normalizes a vector so that each component is -1, 0, or 1.
        /// </summary>
        /// <param name="vector">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        public static Vector2 Normalize(Vector2 vector)
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

        /// <summary>
        /// Checks if a tile exists at the given position in the specified tile map layer.
        /// </summary>
        /// <param name="tilePosition">The tile coordinates to check.</param>
        /// <param name="MapLayer">The tile map layer to check in.</param>
        /// <returns>True if a tile exists at the position; otherwise, false.</returns>
        public static bool IsTileWalkable(Vector2I tilePosition, TileMapLayer MapLayer)
        {
            if (MapLayer.GetCellSourceId(tilePosition) == -1)
            {
                return false;
            }
            return true;
        }

        public static Vector2I? GetTileGlobalPosition(Vector2 globalPosition, TileMapLayer MapLayer)
        {
            Vector2I tilePosition = MapLayer.LocalToMap(globalPosition);
            Vector2 tileGlobalPosition = MapLayer.MapToLocal(tilePosition);
            return (Vector2I)tileGlobalPosition;
        }
    }
}