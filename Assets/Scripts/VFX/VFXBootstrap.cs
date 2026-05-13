using UnityEngine;

public class VFXBootstrap : MonoBehaviour
{
    public GameObject explosionVfxPrefab;
    public GameObject healVfxPrefab;
    public GameObject floatingTextPrefab;
    public GameObject hitVfxPrefab;

    void Awake()
    {
        VFXService.ExplosionPrefab = explosionVfxPrefab;
        VFXService.HealPrefab = healVfxPrefab;
        FloatingTextService.FloatingTextPrefab = floatingTextPrefab;
        VFXService.HitPrefab = hitVfxPrefab;
    }
}