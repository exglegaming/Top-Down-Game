using Godot;
using TopDownGame.scripts.weapons;
using TopDownGame.scripts.weapons.ranged;

namespace TopDownGame.scripts.player;

public partial class WeaponController : Node2D
{
    private Weapon _currentWeapon;
    private Vector2 _targetPosition;

    private WeaponRange _weaponRangePistol;

    public override void _Ready()
    {
        _weaponRangePistol = GetNode<WeaponRange>("WeaponRangePistol");
        _currentWeapon = _weaponRangePistol;
    }

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
