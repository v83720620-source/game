using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Обрабатывает reconnect игроков.
    /// Восстанавливает состояние игрока после reconnect.
    /// </summary>
    public class ReconnectHandler : NetworkBehaviour
    {
        private static ReconnectHandler _instance;
        public static ReconnectHandler Instance => _instance;
        
        [Header("Settings")]
        [SerializeField] private bool _enableReconnect = true;
        
        #pragma warning disable 0414
        [SerializeField] private float _reconnectGracePeriod = 5f; // Grace period после reconnect (TODO: использовать в RestorePlayerState)
        #pragma warning restore 0414
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            
            // Subscribe to client connect event
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            
            Debug.Log("[ReconnectHandler] Initialized on server");
        }
        
        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
        
        /// <summary>
        /// Called when a client connects (or reconnects).
        /// </summary>
        private void OnClientConnected(ulong clientId)
        {
            if (!_enableReconnect) return;
            
            Debug.Log($"[ReconnectHandler] Client {clientId} connected");
            
            // Check if this is a reconnect
            var backfillManager = BackfillManager.Instance;
            if (backfillManager != null && backfillManager.CanPlayerReconnect(clientId))
            {
                Debug.Log($"[ReconnectHandler] ✅ Player {clientId} reconnected within grace period!");
                
                // TODO: Restore player state
                // - Team assignment
                // - Position
                // - Kills/Deaths
                // - Loadout
                
                NotifyPlayerReconnectedClientRpc(clientId);
            }
        }
        
        /// <summary>
        /// Restore player state after reconnect.
        /// </summary>
        private void RestorePlayerState(ulong clientId)
        {
            // Get player object
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                if (client.PlayerObject != null)
                {
                    // TODO: Restore state from saved data
                    Debug.Log($"[ReconnectHandler] Restoring state for player {clientId}");
                    
                    // Example:
                    // - Respawn at safe location
                    // - Restore team
                    // - Restore loadout
                    // - Update scoreboard
                }
            }
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyPlayerReconnectedClientRpc(ulong clientId)
        {
            Debug.Log($"[ReconnectHandler] Player {clientId} reconnected!");
            
            // TODO: Show notification in UI
            // "Player X reconnected"
        }
        
        /// <summary>
        /// Check if player is in reconnect grace period.
        /// </summary>
        public bool IsInGracePeriod(ulong clientId)
        {
            var backfillManager = BackfillManager.Instance;
            return backfillManager != null && backfillManager.CanPlayerReconnect(clientId);
        }
    }
}
