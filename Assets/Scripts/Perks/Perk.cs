using UnityEngine;
using System.Collections.Generic;

public abstract class Perk : ScriptableObject
{
    // Targeting hook
    public virtual Unit PickTarget(Unit self, List<Unit> candidates) => null;

    // Stat modifiers (multiplicadores). Devuelve 1 = no cambia.
    public virtual float DamageMultiplier(Unit self) => 1f;
    public virtual float CooldownMultiplier(Unit self) => 1f;  // <1 = más rápido
    public virtual float RangeMultiplier(Unit self) => 1f;
    public virtual float MoveSpeedMultiplier(Unit self) => 1f;

    // Activos: se llaman desde Unit en cada Update (si quieres usarlo)
    public virtual void Tick(Unit self, float dt) { }
}
