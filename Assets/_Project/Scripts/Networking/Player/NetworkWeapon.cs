using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Network синхронизация оружия.
    /// Server-authoritative hit detection.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkWeapon : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private AdvancedWeapon _weapon;
        [SerializeField] private Camera _playerCamera;
        
        [Header("Weapon Settings")]
        [SerializeField] private float _fireRate = 0.1f;
        [SerializeField] private int _damage = 25;
        [SerializeField] private float _range = 100f;
        [SerializeField] private LayerMask _hitLayers;
        
        private float _nextFireTime;
        
        private void Awake()
        {
            if (_weapon == null)
                _weapon = GetComponent<AdvancedWeapon>();
            
            if (_playerCamera == null)
                _playerCamera = GetComponentInChildren<Camera>();
        }
        
        private void Update()
        {
            if (!IsOwner || !IsSpawned) return;
            
            // Локальный игрок - обрабатываем input
            HandleFireInput();
        }
        
        private void HandleFireInput()
        {
            bool wantsToFire = Input.GetButton("Fire1"); // Mouse 0
            
            if (wantsToFire && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + _fireRate;
                Fire();
            }
        }
        
        private void Fire()
        {
            // Клиент стреляет локально для instant feedback
            PlayFireEffects();
            
            // Отправляем запрос на сервер для hit detection
            if (_playerCamera != null)
            {
                Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                FireServerRpc(ray.origin, ray.direction);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void FireServerRpc(Vector3 origin, Vector3 direction)
        {
            // Сервер проверяет попадание
            Debug.Log($"[NetworkWeapon] Server processing fire from client {OwnerClientId}");
            
            Ray ray = new Ray(origin, direction);
            
            if (Physics.Raycast(ray, out RaycastHit hit, _range, _hitLayers))
            {
                Debug.Log($"[NetworkWeapon] Hit: {hit.collider.name} at {hit.point}");
                
                // Проверяем что попали в игрока
                var hitPlayer = hit.collider.GetComponentInParent<NetworkPlayerHealth>();
                if (hitPlayer != null && hitPlayer.NetworkObject != NetworkObject)
                {
                    // Применяем урон
                    float damage = CalculateDamage(hit);
                    hitPlayer.TakeDamageServerRpc(damage, OwnerClientId);
                    
                    // Уведомляем клиентов о попадании
                    NotifyHitClientRpc(hit.point, hit.normal);
                }
            }
        }
        
        private float CalculateDamage(RaycastHit hit)
        {
            // Проверяем куда попали (голова, тело, конечности)
            var hitBox = hit.collider.GetComponent<HitBox>();
            if (hitBox != null)
            {
                // Используем множитель из DamageInfo
                float multiplier = DamageInfo.GetZoneMultiplier(hitBox.HitZone);
                return _damage * multiplier;
            }
            
            return _damage;
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyHitClientRpc(Vector3 hitPoint, Vector3 hitNormal)
        {
            // Все клиенты проигрывают эффект попадания
            PlayHitEffect(hitPoint, hitNormal);
        }
        
        private void PlayFireEffects()
        {
            // Локальные эффекты выстрела
            if (_weapon != null)
            {
                // Используем существующие эффекты из AdvancedWeapon
                // _weapon.PlayMuzzleFlash();
                // _weapon.PlayFireSound();
            }
            
            Debug.Log("[NetworkWeapon] Playing fire effects");
        }
        
        private void PlayHitEffect(Vector3 position, Vector3 normal)
        {
            // Эффект попадания (искры, следы от пуль)
            Debug.Log($"[NetworkWeapon] Playing hit effect at {position}");
            
            // TODO: Создать пулевое отверстие, эффекты через VFXManager
        }
    }
}
