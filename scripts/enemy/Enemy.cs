using System.Linq;
using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.components;
using TopDownGame.scripts.levels;
using TopDownGame.scripts.player;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.enemy;

public partial class Enemy : CharacterBody2D
{
    private enum EnemyType
    {
        Chase,
        Weapon
    }
    
    private enum EnemyStates
    {
        FindingDestination,
        Moving,
        Attacking
    }
    
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

    [ExportCategory("States")] 
    [Export] private EnemyType _enemyType = EnemyType.Chase;
     
    public LevelRoom ParentRoom;

    private static readonly StringName Hurt = "hurt";
    private static readonly StringName Move = "move";
    private bool _canMove = true;
    private bool _isKilled = false;
    private float _cooldown;
    private EnemyStates _enemyState;
    private Vector2 _moveDestination;

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
        if (_enemyState == EnemyStates.Attacking) ManageWeapon((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Global.Instance.PlayerRef == null) return;
        if (!_canMove) return;

        switch (_enemyType)
        {
            case EnemyType.Chase:
                RunEnemyChase();
                break;
            case EnemyType.Weapon:
                RunEnemyWeapon();
                break;
        }
    }

    private async void RunEnemyWeapon()
    {
        switch (_enemyState)
        {
            case EnemyStates.FindingDestination:
                var localPosition = ParentRoom.GetFreeSpawnPosition();
                _moveDestination = ParentRoom.ToGlobal(localPosition);
                _enemyState = EnemyStates.Moving;
                break;
            case EnemyStates.Moving:
                var direction = GlobalPosition.DirectionTo(_moveDestination);
                Velocity = direction * _moveSpeed;
                MoveAndSlide();
                if (GlobalPosition.DistanceTo(_moveDestination) < 2.0)
                {
                    Velocity = Vector2.Zero;
                    _enemyState = EnemyStates.Attacking;
                }
                break;
            case EnemyStates.Attacking:
                Velocity = Vector2.Zero;
                MoveAndSlide();
                await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
                _enemyState = EnemyStates.FindingDestination;
                break;
        }
    }

    private void RunEnemyChase()
    {
        var direction = GlobalPosition.DirectionTo(Global.Instance.PlayerRef.GlobalPosition);
        direction = (from Enemy enemy in _enemyDetector.GetOverlappingBodies() where enemy != this && enemy.IsInsideTree() 
            select GlobalPosition - enemy.GlobalPosition).Aggregate(direction, (current, vector) => current + 10 * vector.Normalized() / vector.Length());

        Velocity = direction * _chaseSpeed;
        MoveAndSlide();
    }

    private void ManageWeapon(float delta)
    {
        if (_weapon == null) return;
        if (_weaponController == null) return;
        _weaponController.TargetPosition = Global.Instance.PlayerRef.GlobalPosition;
        _weaponController.RotateWeapon();
        
        _cooldown -= delta;
        if (_cooldown <= 0.0f)
        {
            _weaponController.CurrentWeapon.UseWeapon();
            _cooldown = _weaponController.CurrentWeapon.Data.Cooldown;
        }
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
        if (body is Player player)
        {
            player.HealthComponent.TakeDamage(_collisionDamage);
        }
        EnemyDead();
    }

    private async void OnHealthComponentOnUnitDamaged(float amount)
    {
        _healthBar.Value = HealthComponent.CurrentHealth / _maxHealth;
        _animSprite.Material = Global.HitMaterial;
        _animSprite.Play(Hurt);
        await ToSignal(GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);
        _animSprite.Material = null;
        _animSprite.Play(Move);
    }

    private void OnHealthComponentOnUnitDead()
    {
        EnemyDead();
    }
}