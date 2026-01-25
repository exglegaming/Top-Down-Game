using Godot;
using TopDownGame.scripts.autoloads;

namespace TopDownGame.scripts.enemy;

public partial class Enemy : CharacterBody2D
{
    [ExportCategory("References")]
    [Export] private AnimatedSprite2D _animSprite;
    [Export] private Area2D _playerDetector;
    [Export] private AudioStreamPlayer _hurtSound;
    
    private bool _canMove = true;

    public override void _Ready()
    {
        _playerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Global.Instance.PlayerRef == null) return;
        if (!_canMove) return;
        
        var direction = GlobalPosition.DirectionTo(Global.Instance.PlayerRef.GlobalPosition);
        Velocity = direction * 50;
        MoveAndSlide();
        RotateEnemy();
    }

    private void RotateEnemy()
    {
        if (GlobalPosition.X > Global.Instance.PlayerRef.GlobalPosition.X)
        {
            _animSprite.FlipH = true;
        }
        else if (GlobalPosition.X < Global.Instance.PlayerRef.GlobalPosition.X)
        {
            _animSprite.FlipH = false;
        }
    }

    private async void OnPlayerDetectorBodyEntered(Node2D body)
    {
        _canMove = false;
        _animSprite.Play("die");
        await ToSignal(_animSprite, AnimatedSprite2D.SignalName.AnimationFinished);
        EventBus.EmitEnemyDied();
        QueueFree();
    }
}