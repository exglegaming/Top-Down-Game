using Godot;

namespace TopDownGame.scripts.autoloads;

public partial class EventBus : Node
{
    [Signal] public delegate void PlayerHealthUpdatedEventHandler(float currentHealth, float maxHealth);

    public static EventBus Instance { get; private set;}

    public override void _EnterTree()
    {
        Instance = this;
    }

    public static void EmitPlayerHealthUpdated(float currentHealth, float maxHealth)
    {
        Instance.EmitSignal(SignalName.PlayerHealthUpdated, currentHealth, maxHealth);
    }
}
