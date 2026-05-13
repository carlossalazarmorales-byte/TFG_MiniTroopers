using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public float lifeTime = 0.06f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Vector3 start, Vector3 end)
    {
        transform.position = (start + end) / 2f;

        Vector3 dir = end - start;
        float dist = dir.magnitude;

        transform.right = dir.normalized;

        Vector3 scale = transform.localScale;
        scale.x = dist;
        transform.localScale = scale;
    }
}