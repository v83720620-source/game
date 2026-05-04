using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using FlumpGame.Network.Bots;

namespace FlumpGame.Network.AI
{
    /// <summary>
    /// Spawns AI bots in multiplayer matches.
    /// Server-only bot management.
    /// Follows Unity Netcode spawning best practices:
    /// - Instantiate() then NetworkObject.Spawn()
    /// - Register prefabs in NetworkManager
    /// - No nested NetworkObjects
    /// </summary>
    public class NetworkBotSpawner : NetworkBehaviour
    {
        private static NetworkBotSpawner _instance;
        public static NetworkBotSpawner Instance => _instance;
        
        [Header("Bot Settings")]
        [SerializeField] private GameObject _botPrefab;
        [SerializeField] private int _maxBotsPerTeam = 3;
        [SerializeField] private bool _autoFillTeams = true;
        [SerializeField] private BotDifficulty _defaultDifficulty = BotDifficulty.Normal;
        
        // Spawned bots tracking
        private List<NetworkBotController> _spawnedBots = new List<NetworkBotController>();
        
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
            
            Debug.Log("[NetworkBotSpawner] Initialized on server");
        }
        
        /// <summary>
        /// Spawn bot on specific team (Server only).
        /// Uses Unity Netcode spawning pattern: Instantiate → Spawn.
        /// </summary>
        public NetworkBotController SpawnBot(int teamId, BotDifficulty difficulty = BotDifficulty.Normal)
        {
            if (!IsServer)
            {
                Debug.LogError("[NetworkBotSpawner] Only server can spawn bots!");
                return null;
            }
            
            if (_botPrefab == null)
            {
                Debug.LogError("[NetworkBotSpawner] Bot prefab not assigned!");
                return null;
            }
            
            // Get spawn position
            var spawnManager = FlumpGame.Network.Match.NetworkSpawnManager.Instance;
            if (spawnManager == null)
            {
                Debug.LogError("[NetworkBotSpawner] NetworkSpawnManager not found!");
                return null;
            }
            
            // Use high client ID for bots (1000+) to avoid conflicts with real players
            ulong fakeBotClientId = 1000 + (ulong)_spawnedBots.Count;
            
            Vector3 spawnPos = spawnManager.GetSpawnPosition(fakeBotClientId, teamId);
            Quaternion spawnRot = spawnManager.GetSpawnRotation(fakeBotClientId, teamId);
            
            // Unity Netcode Best Practice: Instantiate first
            GameObject botObj = Instantiate(_botPrefab, spawnPos, spawnRot);
            
            // Get NetworkObject component
            var networkObject = botObj.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("[NetworkBotSpawner] Bot prefab must have NetworkObject component!");
                Destroy(botObj);
                return null;
            }
            
            // Unity Netcode Best Practice: Then Spawn on network
            networkObject.Spawn();
            
            // Setup bot after spawning
            var botController = botObj.GetComponent<NetworkBotController>();
            if (botController != null)
            {
                // Generate bot data
                string botName = BotNameGenerator.GenerateName();
                BotData botData = new BotData(botName, teamId, difficulty);
                
                // Setup bot
                botController.SetTeam(teamId);
                botController.SetBotName(botData.botName);
                // TODO: Set difficulty когда будет реализовано
                
                _spawnedBots.Add(botController);
                
                Debug.Log($"[NetworkBotSpawner] Spawned bot '{botData.botName}' (Difficulty: {difficulty}) on Team {teamId}");
            }
            
            return botController;
        }
        
        /// <summary>
        /// Auto-fill teams with bots to reach max players (Server only).
        /// </summary>
        public void AutoFillTeams()
        {
            if (!IsServer) return;
            if (!_autoFillTeams) return;
            
            var teamManager = FlumpGame.Network.Match.NetworkTeamManager.Instance;
            if (teamManager == null)
            {
                Debug.LogWarning("[NetworkBotSpawner] NetworkTeamManager not found!");
                return;
            }
            
            // Count players per team
            int team1PlayerCount = teamManager.GetTeamPlayerCount(1);
            int team2PlayerCount = teamManager.GetTeamPlayerCount(2);
            
            // Count bots per team
            int team1BotCount = CountBotsInTeam(1);
            int team2BotCount = CountBotsInTeam(2);
            
            // Fill Team 1
            int team1Needed = Mathf.Max(0, _maxBotsPerTeam - team1PlayerCount - team1BotCount);
            for (int i = 0; i < team1Needed; i++)
            {
                SpawnBot(1, _defaultDifficulty);
            }
            
            // Fill Team 2
            int team2Needed = Mathf.Max(0, _maxBotsPerTeam - team2PlayerCount - team2BotCount);
            for (int i = 0; i < team2Needed; i++)
            {
                SpawnBot(2, _defaultDifficulty);
            }
            
            Debug.Log($"[NetworkBotSpawner] Auto-filled teams: T1=+{team1Needed} bots, T2=+{team2Needed} bots");
        }
        
        private int CountBotsInTeam(int teamId)
        {
            int count = 0;
            foreach (var bot in _spawnedBots)
            {
                if (bot != null && bot.TeamId == teamId && bot.IsActive)
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// Spawn multiple bots at once (convenience method).
        /// </summary>
        public void SpawnBots(int count, int teamId, BotDifficulty difficulty = BotDifficulty.Normal)
        {
            if (!IsServer) return;
            
            for (int i = 0; i < count; i++)
            {
                SpawnBot(teamId, difficulty);
            }
            
            Debug.Log($"[NetworkBotSpawner] Spawned {count} bots on Team {teamId}");
        }
        
        /// <summary>
        /// Remove all bots (Server only).
        /// Properly despawns before destroying.
        /// </summary>
        public void DespawnAllBots()
        {
            if (!IsServer) return;
            
            foreach (var bot in _spawnedBots)
            {
                if (bot != null && bot.NetworkObject != null)
                {
                    // Unity Netcode Best Practice: Despawn before Destroy
                    bot.NetworkObject.Despawn();
                    Destroy(bot.gameObject);
                }
            }
            
            _spawnedBots.Clear();
            
            Debug.Log("[NetworkBotSpawner] All bots despawned");
        }
    }
}
