using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public float duration = 0.6f;
    public float moveUpSpeed = 0.35f;

    TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();

        if (text == null)
            text = GetComponentInChildren<TMP_Text>();
    }

    public void Setup(string value, Color color)
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        if (text == null) return;

        text.text = value;
        text.color = color;

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = t / duration;

            transform.position = startPos + Vector3.up * (moveUpSpeed * n);

            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, n);
            text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}