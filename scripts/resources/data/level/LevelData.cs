using System.Linq;
using Godot;
using Godot.Collections;

using TopDownGame.scripts.resources.data.items;

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

    public ItemData GetRandomStoreItem()
    {
        var rng = new RandomNumberGenerator();
        rng.Randomize();

        var weights = StoreData.Select(entry => entry.ItemProbability).ToArray();
        
        var index = (int)rng.RandWeighted(weights);
        return StoreData[index].ItemData as ItemData;
    }
}