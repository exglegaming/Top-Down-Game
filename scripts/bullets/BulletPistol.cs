using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.enemy;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.bullets;

public partial class BulletPistol : Area2D
{
    private WeaponData _data;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        if (_data == null) return;
        MoveLocalX(_data.BulletSpeed * (float)delta);
    }

    public void Setup(WeaponData data)
    {
        _data = data;
    }

    private void OnBodyEntered(Node2D body)
    {
        Global.Instance.CreateExplosion(GlobalPosition);
        
        if (body is Enemy enemy)
        {
            Global.Instance.CreateDamageText(_data.Damage, body.GlobalPosition);
            enemy.HealthComponent.TakeDamage(_data.Damage);
        }
        QueueFree();
    }
}
