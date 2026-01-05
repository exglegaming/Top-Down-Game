using Godot;

namespace TopDownGame.scripts.weapons.ranged;

public partial class WeaponRange : Weapon
{
    private Vector2 _direction;
    private Sprite2D _sprite2D;

    public override void _Ready()
    {
        _sprite2D = GetNode<Sprite2D>("%Sprite2D");
    }

    public override void _Process(double delta)
    {
        RotateWeapon();
    }

    private void RotateWeapon()
    {
        _direction = GetGlobalMousePosition() - GlobalPosition;
        _sprite2D.FlipV = _direction.X < 0;
    }
}
