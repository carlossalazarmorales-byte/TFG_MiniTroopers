using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Range Up")]
public class RangeUpPerk : Perk
{
    [Range(1f, 3f)] public float multiplier = 1.25f;
    public override float RangeMultiplier(Unit self) => multiplier;
}
