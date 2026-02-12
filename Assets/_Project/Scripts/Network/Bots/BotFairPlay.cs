using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Система "честной игры" для ботов.
    /// Предотвращает читерское поведение AI.
    /// </summary>
    public class BotFairPlay : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _enableWallhackPrevention = true;
        [SerializeField] private bool _enableSmokeRespect = true;
        [SerializeField] private bool _enable360NoScopeBan = true;
        
        [Header("Wallhack Prevention")]
        [SerializeField] private float _memoryTimeout = 3f; // Забывает позицию через 3s
        [SerializeField] private LayerMask _wallLayers;
        
        [Header("Aim Smoothing")]
        [SerializeField] private float _minTurnTime = 0.2f; // Минимум 0.2s на разворот
        [SerializeField] private float _aimSmoothSpeed = 5f;
        
        // Last Known Position tracking
        private Vector3 _lastKnownTargetPosition;
        private float _lastSightTime;
        private bool _hasLineOfSight;
        
        // Current target
        private Transform _currentTarget;
        
        /// <summary>
        /// Update target tracking (вызывать из BotAI).
        /// </summary>
        public void UpdateTargetTracking(Transform target)
        {
            _currentTarget = target;
            
            if (target == null)
            {
                _hasLineOfSight = false;
                return;
            }
            
            // Check line of sight
            if (_enableWallhackPrevention)
            {
                _hasLineOfSight = HasLineOfSight(target);
                
                if (_hasLineOfSight)
                {
                    // Обновляем последнюю известную позицию
                    _lastKnownTargetPosition = target.position;
                    _lastSightTime = Time.time;
                }
                else
                {
                    // Забываем позицию через timeout
                    if (Time.time - _lastSightTime > _memoryTimeout)
                    {
                        _lastKnownTargetPosition = Vector3.zero;
                    }
                }
            }
            else
            {
                // Без fair play - всегда знаем позицию (чит!)
                _hasLineOfSight = true;
                _lastKnownTargetPosition = target.position;
            }
        }
        
        /// <summary>
        /// Get target position для aiming (Last Known Position).
        /// </summary>
        public Vector3 GetTargetPosition()
        {
            if (!_enableWallhackPrevention || _hasLineOfSight)
            {
                // Видим цель - стреляем точно
                return _currentTarget != null ? _currentTarget.position : _lastKnownTargetPosition;
            }
            
            // Не видим - используем Last Known Position
            return _lastKnownTargetPosition;
        }
        
        /// <summary>
        /// Check if bot has line of sight to target.
        /// </summary>
        private bool HasLineOfSight(Transform target)
        {
            if (target == null) return false;
            
            Vector3 direction = target.position - transform.position;
            float distance = direction.magnitude;
            
            // Raycast для проверки препятствий
            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, _wallLayers))
            {
                // Что-то между ботом и целью
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Check if bot should respect smoke grenade.
        /// </summary>
        public bool IsTargetInSmoke(Transform target)
        {
            if (!_enableSmokeRespect) return false;
            if (target == null) return false;
            
            // TODO: Проверка на дым (когда будут smoke grenades)
            // - Raycast к цели
            // - Проверка пересечения с smoke volume
            // - Если в дыму → возвращаем true
            
            return false;
        }
        
        /// <summary>
        /// Get smooth aim rotation (предотвращает 360 no-scope).
        /// </summary>
        public Quaternion GetSmoothAimRotation(Vector3 targetPosition, float deltaTime)
        {
            if (!_enable360NoScopeBan)
            {
                // Без fair play - мгновенный поворот
                Vector3 direction = targetPosition - transform.position;
                return Quaternion.LookRotation(direction);
            }
            
            // Smooth rotation к цели
            Vector3 targetDirection = targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            
            // Lerp для плавного поворота
            float smoothSpeed = _aimSmoothSpeed * deltaTime;
            return Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
        }
        
        /// <summary>
        /// Check if bot can instantly turn and shoot (360 no-scope ban).
        /// </summary>
        public bool CanInstantShoot(Vector3 targetPosition)
        {
            if (!_enable360NoScopeBan) return true;
            
            // Проверяем угол к цели
            Vector3 direction = (targetPosition - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, direction);
            
            // Если угол больше 90° - нужно время на разворот
            if (angle > 90f)
            {
                float turnTimeNeeded = angle / (360f / _minTurnTime);
                
                // TODO: Track turn start time и проверять elapsed time
                return false; // Cannot instant shoot
            }
            
            return true;
        }
        
        /// <summary>
        /// Get accuracy multiplier based on fair play rules.
        /// </summary>
        public float GetAccuracyMultiplier()
        {
            float multiplier = 1f;
            
            // Reduce accuracy if no line of sight
            if (_enableWallhackPrevention && !_hasLineOfSight)
            {
                multiplier *= 0.1f; // Стреляет куда-то наугад
            }
            
            // Reduce accuracy if target in smoke
            if (_enableSmokeRespect && _currentTarget != null && IsTargetInSmoke(_currentTarget))
            {
                multiplier *= 0.3f; // Плохая видимость в дыму
            }
            
            return multiplier;
        }
        
        /// <summary>
        /// Should bot shoot right now? (Fair play check).
        /// </summary>
        public bool ShouldShoot(Vector3 targetPosition)
        {
            // No line of sight - don't shoot
            if (_enableWallhackPrevention && !_hasLineOfSight)
            {
                return false;
            }
            
            // In smoke - shoot rarely
            if (_enableSmokeRespect && _currentTarget != null && IsTargetInSmoke(_currentTarget))
            {
                // 30% chance to shoot in smoke
                return Random.value < 0.3f;
            }
            
            // Cannot instant turn - don't shoot
            if (!CanInstantShoot(targetPosition))
            {
                return false;
            }
            
            return true;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_currentTarget != null)
            {
                // Draw line to target
                Gizmos.color = _hasLineOfSight ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, _currentTarget.position);
                
                // Draw last known position
                if (_lastKnownTargetPosition != Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(_lastKnownTargetPosition, 0.5f);
                }
            }
        }
    }
}
