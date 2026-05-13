using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    [Header("Team")]
    public bool isEnemy;

    [Header("Data")]
    public UnitData data;

    float hpMult = 1f;
    float dmgMult = 1f;

    float stunTimer = 0f;

    float hp;
    float cooldownTimer;
    Unit currentTarget;
    Coroutine hitFlashRoutine;

    SpriteRenderer sr;
    Color originalColor;

    Animator anim;
    bool wasMovingLastFrame = false;
    bool isDying = false;

    public float CurrentHP => hp;
    public float MaxHP => data != null ? data.maxHP * hpMult : 0f;
    public bool IsAlive => hp > 0f;
    public float HpNormalized => (MaxHP <= 0f) ? 0f : hp / MaxHP;
    public bool IsAttackReady => cooldownTimer <= 0f;

    public GameObject bulletTracerPrefab;

    [Header("VFX Points")]
    public Transform floatingTextPoint;

    public void SetRuntimeMultipliers(float hpMultiplier, float damageMultiplier)
    {
        hpMult = Mathf.Max(0.1f, hpMultiplier);
        dmgMult = Mathf.Max(0.1f, damageMultiplier);

        if (data != null)
        {
            float oldMax = data.maxHP;
            if (Mathf.Abs(hp - oldMax) < 0.01f)
                hp = MaxHP;
        }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;

        if (data != null)
        {
            hp = data.maxHP * hpMult;
            cooldownTimer = Random.Range(0f, data.attackCooldown);
        }
    }

    void Update()
    {
        if (isDying || !IsAlive || data == null) return;
        if (CombatState.Paused) return;

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            SetMoving(false);
            return;
        }

        var perkController = GetComponent<PerkController>();
        if (perkController != null)
            perkController.Tick(this, Time.deltaTime);

        cooldownTimer -= Time.deltaTime;

        if (currentTarget == null || !currentTarget.IsAlive)
            currentTarget = FindTarget(perkController);

        if (currentTarget == null)
        {
            SetMoving(false);
            return;
        }

        float dist = Vector2.Distance(transform.position, currentTarget.transform.position);

        float range = data.attackRange;
        if (perkController != null)
            range *= perkController.RangeMultiplier(this);

        if (dist > range)
        {
            SetMoving(true);
            MoveTowards(currentTarget.transform.position, perkController);
            return;
        }

        SetMoving(false);

        var bazooka = GetComponent<BazookaBehaviour>();
        if (bazooka != null)
        {
            bazooka.AttemptAttack(currentTarget);
            return;
        }

        AttemptSingleTargetAttack(currentTarget, perkController);
    }

    Unit FindTarget(PerkController perkController)
    {
        var candidates = new System.Collections.Generic.List<Unit>();

        foreach (var u in GetAllEnemyUnits())
        {
            if (u != null && u.IsAlive && u.data != null)
                candidates.Add(u);
        }

        if (candidates.Count == 0)
            return null;

        // 1) Perks
        if (perkController != null)
        {
            Unit perkPick = perkController.PickTarget(this, candidates);
            if (perkPick != null) return perkPick;
        }

        // 2) SniperBehaviour
        var sniper = GetComponent<SniperBehaviour>();
        if (sniper != null)
        {
            float range = data.attackRange;
            if (perkController != null)
                range *= perkController.RangeMultiplier(this);

            Unit picked = sniper.PickTarget(candidates.ToArray(), range);
            if (picked != null) return picked;
        }

        // 3) Fallback normal
        return FindBestEnemyTarget(candidates.ToArray());
    }

    Unit[] GetAllEnemyUnits()
    {
        string enemyTag = isEnemy ? "Ally" : "Enemy";
        GameObject[] gos = GameObject.FindGameObjectsWithTag(enemyTag);

        Unit[] units = new Unit[gos.Length];
        for (int i = 0; i < gos.Length; i++)
            units[i] = gos[i].GetComponent<Unit>();

        return units;
    }

    Unit FindBestEnemyTarget(Unit[] candidates)
    {
        Unit best = null;
        float bestScore = float.MaxValue;

        foreach (var u in candidates)
        {
            if (u == null || !u.IsAlive || u.data == null) continue;

            float dist = Vector2.Distance(transform.position, u.transform.position);
            float prio = Mathf.Max(0.01f, u.data.targetPriority);
            float score = dist / prio;

            if (score < bestScore)
            {
                bestScore = score;
                best = u;
            }
        }

        return best;
    }

    void MoveTowards(Vector3 targetPos, PerkController perkController)
    {
        float speed = data.moveSpeed;

        if (perkController != null)
            speed *= perkController.MoveSpeedMultiplier(this);

        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void AttemptSingleTargetAttack(Unit target, PerkController perkController)
    {
        if (!IsAttackReady) return;

        float cd = data.attackCooldown;
        float dmg = data.damage;

        if (perkController != null)
        {
            cd *= perkController.CooldownMultiplier(this);
            dmg *= perkController.DamageMultiplier(this);
        }

        cooldownTimer = cd;

        if (anim != null)
            anim.SetTrigger("Shoot");

        SpawnBulletTracer(target);
        target.TakeDamage(dmg);
    }

    void SpawnBulletTracer(Unit target)
    {
        if (bulletTracerPrefab == null || target == null)
            return;

        GameObject tracer = Instantiate(
            bulletTracerPrefab);

        BulletTracer bt = tracer.GetComponent<BulletTracer>();

        if (bt != null)
        {
            bt.Setup(
                transform.position,
                target.transform.position);
        }
    }

    void SetMoving(bool moving)
    {
        if (wasMovingLastFrame == moving) return;

        wasMovingLastFrame = moving;

        if (anim != null)
            anim.SetBool("IsMoving", moving);
    }

    public void ConsumeAttackCooldown()
    {
        if (data == null) return;
        cooldownTimer = data.attackCooldown;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive || isDying) return;

        hp -= amount;

        VFXService.SpawnHit(GetFloatingTextPosition());

        FloatingTextService.SpawnText(
            GetFloatingTextPosition(),
            "-" + Mathf.RoundToInt(amount).ToString(),
            new Color(1f, 0.35f, 0.2f));

        if (hp <= 0f)
        {
            hp = 0f;
            Die();
            return;
        }

        if (sr != null)
        {
            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash(Color.white));
        }
    }

    void Die()
    {
        if (isDying) return;

        isDying = true;
        SetMoving(false);

        if (hitFlashRoutine != null)
        {
            StopCoroutine(hitFlashRoutine);
            hitFlashRoutine = null;
        }

        if (sr != null)
            sr.color = originalColor;

        if (anim != null)
        {
            anim.ResetTrigger("Shoot");
            anim.ResetTrigger("Heal");

            anim.SetBool("IsMoving", false);
            anim.SetTrigger("Die");
        }

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        if (!IsAlive || data == null) return;

        hp += amount;
        FloatingTextService.SpawnText(
            GetFloatingTextPosition(),
            "+" + Mathf.RoundToInt(amount).ToString(),
            Color.green);
        if (hp > MaxHP)
            hp = MaxHP;

        if (sr != null)
        {
            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash(new Color(0.5f, 1f, 0.5f)));
        }
    }

    public void ApplyStun(float seconds)
    {
        stunTimer = Mathf.Max(stunTimer, seconds);
    }

    IEnumerator HitFlash(Color flashColor)
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(0.08f);
        sr.color = originalColor;
    }

    Vector3 GetFloatingTextPosition()
    {
        if (sr != null)
        {
            Bounds b = sr.bounds;
            return new Vector3(
                b.center.x,
                b.max.y + 0.08f,
                transform.position.z
            );
        }

        return transform.position + Vector3.up * 0.3f;
    }
}