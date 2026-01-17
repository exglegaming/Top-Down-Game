using Godot;

namespace TopDownGame.scripts.extra;

public partial class DamageText : Control
{
    [Export] private Label _label;

    public void Setup(float value)
    {
        _label.Text = value.ToString();
        
        var tween = CreateTween();
        tween.TweenInterval(0.5);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
