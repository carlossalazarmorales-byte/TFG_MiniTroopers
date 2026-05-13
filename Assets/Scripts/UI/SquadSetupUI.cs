using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SquadSetupUI : MonoBehaviour
{
    [Header("Config (se ajusta según modo)")]
    public int testMissionSquadSize = 3;
    public int waveModeSquadSize = 5;

    public List<TroopEntry> availableTroops = new List<TroopEntry>();

    [Header("UI References")]
    public Transform troopButtonContainer;
    public Button troopButtonTemplate;

    public TextMeshProUGUI[] squadSlotTexts; // pon Size 5 en Inspector
    public Button startMissionButton;
    public Button clearButton;

    int squadSize;

    void Start()
    {
        squadSize = (GameSession.NextMode == GameMode.WaveMode) ? waveModeSquadSize : testMissionSquadSize;

        GameSession.ClearSquad();
        BuildTroopButtons();
        RefreshSlots();

        startMissionButton.onClick.AddListener(OnStart);
        clearButton.onClick.AddListener(OnClear);
    }

    void BuildTroopButtons()
    {
        for (int i = troopButtonContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = troopButtonContainer.GetChild(i);
            if (child != troopButtonTemplate.transform)
                Destroy(child.gameObject);
        }

        foreach (var troop in availableTroops)
        {
            if (troop == null || troop.prefab == null) continue;

            var btn = Instantiate(troopButtonTemplate, troopButtonContainer);
            btn.gameObject.SetActive(true);

            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = troop.displayName;

            btn.onClick.AddListener(() =>
            {
                GameSession.AddToSquad(troop, squadSize);
                RefreshSlots();
            });
        }
    }

    void RefreshSlots()
    {
        // mostramos hasta 5 slots, pero solo usamos squadSize según modo
        for (int i = 0; i < squadSlotTexts.Length; i++)
        {
            if (i >= squadSize)
            {
                squadSlotTexts[i].text = "";
                continue;
            }

            if (i < GameSession.SelectedTroops.Count && GameSession.SelectedTroops[i] != null)
                squadSlotTexts[i].text = $"Slot {i + 1}: {GameSession.SelectedTroops[i].displayName}";
            else
                squadSlotTexts[i].text = $"Slot {i + 1}: (vacío)";
        }

        startMissionButton.interactable = GameSession.SelectedTroops.Count > 0;
    }

    void OnClear()
    {
        GameSession.ClearSquad();
        RefreshSlots();
    }

    void OnStart()
    {
        if (GameSession.NextMode == GameMode.WaveMode)
        {
            SquadSave.SaveSquad(GameSession.SelectedTroops);
            SceneManager.LoadScene("WaveMission");
        }
        else
        {
            SceneManager.LoadScene("Mission");
        }
    }
}
