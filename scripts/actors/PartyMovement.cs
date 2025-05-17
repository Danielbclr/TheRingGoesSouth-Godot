using Godot;
using System;

public partial class PartyMovement : Node2D
{
	[Export]
	public TileMapLayer MapLayer { get; set; }

	[Export]
	public float MoveDelay { get; set; } = 0.25f;

	public Vector2I CurrentPosition { get; private set; }

	private bool _isMoving;
	private Vector2 _halfTileSize;
	private Vector2I _actualTileSize;

	public override void _Ready()
	{
		if (MapLayer == null)
		{
			GD.PrintErr("PartyMovement: MapLayer is not set in the editor.");
			GetTree().Quit(); 
			return;
		}

		if (MapLayer.TileSet == null)
		{
			GD.PrintErr("PartyMovement: MapLayer does not have a TileSet assigned.");
			GetTree().Quit(); 
			return;
		}

		_actualTileSize = MapLayer.TileSet.TileSize;
		GD.Print($"PartyMovement: TileSet.TileSize is {_actualTileSize}. Tile Width: {_actualTileSize.X}, Tile Height: {_actualTileSize.Y}");

		_halfTileSize = new Vector2(_actualTileSize.X / 2.0f, _actualTileSize.Y / 2.0f);
		GD.Print($"PartyMovement: Calculated _halfTileSize is {_halfTileSize}.");


		GD.Print($"PartyMovement: Initial GlobalPosition before LocalToMap: {GlobalPosition}");
		CurrentPosition = MapLayer.LocalToMap(GlobalPosition); 
		GD.Print($"PartyMovement: Initial CurrentPosition (map coords) after LocalToMap: {CurrentPosition}");
		SnapToGrid(CurrentPosition);
		GD.Print($"PartyMovement: GlobalPosition after initial SnapToGrid: {GlobalPosition}");
	}

	private void SnapToGrid(Vector2I gridCoords)
	{
		GlobalPosition = MapLayer.MapToLocal(gridCoords) + _halfTileSize;
		CurrentPosition = gridCoords;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (MapLayer == null || _isMoving)
		{
			return;
		}

		Vector2I moveDirection = Vector2I.Zero;

		if (Input.IsActionJustPressed("ui_right")) moveDirection.X += 1;
		if (Input.IsActionJustPressed("ui_left")) moveDirection.X -= 1;
		if (Input.IsActionJustPressed("ui_down")) moveDirection.Y += 1;
		if (Input.IsActionJustPressed("ui_up")) moveDirection.Y -= 1;
		

		if (moveDirection != Vector2I.Zero)
		{
			Vector2I targetGridCoords = CurrentPosition + moveDirection;
			TryMoveTo(targetGridCoords);
		}
	}

	private void TryMoveTo(Vector2I targetGridCoords)
	{
		if (!IsTileWalkable(targetGridCoords))
		{
			GD.Print($"Cannot move to {targetGridCoords}, tile is not walkable or out of bounds.");
			return;
		}

		_isMoving = true;
		Vector2 targetWorldPosition = MapLayer.MapToLocal(targetGridCoords) + _halfTileSize;

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "global_position", targetWorldPosition, MoveDelay)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);

		tween.Finished += () => OnMoveFinished(targetGridCoords);

	}

	private void OnMoveFinished(Vector2I newGridCoords)
	{
		CurrentPosition = newGridCoords;
		_isMoving = false;

	}

	private bool IsTileWalkable(Vector2I gridCoords)
	{
		if (MapLayer == null) return false;

		// Check if the target coordinates are within the used rectangle of the map
		// This helps prevent moving to "empty" areas outside your designed map space.
		Rect2I usedRect = MapLayer.GetUsedRect();
		if (!usedRect.HasPoint(gridCoords) && usedRect.Size != Vector2I.Zero) // Check Size to avoid issues with empty maps
		{
			// GD.Print($"Target {gridCoords} is outside of used map bounds: {usedRect}");
			// return false; // Uncomment if you want to strictly limit to GetUsedRect()
		}

		// Check the tile data for the specific cell
		// Layer 0 is assumed for walkability checks here.
		TileData cellData = MapLayer.GetCellTileData(gridCoords); 
		if (cellData == null)
		{
			// Null TileData might mean an empty cell.
			// Depending on your design, empty cells could be walkable or not.
			// For now, let's assume empty cells are walkable unless explicitly made non-walkable.
			// GD.Print($"Cell {gridCoords} has no TileData (empty).");
		}
		else
		{
			// Example: Check for a custom data layer named "walkable"
			// You would set this up in your TileSet editor.
			// Variant isWalkableData = cellData.GetCustomData("walkable");
			// if (isWalkableData.VariantType == Variant.Type.Bool && !isWalkableData.AsBool())
			// {
			//     GD.Print($"Cell {gridCoords} is not walkable based on custom data.");
			//     return false;
			// }

			// Example: Check for a physics layer if you use collision for walls
			// uint collisionLayerBit = 0; // The bit for your "walls" physics layer
			// if ((cellData.GetCollisionPolygonsCount(collisionLayerBit) > 0) || 
			//     (cellData.GetCollisionPolygonsCount(collisionLayerBit, true) > 0)) // Check transformed too
			// {
			//     GD.Print($"Cell {gridCoords} has collision on physics layer {collisionLayerBit}.");
			//     return false;
			// }
		}
		
		// TODO: Implement more sophisticated walkability checks based on your TileSet setup:
		// 1. Custom TileData properties (e.g., "is_walkable", "movement_cost").
		// 2. Physics layers defined in your TileSet for collision.
		// 3. Navigation layers if using Godot's navigation system.

		return true; // Placeholder: all tiles are considered walkable for now
	}
}
