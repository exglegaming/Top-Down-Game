using Godot;

namespace TopDownGame.scripts.resources.data.level;

[GlobalClass]
public partial class LevelStoreData : Resource
{
    [Export] public Resource ItemData;
    [Export] public float ItemProbability;
}