using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    TestMission = 0,
    WaveMode = 1
}

public static class GameSession
{
    public static readonly List<TroopEntry> SelectedTroops = new List<TroopEntry>();

    // Qué modo vamos a jugar (se decide en MainMenu)
    public static GameMode NextMode = GameMode.TestMission;

    // Resultado de la última partida
    public static bool LastVictory = false;
    public static int LastGoldEarned = 0;
    public static int LastSurvivors = 0;

    // Extra para Wave Mode
    public static int LastWavesCleared = 0;
    public static GameMode LastMode = GameMode.TestMission;
    public static float LastMatchTimeSeconds = 0f;

    public static void ClearSquad()
    {
        SelectedTroops.Clear();
    }

    public static void AddToSquad(TroopEntry troop, int maxSize)
    {
        if (troop == null || troop.prefab == null) return;
        if (SelectedTroops.Count >= maxSize) return;
        SelectedTroops.Add(troop);
    }

    public static void RemoveAt(int index)
    {
        if (index < 0 || index >= SelectedTroops.Count) return;
        SelectedTroops.RemoveAt(index);
    }

    public static void SetLastResult(bool victory, int goldEarned, int survivors, int wavesCleared, GameMode mode, float matchTimeSeconds)
    {
        LastVictory = victory;
        LastGoldEarned = Mathf.Max(0, goldEarned);
        LastSurvivors = Mathf.Max(0, survivors);
        LastWavesCleared = Mathf.Max(0, wavesCleared);
        LastMode = mode;
        LastMatchTimeSeconds = Mathf.Max(0f, matchTimeSeconds);
    }
}
