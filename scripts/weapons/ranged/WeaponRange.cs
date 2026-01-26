using Godot;
using TopDownGame.scripts.bullets;

namespace TopDownGame.scripts.weapons.ranged;

public partial class WeaponRange : Weapon
{
    [Export] private Sprite2D _sprite2D;
    [Export] private Marker2D _firePosition;

    private Vector2 _direction;

    public override void _Process(double delta)
    {
        RotateWeapon();
    }

    public override void UseWeapon()
    {
        var bullet = (BulletPistol)Data.BulletScene.Instantiate();
        bullet.Setup(Data);
        bullet.GlobalPosition = _firePosition.GlobalPosition;
        bullet.GlobalRotation = Pivot.GlobalRotation + Mathf.DegToRad((float)GD.RandRange(-Data.Spread, Data.Spread));
        GetTree().Root.AddChild(bullet);
    }

    private void RotateWeapon()
    {
        _direction = GetGlobalMousePosition() - GlobalPosition;
        _sprite2D.FlipV = _direction.X < 0;
    }
}
