using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.AI
{
    /// <summary>
    /// Network controller for AI bot.
    /// Server-authoritative - all AI logic runs on server only.
    /// Based on Unity Netcode best practices 2.7.0+
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkBotController : NetworkBehaviour
    {
        [Header("Bot Components")]
        [SerializeField] private BotAI _botAI;
        [SerializeField] private BotVision _botVision;
        [SerializeField] private BotWeapon _botWeapon;
        [SerializeField] private PlayerHealth _health;
        
        [Header("Bot Info")]
        [SerializeField] private string _botName = "Bot";
        
        // Network Variables (auto-sync across network)
        private NetworkVariable<int> _teamId = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private NetworkVariable<bool> _isActive = new NetworkVariable<bool>(
            true,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        // Properties
        public int TeamId => _teamId.Value;
        public bool IsActive => _isActive.Value;
        public string BotName => _botName;
        public ulong BotId => NetworkObjectId;
        
        private void Awake()
        {
            // Cache components
            if (_botAI == null)
                _botAI = GetComponent<BotAI>();
            
            if (_botVision == null)
                _botVision = GetComponent<BotVision>();
            
            if (_botWeapon == null)
                _botWeapon = GetComponent<BotWeapon>();
            
            if (_health == null)
                _health = GetComponent<PlayerHealth>();
        }
        
        public override void OnNetworkSpawn()
        {
            // Server Authority: only server controls AI
            if (IsServer)
            {
                SetupServerBot();
            }
            else
            {
                SetupClientBot();
            }
            
            Debug.Log($"[NetworkBot] Spawned! IsServer: {IsServer}, Team: {TeamId}, Name: {BotName}");
        }
        
        private void SetupServerBot()
        {
            // Server manages AI
            if (_botAI != null)
                _botAI.enabled = true;
            
            if (_botVision != null)
                _botVision.enabled = true;
            
            if (_botWeapon != null)
                _botWeapon.enabled = true;
            
            // Subscribe to death event
            if (_health != null)
            {
                _health.OnDeath += HandleBotDeath;
            }
        }
        
        private void SetupClientBot()
        {
            // Clients don't control AI (visual only)
            if (_botAI != null)
                _botAI.enabled = false;
            
            if (_botVision != null)
                _botVision.enabled = false;
            
            if (_botWeapon != null)
                _botWeapon.enabled = false;
        }
        
        private void HandleBotDeath(GameObject killer)
        {
            if (!IsServer) return;
            
            Debug.Log($"[NetworkBot] {BotName} killed by {killer?.name ?? "unknown"}");
            
            // Notify game mode
            NotifyGameModeOfDeath(killer);
            
            // Schedule respawn
            StartCoroutine(RespawnCoroutine());
        }
        
        private void NotifyGameModeOfDeath(GameObject killer)
        {
            if (!IsServer) return;
            
            // Get killer's ClientId
            ulong killerClientId = 0;
            
            var killerPlayer = killer?.GetComponent<FlumpGame.Network.Player.NetworkPlayerController>();
            if (killerPlayer != null)
            {
                killerClientId = killerPlayer.OwnerClientId;
            }
            
            // Notify game modes
            var tdmMode = FlumpGame.GameModes.Network.NetworkTDM3v3Mode.Instance;
            if (tdmMode != null)
            {
                tdmMode.OnPlayerKilled(killerClientId, BotId);
            }
        }
        
        private System.Collections.IEnumerator RespawnCoroutine()
        {
            // Deactivate bot
            _isActive.Value = false;
            
            // Wait 3 seconds
            yield return new WaitForSeconds(3f);
            
            // Respawn
            RespawnBot();
        }
        
        private void RespawnBot()
        {
            if (!IsServer) return;
            
            // Get spawn point from NetworkSpawnManager
            var spawnManager = FlumpGame.Network.Match.NetworkSpawnManager.Instance;
            if (spawnManager == null)
            {
                Debug.LogError("[NetworkBot] NetworkSpawnManager not found!");
                return;
            }
            
            Vector3 spawnPos = spawnManager.GetSpawnPosition(BotId, TeamId);
            Quaternion spawnRot = spawnManager.GetSpawnRotation(BotId, TeamId);
            
            // Teleport
            transform.position = spawnPos;
            transform.rotation = spawnRot;
            
            // Reset health
            if (_health != null)
            {
                _health.Respawn();
            }
            
            // Activate
            _isActive.Value = true;
            
            Debug.Log($"[NetworkBot] {BotName} respawned at {spawnPos}");
        }
        
        /// <summary>
        /// Set bot team (Server only).
        /// </summary>
        public void SetTeam(int teamId)
        {
            if (!IsServer) return;
            
            _teamId.Value = teamId;
            Debug.Log($"[NetworkBot] {BotName} assigned to Team {teamId}");
        }
        
        /// <summary>
        /// Set bot name (Server only).
        /// </summary>
        public void SetBotName(string name)
        {
            if (!IsServer) return;
            
            _botName = name;
        }
        
        public override void OnNetworkDespawn()
        {
            // Unsubscribe events
            if (_health != null)
            {
                _health.OnDeath -= HandleBotDeath;
            }
            
            Debug.Log($"[NetworkBot] {BotName} despawned");
        }
    }
}
