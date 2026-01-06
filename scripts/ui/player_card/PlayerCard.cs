using Godot;
using TopDownGame.scripts.resources.data.player;

namespace TopDownGame.scripts.ui.player_card;

public partial class PlayerCard : TextureButton
{
    private PlayerData _data;
    private TextureRect _icon;

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("Icon");
    }

    public void SetData(PlayerData value)
    {
        _data = value;
        _icon.Texture = _data.Icon;
    }
}
