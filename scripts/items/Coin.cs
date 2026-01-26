using Godot;
using TopDownGame.scripts.autoloads;

namespace TopDownGame.scripts.items;

public partial class Coin : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        Global.Instance.Coins++;
        EventBus.EmitCoinPicked();
        QueueFree();
    }
}