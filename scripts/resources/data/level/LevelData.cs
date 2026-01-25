using Godot;
using Godot.Collections;

namespace TopDownGame.scripts.resources.data.level;

[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public int NumSubLevels = 4;
    [Export] public int NumRooms = 10;
    [Export] public Vector2I RoomSize = new Vector2I(384, 384);
    [Export] public PackedScene RoomScene;
    [Export] public PackedScene HCorridor;
    [Export] public PackedScene VCorridor;
    [Export] public Vector2I CorridorSize = new Vector2I(192, 192);
    [Export] public int MinEnemiesPerRoom = 5;
    [Export] public int MaxEnemiesPerRoom = 10;
    [Export] public int MaxPropsPerRoom = 5;
    [Export] public Array<PackedScene> Props;
    [Export] public Array<PackedScene> EnemyScenes;
    [Export] public Array<LevelStoreData> StoreData;
}