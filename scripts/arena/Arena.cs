using Godot;
using System.Linq;
using Godot.Collections;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.extra;
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
    [Export] private Array<LevelData> _levels;
    [Export] private MapController _mapController;
    [Export] private EnemySpawner _enemySpawner;
    [Export] private Label _totalCoins;
    [Export] private AudioStreamPlayer _coinSound;
    [Export] private Node2D _dungeon;
    [Export] private Label _levelTitle;
    
    public LevelRoom CurrentRoom;
    private readonly System.Collections.Generic.Dictionary<Vector2I, LevelRoom> _grid = new();
    private EventBus _eventBus;
    private Vector2I _startRoomCoord;
    private Vector2I _endRoomCoord;
    private Vector2I _storeRoomCoord;
    private Vector2I _gridCellSize;
    private Player _player;
    private LevelData  _levelData;
    private int _currentLevelIndex = 0;
    private int _currentSubLevel = 1;

    public override async void _Ready()
    {
        _eventBus = GetNode<EventBus>("/root/EventBus");
        Cursor.Instance.Sprite2D.Texture = _arenaCursor;

        _levelData = _levels[0];
        GenerateDungeon();
        
        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
        ShowLevelTitle();
        
        _eventBus.PlayerHealthUpdated += OnPlayerHealthUpdated;
        _eventBus.PlayerRoomEntered += OnPlayerRoomEntered;
        _eventBus.RoomCleared += OnRoomCleared;
        _eventBus.CoinPicked += OnCoinPicked;
        _eventBus.PortalReached += OnPortalReached;
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(_totalCoins))
        {
            _totalCoins.Text = $"{Global.Instance.Coins}";
        }
    
        if (IsInstanceValid(Global.Instance.PlayerRef))
        {
            _manaBar.Value = Global.Instance.PlayerRef.CurrentMana / Global.Instance.PlayerRef.Data.Magic;
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            CurrentRoom.UnlockRoom();
            CurrentRoom.IsCleared = true;
        }
    }

    private async void GenerateDungeon()
    {
        foreach (var child in _dungeon.GetChildren())
        {
            child.QueueFree();
        }

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        if (_player != null)
        {
            _player.QueueFree();
            Global.Instance.PlayerRef = null;
        }
        
        _mapController.Reset();
        foreach (var child in _mapController.GetChildren())
        {
            child.QueueFree();
        }
        
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
        
        GD.Print("Dungeon generation complete");
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
           _dungeon.AddChild(roomInstance);
            roomInstance.CreateProps(_levelData);
            
            // Link each coord with a room instance
            _grid[roomCoord] = roomInstance;

            if (roomCoord == _storeRoomCoord)
            {
                roomInstance.IsCleared = true;
                roomInstance.SetupRoomAsShop(_levelData);
            }

            if (roomCoord == _endRoomCoord)
            {
                roomInstance.IsCleared = true;
                roomInstance.SetUpRoomAsPortal();
            }
            
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
                _dungeon.AddChild(corridor);
            }
            
            // Create down connection
            var downNeighborCoord = roomCoord + Vector2I.Down;
            if (_grid.TryGetValue(downNeighborCoord, out var value1))
            {
                var corridor = (Node2D)_levelData.VCorridor.Instantiate();
                corridor.Position = roomInstance.Position + new Vector2(0, (float)(_gridCellSize.Y / 2.0));
                _dungeon.AddChild(corridor);
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
        
        var candidateCoords = _grid.Keys.ToList();
        candidateCoords.Remove(_startRoomCoord);
        candidateCoords.Remove(_endRoomCoord);

        if (candidateCoords.Count > 0)
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();

            _storeRoomCoord = candidateCoords[rng.RandiRange(0, candidateCoords.Count - 1)];
        }
        else
        {
            _storeRoomCoord = Vector2I.MaxValue;
            GD.Print("No shop coord.");
        }
    }

    private void ShowLevelTitle()
    {
        _levelTitle.Text = $"{_currentLevelIndex + 1}-{_currentSubLevel}";
        var tween = CreateTween();
        _levelTitle.SelfModulate = new Color(1f, 1f, 1f, 0f);
        tween.TweenProperty(_levelTitle, "self_modulate", new Color(1f, 1f, 1f, 1f), 1.0);
        tween.TweenInterval(1.0);
        tween.TweenProperty(_levelTitle, "self_modulate", new Color(1f, 1f, 1f, 0f), 1.0);
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
        var tilePosition = CurrentRoom.GetFreeSpawnPosition();
        var chestPosition = CurrentRoom.ToGlobal(tilePosition);
        var chestInstance = (Chest)Global.ChestScene.Instantiate();
        _dungeon.CallDeferred("add_child", chestInstance);
        chestInstance.GlobalPosition = chestPosition;
    }

    private void OnCoinPicked()
    {
        _coinSound.Play();
    }

    private async void OnPortalReached()
    {
        var tweenIn = Transition.Instance.ShowTransitionIn();
        await ToSignal(tweenIn, Tween.SignalName.Finished);

        if (_currentSubLevel < _levelData.NumSubLevels)
        {
            _currentSubLevel++;
            GenerateDungeon();
        }
        else
        {
            _currentLevelIndex++;
            if (_currentLevelIndex < _levels.Count)
            {
                _currentSubLevel = 1;
                _levelData = _levels[_currentLevelIndex];
                GenerateDungeon();
            }
            else
            {
                GD.Print("No more levels.");
                Transition.Instance.TransitionTo("uid://bdmo5icd2xpue");
            }
        }
        
        var tweenOut = Transition.Instance.ShowTransitionOut();
        await ToSignal(tweenOut, Tween.SignalName.Finished);
        ShowLevelTitle();
    }
}
