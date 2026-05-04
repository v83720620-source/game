using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Bot AI controller.
/// Handles patrol, chase, and attack behaviors.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BotVision))]
[RequireComponent(typeof(BotWeapon))]
public class BotAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float _patrolRadius = 20f;
    [SerializeField] private float _patrolWaitTime = 2f;
    [SerializeField] private float _chaseDistance = 30f;
    [SerializeField] private float _attackDistance = 20f;
    [SerializeField] private float _stopDistance = 10f;
    
    [Header("Patrol Points")]
    [SerializeField] private Transform[] _patrolPoints;
    
    // Components
    private NavMeshAgent _agent;
    private BotVision _vision;
    private BotWeapon _weapon;
    private PlayerHealth _health;
    
    // State
    private BotState _currentState;
    private Vector3 _patrolDestination;
    private float _patrolWaitTimer;
    private int _currentPatrolIndex;
    
    private enum BotState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<BotVision>();
        _weapon = GetComponent<BotWeapon>();
        _health = GetComponent<PlayerHealth>();
    }
    
    private void Start()
    {
        _currentState = BotState.Patrol;
        SetNewPatrolDestination();
        
        // Subscribe to death
        if (_health != null)
        {
            _health.OnDeath += HandleDeath;
        }
    }
    
    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDeath -= HandleDeath;
        }
    }
    
    private void Update()
    {
        if (_health != null && _health.IsDead)
            return;
        
        UpdateAI();
    }
    
    private void UpdateAI()
    {
        // Check for target
        if (_vision.HasTarget)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _vision.DetectedTarget.position);
            
            if (distanceToTarget <= _attackDistance)
            {
                _currentState = BotState.Attack;
            }
            else if (distanceToTarget <= _chaseDistance)
            {
                _currentState = BotState.Chase;
            }
        }
        else
        {
            if (_currentState == BotState.Chase || _currentState == BotState.Attack)
            {
                _currentState = BotState.Patrol;
                SetNewPatrolDestination();
            }
        }
        
        // Execute state behavior
        switch (_currentState)
        {
            case BotState.Idle:
                HandleIdle();
                break;
            
            case BotState.Patrol:
                HandlePatrol();
                break;
            
            case BotState.Chase:
                HandleChase();
                break;
            
            case BotState.Attack:
                HandleAttack();
                break;
        }
    }
    
    private void HandleIdle()
    {
        _agent.isStopped = true;
    }
    
    private void HandlePatrol()
    {
        _agent.isStopped = false;
        
        // Check if reached destination
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _patrolWaitTimer += Time.deltaTime;
            
            if (_patrolWaitTimer >= _patrolWaitTime)
            {
                SetNewPatrolDestination();
                _patrolWaitTimer = 0f;
            }
        }
    }
    
    private void HandleChase()
    {
        if (!_vision.HasTarget)
            return;
        
        _agent.isStopped = false;
        _agent.SetDestination(_vision.DetectedTarget.position);
    }
    
    private void HandleAttack()
    {
        if (!_vision.HasTarget)
            return;
        
        float distanceToTarget = Vector3.Distance(transform.position, _vision.DetectedTarget.position);
        
        // Stop moving if close enough
        if (distanceToTarget <= _stopDistance)
        {
            _agent.isStopped = true;
        }
        else
        {
            _agent.isStopped = false;
            _agent.SetDestination(_vision.DetectedTarget.position);
        }
        
        // Aim at target
        Vector3 targetPosition = _vision.DetectedTarget.position + Vector3.up;
        _weapon.AimAt(targetPosition);
        
        // Shoot
        if (_weapon.CanFire && _vision.CanSeePosition(targetPosition))
        {
            _weapon.TryShootAt(targetPosition);
        }
    }
    
    private void SetNewPatrolDestination()
    {
        if (_patrolPoints != null && _patrolPoints.Length > 0)
        {
            // Use patrol points
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
        }
        else
        {
            // Random patrol
            Vector3 randomDirection = Random.insideUnitSphere * _patrolRadius;
            randomDirection += transform.position;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, _patrolRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }
    
    private void HandleDeath(GameObject killer)
    {
        _agent.isStopped = true;
        enabled = false;
        
        string killerName = killer != null ? killer.name : "Unknown";
        Debug.Log($"Bot {gameObject.name} died! Killed by: {killerName}");
        
        // TODO: Play death animation, disable colliders, etc.
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw patrol radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _patrolRadius);
        
        // Draw chase distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        
        // Draw attack distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackDistance);
        
        // Draw patrol points
        if (_patrolPoints != null && _patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            foreach (Transform point in _patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.5f);
                }
            }
        }
    }
}
