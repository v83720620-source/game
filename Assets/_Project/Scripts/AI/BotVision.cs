using UnityEngine;

/// <summary>
/// Bot vision system.
/// Detects player using raycast and field of view.
/// </summary>
public class BotVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float _sightRange = 30f;
    [SerializeField] private float _sightAngle = 60f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;
    
    [Header("Detection")]
    [SerializeField] private float _detectionTime = 0.5f;
    
    // State
    private Transform _detectedTarget;
    private float _detectionProgress;
    
    // Properties
    public Transform DetectedTarget => _detectedTarget;
    public bool HasTarget => _detectedTarget != null;
    public float DetectionProgress => _detectionProgress;
    
    private void Update()
    {
        CheckForTargets();
    }
    
    private void CheckForTargets()
    {
        // Find all potential targets in range
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, _sightRange, _targetMask);
        
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (Collider target in targetsInRange)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            
            // Check if target is within field of view
            if (angleToTarget < _sightAngle / 2f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                
                // Check if not blocked by obstacle
                if (!Physics.Raycast(transform.position + Vector3.up, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = target.transform;
                    }
                }
            }
        }
        
        // Update detection
        if (closestTarget != null)
        {
            if (_detectedTarget == closestTarget)
            {
                // Already tracking this target
                _detectionProgress = 1f;
            }
            else
            {
                // New target - increase detection progress
                _detectionProgress += Time.deltaTime / _detectionTime;
                
                if (_detectionProgress >= 1f)
                {
                    _detectedTarget = closestTarget;
                    _detectionProgress = 1f;
                }
            }
        }
        else
        {
            // Lost target
            _detectionProgress -= Time.deltaTime / _detectionTime;
            
            if (_detectionProgress <= 0f)
            {
                _detectedTarget = null;
                _detectionProgress = 0f;
            }
        }
    }
    
    /// <summary>
    /// Check if position is visible.
    /// </summary>
    public bool CanSeePosition(Vector3 position)
    {
        Vector3 directionToPosition = (position - transform.position).normalized;
        float distanceToPosition = Vector3.Distance(transform.position, position);
        
        if (distanceToPosition > _sightRange)
            return false;
        
        float angleToPosition = Vector3.Angle(transform.forward, directionToPosition);
        if (angleToPosition > _sightAngle / 2f)
            return false;
        
        // Check obstacles
        return !Physics.Raycast(transform.position + Vector3.up, directionToPosition, distanceToPosition, _obstacleMask);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw sight range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        
        // Draw field of view
        Vector3 leftBoundary = Quaternion.Euler(0, -_sightAngle / 2f, 0) * transform.forward * _sightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, _sightAngle / 2f, 0) * transform.forward * _sightRange;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        // Draw detected target
        if (HasTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _detectedTarget.position);
        }
    }
}
