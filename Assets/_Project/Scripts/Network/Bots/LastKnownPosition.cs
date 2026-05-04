using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Компонент для хранения Last Known Position цели.
    /// Используется для предотвращения wallhack у ботов.
    /// </summary>
    public class LastKnownPosition : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _memoryDuration = 3f; // Помнит позицию 3 секунды
        [SerializeField] private float _updateInterval = 0.5f; // Обновляет каждые 0.5s
        [SerializeField] private LayerMask _obstacleLayers; // Стены, препятствия
        
        // Target tracking
        private Transform _target;
        private Vector3 _lastKnownPosition;
        private float _lastSeenTime;
        private float _lastUpdateTime;
        private bool _hasLineOfSight;
        
        // Visual debug
        [Header("Debug")]
        [SerializeField] private bool _showDebugGizmos = false;
        
        /// <summary>
        /// Set target to track.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
            
            if (target != null)
            {
                _lastKnownPosition = target.position;
                _lastSeenTime = Time.time;
                _hasLineOfSight = true;
            }
        }
        
        /// <summary>
        /// Clear target.
        /// </summary>
        public void ClearTarget()
        {
            _target = null;
            _hasLineOfSight = false;
        }
        
        private void Update()
        {
            if (_target == null) return;
            
            // Update LOS periodically (не каждый кадр для оптимизации)
            if (Time.time - _lastUpdateTime >= _updateInterval)
            {
                _lastUpdateTime = Time.time;
                UpdateLineOfSight();
            }
        }
        
        /// <summary>
        /// Update line of sight check.
        /// </summary>
        private void UpdateLineOfSight()
        {
            if (_target == null) return;
            
            Vector3 directionToTarget = _target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            
            // Raycast to check for obstacles
            bool hasLOS = !Physics.Raycast(
                transform.position + Vector3.up, // Start slightly above ground
                directionToTarget.normalized,
                out RaycastHit hit,
                distanceToTarget,
                _obstacleLayers
            );
            
            _hasLineOfSight = hasLOS;
            
            if (hasLOS)
            {
                // Update last known position
                _lastKnownPosition = _target.position;
                _lastSeenTime = Time.time;
            }
            else
            {
                // Lost sight - check if memory expired
                if (Time.time - _lastSeenTime > _memoryDuration)
                {
                    // Забываем позицию
                    _lastKnownPosition = transform.position; // Default to bot's position
                }
            }
        }
        
        /// <summary>
        /// Get position to aim at.
        /// </summary>
        public Vector3 GetAimPosition()
        {
            if (_target == null) return _lastKnownPosition;
            
            // If has LOS - return actual position
            if (_hasLineOfSight)
            {
                return _target.position;
            }
            
            // No LOS - return last known position
            return _lastKnownPosition;
        }
        
        /// <summary>
        /// Check if target is currently visible.
        /// </summary>
        public bool HasLineOfSight()
        {
            return _hasLineOfSight;
        }
        
        /// <summary>
        /// Check if memory is still valid.
        /// </summary>
        public bool IsMemoryValid()
        {
            return Time.time - _lastSeenTime <= _memoryDuration;
        }
        
        /// <summary>
        /// Get time since last sight.
        /// </summary>
        public float GetTimeSinceLastSight()
        {
            return Time.time - _lastSeenTime;
        }
        
        /// <summary>
        /// Force update last known position (для тестирования).
        /// </summary>
        public void ForceUpdatePosition(Vector3 position)
        {
            _lastKnownPosition = position;
            _lastSeenTime = Time.time;
        }
        
        private void OnDrawGizmos()
        {
            if (!_showDebugGizmos) return;
            
            // Draw last known position
            if (_lastKnownPosition != Vector3.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_lastKnownPosition, 0.5f);
                Gizmos.DrawLine(transform.position, _lastKnownPosition);
            }
            
            // Draw line to actual target
            if (_target != null)
            {
                Gizmos.color = _hasLineOfSight ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, _target.position);
            }
        }
    }
}
