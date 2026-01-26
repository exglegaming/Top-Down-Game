using Godot;
using System.Collections.Generic;
using System.Linq;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.levels;
using TopDownGame.scripts.player;
using TopDownGame.scripts.resources.data.level;

namespace TopDownGame.scripts.arena;

public partial class Arena : Node2D
{
    [ExportCategory("References")]
    [Export] private Texture2D _arenaCursor;
    [Export] private TextureProgressBar _healthBar;
    [Export] private TextureProgressBar _manaBar;
    [Export] private LevelData _levelData;
    [Export] private MapController _mapController;
    [Export] private EnemySpawner _enemySpawner;
    [Export] private Label _totalCoins;
    [Export] private AudioStreamPlayer _coinSound;
    
    public LevelRoom CurrentRoom;
    private readonly Dictionary<Vector2I, LevelRoom> _grid = new();
    private EventBus _eventBus;
    private Vector2I _startRoomCoord;
    private Vector2I _endRoomCoord;
    private Vector2I _gridCellSize;
    private Player _player;

    public override void _Ready()
    {
        _eventBus = GetNode<EventBus>("/root/EventBus");

        Cursor.Instance.Sprite2D.Texture = _arenaCursor;

        _gridCellSize = new Vector2I(
            _levelData.RoomSize.X + _levelData.CorridorSize.X,
            _levelData.RoomSize.Y + _levelData.CorridorSize.Y
            );

        GenerateLevelLayout();
        SelectSpecialRooms();
        CreateRooms();
        CreateCorridors();
        LoadGameSelection();
        
        var firstRoom = _grid[Vector2I.Zero];
        firstRoom.IsCleared = true;

        _eventBus.PlayerHealthUpdated += OnPlayerHealthUpdated;
        _eventBus.PlayerRoomEntered += OnPlayerRoomEntered;
        _eventBus.RoomCleared += OnRoomCleared;
        _eventBus.CoinPicked += OnCoinPicked;
    }

    public override void _Process(double delta)
    {
        _totalCoins.Text = $"{Global.Instance.Coins}";
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            CurrentRoom.UnlockRoom();
            CurrentRoom.IsCleared = true;
        }
    }

    private void GenerateLevelLayout()
    {
        _grid.Clear();
        
        GD.Print("Creating layout...");
        var currentCoord = Vector2I.Zero;
        _grid[currentCoord] = null;
        var directions = new[] { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };

        while (_grid.Count < _levelData.NumRooms)
        {
            if (GD.Randf() > 0.5)
            {
                var keys = _grid.Keys.ToArray();
                currentCoord = keys[GD.RandRange(0, keys.Length - 1)];
            }
            
            var randomDirection = directions[GD.RandRange(0, directions.Length - 1)];
            var nextCoord = currentCoord + randomDirection;

            // If nextCoord exists, find a new one
            var attempts = 0;
            while (_grid.ContainsKey(nextCoord) && attempts < 10)
            {
                randomDirection = directions[GD.RandRange(0, directions.Length - 1)];
                nextCoord = currentCoord + randomDirection;
                attempts++;
            }

            // If nextCoord is valid, add to grid
            _grid.TryAdd(nextCoord, null);
        }
        
        foreach (var key in _grid.Keys)
        {
            GD.Print(key);
        }
    }

    private void CreateRooms()
    {
        GD.Print("Creating rooms...");
        foreach (var roomCoord in _grid.Keys)
        {
            var roomInstance = (LevelRoom)_levelData.RoomScene.Instantiate();
            roomInstance.Position = roomCoord * _gridCellSize;
            AddChild(roomInstance);
            roomInstance.CreateProps(_levelData);
            
            // Link each coord with a room instance
            _grid[roomCoord] = roomInstance;
            
            // Connect rooms using directions
            ConnectRooms(roomCoord, roomInstance);
        }
    }

    private void CreateCorridors()
    {
        GD.Print("Creating corridors...");
        foreach (var roomCoord in _grid.Keys)
        {
            var roomInstance = _grid[roomCoord];
            
            // Create right connection
            var rightNeighborCoord = roomCoord + Vector2I.Right;
            if (_grid.TryGetValue(rightNeighborCoord, out var value))
            {
                var corridor = (Node2D)_levelData.HCorridor.Instantiate();
                corridor.Position = roomInstance.Position + new Vector2((float)(_gridCellSize.X / 2.0), 0);
                AddChild(corridor);
            }
            
            // Create down connection
            var downNeighborCoord = roomCoord + Vector2I.Down;
            if (_grid.TryGetValue(downNeighborCoord, out var value1))
            {
                var corridor = (Node2D)_levelData.VCorridor.Instantiate();
                corridor.Position = roomInstance.Position + new Vector2(0, (float)(_gridCellSize.Y / 2.0));
                AddChild(corridor);
            }
        }
    }

    private void ConnectRooms(Vector2I roomCoord, LevelRoom roomInstance)
    {
        var directions = new[] { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };
        foreach (var direction in directions)
        {
            var neighborCoord = roomCoord + direction;
            if (_grid.ContainsKey(neighborCoord))
            {
                roomInstance.OpenWall(direction);
            }
        }
    }

    private void SelectSpecialRooms()
    {
        _startRoomCoord = Vector2I.Zero;
        _endRoomCoord = FindFarthestRoom();
        GD.Print($"Start room coordinate: {_startRoomCoord}");
        GD.Print($"End room coordinate: {_endRoomCoord}");
    }

    private Vector2I FindFarthestRoom()
    {
        var farthestRoomCoord = _startRoomCoord;
        var maxDistance = 0.0;
        foreach (var roomCoord in _grid.Keys)
        {
            var distance = _startRoomCoord.DistanceTo(roomCoord);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoomCoord = roomCoord;
            }
        }
        return farthestRoomCoord;
    }

    private void LoadGameSelection()
    {
        _player = (Player)Global.Instance.GetPlayer().Instantiate();
        var firstRoom = _grid[Vector2I.Zero];
        var spawnPosition = firstRoom.PlayerSpawnPosition;
        AddChild(_player);
        _player.GlobalPosition = spawnPosition.GlobalPosition;
        _player.WeaponController.EquipWeapon(Global.Instance.SelectedWeapon);
        Global.Instance.PlayerRef = _player;
    }

    private Vector2I FindCoordFromRoom(LevelRoom room)
    {
        foreach (var coord in _grid.Keys)
        {
            if (_grid[coord] == room)
            {
                return coord;
            }
        }
        return Vector2I.MaxValue;
    }

    private void OnPlayerHealthUpdated(float currentHealth, float maxHealth)
    {
        _healthBar.Value = currentHealth / maxHealth;
    }

    private void OnPlayerRoomEntered(LevelRoom room)
    {
        if (room != CurrentRoom)
        {
            CurrentRoom = room;
            
            var absoluteCoord = FindCoordFromRoom(room);
            var relativeCoord = absoluteCoord - _startRoomCoord;
            _mapController.UpdateOnRoomEnter(relativeCoord);
            
            if (!room.IsCleared)
            {
                room.LockRoom();
                _enemySpawner.SpawnEnemies(_levelData, room);
            }
        }
    }

    private void OnRoomCleared()
    {
        CurrentRoom.UnlockRoom();
        CurrentRoom.IsCleared = true;
    }

    private void OnCoinPicked()
    {
        _coinSound.Play();
    }
}
