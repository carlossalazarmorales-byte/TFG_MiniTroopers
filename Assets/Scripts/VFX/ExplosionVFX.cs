using UnityEngine;
using System.Collections;

public class ExplosionVFX : MonoBehaviour
{
    public float maxScale = 1.5f;
    public float duration = 0.25f;

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

            // Escala: crece rápido
            float scale = Mathf.Lerp(0.1f, maxScale, n);
            transform.localScale = new Vector3(scale, scale, 1f);

            // Alpha: desaparece
            if (sr != null)
            {
                Color c = baseColor;
                c.a = Mathf.Lerp(0.6f, 0f, n);
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
