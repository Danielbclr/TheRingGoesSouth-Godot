using Godot;

using Godot.Collections;

using System;

using System.Threading.Tasks;
public partial class PartyHexMovement : Node2D

{
	[Export]
	public TileMapLayer MapLayer { get; set; }
	[Export]
	public float MoveDelay { get; set; } = 0.25f;
	[Export]
	public Vector2 offset = new(0, -6);
	private bool _isMoving;
	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("left_click"))
		{
			_ = Move();
		}
	}
	private async Task Move()
	{
		if (_isMoving)
		{
			return;
		}
		Vector2 mousePosition = GetGlobalMousePosition() - offset;
		Vector2I? isTargetValid = GetTileGlobalPosition(mousePosition);
		GD.Print($"Mouse position: {mousePosition}");
		GD.Print($"Target position: {isTargetValid}");
		if (isTargetValid == null)
		{
			GD.Print("Invalid target");
			return;
		}
		_isMoving = true;
		Array<Vector2I> movementArray = GetRoute(mousePosition);
		foreach (Vector2I move in movementArray)
		{
			Vector2 playerTilePosition = MapLayer.LocalToMap((Vector2I)GlobalPosition);
			Vector2 targetTilePosition = (Vector2I)(playerTilePosition + move);
			Vector2 targetPosition = MapLayer.MapToLocal((Vector2I)targetTilePosition) + offset;
			GD.Print($"Player at {playerTilePosition}");
			GD.Print($"Target at {targetTilePosition}");
			GD.Print($"Moving to {targetPosition}");
			Tween tween = CreateTween();
			tween.TweenProperty(this, "global_position", targetPosition, MoveDelay);
			tween.SetEase(Tween.EaseType.Out);
			tween.SetTrans(Tween.TransitionType.Expo);
			await ToSignal(tween, "finished");
		}
		_isMoving = false;
	}
	private Vector2I? GetTileGlobalPosition(Vector2 mousePosition)
	{
		Vector2I mouseTilePosition = MapLayer.LocalToMap(mousePosition);
		Vector2 mouseTileData = MapLayer.GetCellAtlasCoords(mouseTilePosition);
		if (mouseTileData == new Vector2(-1, -1))
		{
			GD.Print("Invalid tile");
			return null;
		}
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
		return MapLayer.GetCellAtlasCoords(tilePosition) != new Vector2(-1, -1);
	}
}