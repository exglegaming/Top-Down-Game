using Godot;

namespace TopDownGame.Scritps.Autoloads;

public partial class Cursor : CanvasLayer
{
    public static Cursor Instance { get; private set; }

    public Sprite2D _sprite2D;

    public override void _Notification(int what) 
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }

    public override void _Ready()
    {
        // Instance = this;
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Process(double delta)
    {
        _sprite2D.Position = GetViewport().GetMousePosition();
    }
}
