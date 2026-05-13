using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IsometricSorter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Cuanto más bajo en Y, más delante se dibuja
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
