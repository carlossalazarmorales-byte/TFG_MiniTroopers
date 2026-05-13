using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Move Speed Up")]
public class MoveSpeedUpPerk : Perk
{
    [Range(1f, 3f)] public float multiplier = 1.2f;
    public override float MoveSpeedMultiplier(Unit self) => multiplier;
}
