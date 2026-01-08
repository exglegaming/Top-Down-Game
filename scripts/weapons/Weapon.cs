using Godot;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.weapons;

public partial class Weapon : Node2D
{
    [ExportCategory("References")]
    [Export] public WeaponData Data { get; set; }

    protected Node2D Pivot;

    public override void _Ready()
    {
        Pivot = GetNode<Node2D>("Pivot");
    }

    public virtual void UseWeapon()
    {
        
    }
}
