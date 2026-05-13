using UnityEngine;
using System.Collections.Generic;

public static class PerkUtils
{
    public static Unit FindClosestEnemy(Unit self)
    {
        string tag = self.isEnemy ? "Ally" : "Enemy";
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);

        Unit best = null;
        float bestDist = float.MaxValue;
        Vector3 pos = self.transform.position;

        foreach (var go in gos)
        {
            Unit u = go.GetComponent<Unit>();
            if (u == null || !u.IsAlive) continue;

            float d = Vector2.Distance(pos, u.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = u;
            }
        }
        return best;
    }

    public static void DamageEnemiesInRadius(Unit self, Vector3 center, float radius, float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (var h in hits)
        {
            Unit u = h.GetComponent<Unit>();
            if (u != null && u.IsAlive && u.isEnemy != self.isEnemy)
                u.TakeDamage(damage);
        }
    }

    public static void StunEnemiesInRadius(Unit self, Vector3 center, float radius, float seconds)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (var h in hits)
        {
            Unit u = h.GetComponent<Unit>();
            if (u != null && u.IsAlive && u.isEnemy != self.isEnemy)
                u.ApplyStun(seconds);
        }
    }
}
