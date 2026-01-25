using Godot;
using Godot.Collections;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.enemy;
using TopDownGame.scripts.levels;
using TopDownGame.scripts.resources.data.level;

namespace TopDownGame.scripts.arena;

public partial class EnemySpawner : Node2D
{
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
            var randomScene = data.EnemyScenes.PickRandom();
            var enemy = (Enemy)randomScene.Instantiate();
            _enemies.Add(enemy);
            GetParent().AddChild(enemy);
            var spawnLocalPosition = room.GetFreeSpawnPosition();
            var spawnGlobalPosition = room.ToGlobal(spawnLocalPosition);
            enemy.GlobalPosition = spawnGlobalPosition;
        }

    }

    private void OnEnemyDied()
    {
        _enemiesKilled++;
        if (_enemiesKilled >= _enemies.Count)
        {
            EventBus.EmitRoomCleared();
            _enemies.Clear();
            _enemiesKilled = 0;
        }
    }
}