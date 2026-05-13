using UnityEngine;

public class BazookaBehaviour : MonoBehaviour
{
    [Header("AoE Settings")]
    [Tooltip("Radio de explosión alrededor del objetivo.")]
    public float splashRadius = 1.5f;

    [Tooltip("Multiplicador de daño para objetivos en el borde (0.5 = mitad).")]
    [Range(0f, 1f)]
    public float edgeDamageMultiplier = 0.6f;

    [Tooltip("Si tienes un prefab de explosión (opcional), se instancia en el objetivo.")]
    public GameObject explosionVfxPrefab;

    Unit self;

    void Awake()
    {
        self = GetComponent<Unit>();
    }

    public void AttemptAttack(Unit target)
    {
        if (self == null || self.data == null) return;
        if (target == null || !target.IsAlive) return;

        if (!self.IsAttackReady) return;

        // Consume cooldown
        self.ConsumeAttackCooldown();

        // VFX opcional
        if (explosionVfxPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVfxPrefab, target.transform.position, Quaternion.identity);
            Destroy(vfx, 1.5f);
        }

        // Daño en área: afectamos a todos los enemigos dentro del radio
        DamageEnemiesInRadius(target.transform.position, splashRadius, self.data.damage);
    }

    void DamageEnemiesInRadius(Vector3 center, float radius, float baseDamage)
    {
        string enemyTag = self.isEnemy ? "Ally" : "Enemy";
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float r2 = radius * radius;

        foreach (var go in enemies)
        {
            Unit u = go.GetComponent<Unit>();
            if (u == null || !u.IsAlive) continue;

            Vector3 pos = u.transform.position;
            float dx = pos.x - center.x;
            float dy = pos.y - center.y;
            float dist2 = dx * dx + dy * dy;

            if (dist2 > r2) continue;

            // Falloff simple: cerca = daño completo, borde = daño reducido
            float dist = Mathf.Sqrt(dist2);
            float t = Mathf.Clamp01(dist / radius); // 0 centro, 1 borde
            float mult = Mathf.Lerp(1f, edgeDamageMultiplier, t);

            u.TakeDamage(baseDamage * mult);
        }
    }
}
