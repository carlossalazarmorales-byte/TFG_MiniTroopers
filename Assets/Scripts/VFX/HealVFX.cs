using UnityEngine;
using System.Collections;

public class HealVFX : MonoBehaviour
{
    public float duration = 0.45f;
    public float floatUpAmount = 0.35f;
    public float maxScale = 0.45f;

    SpriteRenderer sr;
    Color baseColor;
    Vector3 startPos;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            baseColor = sr.color;

        startPos = transform.position;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / duration);

            transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * floatUpAmount, n);

            float scale = Mathf.Lerp(0.15f, maxScale, n);
            transform.localScale = new Vector3(scale, scale, 1f);

            if (sr != null)
            {
                Color c = baseColor;
                c.a = Mathf.Lerp(0.7f, 0f, n);
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}