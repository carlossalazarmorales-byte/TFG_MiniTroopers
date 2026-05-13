using System.Collections.Generic;
using UnityEngine;

public static class PlayerProfile
{
    const string OWNED_TROOPS_KEY = "PLAYER_OWNED_TROOPS";
    const string EQUIPPED_SQUAD_KEY = "PLAYER_EQUIPPED_SQUAD";
    const string TROOP_PERKS_KEY = "PLAYER_TROOP_PERKS";
    const string OWNED_PERKS_KEY = "PLAYER_OWNED_PERKS";

    public static void InitializeDefaultProfile(List<TroopEntry> defaultTroops)
    {
        if (!HasOwnedTroops())
        {
            SaveOwnedTroops(defaultTroops);
            SaveEquippedSquad(defaultTroops);
        }
    }

    public static bool HasOwnedTroops()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString(OWNED_TROOPS_KEY, ""));
    }

    public static void SaveOwnedTroops(List<TroopEntry> troops)
    {
        PlayerPrefs.SetString(OWNED_TROOPS_KEY, TroopsToString(troops));
        PlayerPrefs.Save();
    }

    public static void SaveEquippedSquad(List<TroopEntry> troops)
    {
        PlayerPrefs.SetString(EQUIPPED_SQUAD_KEY, TroopsToString(troops));
        PlayerPrefs.Save();
    }

    public static string[] LoadOwnedTroopNames()
    {
        return LoadNames(OWNED_TROOPS_KEY);
    }

    public static string[] LoadEquippedSquadNames()
    {
        return LoadNames(EQUIPPED_SQUAD_KEY);
    }

    static string[] LoadNames(string key)
    {
        string raw = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(raw))
            return new string[0];

        return raw.Split('|');
    }

    static string TroopsToString(List<TroopEntry> troops)
    {
        if (troops == null || troops.Count == 0)
            return "";

        List<string> names = new List<string>();

        foreach (TroopEntry troop in troops)
        {
            if (troop != null)
                names.Add(troop.name);
        }

        return string.Join("|", names);
    }

    public static void ResetProfile()
    {
        PlayerPrefs.DeleteKey(OWNED_TROOPS_KEY);
        PlayerPrefs.DeleteKey(EQUIPPED_SQUAD_KEY);
        PlayerPrefs.Save();
    }

    public static void SaveTroopPerks(Dictionary<string, List<PerkEntry>> troopPerks)
    {
        List<string> entries = new List<string>();

        foreach (var pair in troopPerks)
        {
            string troopName = pair.Key;

            List<string> perkIds = new List<string>();
            foreach (PerkEntry perk in pair.Value)
            {
                if (perk != null)
                    perkIds.Add(perk.perkId);
            }

            entries.Add(troopName + ":" + string.Join(",", perkIds));
        }

        PlayerPrefs.SetString(TROOP_PERKS_KEY, string.Join("|", entries));
        PlayerPrefs.Save();
    }

    public static Dictionary<string, List<string>> LoadTroopPerkIds()
    {
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

        string raw = PlayerPrefs.GetString(TROOP_PERKS_KEY, "");
        if (string.IsNullOrEmpty(raw))
            return result;

        string[] troopEntries = raw.Split('|');

        foreach (string entry in troopEntries)
        {
            if (string.IsNullOrEmpty(entry)) continue;

            string[] parts = entry.Split(':');
            if (parts.Length != 2) continue;

            string troopName = parts[0];
            string[] perkIds = parts[1].Split(',');

            result[troopName] = new List<string>();

            foreach (string id in perkIds)
            {
                if (!string.IsNullOrEmpty(id))
                    result[troopName].Add(id);
            }
        }

        return result;
    }

    public static void SaveOwnedPerks(List<PerkEntry> perks)
    {
        List<string> ids = new List<string>();

        foreach (PerkEntry perk in perks)
        {
            if (perk != null)
                ids.Add(perk.perkId);
        }

        PlayerPrefs.SetString(OWNED_PERKS_KEY, string.Join("|", ids));
        PlayerPrefs.Save();
    }

    public static string[] LoadOwnedPerkIds()
    {
        string raw = PlayerPrefs.GetString(OWNED_PERKS_KEY, "");

        if (string.IsNullOrEmpty(raw))
            return new string[0];

        return raw.Split('|');
    }
}