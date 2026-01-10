using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.ui.weapon_card;

public partial class WeaponCard : TextureButton
{
    private WeaponData _data;
    private TextureRect _icon;
    private AudioStreamPlayer _hoverSound;

    public TextureRect Selector;

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("Icon");
        _hoverSound = GetNode<AudioStreamPlayer>("HoverSound");
        Selector = GetNode<TextureRect>("Selector");

        MouseEntered += OnMouseEntered;
    }

    public void SetData(WeaponData value)
    {
        _data = value;
        _icon.Texture = _data.Icon;
    }

    private void OnMouseEntered()
    {
        _hoverSound.Play();
        Global.Instance.Oscilator(_icon);
    }
}
