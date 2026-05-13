using UnityEngine;
using System.Collections.Generic;

public class PerkController : MonoBehaviour
{
    [Header("Equipped Perks")]
    public List<Perk> perks = new List<Perk>();

    [Header("Rules")]
    public int maxPerks = 5;
    public bool preventDuplicates = true;

    void Awake()
    {
        EnforceRulesRuntime();
    }

    // IMPORTANTE:
    // No usamos OnValidate para no borrar los huecos null del Inspector
    // mientras estás configurando perks manualmente.

    public void EnforceRulesRuntime()
    {
        // 1) quitar nulls
        perks.RemoveAll(p => p == null);

        // 2) quitar duplicados
        if (preventDuplicates)
        {
            HashSet<Perk> seen = new HashSet<Perk>();
            for (int i = perks.Count - 1; i >= 0; i--)
            {
                if (seen.Contains(perks[i]))
                    perks.RemoveAt(i);
                else
                    seen.Add(perks[i]);
            }
        }

        // 3) limitar cantidad
        if (maxPerks > 0 && perks.Count > maxPerks)
        {
            perks.RemoveRange(maxPerks, perks.Count - maxPerks);
        }
    }

    // ---------------- Targeting ----------------

    public Unit PickTarget(Unit self, List<Unit> candidates)
    {
        for (int i = 0; i < perks.Count; i++)
        {
            if (perks[i] == null) continue;

            Unit picked = perks[i].PickTarget(self, candidates);
            if (picked != null) return picked;
        }

        return null;
    }

    // ---------------- Stat multipliers ----------------

    public float DamageMultiplier(Unit self)
    {
        float m = 1f;
        for (int i = 0; i < perks.Count; i++)
            if (perks[i] != null)
                m *= perks[i].DamageMultiplier(self);

        return m;
    }

    public float CooldownMultiplier(Unit self)
    {
        float m = 1f;
        for (int i = 0; i < perks.Count; i++)
            if (perks[i] != null)
                m *= perks[i].CooldownMultiplier(self);

        return m;
    }

    public float RangeMultiplier(Unit self)
    {
        float m = 1f;
        for (int i = 0; i < perks.Count; i++)
            if (perks[i] != null)
                m *= perks[i].RangeMultiplier(self);

        return m;
    }

    public float MoveSpeedMultiplier(Unit self)
    {
        float m = 1f;
        for (int i = 0; i < perks.Count; i++)
            if (perks[i] != null)
                m *= perks[i].MoveSpeedMultiplier(self);

        return m;
    }

    // ---------------- Active perks ----------------

    public void Tick(Unit self, float dt)
    {
        for (int i = 0; i < perks.Count; i++)
            if (perks[i] != null)
                perks[i].Tick(self, dt);
    }
}