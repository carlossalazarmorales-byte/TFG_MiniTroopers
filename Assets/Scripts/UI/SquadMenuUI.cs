using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SquadMenuUI : MonoBehaviour
{
    [Header("References")]
    public Transform unitsBox;

    [Header("Selected Troop Panel")]
    public Image selectedTroopImage;
    public TextMeshProUGUI selectedTroopNameText;
    public TextMeshProUGUI selectedTroopStatsText;
    public Button equipButton;

    [Header("Perks")]
    public Transform perksBox;
    public GameObject perkCardPrefab;
    public SquadPerkSlotUI[] selectedPerkSlots;

    public GameObject troopCardPrefab;
    public SquadSlotUI[] squadSlots;
    public Button saveButton;
    public Button backButton;

    public Button clearButton;
    public Button autoFillButton;

    List<TroopEntry> ownedTroops = new List<TroopEntry>();
    List<TroopEntry> equippedSquad = new List<TroopEntry>();
    List<PerkEntry> unlockedPerks = new List<PerkEntry>();
    TroopEntry selectedTroop;
    Dictionary<string, List<PerkEntry>> troopPerks = new Dictionary<string, List<PerkEntry>>();

    const int MAX_SQUAD_SIZE = 5;
    

    void Start()
    {
        LoadData();
        LoadPerks();
        RefreshAll();

        saveButton.onClick.AddListener(SaveSquad);
        backButton.onClick.AddListener(BackToMainMenu);
        equipButton.onClick.AddListener(EquipSelectedTroop);
        clearButton.onClick.AddListener(ClearAll);
        autoFillButton.onClick.AddListener(AutoFill);
        ClearSelectedTroopPanel();
        RefreshSelectedPerkSlots();
    }

    void LoadData()
    {
        ownedTroops.Clear();
        equippedSquad.Clear();

        string[] ownedNames = PlayerProfile.LoadOwnedTroopNames();
        ownedTroops = TroopDatabase.Instance.GetTroopsFromNames(ownedNames);

        string[] equippedNames = PlayerProfile.LoadEquippedSquadNames();
        equippedSquad = TroopDatabase.Instance.GetTroopsFromNames(equippedNames);

        while (equippedSquad.Count < MAX_SQUAD_SIZE)
            equippedSquad.Add(null);

        if (equippedSquad.Count > MAX_SQUAD_SIZE)
            equippedSquad.RemoveRange(MAX_SQUAD_SIZE, equippedSquad.Count - MAX_SQUAD_SIZE);

        LoadSavedTroopPerks();
    }

    void LoadSavedTroopPerks()
    {
        troopPerks.Clear();

        if (PerkDatabase.Instance == null)
            return;

        Dictionary<string, List<string>> saved = PlayerProfile.LoadTroopPerkIds();

        foreach (var pair in saved)
        {
            troopPerks[pair.Key] = new List<PerkEntry>();

            foreach (string perkId in pair.Value)
            {
                PerkEntry perk = PerkDatabase.Instance.GetPerkById(perkId);
                if (perk != null)
                    troopPerks[pair.Key].Add(perk);
            }
        }
    }

    void RefreshAll()
    {
        RefreshSlots();
        BuildOwnedTroopCards();
        BuildPerkCards();
    }

    void BuildOwnedTroopCards()
    {
        foreach (Transform child in unitsBox)
            Destroy(child.gameObject);

        foreach (TroopEntry troop in ownedTroops)
        {
            if (troop == null) continue;

            if (IsTroopEquipped(troop))
                continue;

            GameObject obj = Instantiate(troopCardPrefab, unitsBox);
            TroopCardUI card = obj.GetComponent<TroopCardUI>();

            if (card != null)
                card.Setup(troop, troop.cardSprite, this);
        }
    }

    void RefreshSlots()
    {
        for (int i = 0; i < squadSlots.Length; i++)
        {
            squadSlots[i].Setup(i, this);

            TroopEntry troop = i < equippedSquad.Count ? equippedSquad[i] : null;
            List<PerkEntry> perks = troop != null ? GetPerksForTroop(troop) : null;

            squadSlots[i].SetTroop(troop, perks);
        }
    }

    public void TryEquipTroop(TroopEntry troop)
    {
        if (troop == null) return;

        if (IsTroopEquipped(troop))
            return;

        for (int i = 0; i < equippedSquad.Count; i++)
        {
            if (equippedSquad[i] == null)
            {
                equippedSquad[i] = troop;
                RefreshAll();
                return;
            }
        }

        Debug.Log("Squad full.");
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= equippedSquad.Count) return;

        TroopEntry removedTroop = equippedSquad[index];

        if (removedTroop != null && troopPerks.ContainsKey(removedTroop.name))
            troopPerks[removedTroop.name].Clear();

        equippedSquad[index] = null;

        if (selectedTroop == removedTroop)
        {
            selectedTroop = null;
            ClearSelectedTroopPanel();
        }

        RefreshAll();
        BuildPerkCards();
    }

    public bool IsTroopEquipped(TroopEntry troop)
    {
        if (troop == null) return false;

        foreach (TroopEntry equipped in equippedSquad)
        {
            if (equipped == null) continue;

            if (equipped.name == troop.name)
                return true;
        }

        return false;
    }

    void SaveSquad()
    {
        List<TroopEntry> cleanSquad = new List<TroopEntry>();

        foreach (TroopEntry troop in equippedSquad)
        {
            if (troop != null)
                cleanSquad.Add(troop);
        }

        PlayerProfile.SaveEquippedSquad(cleanSquad);
        PlayerProfile.SaveTroopPerks(troopPerks);
        Debug.Log("Squad saved.");
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SelectTroop(TroopEntry troop)
    {
        selectedTroop = troop;
        RefreshSelectedTroopPanel();
        RefreshSelectedPerkSlots();
        BuildPerkCards();
    }

    void RefreshSelectedTroopPanel()
    {
        if (selectedTroop == null)
        {
            ClearSelectedTroopPanel();
            return;
        }

        if (selectedTroopImage != null)
        {
            selectedTroopImage.enabled = true;
            selectedTroopImage.sprite = selectedTroop.cardSprite;
        }

        if (selectedTroopNameText != null)
            selectedTroopNameText.text = selectedTroop.displayName;

        if (selectedTroopStatsText != null && selectedTroop.prefab != null)
        {
            Unit unit = selectedTroop.prefab.GetComponent<Unit>();

            if (unit != null && unit.data != null)
            {
                UnitData data = unit.data;

                selectedTroopStatsText.text =
                    $"HP: {data.maxHP}\n" +
                    $"Damage: {data.damage}\n" +
                    $"Range: {data.attackRange}\n" +
                    $"Cooldown: {data.attackCooldown}";
            }
            else
            {
                selectedTroopStatsText.text = "";
            }
        }
    }

    void ClearSelectedTroopPanel()
    {
        if (selectedTroopImage != null)
            selectedTroopImage.enabled = false;

        if (selectedTroopNameText != null)
            selectedTroopNameText.text = "";

        if (selectedTroopStatsText != null)
            selectedTroopStatsText.text = "";

        RefreshSelectedPerkSlots();
        BuildPerkCards();
    }

    void EquipSelectedTroop()
    {
        if (selectedTroop == null) return;

        TryEquipTroop(selectedTroop);
        selectedTroop = null;
        ClearSelectedTroopPanel();
    }

    void LoadPerks()
    {
        unlockedPerks.Clear();

        if (PerkDatabase.Instance == null)
        {
            Debug.LogError("No PerkDatabase found in scene.");
            return;
        }

        unlockedPerks.AddRange(PerkDatabase.Instance.allPerks);
    }

    void BuildPerkCards()
    {
        foreach (Transform child in perksBox)
            Destroy(child.gameObject);

        foreach (PerkEntry perk in unlockedPerks)
        {
            if (IsPerkEquippedAnywhere(perk))
                continue;

            GameObject obj = Instantiate(perkCardPrefab, perksBox);
            PerkCardUI card = obj.GetComponent<PerkCardUI>();

            if (card != null)
                card.Setup(perk, this);
        }
    }

    List<PerkEntry> GetPerksForTroop(TroopEntry troop)
    {
        if (troop == null)
            return new List<PerkEntry>();

        if (!troopPerks.ContainsKey(troop.name))
            troopPerks[troop.name] = new List<PerkEntry>();

        return troopPerks[troop.name];
    }

    public void TryEquipPerk(PerkEntry perk)
    {
        if (perk == null) return;

        if (selectedTroop == null)
        {
            Debug.Log("Select a troop first.");
            return;
        }

        if (IsPerkEquippedAnywhere(perk))
            return;

        List<PerkEntry> perks = GetPerksForTroop(selectedTroop);

        if (perks.Count >= 3)
        {
            Debug.Log("This troop already has 3 perks.");
            return;
        }

        perks.Add(perk);

        RefreshSelectedPerkSlots();
        BuildPerkCards();
    }

    public void ClearSelectedTroopPerk(int index)
    {
        if (selectedTroop == null) return;

        List<PerkEntry> perks = GetPerksForTroop(selectedTroop);

        if (index < 0 || index >= perks.Count) return;

        perks.RemoveAt(index);

        RefreshSelectedPerkSlots();
        BuildPerkCards();
    }

    void RefreshSelectedPerkSlots()
    {
        for (int i = 0; i < selectedPerkSlots.Length; i++)
        {
            selectedPerkSlots[i].Setup(i, this);

            PerkEntry perk = null;

            if (selectedTroop != null)
            {
                List<PerkEntry> perks = GetPerksForTroop(selectedTroop);
                if (i < perks.Count)
                    perk = perks[i];
            }

            selectedPerkSlots[i].SetPerk(perk);
        }
    }

    bool IsPerkEquippedAnywhere(PerkEntry perk)
    {
        if (perk == null) return false;

        foreach (var pair in troopPerks)
        {
            foreach (PerkEntry equipped in pair.Value)
            {
                if (equipped != null && equipped.perkId == perk.perkId)
                    return true;
            }
        }

        return false;
    }

    void ClearAll()
    {
        for (int i = 0; i < equippedSquad.Count; i++)
            equippedSquad[i] = null;

        troopPerks.Clear();
        selectedTroop = null;

        ClearSelectedTroopPanel();
        RefreshAll();
    }

    void AutoFill()
    {
        List<TroopEntry> availableTroops = new List<TroopEntry>();

        foreach (TroopEntry troop in ownedTroops)
        {
            if (troop != null && !IsTroopEquipped(troop))
                availableTroops.Add(troop);
        }

        Shuffle(availableTroops);

        int troopIndex = 0;

        for (int i = 0; i < equippedSquad.Count; i++)
        {
            if (equippedSquad[i] == null && troopIndex < availableTroops.Count)
            {
                equippedSquad[i] = availableTroops[troopIndex];
                troopIndex++;
            }
        }

        AutoAssignPerks();

        selectedTroop = null;
        ClearSelectedTroopPanel();
        RefreshAll();
    }

    void AutoAssignPerks()
    {
        List<PerkEntry> availablePerks = new List<PerkEntry>(unlockedPerks);
        Shuffle(availablePerks);

        foreach (TroopEntry troop in equippedSquad)
        {
            if (troop == null) continue;

            if (!troopPerks.ContainsKey(troop.name))
                troopPerks[troop.name] = new List<PerkEntry>();

            while (troopPerks[troop.name].Count < 3 && availablePerks.Count > 0)
            {
                PerkEntry perk = availablePerks[0];
                availablePerks.RemoveAt(0);

                if (!IsPerkEquippedAnywhere(perk))
                    troopPerks[troop.name].Add(perk);
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}