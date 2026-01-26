using System.Linq;
using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.components;
using TopDownGame.scripts.player;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.enemy;

public partial class Enemy : CharacterBody2D
{
    [ExportCategory("References")]
    [Export] private AnimatedSprite2D _animSprite;
    [Export] private Area2D _playerDetector;
    [Export] private Area2D _enemyDetector;
    [Export] private AudioStreamPlayer _hurtSound;
    [Export] private ProgressBar _healthBar;
    [Export] public HealthComponent HealthComponent { get; private set; }
    [Export] private Texture2D _deadTexture;
    [Export] private WeaponController _weaponController;
    
    [ExportCategory("EnemyData")]
    [Export] private float _maxHealth = 5.0f;
    [Export] private float _collisionDamage = 2.0f;
    [ExportGroup("EnemyChase")]
    [Export] private float _chaseSpeed = 60.0f;
    [ExportGroup("EnemyWeapon")]
    [Export] private float _moveSpeed = 40.0f;
    [Export] private WeaponData _weapon;
     
    private bool _canMove = true;
    private bool _isKilled = false;

    public override void _Ready()
    {
        _healthBar.Value = 1f;
        HealthComponent.InitHealth(_maxHealth);

        if (_weapon == null) return;
        _weaponController.EquipWeapon(_weapon);
        
        _playerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
        HealthComponent.OnUnitDamaged += OnHealthComponentOnUnitDamaged;
        HealthComponent.OnUnitDead += OnHealthComponentOnUnitDead;
    }

    public override void _Process(double delta)
    {
        if (Global.Instance.PlayerRef == null) return;
        RotateEnemy();
        ManageWeapon();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Global.Instance.PlayerRef == null) return;
        if (!_canMove) return;
        
        var direction = GlobalPosition.DirectionTo(Global.Instance.PlayerRef.GlobalPosition);
        direction = (from Enemy enemy in _enemyDetector.GetOverlappingBodies() where enemy != this && enemy.IsInsideTree() 
            select GlobalPosition - enemy.GlobalPosition).Aggregate(direction, (current, vector) => current + 10 * vector.Normalized() / vector.Length());

        Velocity = direction * _chaseSpeed;
        MoveAndSlide();
        RotateEnemy();
    }

    private void ManageWeapon()
    {
        if (_weapon == null) return;
        if (_weaponController == null) return;
        _weaponController.TargetPosition = Global.Instance.PlayerRef.GlobalPosition;
        _weaponController.RotateWeapon();
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

    private void EnemyDead()
    {
        if (_isKilled) return;
        
        _canMove = false;
        _isKilled = true;
        Global.Instance.CreateDeadParticle(_deadTexture, GlobalPosition);
        EventBus.EmitEnemyDied();
        QueueFree();
    }

    private void OnPlayerDetectorBodyEntered(Node2D body)
    {
        EnemyDead();
    }

    private async void OnHealthComponentOnUnitDamaged(float amount)
    {
        _healthBar.Value = HealthComponent.CurrentHealth / _maxHealth;
        _animSprite.Material = Global.HitMaterial;
        await ToSignal(GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);
        _animSprite.Material = null;
    }

    private void OnHealthComponentOnUnitDead()
    {
        EnemyDead();
    }
}