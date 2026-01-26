using Godot;
using Godot.Collections;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.enemy;
using TopDownGame.scripts.levels;
using TopDownGame.scripts.resources.data.level;

namespace TopDownGame.scripts.arena;

public partial class EnemySpawner : Node2D
{
    [ExportCategory("References")]
    [Export] private Arena _arena;
    
    private Array<Enemy> _enemies = [];
    private int _enemiesKilled;

    public override void _Ready()
    {
        EventBus.Instance.EnemyDied += OnEnemyDied;
    }

    public async void SpawnEnemies(LevelData data, LevelRoom room)
    {
        if (data.EnemyScenes == null) return;

        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        
        var amount = GD.RandRange(data.MinEnemiesPerRoom, data.MaxEnemiesPerRoom);
        for (var i = 0; i < amount; i++)
        {
            var spawnLocalPosition = room.GetFreeSpawnPosition();
            var spawnGlobalPosition = room.ToGlobal(spawnLocalPosition);

            var marker = (Node2D)Global.SpawnMarkerScene.Instantiate();
            marker.GlobalPosition = spawnGlobalPosition;
            GetParent().AddChild(marker);
            var animPlayer = marker.GetNode<AnimationPlayer>("AnimationPlayer");
            await ToSignal(animPlayer, AnimationPlayer.SignalName.AnimationFinished);
            
            var randomScene = data.EnemyScenes.PickRandom();
            var enemy = (Enemy)randomScene.Instantiate();
            GetParent().AddChild(enemy);
            enemy.ParentRoom = _arena.CurrentRoom;
            enemy.GlobalPosition = spawnGlobalPosition;
            _enemies.Add(enemy);
        }
    }

    private void OnEnemyDied()
    {
        _enemiesKilled++;
        GD.Print($"Enemies killed: {_enemiesKilled}");
        if (_enemiesKilled >= _enemies.Count)
        {
            EventBus.EmitRoomCleared();
            _enemies.Clear();
            _enemiesKilled = 0;
        }
    }
}