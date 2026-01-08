using Godot;
using TopDownGame.scripts.autoloads;
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

    public void EquipWeapon()
    {
        var weapon = Global.Instance.GetWeapon().Instantiate() as Weapon;
        weapon.GlobalPosition = new Vector2(0, -8); // Need to verify this works correctly
        _currentWeapon = weapon;
        _currentWeapon.Data = Global.Instance.SelectedWeapon;
        AddChild(weapon);
    }

    private void RotateWeapon()
    {
        if (_currentWeapon == null) return;

        _currentWeapon.LookAt(_targetPosition);
    }
}
