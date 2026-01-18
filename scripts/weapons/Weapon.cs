using Godot;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.weapons;

public partial class Weapon : Node2D
{
    [ExportCategory("References")]
    [Export] public WeaponData Data { get; set; }
    [Export] public Node2D Pivot;

    protected virtual void UseWeapon()
    {
        
    }
}
