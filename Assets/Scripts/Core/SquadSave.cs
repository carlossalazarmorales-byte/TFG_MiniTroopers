using System.Collections.Generic;
using UnityEngine;

public static class SquadSave
{
    const string KEY = "SAVED_SQUAD";

    public static void SaveSquad(List<TroopEntry> troops)
    {
        if (troops == null || troops.Count == 0)
        {
            PlayerPrefs.DeleteKey(KEY);
            PlayerPrefs.Save();
            return;
        }

        // Guardamos por nombre del asset (TE_Healer, TE_Sniper...)
        // (Más tarde, si quieres, le ponemos IDs más robustos)
        List<string> names = new List<string>();
        foreach (var t in troops)
            if (t != null) names.Add(t.name);

        PlayerPrefs.SetString(KEY, string.Join("|", names));
        PlayerPrefs.Save();
    }

    public static string[] LoadSquadNames()
    {
        string raw = PlayerPrefs.GetString(KEY, "");
        if (string.IsNullOrEmpty(raw)) return new string[0];
        return raw.Split('|');
    }

    public static bool HasSavedSquad()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString(KEY, ""));
    }
}
