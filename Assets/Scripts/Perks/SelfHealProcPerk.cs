using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Self Heal Proc")]
public class SelfHealProcPerk : Perk
{
    public float intervalSeconds = 6f;
    public float healAmount = 6f;
    [Range(0f, 1f)] public float healIfBelowHpPercent = 0.5f;

    // Estado runtime por unidad (simple)
    readonly System.Collections.Generic.Dictionary<int, float> timers = new();

    public override void Tick(Unit self, float dt)
    {
        if (self == null || !self.IsAlive) return;

        int id = self.GetInstanceID();
        if (!timers.ContainsKey(id)) timers[id] = intervalSeconds;

        timers[id] -= dt;
        if (timers[id] > 0f) return;

        timers[id] = intervalSeconds;

        float max = self.MaxHP;
        if (max <= 0) return;

        if (self.CurrentHP / max <= healIfBelowHpPercent)
            self.Heal(healAmount);
    }
}
