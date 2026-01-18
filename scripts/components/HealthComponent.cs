using System;
using Godot;

namespace TopDownGame.scripts.components;

public partial class HealthComponent : Node
{
    [Signal] public delegate void OnUnitDamagedEventHandler(float amount);
    [Signal] public delegate void OnUnitHealedEventHandler(float amount);
    [Signal] public delegate void OnUnitDeadEventHandler();

    public float CurrentHealth { get; private set;}
    private float _maxHealth;

    public void InitHealth(float value)
    {
        CurrentHealth = value;
        _maxHealth = value;
    }

    public void TakeDamage(float value)
    {
        if (CurrentHealth > 0)
        {
            CurrentHealth -= value;
            EmitSignal(SignalName.OnUnitDamaged, value);

            if (CurrentHealth <= 0) Die();
        }
    }

    public void Die()
    {
        CurrentHealth = 0.0f;
        EmitSignal(SignalName.OnUnitDead);
    }

    public void Heal(float value)
    {
        if (CurrentHealth >= _maxHealth) return;

        CurrentHealth = Math.Min(_maxHealth, CurrentHealth + value);
        EmitSignal(SignalName.OnUnitHealed, value);
    }
}
