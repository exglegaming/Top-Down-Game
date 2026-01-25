using Godot;

namespace TopDownGame.scripts.ui.map_cell;

public partial class MapCell : Control
{
    [ExportCategory("References")]
    [Export] private TextureRect _playerIcon;

    public override void _Ready()
    {
        SetPlayerActive(false);
    }

    public void SetPlayerActive(bool value)
    {
        _playerIcon.Visible = value;
    }
}