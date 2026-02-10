using Unity.Netcode;
using UnityEngine;
using FlumpGame.Network.Match;

namespace FlumpGame.GameModes.Network
{
    /// <summary>
    /// Duel 1v1 Mode - Первый до 5 фрагов побеждает.
    /// </summary>
    public class NetworkDuelMode : NetworkBehaviour
    {
        private static NetworkDuelMode _instance;
        public static NetworkDuelMode Instance => _instance;
        
        [Header("Duel Settings")]
        [SerializeField] private int _killsToWin = 5;
        #pragma warning disable CS0414 // Field is assigned but never used (зарезервировано для будущего использования)
        [SerializeField] private float _matchDuration = 180f; // 3 минуты
        [SerializeField] private float _overtimeDuration = 60f; // 1 минута Sudden Death
        #pragma warning restore CS0414
        
        [Header("Spawn Settings")]
        [SerializeField] private float _respawnDelay = 3f;
        #pragma warning disable CS0414 // Field is assigned but never used (зарезервировано для будущего использования)
        [SerializeField] private float _minSpawnDistanceFromEnemy = 20f;
        #pragma warning restore CS0414
        
        // Network Variables
        private NetworkVariable<int> _player1Kills = new NetworkVariable<int>(0);
        private NetworkVariable<int> _player2Kills = new NetworkVariable<int>(0);
        private NetworkVariable<bool> _isOvertime = new NetworkVariable<bool>(false);
        
        // State
        private ulong _player1ClientId;
        private ulong _player2ClientId;
        private bool _matchStarted = false;
        
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
            if (IsServer)
            {
                // Находим двух игроков
                FindDuelPlayers();
                
                // Настраиваем матч
                SetupDuelMatch();
            }
            
            // Подписываемся на изменения
            _player1Kills.OnValueChanged += OnKillsChanged;
            _player2Kills.OnValueChanged += OnKillsChanged;
            _isOvertime.OnValueChanged += OnOvertimeChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            _player1Kills.OnValueChanged -= OnKillsChanged;
            _player2Kills.OnValueChanged -= OnKillsChanged;
            _isOvertime.OnValueChanged -= OnOvertimeChanged;
        }
        
        private void FindDuelPlayers()
        {
            if (!IsServer) return;
            
            var connectedClients = NetworkManager.Singleton.ConnectedClientsIds;
            
            if (connectedClients.Count < 2)
            {
                Debug.LogWarning("[NetworkDuelMode] Need 2 players for Duel!");
                return;
            }
            
            _player1ClientId = connectedClients[0];
            _player2ClientId = connectedClients[1];
            
            Debug.Log($"[NetworkDuelMode] Duel: Player {_player1ClientId} vs Player {_player2ClientId}");
        }
        
        private void SetupDuelMatch()
        {
            if (!IsServer) return;
            
            Debug.Log($"[NetworkDuelMode] Duel match setup complete");
            
            _matchStarted = true;
            
            // Start match (прямой вызов на сервере)
            if (NetworkMatchManager.Instance != null)
            {
                NetworkMatchManager.Instance.StartMatch();
            }
        }
        
        /// <summary>
        /// Вызывается когда игрок убивает другого (из NetworkPlayerHealth).
        /// </summary>
        public void OnPlayerKilled(ulong killerClientId, ulong victimClientId)
        {
            if (!IsServer) return;
            
            if (!_matchStarted) return;
            
            // Определяем какой игрок убил
            if (killerClientId == _player1ClientId)
            {
                _player1Kills.Value++;
                Debug.Log($"[NetworkDuelMode] Player 1 killed Player 2! Score: {_player1Kills.Value} - {_player2Kills.Value}");
            }
            else if (killerClientId == _player2ClientId)
            {
                _player2Kills.Value++;
                Debug.Log($"[NetworkDuelMode] Player 2 killed Player 1! Score: {_player1Kills.Value} - {_player2Kills.Value}");
            }
            
            // Проверяем условия победы
            CheckWinCondition();
            
            // Respawn victim
            RespawnPlayer(victimClientId);
        }
        
        private void CheckWinCondition()
        {
            if (!IsServer) return;
            
            // В Overtime - первый фраг побеждает
            if (_isOvertime.Value)
            {
                ulong winner = _player1Kills.Value > _player2Kills.Value ? _player1ClientId : _player2ClientId;
                EndMatch(winner);
                return;
            }
            
            // Проверка first to 5 kills
            if (_player1Kills.Value >= _killsToWin)
            {
                EndMatch(_player1ClientId);
            }
            else if (_player2Kills.Value >= _killsToWin)
            {
                EndMatch(_player2ClientId);
            }
            
            // Проверка overtime (если время вышло)
            if (NetworkMatchManager.Instance != null && 
                NetworkMatchManager.Instance.MatchTimer <= 0f && 
                !_isOvertime.Value)
            {
                StartOvertime();
            }
        }
        
        private void StartOvertime()
        {
            if (!IsServer) return;
            
            if (_player1Kills.Value == _player2Kills.Value)
            {
                // Ничья - начинаем Sudden Death
                _isOvertime.Value = true;
                
                Debug.Log("[NetworkDuelMode] OVERTIME! Sudden Death - next kill wins!");
                
                // Notify clients
                NotifyOvertimeClientRpc();
            }
            else
            {
                // У кого-то больше киллов - он победил
                ulong winner = _player1Kills.Value > _player2Kills.Value ? _player1ClientId : _player2ClientId;
                EndMatch(winner);
            }
        }
        
        private void EndMatch(ulong winnerClientId)
        {
            if (!IsServer) return;
            
            _matchStarted = false;
            
            Debug.Log($"[NetworkDuelMode] Match ended! Winner: Player {winnerClientId}");
            
            // Notify clients
            NotifyMatchEndClientRpc(winnerClientId, _player1Kills.Value, _player2Kills.Value);
        }
        
        private void RespawnPlayer(ulong clientId)
        {
            if (!IsServer) return;
            
            StartCoroutine(RespawnPlayerDelayed(clientId));
        }
        
        private System.Collections.IEnumerator RespawnPlayerDelayed(ulong clientId)
        {
            yield return new UnityEngine.WaitForSeconds(_respawnDelay);
            
            // Получаем spawn position (далеко от врага)
            Vector3 spawnPos = FlumpGame.Network.Match.NetworkSpawnManager.Instance.GetSafeSpawnPosition(clientId, 0); // team 0 = neutral
            Quaternion spawnRot = FlumpGame.Network.Match.NetworkSpawnManager.Instance.GetSpawnRotation(clientId, 0);
            
            // Находим player object
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                if (client.PlayerObject != null)
                {
                    // Teleport player
                    client.PlayerObject.transform.position = spawnPos;
                    client.PlayerObject.transform.rotation = spawnRot;
                    
                    Debug.Log($"[NetworkDuelMode] Player {clientId} respawned at {spawnPos}");
                }
            }
        }
        
        // ============================================
        // CLIENT RPCs
        // ============================================
        
        [ClientRpc]
        private void NotifyOvertimeClientRpc()
        {
            Debug.Log("[NetworkDuelMode] OVERTIME! Sudden Death!");
            // Show UI notification, play sound
        }
        
        [ClientRpc]
        private void NotifyMatchEndClientRpc(ulong winnerClientId, int player1Score, int player2Score)
        {
            Debug.Log($"[NetworkDuelMode] Winner: Player {winnerClientId}");
            Debug.Log($"[NetworkDuelMode] Final Score: {player1Score} - {player2Score}");
            // Show end screen
        }
        
        // ============================================
        // CALLBACKS
        // ============================================
        
        private void OnKillsChanged(int oldValue, int newValue)
        {
            Debug.Log($"[NetworkDuelMode] Score updated: {_player1Kills.Value} - {_player2Kills.Value}");
            // Update UI
        }
        
        private void OnOvertimeChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                Debug.Log("[NetworkDuelMode] OVERTIME ACTIVATED!");
            }
        }
        
        // ============================================
        // PUBLIC API
        // ============================================
        
        public int GetPlayerKills(ulong clientId)
        {
            if (clientId == _player1ClientId) return _player1Kills.Value;
            if (clientId == _player2ClientId) return _player2Kills.Value;
            return 0;
        }
        
        public bool IsOvertime => _isOvertime.Value;
    }
}
