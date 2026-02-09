using Unity.Netcode;
using UnityEngine;
using System;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Network синхронизация здоровья игрока.
    /// Server-authoritative: только сервер может изменять HP.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(PlayerHealth))]
    public class NetworkPlayerHealth : NetworkBehaviour
    {
        private PlayerHealth _playerHealth;
        
        // Синхронизированное здоровье
        private NetworkVariable<float> _networkHealth = new NetworkVariable<float>(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        
        // События
        public event Action<float> OnHealthChanged;
        public event Action<ulong> OnDeath; // Killer's client ID
        
        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Сервер управляет здоровьем
                _networkHealth.Value = _playerHealth.CurrentHealth;
                
                // Подписываемся на локальные события
                _playerHealth.OnDamageReceived += OnLocalDamageReceived;
                _playerHealth.OnDeath += OnLocalDeath;
            }
            
            // Все клиенты слушают изменения
            _networkHealth.OnValueChanged += OnNetworkHealthChanged;
            
            // Синхронизируем начальное значение
            UpdateLocalHealth(_networkHealth.Value);
        }
        
        public override void OnNetworkDespawn()
        {
            _networkHealth.OnValueChanged -= OnNetworkHealthChanged;
            
            if (IsServer)
            {
                _playerHealth.OnDamageReceived -= OnLocalDamageReceived;
                _playerHealth.OnDeath -= OnLocalDeath;
            }
        }
        
        /// <summary>
        /// Применить урон (вызывается на сервере).
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(float damage, ulong attackerClientId)
        {
            if (!IsServer) return;
            
            Debug.Log($"[NetworkHealth] Server applying {damage} damage from client {attackerClientId}");
            
            // Применяем урон локально на сервере
            GameObject attacker = GetPlayerByClientId(attackerClientId);
            _playerHealth.TakeDamage(damage, attacker);
            
            // NetworkVariable автоматически синхронизируется
            _networkHealth.Value = _playerHealth.CurrentHealth;
        }
        
        private void OnLocalDamageReceived(float damage, GameObject attacker)
        {
            // Вызывается на сервере когда локальный PlayerHealth получил урон
            Debug.Log($"[NetworkHealth] Local damage received: {damage}");
        }
        
        private void OnLocalDeath(GameObject killer)
        {
            // Вызывается на сервере когда игрок умер
            Debug.Log($"[NetworkHealth] Player died! Killer: {killer?.name}");
            
            ulong killerClientId = killer != null ? GetClientIdFromPlayer(killer) : OwnerClientId;
            
            // Уведомляем всех клиентов о смерти
            NotifyDeathClientRpc(killerClientId);
        }
        
        [ClientRpc]
        private void NotifyDeathClientRpc(ulong killerClientId)
        {
            Debug.Log($"[NetworkHealth] Death notification! Killer client: {killerClientId}");
            OnDeath?.Invoke(killerClientId);
            
            // TODO: Проиграть анимацию смерти, звук, эффекты
        }
        
        private void OnNetworkHealthChanged(float previousValue, float newValue)
        {
            UpdateLocalHealth(newValue);
            OnHealthChanged?.Invoke(newValue);
        }
        
        private void UpdateLocalHealth(float health)
        {
            if (_playerHealth != null)
            {
                _playerHealth.SetHealth(health);
            }
        }
        
        private GameObject GetPlayerByClientId(ulong clientId)
        {
            if (NetworkManager.Singleton == null) return null;
            
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                return client.PlayerObject?.gameObject;
            }
            
            return null;
        }
        
        private ulong GetClientIdFromPlayer(GameObject player)
        {
            var networkObject = player.GetComponent<NetworkObject>();
            return networkObject != null ? networkObject.OwnerClientId : 0;
        }
        
        /// <summary>
        /// Respawn игрока с полным здоровьем (вызывается на сервере).
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void RespawnServerRpc()
        {
            if (!IsServer) return;
            
            _playerHealth.Respawn();
            _networkHealth.Value = _playerHealth.MaxHealth;
            
            Debug.Log($"[NetworkHealth] Player {OwnerClientId} respawned");
        }
    }
}
