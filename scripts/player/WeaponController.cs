using Godot;
using TopDownGame.scripts.weapons;

namespace TopDownGame.scripts.player;

public partial class WeaponController : Node2D
{
    private Weapon _currentWeapon;
    private Vector2 _targetPosition;

    public override void _Process(double delta)
    {
        _targetPosition = GetGlobalMousePosition();
        RotateWeapon();
    }

    private void RotateWeapon()
    {
        if (_currentWeapon == null) return;

        _currentWeapon.LookAt(_targetPosition);
    }
}
