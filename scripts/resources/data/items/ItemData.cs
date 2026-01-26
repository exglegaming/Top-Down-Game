using Godot;

namespace TopDownGame.scripts.resources.data.items;

[GlobalClass]
public partial class ItemData : Resource
{
    public enum ItemRarity
    {
        Common,
        Rare,
        Epic
    }
    
    [Export] public Texture2D Icon { get; private set; }
    [Export] public string Id { get; private set; }
    [Export] public string Name { get; private set; }
    [Export] public float Value { get; private set; }
    [Export] public float Price { get; private set; }
    [Export] public ItemRarity Rarity { get; private set; } =  ItemRarity.Common;
    [Export(PropertyHint.MultilineText)] public string Description { get; private set; }
}