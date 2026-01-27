using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.components;
using TopDownGame.scripts.resources.data.player;

namespace TopDownGame.scripts.player;

public partial class Player : CharacterBody2D
{
    private static readonly StringName Shoot = "shoot";
    
    [ExportCategory("References")]
    [Export] private Node2D _visuals;
    [Export] private AnimatedSprite2D _animSprite;
    [Export] public HealthComponent HealthComponent;
    [Export] public WeaponController WeaponController;
    [Export] public PlayerData Data;

    public float CurrentMana;
    
    private bool _canMove = true;
    private Vector2 _movement;
    private Vector2 _direction;
    private float _cooldown;
    
    public override void _Ready()
    {
        CurrentMana = Data.Magic;
        HealthComponent.InitHealth(Data.MaxHp);

        HealthComponent.OnUnitDamaged += OnHealthComponentOnUnitDamaged;
        HealthComponent.OnUnitDead += OnHealthComponentOnUnitDead;
        HealthComponent.OnUnitHealed += OnHealthComponentOnUnitHealed;
    }

    public override void _Process(double delta)
    {
        WeaponController.TargetPosition = GetGlobalMousePosition();
        WeaponController.RotateWeapon();
        
        _cooldown -= (float)delta;
        if (Input.IsActionPressed(Shoot) && CurrentMana >= WeaponController.CurrentWeapon.Data.ManaCost)
        {
            if (_cooldown <= 0)
            {
                WeaponController.CurrentWeapon.UseWeapon();
                _cooldown = WeaponController.CurrentWeapon.Data.Cooldown;
                UseMana(WeaponController.CurrentWeapon.Data.ManaCost);
            }
        }
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

    private void RotatePlayer()
    {
        if (_direction != Vector2.Zero && _direction.X >= 0.1) _visuals.Scale = new Vector2(1.25f, 1.25f);
        else if (_direction != Vector2.Zero && _direction.X <= -0.1) _visuals.Scale = new Vector2(-1.25f, 1.25f);
    }

    private void UseMana(float value)
    {
        if (CurrentMana < value) return;
        CurrentMana -= value;
    }

    private void OnHealthComponentOnUnitDamaged(float amount)
    {
        EventBus.EmitPlayerHealthUpdated(HealthComponent.CurrentHealth, Data.MaxHp);
    }

    private void OnHealthComponentOnUnitDead()
    {
        QueueFree();
    }

    private void OnHealthComponentOnUnitHealed(float amount)
    {
        EventBus.EmitPlayerHealthUpdated(HealthComponent.CurrentHealth, Data.MaxHp);
    }
}
