using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.resources.data.items;

namespace TopDownGame.scripts.items;

public partial class StoreItem : Area2D
{
    [ExportCategory("References")]
    [Export] private Sprite2D _glow;
    [Export] private Sprite2D _sprite;
    [Export] private RichTextLabel _price;
    
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
    }

    private void OnBodyExited(Node2D body)
    {
        _canBuyItem = false;
    }
}