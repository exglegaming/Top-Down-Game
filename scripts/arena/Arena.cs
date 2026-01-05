using System;
using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.player;

namespace TopDownGame.scripts.arena;

public partial class Arena : Node2D
{
    [ExportCategory("References")]
    [Export] private Texture2D _arenaCursor;

    private TextureProgressBar _healthBar;
    private TextureProgressBar _manaBar;

    public override void _Ready()
    {
        _healthBar = GetNode<TextureProgressBar>("%HealthBar");
        _manaBar = GetNode<TextureProgressBar>("%ManaBar");

        Cursor.Instance.Sprite2D.Texture = _arenaCursor;
        EventBus.Instance.Connect(EventBus.SignalName.PlayerHealthUpdated, Callable.From<Player>(OnHealthComponentOnUnitDamaged));
    }

    private void OnHealthComponentOnUnitDamaged(Player player)
    {
        _healthBar.Value = player.HealthComponent.CurrentHealth / player.Data.MaxHP;
    }
}
