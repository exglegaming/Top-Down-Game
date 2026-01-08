using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.resources.data.player;

namespace TopDownGame.scripts.ui.player_card;

public partial class PlayerCard : TextureButton
{
    private PlayerData _data;
    private TextureRect _icon;
    private AudioStreamPlayer _hoverSound;

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("Icon");
        _hoverSound = GetNode<AudioStreamPlayer>("HoverSound");

        MouseEntered += OnMouseEntered;
    }

    public void SetData(PlayerData value)
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
