using UnityEngine;
using UnityEngine.UI;

public class PerkCardUI : MonoBehaviour
{
    public Image iconImage;
    public Button button;

    PerkEntry perk;
    SquadMenuUI squadMenu;

    public void Setup(PerkEntry perkEntry, SquadMenuUI owner)
    {
        perk = perkEntry;
        squadMenu = owner;

        if (iconImage != null)
            iconImage.sprite = perk.cardSprite;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (squadMenu != null)
            squadMenu.TryEquipPerk(perk);
    }
}