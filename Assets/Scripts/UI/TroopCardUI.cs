using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroopCardUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public Button button;

    TroopEntry troop;
    SquadMenuUI squadMenu;

    public void Setup(TroopEntry troopEntry, Sprite cardSprite, SquadMenuUI owner)
    {
        troop = troopEntry;
        squadMenu = owner;

        if (iconImage != null)
        {
            iconImage.sprite = cardSprite;
            iconImage.color = Color.white;
        }

        if (nameText != null)
            nameText.text = troopEntry.displayName;

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (squadMenu == null || troop == null) return;

        squadMenu.SelectTroop(troop);
    }
}