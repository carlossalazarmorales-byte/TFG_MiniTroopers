using UnityEngine;
using System.Collections.Generic;

public class HealerBehaviour : MonoBehaviour
{
    [Header("Heal")]
    public float healAmount = 35f;
    public float healInterval = 3.0f;
    public float healRange = 0.75f;

    [Header("Movement")]
    public float stopDistance = 0.65f;

    [Range(0f, 1f)]
    public float healChance = 1.0f;

    Unit self;
    Animator anim;
    float timer;
    Unit currentHealTarget;

    void Awake()
    {
        self = GetComponent<Unit>();
        anim = GetComponent<Animator>();
        timer = Random.Range(0f, healInterval);
    }

    void Update()
    {
        if (CombatState.Paused) return;
        if (self == null || !self.IsAlive) return;

        timer -= Time.deltaTime;

        if (currentHealTarget == null || !currentHealTarget.IsAlive || currentHealTarget.CurrentHP >= currentHealTarget.MaxHP - 0.01f)
            currentHealTarget = PickRandomWoundedAlly();

        if (currentHealTarget == null)
            return;

        float dist = Vector2.Distance(transform.position, currentHealTarget.transform.position);

        if (dist > stopDistance)
        {
            MoveTowards(currentHealTarget.transform.position);
            SetMoving(true);
            return;
        }

        SetMoving(false);

        if (timer > 0f) return;

        timer = healInterval;

        if (Random.value > healChance) return;

        HealTarget(currentHealTarget);
    }

    void HealTarget(Unit target)
    {
        if (target == null || !target.IsAlive) return;

        if (anim != null)
            anim.SetTrigger("Heal");

        target.Heal(healAmount);
        VFXService.SpawnHeal(target.transform.position + Vector3.up * 0.55f);
    }

    Unit PickRandomWoundedAlly()
    {
        string allyTag = self.isEnemy ? "Enemy" : "Ally";
        GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);

        List<Unit> wounded = new List<Unit>();

        foreach (var go in allies)
        {
            Unit u = go.GetComponent<Unit>();
            if (u == null || !u.IsAlive) continue;
            if (u.CurrentHP >= u.MaxHP - 0.01f) continue;

            wounded.Add(u);
        }

        if (wounded.Count == 0) return null;

        return wounded[Random.Range(0, wounded.Count)];
    }

    void MoveTowards(Vector3 targetPos)
    {
        float speed = self.data != null ? self.data.moveSpeed : 2f;
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void SetMoving(bool moving)
    {
        if (anim != null)
            anim.SetBool("IsMoving", moving);
    }
}