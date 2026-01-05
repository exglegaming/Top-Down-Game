using Godot;

namespace TopDownGame.scripts.resources.data.player;

[GlobalClass]
public partial class PlayerData : Resource
{
    [Export] public Texture Icon { get; private set; }
    [Export] public string ID { get; private set; }
    [Export] public float MaxHP { get; private set; }
    [Export] public float MoveSpeed { get; private set; }
    [Export] public float Magic { get; private set; }
}
