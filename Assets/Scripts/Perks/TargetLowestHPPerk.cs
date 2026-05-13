using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Target Lowest HP")]
public class TargetLowestHPPerk : Perk
{
    public override Unit PickTarget(Unit self, List<Unit> candidates)
    {
        Unit best = null;
        float bestHp = float.MaxValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            Unit u = candidates[i];
            if (u == null || !u.IsAlive) continue;

            if (u.CurrentHP < bestHp)
            {
                bestHp = u.CurrentHP;
                best = u;
            }
        }
        return best;
    }
}
