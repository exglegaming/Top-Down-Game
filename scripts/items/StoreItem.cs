using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.resources.data.items;
using TopDownGame.scripts.ui.description_panel;

namespace TopDownGame.scripts.items;

public partial class StoreItem : Area2D
{
    [ExportCategory("References")]
    [Export] private Sprite2D _glow;
    [Export] private Sprite2D _sprite;
    [Export] private RichTextLabel _price;
    [Export] private DescriptionPanel _descriptionPanel;
    
    [ExportGroup("Glow Color")]
    [Export] private Color _commonGlow;
    [Export] private Color _rareGlow;
    [Export] private Color _epicGlow;

    private ItemData _data;
    private bool _canBuyItem;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && _canBuyItem)
        {
            BuyItem();
        }
    }

    public void Setup(ItemData itemData)
    {
        _data = itemData;
        _sprite.Texture = _data.Icon;
        _glow.SelfModulate = GetRarityColor();
        _price.Text = $"[code][img=10]assets/sprites/coin.png[/img][/code]{_data.Price}";
        _descriptionPanel.SetText(_data.Description);
    }

    private void BuyItem()
    {
        if (_data == null) return;
        if (Global.Instance.Coins < _data.Price) return;
        
        switch (_data.Id)
        {
            case "Potion":
                Global.Instance.PlayerRef.HealthComponent.Heal(_data.Value);
                break;
        }
        Global.Instance.Coins -= _data.Price;
        
        QueueFree();
    }

    private Color GetRarityColor()
    {
        return _data.Rarity switch
        {
            ItemData.ItemRarity.Common => _commonGlow,
            ItemData.ItemRarity.Rare => _rareGlow,
            ItemData.ItemRarity.Epic => _epicGlow,
            _ => Colors.White
        };
    }

    private void OnBodyEntered(Node2D body)
    {
        _canBuyItem = true;
        _descriptionPanel.Show();
        var dampedOscillator = GetNode<GodotObject>("/root/DampedOscillator");
        dampedOscillator.Call("animate", _descriptionPanel, "scale", (float)GD.RandRange(400, 450),  (float)GD.RandRange(5, 10), (float)GD.RandRange(10, 15), 0.5);
        dampedOscillator.Call("animate", _descriptionPanel, "rotation_degrees", 300, 7.5, 15, (float)GD.RandRange(-20, 20) * 0.5);
    }

    private void OnBodyExited(Node2D body)
    {
        _canBuyItem = false;
        _descriptionPanel.Hide();
    }
}