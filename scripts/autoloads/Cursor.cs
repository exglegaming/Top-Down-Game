using Godot;

namespace TopDownGame.scripts.autoloads;

public partial class Cursor : CanvasLayer
{
    public static Cursor Instance { get; private set; }

    public Sprite2D Sprite2D;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>("Sprite2D");
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Process(double delta)
    {
        Sprite2D.Position = GetViewport().GetMousePosition();
    }
}
