using Godot;
using TopDownGame.scripts.resources.data.player;
using TopDownGame.scripts.ui.description_panel;

namespace TopDownGame.scripts.ui.player_card;

public partial class PlayerCard : TextureButton
{
    [ExportCategory("References")]
    [Export] private TextureRect _icon;
    [Export] private AudioStreamPlayer _hoverSound;
    [Export] private DescriptionPanel _descriptionPanel;
    [Export] public TextureRect Selector;

    private PlayerData _data;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    public void SetData(PlayerData value)
    {
        _data = value;
        _icon.Texture = _data.Icon;
        SetDescription();
    }

    private void SetDescription()
    {
        var playerInfo = $"Player: {_data.ID}\nHP: {_data.MaxHP}\nSpeed: {_data.MoveSpeed}\nMagic: {_data.Magic}";
        _descriptionPanel.SetText(playerInfo);
        
    }

    private void OnMouseEntered()
    {
        _hoverSound.Play();
        var dampedOscilator = GetNode<GodotObject>("/root/DampedOscillator");
        dampedOscilator.Call("animate", _icon, "scale", (float)GD.RandRange(400, 450), (float)GD.RandRange(5, 10), (float)GD.RandRange(10, 15), 0.5);

        _descriptionPanel.Show();
        dampedOscilator.Call("animate", _descriptionPanel, "scale", (float)GD.RandRange(400, 450), (float)GD.RandRange(5, 10), (float)GD.RandRange(10, 15), 0.5);
        dampedOscilator.Call("animate", _descriptionPanel, "rotation_degrees", 300, 7.5, 15, 0.5 * GD.RandRange(-20, 20));
    }

    private void OnMouseExited()
    {
        _descriptionPanel.Hide();
    }
}
