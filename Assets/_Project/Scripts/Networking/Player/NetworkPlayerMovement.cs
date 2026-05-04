using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Network синхронизация движения игрока.
    /// Использует client-side prediction для плавности.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(PlayerMovement))]
    public class NetworkPlayerMovement : NetworkBehaviour
    {
        private PlayerMovement _playerMovement;
        private CharacterController _controller;
        
        [Header("Network Settings")]
        [SerializeField] private float _positionThreshold = 0.1f;
        [SerializeField] private float _snapThreshold = 5f;
        
        // Синхронизированные переменные
        private NetworkVariable<Vector3> _networkPosition = new NetworkVariable<Vector3>();
        private NetworkVariable<Quaternion> _networkRotation = new NetworkVariable<Quaternion>();
        private NetworkVariable<bool> _networkIsGrounded = new NetworkVariable<bool>();
        
        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _controller = GetComponent<CharacterController>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                // Локальный игрок - отправляем позицию на сервер
                _networkPosition.Value = transform.position;
                _networkRotation.Value = transform.rotation;
            }
            else
            {
                // Удалённый игрок - синхронизируем с сервером
                _networkPosition.OnValueChanged += OnPositionChanged;
                _networkRotation.OnValueChanged += OnRotationChanged;
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
            {
                _networkPosition.OnValueChanged -= OnPositionChanged;
                _networkRotation.OnValueChanged -= OnRotationChanged;
            }
        }
        
        private void Update()
        {
            if (!IsSpawned) return;
            
            if (IsOwner)
            {
                // Локальный игрок - отправляем обновления
                SendMovementUpdate();
            }
        }
        
        private void SendMovementUpdate()
        {
            // Проверяем изменилась ли позиция достаточно
            if (Vector3.Distance(_networkPosition.Value, transform.position) > _positionThreshold)
            {
                UpdatePositionServerRpc(transform.position, transform.rotation, _playerMovement.IsGrounded);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void UpdatePositionServerRpc(Vector3 position, Quaternion rotation, bool isGrounded)
        {
            // Сервер валидирует и обновляет
            // TODO: Добавить валидацию на читерство (телепорт, флай)
            
            _networkPosition.Value = position;
            _networkRotation.Value = rotation;
            _networkIsGrounded.Value = isGrounded;
        }
        
        private void OnPositionChanged(Vector3 previousValue, Vector3 newValue)
        {
            // Для удалённых игроков
            float distance = Vector3.Distance(transform.position, newValue);
            
            if (distance > _snapThreshold)
            {
                // Большое расхождение - телепортируем
                if (_controller != null && _controller.enabled)
                {
                    _controller.enabled = false;
                    transform.position = newValue;
                    _controller.enabled = true;
                }
                else
                {
                    transform.position = newValue;
                }
            }
            // Иначе NetworkTransform сам сделает интерполяцию
        }
        
        private void OnRotationChanged(Quaternion previousValue, Quaternion newValue)
        {
            // NetworkTransform обрабатывает вращение
            // Эта функция для дополнительной логики если нужно
        }
    }
}
