using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public Button button;
    public Sprite emptySlotSprite;

    [Header("Perk Icons")]
    public Image[] perkIcons;

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

    public void SetTroop(TroopEntry troop, List<PerkEntry> perks)
    {
        if (iconImage == null)
            return;

        iconImage.enabled = true;

        if (troop == null)
        {
            iconImage.sprite = emptySlotSprite;
            iconImage.color = Color.white;
            SetPerkIcons(null);

            if (nameText != null)
                nameText.text = "";

            return;
        }

        iconImage.sprite = troop.cardSprite;
        iconImage.color = Color.white;
        SetPerkIcons(perks);

        if (nameText != null)
            nameText.text = troop.displayName;
    }

    void SetPerkIcons(List<PerkEntry> perks)
    {
        if (perkIcons == null) return;

        for (int i = 0; i < perkIcons.Length; i++)
        {
            if (perkIcons[i] == null) continue;

            if (perks != null && i < perks.Count && perks[i] != null)
            {
                perkIcons[i].enabled = true;
                perkIcons[i].sprite = perks[i].cardSprite;
                perkIcons[i].color = Color.white;
            }
            else
            {
                perkIcons[i].enabled = false;
            }
        }
    }

    void OnClick()
    {
        if (squadMenu != null)
            squadMenu.ClearSlot(index);
    }
}