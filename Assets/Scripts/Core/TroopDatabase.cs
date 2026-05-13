using System.Collections.Generic;
using UnityEngine;

public class TroopDatabase : MonoBehaviour
{
    public static TroopDatabase Instance { get; private set; }

    [Header("All Troops")]
    public List<TroopEntry> allTroops = new List<TroopEntry>();

    void Awake()
    {
        Instance = this;
    }

    public TroopEntry GetTroopByName(string troopName)
    {
        foreach (TroopEntry troop in allTroops)
        {
            if (troop != null && troop.name == troopName)
                return troop;
        }

        return null;
    }

    public List<TroopEntry> GetTroopsFromNames(string[] names)
    {
        List<TroopEntry> result = new List<TroopEntry>();

        foreach (string name in names)
        {
            TroopEntry troop = GetTroopByName(name);
            if (troop != null)
                result.Add(troop);
        }

        return result;
    }
}