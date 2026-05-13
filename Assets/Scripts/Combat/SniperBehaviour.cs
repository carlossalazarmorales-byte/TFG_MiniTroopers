using UnityEngine;

public class SniperBehaviour : MonoBehaviour
{
    Unit self;

    void Awake()
    {
        self = GetComponent<Unit>();
    }

    // Devuelve el objetivo que debe priorizar el sniper:
    // - Si hay enemigos en rango: el MÁS LEJANO en rango
    // - Si no hay en rango: el MÁS CERCANO (para acercarse y empezar a disparar)
    public Unit PickTarget(Unit[] enemies, float attackRange)
    {
        if (self == null || enemies == null || enemies.Length == 0) return null;

        Unit farthestInRange = null;
        float farthestDist = -1f;

        Unit nearest = null;
        float nearestDist = float.MaxValue;

        Vector2 myPos = self.transform.position;
        float range2 = attackRange * attackRange;

        foreach (var u in enemies)
        {
            if (u == null || !u.IsAlive || u.data == null) continue;

            Vector2 p = u.transform.position;
            float dx = p.x - myPos.x;
            float dy = p.y - myPos.y;
            float d2 = dx * dx + dy * dy;

            if (d2 <= range2)
            {
                if (d2 > farthestDist)
                {
                    farthestDist = d2;
                    farthestInRange = u;
                }
            }
            else
            {
                if (d2 < nearestDist)
                {
                    nearestDist = d2;
                    nearest = u;
                }
            }
        }

        return farthestInRange != null ? farthestInRange : nearest;
    }
}
