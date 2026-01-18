using Godot;

namespace TopDownGame.scripts.resources.data.level;

[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public int NumSubLevels = 4;
    [Export] public int NumRooms = 10;
    [Export] public Vector2I RoomSize = new Vector2I(384, 384);
    [Export] public PackedScene RoomScene;
    [Export] public int MinEnemiesPerRoom = 5;
    [Export] public int MaxEnemiesPerRoom = 10;
    [Export] public PackedScene[] EnemyScenes;
    [Export] public LevelStoreData[] StoreData;
}