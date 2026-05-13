using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs (aleatorio)")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Positions")]
    public Transform[] allySpawnPoints;
    public Transform[] enemySpawnPoints;

    [Header("Rewards")]
    public int goldPerSurvivor = 10;

    List<Unit> allies = new List<Unit>();
    List<Unit> enemies = new List<Unit>();
    bool finished = false;

    void Start()
    {
        SpawnSelectedAllies();
        SpawnTeamRandom(enemyPrefabs, enemySpawnPoints, true, enemies, "Enemy");
    }

    void Update()
    {
        if (finished) return;

        allies.RemoveAll(u => u == null);
        enemies.RemoveAll(u => u == null);

        if (allies.Count == 0)
            EndMission(victory: false);
        else if (enemies.Count == 0)
            EndMission(victory: true);
    }

    void EndMission(bool victory)
    {
        finished = true;

        int survivors = allies.Count;
        int earned = 0;

        if (victory)
        {
            earned = survivors * goldPerSurvivor;
            PlayerProgress.AddGold(earned);
        }

        GameSession.SetLastResult(victory, earned, survivors, 0, GameMode.TestMission, 0f);

        SceneManager.LoadScene("Results");
    }

    void SpawnSelectedAllies()
    {
        if (GameSession.SelectedTroops.Count == 0)
        {
            Debug.LogWarning("No hay escuadrón seleccionado. No se spawnean aliados.");
            return;
        }

        int count = Mathf.Min(allySpawnPoints.Length, GameSession.SelectedTroops.Count);

        for (int i = 0; i < count; i++)
        {
            TroopEntry entry = GameSession.SelectedTroops[i];
            if (entry == null || entry.prefab == null) continue;

            Transform p = allySpawnPoints[i];
            GameObject go = Instantiate(entry.prefab, p.position, Quaternion.identity);
            go.tag = "Ally";

            Unit u = go.GetComponent<Unit>();
            if (u != null) u.isEnemy = false;

            allies.Add(u);
        }
    }

    void SpawnTeamRandom(
        GameObject[] prefabs,
        Transform[] points,
        bool isEnemy,
        List<Unit> list,
        string tag
    )
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs asignados en el spawner para " + (isEnemy ? "Enemy" : "Ally"));
            return;
        }

        foreach (var p in points)
        {
            GameObject chosen = prefabs[Random.Range(0, prefabs.Length)];
            GameObject go = Instantiate(chosen, p.position, Quaternion.identity);

            go.tag = tag;

            Unit u = go.GetComponent<Unit>();
            if (u != null) u.isEnemy = isEnemy;

            list.Add(u);
        }
    }
}
