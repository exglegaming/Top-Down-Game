using Godot;
using TopDownGame.scripts.autoloads;

namespace TopDownGame.scripts.extra;

public partial class Portal : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        EventBus.EmitPortalReached();
    }
}

