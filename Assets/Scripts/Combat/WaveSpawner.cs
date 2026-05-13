using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs by Role")]
    public GameObject[] basicEnemyPrefabs;
    public GameObject[] sniperEnemyPrefabs;
    public GameObject[] bazookaEnemyPrefabs;

    [Header("Spawn Positions")]
    public Transform[] allySpawnPoints;   // 5 puntos
    public Transform[] enemySpawnPoints;  // ideal 10+

    [Header("UI")]
    public TextMeshProUGUI waveText;

    [Header("Timing")]
    public float startDelaySeconds = 3f;
    public float allyMoveSpeed = 1.2f;
    public float enemyMoveSpeed = 1.2f;
    public float offscreenOffset = 4f;

    [Header("Rewards")]
    public int goldPerWaveCleared = 10;

    [Header("Difficulty Scaling (Option B - can stay default for now)")]
    public float hpMultiplierPerWave = 0.06f;      // +6% hp por wave
    public float damageMultiplierPerWave = 0.04f;  // +4% daño por wave

    List<Unit> allies = new List<Unit>();
    List<Unit> enemies = new List<Unit>();

    int wave = 0;
    int goldEarnedThisRun = 0;
    bool finished = false;
    bool busyRoutine = false;
    float matchTimer = 0f;

    void Start()
    {
        CombatState.Paused = true;
        SpawnSelectedAlliesOffscreen();
        StartCoroutine(IntroRoutine());
    }

    void Update()
    {
        if (finished) return;

        matchTimer += Time.deltaTime;

        if (busyRoutine) return;

        allies.RemoveAll(u => u == null);
        enemies.RemoveAll(u => u == null);

        if (allies.Count == 0)
        {
            EndRun();
            return;
        }

        if (enemies.Count == 0 && wave > 0)
        {
            busyRoutine = true;
            StartCoroutine(NextWaveRoutine());
        }
    }

    IEnumerator IntroRoutine()
    {
        busyRoutine = true;

        yield return MoveUnitsToSpawns(allies, allySpawnPoints, allyMoveSpeed);

        wave = 1;
        SpawnEnemiesOffscreen(wave);
        yield return MoveUnitsToSpawns(enemies, enemySpawnPoints, enemyMoveSpeed);

        yield return CountdownRoutine(wave);

        CombatState.Paused = false;
        UpdateWaveHUDCombat();

        busyRoutine = false;
    }

    IEnumerator NextWaveRoutine()
    {
        CombatState.Paused = true;

        goldEarnedThisRun += goldPerWaveCleared;
        PlayerProgress.AddGold(goldPerWaveCleared);

        UpdateWaveHUDBetween(wave + 1);
        yield return MoveUnitsToSpawns(allies, allySpawnPoints, allyMoveSpeed);

        wave++;
        SpawnEnemiesOffscreen(wave);
        yield return MoveUnitsToSpawns(enemies, enemySpawnPoints, enemyMoveSpeed);

        yield return CountdownRoutine(wave);

        CombatState.Paused = false;
        UpdateWaveHUDCombat();

        busyRoutine = false;
    }

    IEnumerator CountdownRoutine(int waveNumber)
    {
        float t = startDelaySeconds;
        while (t > 0f)
        {
            int secs = Mathf.CeilToInt(t);
            if (waveText != null) waveText.text = $"WAVE {waveNumber} - STARTS IN {secs}";
            t -= Time.deltaTime;
            yield return null;
        }
    }

    void UpdateWaveHUDCombat()
    {
        if (waveText != null) waveText.text = $"WAVE {wave}";
    }

    void UpdateWaveHUDBetween(int nextWave)
    {
        if (waveText != null) waveText.text = $"WAVE {nextWave}";
    }

    void EndRun()
    {
        finished = true;
        CombatState.Paused = false;

        int wavesCleared = Mathf.Max(0, wave - 1);

        GameSession.SetLastResult(
            victory: false,
            goldEarned: goldEarnedThisRun,
            survivors: 0,
            wavesCleared: wavesCleared,
            mode: GameMode.WaveMode,
            matchTimeSeconds: matchTimer
        );

        SceneManager.LoadScene("Results");
    }

    // ---------------- Allies spawn ----------------

    void SpawnSelectedAlliesOffscreen()
    {
        allies.Clear();

        // (Opción C más adelante hará esto desde escuadrón guardado)
        if (GameSession.SelectedTroops.Count == 0)
        {
            Debug.LogWarning("No hay escuadrón seleccionado.");
            return;
        }

        int count = Mathf.Min(allySpawnPoints.Length, GameSession.SelectedTroops.Count);

        float minX = allySpawnPoints[0].position.x;
        for (int i = 1; i < allySpawnPoints.Length; i++)
            minX = Mathf.Min(minX, allySpawnPoints[i].position.x);

        float offX = minX - offscreenOffset;

        for (int i = 0; i < count; i++)
        {
            TroopEntry entry = GameSession.SelectedTroops[i];
            if (entry == null || entry.prefab == null) continue;

            Vector3 startPos = allySpawnPoints[i].position;
            startPos.x = offX;

            GameObject go = Instantiate(entry.prefab, startPos, Quaternion.identity);
            go.tag = "Ally";

            Unit u = go.GetComponent<Unit>();
            if (u != null) u.isEnemy = false;

            allies.Add(u);
        }
    }

    // ---------------- Enemies spawn (by role) ----------------

    void SpawnEnemiesOffscreen(int count)
    {
        enemies.Clear();

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogError("No hay enemySpawnPoints en WaveSpawner.");
            return;
        }

        float maxX = enemySpawnPoints[0].position.x;
        for (int i = 1; i < enemySpawnPoints.Length; i++)
            maxX = Mathf.Max(maxX, enemySpawnPoints[i].position.x);

        float offX = maxX + offscreenOffset;

        int actual = Mathf.Min(count, enemySpawnPoints.Length);

        // spawnpoints aleatorios sin repetir
        List<int> indices = new List<int>();
        for (int i = 0; i < enemySpawnPoints.Length; i++) indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        // pool según wave:
        // 1-3: basic
        // 4-6: basic + sniper
        // 7+: basic + sniper + healer
        List<GameObject> pool = BuildEnemyPoolForWave(wave);

        if (pool.Count == 0)
        {
            Debug.LogError("Enemy pool vacío. Revisa arrays basic/sniper/healer en WaveSpawner.");
            return;
        }

        for (int i = 0; i < actual; i++)
        {
            Transform targetSpawn = enemySpawnPoints[indices[i]];

            Vector3 startPos = targetSpawn.position;
            startPos.x = offX;

            GameObject chosen = pool[Random.Range(0, pool.Count)];
            GameObject go = Instantiate(chosen, startPos, Quaternion.identity);
            go.tag = "Enemy";

            Unit u = go.GetComponent<Unit>();
            if (u != null)
            {
                u.isEnemy = true;

                // Opción B: escalado por wave (puede quedarse, no molesta)
                float hpMult = 1f + hpMultiplierPerWave * Mathf.Max(0, wave - 1);
                float dmgMult = 1f + damageMultiplierPerWave * Mathf.Max(0, wave - 1);
                u.SetRuntimeMultipliers(hpMult, dmgMult);
            }

            enemies.Add(u);
        }
    }

    List<GameObject> BuildEnemyPoolForWave(int w)
    {
        List<GameObject> pool = new List<GameObject>();

        void AddAll(GameObject[] arr)
        {
            if (arr == null) return;
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] != null) pool.Add(arr[i]);
        }

        // 1-3: básicos (incluye healer)
        AddAll(basicEnemyPrefabs);

        // 4-6: + sniper
        if (w >= 4) AddAll(sniperEnemyPrefabs);

        // 7+: + bazooka (más chetado)
        if (w >= 7) AddAll(bazookaEnemyPrefabs);

        return pool;
    }


    // ---------------- Movement (walking) ----------------

    IEnumerator MoveUnitsToSpawns(List<Unit> units, Transform[] spawnPoints, float speed)
    {
        int count = Mathf.Min(units.Count, spawnPoints.Length);

        // Desactivar Unit mientras caminan (evita interferencias)
        for (int i = 0; i < count; i++)
        {
            if (units[i] != null)
            {
                units[i].enabled = false;

                Animator anim = units[i].GetComponent<Animator>();
                if (anim != null)
                    anim.SetBool("IsMoving", true);
            }
        }

        Vector3[] targets = new Vector3[count];
        for (int i = 0; i < count; i++)
            targets[i] = spawnPoints[i].position;

        bool anyMoving = true;
        while (anyMoving)
        {
            anyMoving = false;

            for (int i = 0; i < count; i++)
            {
                Unit u = units[i];
                if (u == null) continue;

                Vector3 newPos = Vector3.MoveTowards(u.transform.position, targets[i], speed * Time.deltaTime);
                u.transform.position = newPos;

                if ((newPos - targets[i]).sqrMagnitude > 0.0001f)
                    anyMoving = true;
            }

            yield return null;
        }

        for (int i = 0; i < count; i++)
        {
            if (units[i] != null)
            {
                Animator anim = units[i].GetComponent<Animator>();
                if (anim != null)
                    anim.SetBool("IsMoving", false);

                units[i].enabled = true;
            }
        }
    }
}
