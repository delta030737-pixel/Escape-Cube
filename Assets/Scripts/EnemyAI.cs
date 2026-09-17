using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol, Chase }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Detection Ranges")]
    public float chaseRange = 15f;
    public float loseInterestRange = 22f;
    public float catchRange = 1.5f;

    [Header("Speed")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    [Header("Optimization")]
    public float targetUpdateInterval = 0.2f;

    private NavMeshAgent agent;
    private EnemyVisualEffect visualEffect;
    private EnemyState state = EnemyState.Patrol;
    private int patrolIndex;
    private float targetUpdateTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        visualEffect = GetComponent<EnemyVisualEffect>();

        agent.speed = patrolSpeed;

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError(gameObject.name + ": หา NavMesh ใกล้เคียงไม่เจอเลย เช็คว่า Bake NavMesh ครอบคลุมจุดนี้หรือยัง");
            }
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
            SafeSetDestination(patrolPoints[0].position);
    }

    void SafeSetDestination(Vector3 destination)
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                agent.SetDestination(destination);
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            UpdateState(dist);

            if (state == EnemyState.Chase)
            {
                targetUpdateTimer += Time.deltaTime;
                if (targetUpdateTimer >= targetUpdateInterval)
                {
                    targetUpdateTimer = 0f;
                    SafeSetDestination(player.position);
                }

                if (dist < catchRange)
                {
                    CatchPlayer();
                }
                return;
            }
        }

        Patrol();
    }

    void UpdateState(float dist)
    {
        if (state == EnemyState.Patrol && dist < chaseRange)
        {
            EnterChaseState();
        }
        else if (state == EnemyState.Chase && dist > loseInterestRange)
        {
            EnterPatrolState();
        }
    }

    void EnterChaseState()
    {
        state = EnemyState.Chase;
        agent.speed = chaseSpeed;
        targetUpdateTimer = targetUpdateInterval;
        visualEffect?.SetChasing(true);
    }

    void EnterPatrolState()
    {
        state = EnemyState.Patrol;
        agent.speed = patrolSpeed;
        visualEffect?.SetChasing(false);

        if (patrolPoints != null && patrolPoints.Length > 0)
            SafeSetDestination(patrolPoints[patrolIndex].position);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            SafeSetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void CatchPlayer()
    {
        GameManager.Instance?.LoseGame();
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchRange);
    }
}