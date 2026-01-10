using Godot;

namespace TopDownGame.scripts.ui.description_panel;

public partial class DescriptionPanel : Control
{
    [ExportCategory("References")]
    [Export] private Label _label;

    public void SetText(string value)
    {
        _label.Text = value;
    }
}
