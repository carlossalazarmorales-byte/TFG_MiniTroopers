using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Troop Entry")]
public class TroopEntry : ScriptableObject
{
    public string displayName;
    public GameObject prefab;   // Prefab aliado de esa tropa (pistola/rifle/sniper/bazooka/healer)
    public Sprite cardSprite;
}
