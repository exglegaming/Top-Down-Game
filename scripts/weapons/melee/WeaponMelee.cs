using Godot;
using TopDownGame.scripts.weapons;

namespace TopDownGame.scripts.weapons.melee;

public partial class WeaponMelee : Weapon
{
    private static readonly StringName Idle = "idle";
    private static readonly StringName Slash = "slash";
    private static readonly StringName Shoot = "shoot";

    [ExportCategory("References")]
    [Export] private Sprite2D _sprite2D;
    [Export] private GpuParticles2D _slashParticle;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private AudioStreamPlayer _slashSound;
    [Export] private Timer _cooldown;

    private bool _canUse = true;

    public override void _Ready()
    {
        _cooldown.WaitTime = Data.Cooldown;

        _cooldown.Timeout += OnCooldownTimeout;
    }

    public override void UseWeapon()
    {
        if (!_canUse) return;

        _canUse = false;
        _cooldown.Start();
        _slashSound.Play();
        _animationPlayer.Play(Slash);

        _slashParticle.GlobalRotation = Pivot.GlobalRotation;
        _slashParticle.Emitting = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(Shoot)) UseWeapon();
    }

    private void OnCooldownTimeout()
    {
        _canUse = true;
        _animationPlayer.Play(Idle);
    }
}
