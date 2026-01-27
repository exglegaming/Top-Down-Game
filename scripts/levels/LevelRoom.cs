using Godot;
using Godot.Collections;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.items;
using TopDownGame.scripts.resources.data.items;
using TopDownGame.scripts.resources.data.level;

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
    [Export] public Array<Marker2D> ItemPositions { get; private set; }
    
    private Array<Vector2I> _tiles = [];
    private System.Collections.Generic.Dictionary<Vector2I, TileMapLayer> _roomWalls;
    private System.Collections.Generic.Dictionary<Vector2I, TileMapLayer> _clearDoorNodes;

    public bool IsCleared;
    
    public override void _Ready()
    {
        _roomWalls = new System.Collections.Generic.Dictionary<Vector2I, TileMapLayer>
        {
            [Vector2I.Up] = WallUp,
            [Vector2I.Right] = WallRight,
            [Vector2I.Down] = WallDown,
            [Vector2I.Left] = WallLeft
        };
        
        _clearDoorNodes = new System.Collections.Generic.Dictionary<Vector2I, TileMapLayer>
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

    public void CreateProps(LevelData data)
    {
        for (var i = 0; i < data.MaxPropsPerRoom; i++)
        {
            var tileCoord = _tiles.PickRandom();
            var tilePosition = TileData.MapToLocal(tileCoord);
            var randomProp = data.Props.PickRandom();
            var instance = (Area2D)randomProp.Instantiate();
            instance.Position = tilePosition;
            AddChild(instance);
        }
    }

    public Vector2 GetFreeSpawnPosition()
    {
        var tileCoord = _tiles.PickRandom();
        return TileData.MapToLocal(tileCoord);
    }
    
    public void SetupRoomAsShop(LevelData data)
    {
        if (data.StoreData.Count == 0) return;

        foreach (var itemPosition in ItemPositions)
        {
            var itemData = data.GetRandomStoreItem();
            var itemInstance = (StoreItem)Global.StoreItemScene.Instantiate();
            AddChild(itemInstance);
            itemInstance.GlobalPosition = itemPosition.GlobalPosition;
            itemInstance.Setup(itemData);
        }
    }
    
    private void RegisterTiles()
    {
        foreach (var tile in TileData.GetUsedCells())
        {
            _tiles.Add(tile);
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