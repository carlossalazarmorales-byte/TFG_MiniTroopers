using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Target Closest")]
public class TargetClosestPerk : Perk
{
    public override Unit PickTarget(Unit self, List<Unit> candidates)
    {
        Unit best = null;
        float bestDist = float.MaxValue;

        Vector3 pos = self.transform.position;

        for (int i = 0; i < candidates.Count; i++)
        {
            Unit u = candidates[i];
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
}
