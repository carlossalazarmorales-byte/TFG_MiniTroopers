using System.Collections.Generic;
using UnityEngine;

public class PerkDatabase : MonoBehaviour
{
    public static PerkDatabase Instance { get; private set; }

    public List<PerkEntry> allPerks = new List<PerkEntry>();

    void Awake()
    {
        Instance = this;
    }

    public PerkEntry GetPerkById(string id)
    {
        foreach (PerkEntry perk in allPerks)
        {
            if (perk != null && perk.perkId == id)
                return perk;
        }

        return null;
    }
}