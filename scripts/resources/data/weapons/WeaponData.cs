using Godot;

namespace TopDownGame.scripts.resources.data.weapons;

[GlobalClass]
public partial class WeaponData : Resource
{
    [Export] public string WeaponName { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    [Export] public float Damage { get; private set; }
    [Export] public float Cooldown { get; private set; }
    [Export] public float ManaCost { get; private set; }
    [Export] public float Spread { get; private set; }
    [Export] public float BulletSpeed { get; private set; }
    [Export] public PackedScene BulletScene { get; private set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; private set; }
}
