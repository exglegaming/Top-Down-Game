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
    
    private readonly Dictionary<Vector2I, LevelRoom> _grid = new();
    private EventBus _eventBus;
    private Vector2I _startRoomCoord;
    private Vector2I _endRoomCoord;

    public override void _Ready()
    {
        _eventBus = GetNode<EventBus>("/root/EventBus");

        Cursor.Instance.Sprite2D.Texture = _arenaCursor;

        GenerateLevelLayout();
        SelectSpecialRooms();

        LoadGameSelection();

        _eventBus.PlayerHealthUpdated += OnPlayerHealthUpdated;
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

            var attempts = 0;
            while (_grid.ContainsKey(nextCoord) && attempts < 10)
            {
                randomDirection = directions[GD.RandRange(0, directions.Length - 1)];
                nextCoord = currentCoord + randomDirection;
                attempts++;
            }

            _grid.TryAdd(nextCoord, null);
        }
        
        foreach (var key in _grid.Keys)
        {
            GD.Print(key);
        }
    }

    private void SelectSpecialRooms()
    {
        
    }

    private void LoadGameSelection()
    {
        var player = Global.Instance.GetPlayer().Instantiate() as Player;
        AddChild(player);
        player?.WeaponController.EquipWeapon();
    }

    private void OnPlayerHealthUpdated(float currentHealth, float maxHealth)
    {
        _healthBar.Value = currentHealth / maxHealth;
    }
}
