using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Info")]
    public string unitName;

    [Header("Stats")]
    public float maxHP = 100f;
    public float moveSpeed = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public float damage = 10f;

    [Header("Targeting")]
    [Tooltip("Cuanto MÁS ALTO, más probable que lo focuseen. 1 = normal. 0.5 = menos prioritario (healer). 2 = muy prioritario.")]
    public float targetPriority = 1f;
}
