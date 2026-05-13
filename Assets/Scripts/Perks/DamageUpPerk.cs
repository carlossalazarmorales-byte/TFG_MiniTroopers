using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Damage Up")]
public class DamageUpPerk : Perk
{
    [Range(1f, 3f)] public float multiplier = 1.2f;
    public override float DamageMultiplier(Unit self) => multiplier;
}
