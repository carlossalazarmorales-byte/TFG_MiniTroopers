using UnityEngine;

public static class VFXService
{
    public static GameObject ExplosionPrefab;
    public static GameObject HealPrefab;
    public static GameObject HitPrefab;

    public static void SpawnExplosion(Vector3 position)
    {
        if (ExplosionPrefab == null) return;
        Object.Instantiate(ExplosionPrefab, position, Quaternion.identity);
    }

    public static void SpawnHeal(Vector3 position)
    {
        if (HealPrefab == null) return;
        Object.Instantiate(HealPrefab, position, Quaternion.identity);
    }

    public static void SpawnHit(Vector3 position)
    {
        if (HitPrefab == null) return;
        Object.Instantiate(HitPrefab, position, Quaternion.identity);
    }
}