using Godot;

namespace TopDownGame.scripts.resources.data.player;

[GlobalClass]
public partial class PlayerData : Resource
{
    [Export] public Texture2D Icon { get; private set; }
    [Export] public string Id { get; private set; }
    [Export] public float MaxHp { get; private set; }
    [Export] public float MoveSpeed { get; private set; }
    [Export] public float Magic { get; private set; }
}
