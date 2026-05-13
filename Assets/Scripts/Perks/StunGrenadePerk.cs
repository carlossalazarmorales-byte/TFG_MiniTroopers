using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Stun Grenade")]
public class StunGrenadePerk : Perk
{
    public float cooldownSeconds = 8f;
    public float radius = 1.6f;
    public float stunDuration = 1.8f;

    readonly System.Collections.Generic.Dictionary<int, float> timers = new();

    public override void Tick(Unit self, float dt)
    {
        if (self == null || !self.IsAlive) return;

        int id = self.GetInstanceID();
        if (!timers.ContainsKey(id)) timers[id] = cooldownSeconds;

        timers[id] -= dt;
        if (timers[id] > 0f) return;
        timers[id] = cooldownSeconds;

        Unit target = PerkUtils.FindClosestEnemy(self);
        if (target == null) return;

        Vector3 center = target.transform.position;
        PerkUtils.StunEnemiesInRadius(self, center, radius, stunDuration);
        VFXService.SpawnExplosion(center);
    }
}
