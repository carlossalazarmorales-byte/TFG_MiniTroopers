using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Frag Grenade")]
public class FragGrenadePerk : Perk
{
    public float cooldownSeconds = 6f;
    public float radius = 1.5f;
    public float damage = 12f;

    // timer runtime “por unidad”
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
        PerkUtils.DamageEnemiesInRadius(self, center, radius, damage);
        VFXService.SpawnExplosion(center);
    }
}
