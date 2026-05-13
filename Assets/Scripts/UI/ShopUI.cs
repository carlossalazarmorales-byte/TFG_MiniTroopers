using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button buyRandomTroopButton;
    public Button buyRandomPerkButton;
    public Button backButton;

    [Header("Gold UI")]
    public TextMeshProUGUI goldText;

    [Header("Costs")]
    public int troopCost = 300;
    public int perkCost = 100;

    void Start()
    {
        buyRandomTroopButton.onClick.AddListener(BuyRandomTroop);
        buyRandomPerkButton.onClick.AddListener(BuyRandomPerk);
        backButton.onClick.AddListener(BackToMainMenu);

        RefreshGoldUI();
    }

    void RefreshGoldUI()
    {
        if (goldText != null)
            goldText.text = PlayerProgress.Gold.ToString();
    }

    void BuyRandomTroop()
    {
        if (PlayerProgress.Gold < troopCost)
        {
            Debug.Log("Not enough gold for troop.");
            return;
        }

        List<TroopEntry> owned = GetOwnedTroops();
        List<TroopEntry> locked = new List<TroopEntry>();

        foreach (TroopEntry troop in TroopDatabase.Instance.allTroops)
        {
            if (troop == null) continue;

            bool alreadyOwned = false;

            foreach (TroopEntry ownedTroop in owned)
            {
                if (ownedTroop != null && ownedTroop.name == troop.name)
                {
                    alreadyOwned = true;
                    break;
                }
            }

            if (!alreadyOwned)
                locked.Add(troop);
        }

        if (locked.Count == 0)
        {
            Debug.Log("All troops already unlocked.");
            return;
        }

        PlayerProgress.Gold -= troopCost;

        TroopEntry reward = locked[Random.Range(0, locked.Count)];
        owned.Add(reward);

        PlayerProfile.SaveOwnedTroops(owned);
        RefreshGoldUI();

        Debug.Log("Unlocked troop: " + reward.displayName);
    }

    void BuyRandomPerk()
    {
        if (PlayerProgress.Gold < perkCost)
        {
            Debug.Log("Not enough gold for perk.");
            return;
        }

        List<PerkEntry> owned = GetOwnedPerks();
        List<PerkEntry> locked = new List<PerkEntry>();

        foreach (PerkEntry perk in PerkDatabase.Instance.allPerks)
        {
            if (perk == null) continue;

            bool alreadyOwned = false;

            foreach (PerkEntry ownedPerk in owned)
            {
                if (ownedPerk != null && ownedPerk.perkId == perk.perkId)
                {
                    alreadyOwned = true;
                    break;
                }
            }

            if (!alreadyOwned)
                locked.Add(perk);
        }

        if (locked.Count == 0)
        {
            Debug.Log("All perks already unlocked.");
            return;
        }

        PlayerProgress.Gold -= perkCost;

        PerkEntry reward = locked[Random.Range(0, locked.Count)];
        owned.Add(reward);

        PlayerProfile.SaveOwnedPerks(owned);
        RefreshGoldUI();

        Debug.Log("Unlocked perk: " + reward.displayName);
    }

    List<TroopEntry> GetOwnedTroops()
    {
        string[] names = PlayerProfile.LoadOwnedTroopNames();
        return TroopDatabase.Instance.GetTroopsFromNames(names);
    }

    List<PerkEntry> GetOwnedPerks()
    {
        List<PerkEntry> result = new List<PerkEntry>();

        string[] ids = PlayerProfile.LoadOwnedPerkIds();

        foreach (string id in ids)
        {
            PerkEntry perk = PerkDatabase.Instance.GetPerkById(id);
            if (perk != null)
                result.Add(perk);
        }

        return result;
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}