using Godot;
using TopDownGame.scripts.levels;

namespace TopDownGame.scripts.autoloads;

public partial class EventBus : Node
{
    [Signal] public delegate void PlayerHealthUpdatedEventHandler(float currentHealth, float maxHealth);
    [Signal] public delegate void PlayerRoomEnteredEventHandler(LevelRoom room);

    public static EventBus Instance { get; private set;}

    public override void _EnterTree()
    {
        Instance = this;
    }

    public static void EmitPlayerHealthUpdated(float currentHealth, float maxHealth)
    {
        Instance.EmitSignal(SignalName.PlayerHealthUpdated, currentHealth, maxHealth);
    }

    public static void EmitPlayerRoomEntered(LevelRoom room)
    {
        Instance.EmitSignal(SignalName.PlayerRoomEntered, room);
    }
}
