using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform fill;

    Unit unit;
    Vector3 baseScale;

    void Awake()
    {
        unit = GetComponentInParent<Unit>();

        if (fill != null)
            baseScale = fill.localScale;
    }

    void LateUpdate()
    {
        if (unit == null || fill == null) return;

        float t = unit.HpNormalized;
        fill.localScale = new Vector3(baseScale.x * t, baseScale.y, baseScale.z);
    }
}
