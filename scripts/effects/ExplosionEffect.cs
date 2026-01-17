using Godot;

namespace TopDownGame.scripts.effects;

public partial class ExplosionEffect : AnimatedSprite2D
{
    public override void _Ready()
    {
        AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished()
    {
        QueueFree();
    }
}
