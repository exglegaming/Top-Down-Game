using System.Collections.Generic;
using Godot;

namespace TopDownGame.scripts.levels;

public partial class LevelRoom : Node2D
{
    [ExportCategory("References")]
    [Export] public TileMapLayer WallUp { get; private set; }
    [Export] public TileMapLayer WallRight { get; private set; }
    [Export] public TileMapLayer WallDown { get; private set; }
    [Export] public TileMapLayer WallLeft { get; private set; }
    
    private Dictionary<Vector2I, TileMapLayer> _roomWalls;

    public override void _Ready()
    {
        _roomWalls = new Dictionary<Vector2I, TileMapLayer>
        {
            [Vector2I.Up] = WallUp,
            [Vector2I.Right] = WallRight,
            [Vector2I.Down] = WallDown,
            [Vector2I.Left] = WallLeft
        };
        
        CloseAllWalls();
    }

    public void OpenWall(Vector2I direction)
    {
        if (_roomWalls.TryGetValue(direction, out var value))
        {
            value.Enabled = false;
        }
    }

    private void CloseAllWalls()
    {
        foreach (var key in _roomWalls.Keys)
        {
            _roomWalls[key].Enabled = true;
        }
    }
}