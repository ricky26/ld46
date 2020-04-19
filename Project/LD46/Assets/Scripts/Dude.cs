using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Dude: MonoBehaviour, IAttackable
{
    public Squad squad;
    public float attackRange = 3.0f;
    public float attackDamage = 1.0f;
    public float attackInterval = 1.0f;
    public bool canAttack = true;
    public GameObject attackPrefab;
    public Transform visual;
    public DudeLoadout loadout;

    private NavMeshAgent navAgent;
    private NavMeshPath navPath;
    private Healthy health;
    private Dude lastTarget;
    private float lastPathUpdate;
    private const float pathUpdateInterval = 0.5f;
    private float lastAttack;

    public Vector3 AttackPosition
    {
        get
        {
            return transform.position + Vector3.up * 0.5f;
        }
    }

    public bool IsValidTarget => visual.gameObject.activeInHierarchy;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<Healthy>();
    }

    private bool IsInRange(Dude otherDude)
    {
        var sqrAttackRange = attackRange * attackRange;
        var sqrRange = (otherDude.transform.position - transform.position).sqrMagnitude;
        return sqrRange <= sqrAttackRange;
    }

    private void Update()
    {
        var now = Time.time;
        var pathDelta = now - lastPathUpdate;
        var attackDelta = now - lastAttack;
        
        if (squad?.team?.layDownArms ?? false)
        {
            return;
        }

        if ((pathDelta >= pathUpdateInterval) && (lastTarget == null || !IsInRange(lastTarget)))
        {
            lastPathUpdate = now;

            var enemyDudes = (squad?.team?.EnemyDudes) ?? Array.Empty<Dude>();
            var bestPath = enemyDudes
                .Where(x => x && x.isActiveAndEnabled && x.IsValidTarget)
                .OrderBy(dude => (transform.position - dude.transform.position).sqrMagnitude)
                .Take(3)
                .Select(dude =>
                {
                    var path = new NavMeshPath();
                    if (navAgent.CalculatePath(dude.transform.position, path))
                    {
                        var dist = Enumerable.Range(0, path.corners.Length - 1)
                            .Select(idx => (path.corners[idx + 1] - path.corners[idx]).sqrMagnitude)
                            .Sum();
                        return new { dude, path, dist };
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where(x => x != null)
                .OrderBy(x => x.dist)
                .FirstOrDefault();

            if (squad?.goalTransform && ((bestPath == null) || (bestPath.dist > attackRange)))
            {
                navAgent.SetDestination(squad.goalTransform.position);
            }
            else if (bestPath != null)
            {
                lastTarget = bestPath.dude;
                navPath = lastTarget.navPath;
                navAgent.SetDestination(lastTarget.transform.position);
            }
        }

        if ((attackDelta >= attackInterval) && (lastTarget != null) && IsInRange(lastTarget))
        {
            lastAttack = now;

            var healthy = lastTarget.GetComponent<Healthy>();
            if (healthy)
            {
                healthy.Damage(attackDamage, this);
            }

            if (attackPrefab)
            {
                var attackGo = Instantiate(attackPrefab, transform.position, Quaternion.identity);
                var lineRenderer = attackGo.GetComponentInChildren<LineRenderer>();
                if (lineRenderer)
                {
                    var attackPos = lastTarget.transform.position;
                    var lastAttackable = lastTarget as IAttackable;
                    if (lastAttackable != null)
                    {
                        attackPos = lastAttackable.AttackPosition;
                    }

                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPositions(new[]
                    {
                        AttackPosition,
                        lastTarget.AttackPosition,
                    });
                }
            }
        }
    }
}