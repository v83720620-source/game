using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using FlumpGame.Network.AI;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Управляет заменой отключившихся игроков ботами.
    /// Server-only система.
    /// </summary>
    public class BackfillManager : NetworkBehaviour
    {
        private static BackfillManager _instance;
        public static BackfillManager Instance => _instance;
        
        [Header("Settings")]
        [SerializeField] private float _reconnectTimeout = 30f; // 30 секунд на reconnect
        [SerializeField] private bool _enableBackfill = true;
        [SerializeField] private BotDifficulty _replacementBotDifficulty = BotDifficulty.Normal;
        
        // Tracking disconnected players
        private Dictionary<ulong, DisconnectedPlayerData> _disconnectedPlayers = new Dictionary<ulong, DisconnectedPlayerData>();
        
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
            
            // Subscribe to network events
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            
            Debug.Log("[BackfillManager] Initialized on server");
        }
        
        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
        
        /// <summary>
        /// Called when a client disconnects.
        /// </summary>
        private void OnClientDisconnected(ulong clientId)
        {
            if (!_enableBackfill) return;
            
            Debug.Log($"[BackfillManager] Client {clientId} disconnected");
            
            // Get player data before disconnect
            var playerData = GetPlayerData(clientId);
            if (playerData == null)
            {
                Debug.LogWarning($"[BackfillManager] No data found for client {clientId}");
                return;
            }
            
            // Store disconnected player data
            var disconnectedData = new DisconnectedPlayerData
            {
                clientId = clientId,
                teamId = playerData.teamId,
                disconnectTime = Time.time,
                playerName = playerData.playerName,
                kills = playerData.kills,
                deaths = playerData.deaths
            };
            
            _disconnectedPlayers[clientId] = disconnectedData;
            
            Debug.Log($"[BackfillManager] Player '{playerData.playerName}' (Team {playerData.teamId}) disconnected. Waiting {_reconnectTimeout}s for reconnect...");
            
            // Start reconnect timer
            StartCoroutine(ReconnectTimer(clientId));
        }
        
        /// <summary>
        /// Timer для ожидания reconnect.
        /// </summary>
        private IEnumerator ReconnectTimer(ulong clientId)
        {
            yield return new WaitForSeconds(_reconnectTimeout);
            
            // Check if player reconnected
            if (NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
            {
                Debug.Log($"[BackfillManager] Player {clientId} reconnected!");
                _disconnectedPlayers.Remove(clientId);
                yield break;
            }
            
            // Player didn't reconnect - spawn bot replacement
            if (_disconnectedPlayers.TryGetValue(clientId, out var playerData))
            {
                Debug.Log($"[BackfillManager] Player {clientId} didn't reconnect. Spawning bot replacement...");
                SpawnReplacementBot(playerData);
                _disconnectedPlayers.Remove(clientId);
            }
        }
        
        /// <summary>
        /// Spawn bot to replace disconnected player.
        /// </summary>
        private void SpawnReplacementBot(DisconnectedPlayerData playerData)
        {
            var botSpawner = NetworkBotSpawner.Instance;
            if (botSpawner == null)
            {
                Debug.LogError("[BackfillManager] NetworkBotSpawner not found!");
                return;
            }
            
            // Spawn bot on the same team
            var bot = botSpawner.SpawnBot(playerData.teamId, _replacementBotDifficulty);
            
            if (bot != null)
            {
                Debug.Log($"[BackfillManager] ✅ Spawned bot '{bot.BotName}' to replace player '{playerData.playerName}' on Team {playerData.teamId}");
                
                // TODO: Можно передать статистику бота (kills/deaths) для continuity
                // bot.SetKills(playerData.kills);
                // bot.SetDeaths(playerData.deaths);
            }
            else
            {
                Debug.LogError($"[BackfillManager] Failed to spawn replacement bot for player {playerData.clientId}");
            }
        }
        
        /// <summary>
        /// Get player data (для сохранения перед disconnect).
        /// </summary>
        private PlayerDataSnapshot GetPlayerData(ulong clientId)
        {
            // Try to get player object
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                if (client.PlayerObject != null)
                {
                    var teamMember = client.PlayerObject.GetComponent<TeamMember>();
                    
                    // Get team
                    int teamId = 0;
                    if (teamMember != null)
                    {
                        teamId = (int)teamMember.Team;
                    }
                    
                    // TODO: Get kills/deaths from game mode manager
                    return new PlayerDataSnapshot
                    {
                        clientId = clientId,
                        teamId = teamId,
                        playerName = $"Player_{clientId}",
                        kills = 0,
                        deaths = 0
                    };
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Manually trigger backfill (for testing).
        /// </summary>
        public void TriggerBackfill(ulong clientId)
        {
            if (!IsServer) return;
            
            OnClientDisconnected(clientId);
        }
        
        /// <summary>
        /// Check if player can reconnect.
        /// </summary>
        public bool CanPlayerReconnect(ulong clientId)
        {
            return _disconnectedPlayers.ContainsKey(clientId);
        }
        
        /// <summary>
        /// Get disconnected players count.
        /// </summary>
        public int GetDisconnectedPlayersCount()
        {
            return _disconnectedPlayers.Count;
        }
    }
    
    /// <summary>
    /// Data snapshot of disconnected player.
    /// </summary>
    [System.Serializable]
    public class DisconnectedPlayerData
    {
        public ulong clientId;
        public int teamId;
        public float disconnectTime;
        public string playerName;
        public int kills;
        public int deaths;
    }
    
    /// <summary>
    /// Player data snapshot.
    /// </summary>
    public class PlayerDataSnapshot
    {
        public ulong clientId;
        public int teamId;
        public string playerName;
        public int kills;
        public int deaths;
    }
}
