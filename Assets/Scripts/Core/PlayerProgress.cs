using UnityEngine;

public static class PlayerProgress
{
    const string GOLD_KEY = "GOLD";

    public static int Gold
    {
        get => PlayerPrefs.GetInt(GOLD_KEY, 0);
        set
        {
            PlayerPrefs.SetInt(GOLD_KEY, Mathf.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount;
    }
}
