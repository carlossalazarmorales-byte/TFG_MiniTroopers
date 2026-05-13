using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Target Highest HP")]
public class TargetHighestHPPerk : Perk
{
    public override Unit PickTarget(Unit self, List<Unit> candidates)
    {
        Unit best = null;
        float bestHp = -1f;

        for (int i = 0; i < candidates.Count; i++)
        {
            Unit u = candidates[i];
            if (u == null || !u.IsAlive) continue;

            if (u.CurrentHP > bestHp)
            {
                bestHp = u.CurrentHP;
                best = u;
            }
        }
        return best;
    }
}
