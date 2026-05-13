using UnityEngine;
using UnityEngine.UI;

public class SquadPerkSlotUI : MonoBehaviour
{
    public Image iconImage;
    public Button button;
    public Sprite emptySprite;

    int index;
    SquadMenuUI squadMenu;

    public void Setup(int slotIndex, SquadMenuUI owner)
    {
        index = slotIndex;
        squadMenu = owner;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    public void SetPerk(PerkEntry perk)
    {
        if (iconImage == null) return;

        iconImage.enabled = true;
        iconImage.sprite = perk == null ? emptySprite : perk.cardSprite;
        iconImage.color = Color.white;
    }

    void OnClick()
    {
        if (squadMenu != null)
            squadMenu.ClearSelectedTroopPerk(index);
    }
}