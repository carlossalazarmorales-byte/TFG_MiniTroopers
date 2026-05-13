using UnityEngine;
using System.Collections;

public class HitVFX : MonoBehaviour
{
    public float duration = 0.15f;
    public float maxScale = 0.3f;

    SpriteRenderer sr;
    Color baseColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            baseColor = sr.color;

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = t / duration;

            float scale = Mathf.Lerp(0.05f, maxScale, n);
            transform.localScale = new Vector3(scale, scale, 1f);

            if (sr != null)
            {
                Color c = baseColor;
                c.a = Mathf.Lerp(0.8f, 0f, n);
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}