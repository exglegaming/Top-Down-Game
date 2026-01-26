using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.resources.data.weapons;
using TopDownGame.scripts.weapons;

namespace TopDownGame.scripts.player;

public partial class WeaponController : Node2D
{
    public Weapon CurrentWeapon;
    public Vector2 TargetPosition;

    public void EquipWeapon(WeaponData data)
    {
        var weaponScene = Global.Instance.AllWeapons[data.WeaponName];
        var weapon = (Weapon)weaponScene.Instantiate();
        if (weapon == null) return;
        weapon.GlobalPosition = new Vector2(0, -8);
        CurrentWeapon = weapon;
        CurrentWeapon.Data = Global.Instance.SelectedWeapon;
        AddChild(weapon);
    }

    public void RotateWeapon()
    {
        CurrentWeapon?.LookAt(TargetPosition);
    }
}
