using System.Collections.Generic;
using Godot;
using TopDownGame.scripts.autoloads;

namespace TopDownGame.scripts.levels;

public partial class LevelRoom : Node2D
{
    [ExportCategory("References")]
    [Export] public TileMapLayer WallUp { get; private set; }
    [Export] public TileMapLayer WallRight { get; private set; }
    [Export] public TileMapLayer WallDown { get; private set; }
    [Export] public TileMapLayer WallLeft { get; private set; }
    [Export] public TileMapLayer DoorUp { get; private set; }
    [Export] public TileMapLayer DoorRight { get; private set; }
    [Export] public TileMapLayer DoorDown { get; private set; }
    [Export] public TileMapLayer DoorLeft { get; private set; }
    [Export] public Marker2D PlayerSpawnPosition { get; private set; }
    [Export] public TileMapLayer TileData { get; private set; }
    [Export] public Area2D PlayerDetector { get; private set; }
    
    private readonly List<Vector2I> _tiles = [];
    private Dictionary<Vector2I, TileMapLayer> _roomWalls;
    private Dictionary<Vector2I, TileMapLayer> _clearDoorNodes;

    public bool IsCleared;
    
    public override void _Ready()
    {
        _roomWalls = new Dictionary<Vector2I, TileMapLayer>
        {
            [Vector2I.Up] = WallUp,
            [Vector2I.Right] = WallRight,
            [Vector2I.Down] = WallDown,
            [Vector2I.Left] = WallLeft
        };
        
        _clearDoorNodes = new Dictionary<Vector2I, TileMapLayer>
        {
            [Vector2I.Up] = DoorUp,
            [Vector2I.Right] = DoorRight,
            [Vector2I.Down] = DoorDown,
            [Vector2I.Left] = DoorLeft
        };
        
        CloseAllWalls();
        RegisterTiles();

        PlayerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
    }

    public void RegisterTiles()
    {
        foreach (var tile in TileData.GetUsedCells())
        {
            _tiles.Add(tile);
        }
    }

    public void UnlockRoom()
    {
        foreach (var direction in _clearDoorNodes.Keys)
        {
            _clearDoorNodes[direction].Enabled = false;
        }
    }

    public void OpenWall(Vector2I direction)
    {
        if (_roomWalls.TryGetValue(direction, out var value))
        {
            value.Enabled = false;
        }
    }
    
    public void LockRoom()
    {
        foreach (var direction in _clearDoorNodes.Keys)
        {
            var wallDoor = _roomWalls[direction];
            var clearDoor = _clearDoorNodes[direction];

            if (IsInstanceValid(wallDoor) && !wallDoor.Enabled)
            {
                clearDoor.Enabled = true;
            }
        }
    }

    private void CloseAllWalls()
    {
        foreach (var key in _roomWalls.Keys)
        {
            _roomWalls[key].Enabled = true;
        }
    }

    private void OnPlayerDetectorBodyEntered(Node2D body)
    {
        EventBus.EmitPlayerRoomEntered(this);
    }
}