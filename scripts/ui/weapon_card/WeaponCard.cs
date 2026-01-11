using Godot;
using TopDownGame.scripts.resources.data.weapons;
using TopDownGame.scripts.ui.description_panel;

namespace TopDownGame.scripts.ui.weapon_card;

public partial class WeaponCard : TextureButton
{
    [ExportCategory("References")]
    [Export] private TextureRect _icon;
    [Export] private DescriptionPanel _descriptionPanel;
    [Export] private AudioStreamPlayer _hoverSound;
    [Export] public TextureRect Selector;

    private WeaponData _data;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    public void SetData(WeaponData value)
    {
        _data = value;
        _icon.Texture = _data.Icon;
        SetDescription();
    }

    private void SetDescription() => _descriptionPanel.SetText(_data.Description);

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
