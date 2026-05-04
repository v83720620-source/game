using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.AI
{
    /// <summary>
    /// Network synchronization for bot health.
    /// Server-authoritative damage validation.
    /// Uses RPC pattern from Unity Netcode 2.7.0+ documentation.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkBotHealth : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth _health;
        
        // Network Variable for HP sync (auto-syncs to all clients)
        private NetworkVariable<float> _networkHealth = new NetworkVariable<float>(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private void Awake()
        {
            if (_health == null)
                _health = GetComponent<PlayerHealth>();
        }
        
        public override void OnNetworkSpawn()
        {
            // Subscribe to health changes
            if (_health != null)
            {
                _health.OnHealthChanged += OnHealthChanged;
            }
            
            // Client synchronization
            if (!IsServer)
            {
                _networkHealth.OnValueChanged += OnNetworkHealthChanged;
                // Apply current value
                if (_health != null)
                {
                    _health.SetHealth(_networkHealth.Value);
                }
            }
        }
        
        private void OnHealthChanged()
        {
            if (!IsServer) return;
            
            // Update network variable (auto-syncs to clients)
            if (_health != null)
            {
                _networkHealth.Value = _health.CurrentHealth;
            }
        }
        
        private void OnNetworkHealthChanged(float oldHealth, float newHealth)
        {
            // Clients apply health change
            if (IsServer) return;
            
            if (_health != null)
            {
                _health.SetHealth(newHealth);
            }
        }
        
        /// <summary>
        /// Apply damage to bot (Server validates).
        /// Uses RPC pattern from Unity Netcode documentation.
        /// </summary>
        [Rpc(SendTo.Server)]
        public void TakeDamageServerRpc(float damage, ulong attackerClientId)
        {
            if (_health != null)
            {
                // PlayerHealth will trigger OnHealthChanged
                // which updates _networkHealth
                // which syncs to all clients automatically
                _health.TakeDamage(damage);
                
                Debug.Log($"[NetworkBotHealth] Took {damage} damage from client {attackerClientId}");
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (_health != null)
            {
                _health.OnHealthChanged -= OnHealthChanged;
            }
            
            _networkHealth.OnValueChanged -= OnNetworkHealthChanged;
        }
    }
}
