using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.components;
using TopDownGame.scripts.resources.data.player;

namespace TopDownGame.scripts.player;

public partial class Player : CharacterBody2D
{
    [ExportCategory("References")]
    [Export] public PlayerData Data;

    private HealthComponent _healthComponent;

    private bool _canMove = true;
    private Vector2 _movement;
    private Vector2 _direction;
    private Node2D _visuals;
    private AnimatedSprite2D _animSprite;

    public override void _Ready()
    {
        _visuals = GetNode<Node2D>("Visuals");
        _animSprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        _healthComponent = GetNode<HealthComponent>("HealthComponent");

        _healthComponent.InitHealth(Data.MaxHP);

        _healthComponent.OnUnitDamaged += OnHealthComponentOnUnitDamaged;
        _healthComponent.OnUnitDead += OnHealthComponentOnUnitDead;
        _healthComponent.OnUnitHealed += OnHealthComponentOnUnitHealed;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_canMove) return;

        _direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (_direction != Vector2.Zero)
        {
            _movement = _direction * Data.MoveSpeed;
            _animSprite.Play("move");
        }
        else
        {
            _movement = Vector2.Zero;
            _animSprite.Play("idle");
        }

        Velocity = _movement;
        RotatePlayer();
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
        {
            _healthComponent.TakeDamage(1);
        }
    }

    private void RotatePlayer()
    {
        if (_direction != Vector2.Zero && _direction.X >= 0.1) _visuals.Scale = new Vector2(1.25f, 1.25f);
        else if (_direction != Vector2.Zero && _direction.X <= -0.1) _visuals.Scale = new Vector2(-1.25f, 1.25f);
    }

    private void OnHealthComponentOnUnitDamaged(float amount)
    {
        EventBus.EmitPlayerHealthUpdated(_healthComponent.CurrentHealth, Data.MaxHP);
    }

    private void OnHealthComponentOnUnitDead()
    {
        QueueFree();
    }

    private void OnHealthComponentOnUnitHealed(float amount)
    {
        
    }
}
