using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perks/Attack Speed Up")]
public class AttackSpeedUpPerk : Perk
{
    [Range(0.2f, 1f)] public float cooldownMultiplier = 0.8f; // 0.8 = 20% más rápido
    public override float CooldownMultiplier(Unit self) => cooldownMultiplier;
}
