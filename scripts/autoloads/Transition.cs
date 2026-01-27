using Godot;

namespace TopDownGame.scripts.autoloads;

public partial class Transition : Node
{
    public static Transition Instance { get; private set; }

    private ColorRect _effect;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        _effect = GetNode<ColorRect>("%Effect");
    }

    public void TransitionTo(string scenePath)
    {
        var tween = CreateTween();
        tween.TweenProperty(_effect.Material, "shader_parameter/progress", 1.0, 1.0);

        tween.Finished += () =>
        {
            GetTree().ChangeSceneToFile(scenePath);

            tween = CreateTween();
            tween.TweenProperty(_effect.Material, "shader_parameter/progress", 0.0, 1.0);
        };
    }

    public Tween ShowTransitionIn()
    {
        var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(_effect.Material, "shader_parameter/progress", 1.0, 1.0);
        return tween;
    }
    
    public Tween ShowTransitionOut()
    {
        var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(_effect.Material, "shader_parameter/progress", 0.0, 1.0);
        return tween;
    }
}
