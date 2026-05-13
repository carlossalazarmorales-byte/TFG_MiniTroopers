using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuModesUI : MonoBehaviour
{
    public void StartWaveMode()
    {
        GameSession.NextMode = GameMode.WaveMode;

        List<TroopEntry> squad = LoadEquippedSquad();

        GameSession.SelectedTroops.Clear();
        GameSession.SelectedTroops.AddRange(squad);

        SceneManager.LoadScene("WaveMission");
    }

    public void StartTestMission()
    {
        GameSession.NextMode = GameMode.TestMission;
        SceneManager.LoadScene("SquadSetup");
    }

    public void OpenSquadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SquadMenu");
    }

    List<TroopEntry> LoadEquippedSquad()
    {
        if (TroopDatabase.Instance == null)
        {
            Debug.LogError("No TroopDatabase found in scene.");
            return new List<TroopEntry>();
        }

        string[] names = PlayerProfile.LoadEquippedSquadNames();
        List<TroopEntry> squad = TroopDatabase.Instance.GetTroopsFromNames(names);

        return squad;
    }

    public void OpenShop()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
    }
}