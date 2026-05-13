using UnityEngine;

[CreateAssetMenu(menuName = "MiniTroopers/Perk Entry")]
public class PerkEntry : ScriptableObject
{
    public string perkId;
    public string displayName;
    public Sprite cardSprite;
}