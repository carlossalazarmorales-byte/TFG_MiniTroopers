using UnityEngine;

public static class FloatingTextService
{
    public static GameObject FloatingTextPrefab;

    public static void SpawnText(Vector3 position, string text, Color color)
    {
        if (FloatingTextPrefab == null)
            return;

        GameObject obj = Object.Instantiate(
            FloatingTextPrefab,
            position,
            Quaternion.identity);

        FloatingText ft = obj.GetComponent<FloatingText>();

        if (ft != null)
            ft.Setup(text, color);
    }
}