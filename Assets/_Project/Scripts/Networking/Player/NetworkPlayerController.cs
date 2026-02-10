using Unity.Netcode;
using UnityEngine;
using FlumpGame.Network.Match;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Главный контроллер network игрока.
    /// Управляет ownership, authority и координирует все network компоненты.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private FirstPersonCamera _camera;
        [SerializeField] private AdvancedWeapon _weapon;
        
        [Header("Network Components")]
        private NetworkPlayerMovement _networkMovement;
        private NetworkPlayerHealth _networkHealth;
        private NetworkWeapon _networkWeapon;
        
        [Header("Visual")]
        [SerializeField] private GameObject _localPlayerVisuals;
        [SerializeField] private GameObject _remotePlayerVisuals;
        [SerializeField] private GameObject _weaponObject; // Weapon GameObject для скрытия на удалённых игроках
        
        private void Awake()
        {
            // Кэшируем network компоненты
            _networkMovement = GetComponent<NetworkPlayerMovement>();
            _networkHealth = GetComponent<NetworkPlayerHealth>();
            _networkWeapon = GetComponent<NetworkWeapon>();
            
            // Кэшируем single-player компоненты
            if (_playerMovement == null)
                _playerMovement = GetComponent<PlayerMovement>();
            if (_playerHealth == null)
                _playerHealth = GetComponent<PlayerHealth>();
            if (_camera == null)
                _camera = GetComponentInChildren<FirstPersonCamera>();
            if (_weapon == null && _weaponObject != null)
                _weapon = _weaponObject.GetComponent<AdvancedWeapon>();
        }
        
        public override void OnNetworkSpawn()
        {
            // ВАЖНО: Инициализируем _weaponObject ДО setup если он не назначен
            if (_weaponObject == null && _weapon != null)
            {
                _weaponObject = _weapon.gameObject;
                Debug.Log("[NetworkPlayer] _weaponObject auto-assigned from _weapon");
            }
            
            // Вызывается когда объект spawned в сети
            if (IsOwner)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupRemotePlayer();
            }
            
            Debug.Log($"[NetworkPlayer] Spawned! IsOwner: {IsOwner}, IsServer: {IsServer}, ClientId: {OwnerClientId}");
        }
        
        private void LateUpdate()
        {
            // АГРЕССИВНАЯ ПРОВЕРКА: Убедимся что оружие в правильном состоянии КАЖДЫЙ КАДР
            // Только для локального игрока (для remote players weapon уже Destroy'ed)
            if (IsOwner && _weaponObject != null)
            {
                if (!_weaponObject.activeSelf)
                {
                    _weaponObject.SetActive(true);
                    Debug.LogWarning($"[NetworkPlayer] FORCED weapon ENABLED for LOCAL player (ClientId: {OwnerClientId})");
                }
            }
        }
        
        private void SetupLocalPlayer()
        {
            // Это наш локальный игрок
            Debug.Log("[NetworkPlayer] Setting up LOCAL player");
            
            // Включаем управление
            if (_playerMovement != null)
                _playerMovement.enabled = true;
            
            // Включаем камеру
            if (_camera != null)
            {
                _camera.enabled = true;
                
                // Включаем AudioListener только для локального игрока
                AudioListener audioListener = _camera.GetComponentInChildren<AudioListener>();
                if (audioListener != null)
                {
                    audioListener.enabled = true;
                    Debug.Log("[NetworkPlayer] AudioListener ENABLED for local player");
                }
            }
            
            // Включаем NetworkWeapon
            if (_networkWeapon != null)
                _networkWeapon.enabled = true;
            
            // Включаем AdvancedWeapon (single-player компонент для эффектов)
            if (_weapon != null)
            {
                _weapon.enabled = true;
                Debug.Log("[NetworkPlayer] AdvancedWeapon ENABLED for LOCAL player");
            }
            
            // ПОКАЗЫВАЕМ Weapon GameObject для локального игрока
            if (_weaponObject != null)
            {
                _weaponObject.SetActive(true);
                Debug.Log($"[NetworkPlayer] Weapon GameObject '{_weaponObject.name}' ENABLED for LOCAL player");
            }
            else
            {
                Debug.LogError("[NetworkPlayer] _weaponObject is NULL! Cannot enable weapon for local player!");
            }
            
            // Визуалы
            if (_localPlayerVisuals != null)
                _localPlayerVisuals.SetActive(true);
            if (_remotePlayerVisuals != null)
                _remotePlayerVisuals.SetActive(false);
            
            // Устанавливаем слой для локального игрока (чтобы не видеть свою модель)
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
        }
        
        private void SetupRemotePlayer()
        {
            // Это удалённый игрок (другой клиент)
            Debug.Log("[NetworkPlayer] Setting up REMOTE player");
            
            // Отключаем управление
            if (_playerMovement != null)
                _playerMovement.enabled = false;
            
            // Отключаем камеру
            if (_camera != null)
            {
                _camera.enabled = false;
                
                // ОТКЛЮЧАЕМ AudioListener для удалённых игроков (иначе будет конфликт!)
                AudioListener audioListener = _camera.GetComponentInChildren<AudioListener>();
                if (audioListener != null)
                {
                    audioListener.enabled = false;
                    Debug.Log("[NetworkPlayer] AudioListener DISABLED for remote player");
                }
            }
            
            // Отключаем NetworkWeapon
            if (_networkWeapon != null)
                _networkWeapon.enabled = false;
            
            // КРИТИЧЕСКИ ВАЖНО: Отключаем AdvancedWeapon для remote players!
            if (_weapon != null)
            {
                _weapon.enabled = false;
                Debug.Log($"[NetworkPlayer] AdvancedWeapon DISABLED for REMOTE player (ClientId: {OwnerClientId})");
            }
            
            // УДАЛЯЕМ Weapon GameObject для удалённых игроков (ВАЖНО!)
            // SetActive(false) НЕ РАБОТАЕТ когда SetLayerRecursively вызывается после!
            if (_weaponObject != null)
            {
                Destroy(_weaponObject);
                Debug.Log($"[NetworkPlayer] Weapon GameObject '{_weaponObject.name}' DESTROYED for REMOTE player (ClientId: {OwnerClientId})");
            }
            else
            {
                Debug.LogError($"[NetworkPlayer] _weaponObject is NULL! Cannot destroy weapon for remote player (ClientId: {OwnerClientId})");
            }
            
            // Визуалы
            if (_localPlayerVisuals != null)
                _localPlayerVisuals.SetActive(false);
            if (_remotePlayerVisuals != null)
                _remotePlayerVisuals.SetActive(true);
            
            // Устанавливаем слой для удалённых игроков
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("RemotePlayer"));
        }
        
        private void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj == null) return;
            
            obj.layer = layer;
            
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
        
        public override void OnNetworkDespawn()
        {
            Debug.Log($"[NetworkPlayer] Despawned! IsOwner: {IsOwner}");
        }
        
        /// <summary>
        /// Получить team ID игрока из NetworkTeamManager.
        /// </summary>
        public int GetTeamId()
        {
            if (NetworkTeamManager.Instance != null)
            {
                return NetworkTeamManager.Instance.GetPlayerTeam(OwnerClientId);
            }
            
            // Fallback
            return 0;
        }
        
        /// <summary>
        /// Получить ClientId владельца этого игрока.
        /// </summary>
        public ulong GetClientId()
        {
            return OwnerClientId;
        }
    }
}
