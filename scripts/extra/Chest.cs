using Godot;
using TopDownGame.scripts.items;

namespace TopDownGame.scripts.extra;

public partial class Chest : StaticBody2D
{
    private static readonly PackedScene CoinScene = GD.Load<PackedScene>("uid://cs1h3fiy3gjo1");
    
    [ExportCategory("References")] 
    [Export] private Sprite2D _chestClose;
    [Export] private Sprite2D _chestOpen;
    [Export] private Marker2D _dropPosition;
    [Export] private AudioStreamPlayer _chestSound;
    [Export] private Area2D _area2D;
    
    [ExportGroup("Coins")]
    [Export] private int _coinAmount = 5;
    
    private bool _collected = false;

    public override void _Ready()
    {
        _area2D.BodyEntered += OnArea2dBodyEntered;
    }

    private void OnArea2dBodyEntered(Node2D body)
    {
        if (_collected) return;
        
        _chestClose.Hide();
        _chestOpen.Show();
        _chestSound.Play();

        for (var i = 0; i < _coinAmount; i++)
        {
            var coin = (Coin)CoinScene.Instantiate();
            GetTree().Root.CallDeferred("add_child", coin);
            var position = _dropPosition.GlobalPosition;
            coin.GlobalPosition = new Vector2((float)GD.RandRange(position.X - 30, position.X + 30), position.Y);
        }
        _collected = true;
    }
}