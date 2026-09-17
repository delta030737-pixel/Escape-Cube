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
    public float catchRange = 0.8f;
    public LayerMask obstacleMask; 

    [Header("Speed")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;

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
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError(gameObject.name + ": ตัวละครอยู่ไกลจาก NavMesh มากเกินไป ลากวางให้ใกล้พื้นสีฟ้ามากขึ้น");
            }
        }

        SetNextPatrolDestination();
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

                if (dist <= catchRange)
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
        if (state == EnemyState.Patrol && CanSeePlayer(dist))
        {
            EnterChaseState();
        }
        else if (state == EnemyState.Chase && dist > loseInterestRange)
        {
            EnterPatrolState();
        }
    }

    bool CanSeePlayer(float dist)
    {
        if (dist > chaseRange) return false;

        Vector3 eyePosition = transform.position + Vector3.up * 0.5f; 
        Vector3 targetPosition = player.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (targetPosition - eyePosition).normalized;

        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, chaseRange))
        {
            if (hit.transform == player || hit.transform.CompareTag("Player") || hit.transform.CompareTag("MainCamera"))
            {
                return true;
            }
        }

        return false;
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

        SetNextPatrolDestination();
    }

    void Patrol()
    {
        if (agent.isOnNavMesh && !agent.pathPending && agent.hasPath && agent.remainingDistance <= 0.5f)
        {
            SetNextPatrolDestination();
        }
    }

    void SetNextPatrolDestination()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            SafeSetDestination(patrolPoints[patrolIndex].position);
        }
        else
        {
            Vector3 randomPoint = GetRandomNavMeshPoint(transform.position, 15f);
            SafeSetDestination(randomPoint);
        }
    }

    Vector3 GetRandomNavMeshPoint(Vector3 center, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range + center;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
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