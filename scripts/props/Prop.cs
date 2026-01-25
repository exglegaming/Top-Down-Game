using Godot;

namespace TopDownGame.scripts.props;

public partial class Prop : Area2D
{
    [ExportCategory("References")]
    [Export] private Sprite2D _sprite;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        var dampedOscillator = GetNode<GodotObject>("/root/DampedOscillator");
        dampedOscillator.Call("animate", _sprite, "scale", 250, 10, 17, 0.5 * GD.RandRange(0, 1));
    }
}