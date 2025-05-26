using Godot;
using Godot.Collections;
using System;

namespace TheRingGoesSouth.scripts.utils
{
    public class HighlightTileHelper : ILoggable
    {
        [Export]
        public bool DEBUG_TAG { get; set; } = false;
        [Export] public PackedScene HighlightTileScene { get; set; } = GD.Load<PackedScene>("res://scenes/utils/HighlightHexTile.tscn");
        [Export] public TileMapLayer MapLayer { get; set; } = null;
        [Export] public Vector2 offset = new Vector2(0, -0);
        [Export] public Color HighlightColor { get; set; } = new Color(1, 0.5f, 0, 0.5f);
        private Array<HighlightHexTile> _highlightedTiles = [];
        private Node _parentNode;

        public HighlightTileHelper(Node parent, TileMapLayer mapLayer)
        {
            _parentNode = parent;
            MapLayer = mapLayer;
            if (MapLayer == null)
            {
                GD.PrintErr("HighlightTileHelper: MapLayer is not set!");
            }
        }
        public void CreateHighlightInstanceAtMapCoord(Vector2I mapCoord)
        {
            if (HighlightTileScene == null)
            {
                GD.PrintErr("HighlightScene is not set in HighlightTileHelper!");
                return;
            }

            HighlightHexTile instance = HighlightTileScene.Instantiate<HighlightHexTile>();
            if (instance != null)
            {

                Vector2 tileWorldPosition = MapLayer.MapToLocal(mapCoord) + offset;
                instance.HighlightColor = HighlightColor;
                instance.GlobalPosition = tileWorldPosition;

                _parentNode.AddChild(instance);
                _highlightedTiles.Add(instance);
            }
        }

        public void HighlightCollection(Array<Vector2I> tileCollection)
        {
            foreach (Vector2I tile in tileCollection)
            {
                CreateHighlightInstanceAtMapCoord(tile);
            }
        }
        
        public void ClearHighlights()
        {
            foreach (HighlightHexTile highlight in _highlightedTiles)
            {
                highlight.QueueFree();
            }
            _highlightedTiles.Clear();
        }
    }
}