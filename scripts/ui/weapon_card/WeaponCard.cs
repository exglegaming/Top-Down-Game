using Godot;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.ui.weapon_card;

public partial class WeaponCard : TextureButton
{
    private WeaponData _data;
    private TextureRect _icon;

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("Icon");
    }

    public void SetData(WeaponData value)
    {
        _data = value;
        _icon.Texture = _data.Icon;
    }
}
