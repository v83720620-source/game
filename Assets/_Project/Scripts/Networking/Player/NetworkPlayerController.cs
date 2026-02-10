using Unity.Netcode;
using UnityEngine;

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
        }
        
        public override void OnNetworkSpawn()
        {
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
            
            // Включаем оружие
            if (_weapon != null)
                _weapon.enabled = true;
            
            // ПОКАЗЫВАЕМ Weapon GameObject для локального игрока
            if (_weaponObject == null && _weapon != null)
                _weaponObject = _weapon.gameObject;
            
            if (_weaponObject != null)
            {
                _weaponObject.SetActive(true);
                Debug.Log("[NetworkPlayer] Weapon GameObject ENABLED for local player");
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
            
            // Отключаем локальное оружие
            if (_weapon != null)
                _weapon.enabled = false;
            
            // СКРЫВАЕМ Weapon GameObject для удалённых игроков (ВАЖНО!)
            if (_weaponObject == null && _weapon != null)
                _weaponObject = _weapon.gameObject;
            
            if (_weaponObject != null)
            {
                _weaponObject.SetActive(false);
                Debug.Log("[NetworkPlayer] Weapon GameObject DISABLED for remote player - prevents flying weapon bug!");
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
        /// Получить team ID игрока (для будущего использования).
        /// </summary>
        public int GetTeamId()
        {
            // TODO: Реализовать team assignment в Этапе 15
            return IsOwner ? 1 : 2; // Временно
        }
    }
}
