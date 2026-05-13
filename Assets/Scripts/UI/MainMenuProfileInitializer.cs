using System.Collections.Generic;
using UnityEngine;

public class MainMenuProfileInitializer : MonoBehaviour
{
    [Header("Default Starting Troops")]
    public List<TroopEntry> startingTroops = new List<TroopEntry>();

    void Awake()
    {
        PlayerProfile.InitializeDefaultProfile(startingTroops);
    }
}