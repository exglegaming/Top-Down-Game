using Godot;
using TopDownGame.scripts.autoloads;

namespace TopDownGame.scripts.arena;

public partial class Arena : Node2D
{
    [ExportCategory("References")]
    [Export] private Texture2D _arenaCursor;

    private TextureProgressBar _healthBar;
    private TextureProgressBar _manaBar;
    private EventBus _eventBus;

    public override void _Ready()
    {
        _healthBar = GetNode<TextureProgressBar>("%HealthBar");
        _manaBar = GetNode<TextureProgressBar>("%ManaBar");
        _eventBus = GetNode<EventBus>("/root/EventBus");

        Cursor.Instance.Sprite2D.Texture = _arenaCursor;
        _eventBus.PlayerHealthUpdated += OnPlayerHealthUpdated;
    }

    private void OnPlayerHealthUpdated(float currentHealth, float maxHealth)
    {
        _healthBar.Value = currentHealth / maxHealth;
    }
}
