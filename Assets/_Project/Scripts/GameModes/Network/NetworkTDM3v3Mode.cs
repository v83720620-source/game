using Unity.Netcode;
using UnityEngine;
using FlumpGame.Network.Match;
using System.Collections.Generic;

namespace FlumpGame.GameModes.Network
{
    /// <summary>
    /// Team Deathmatch 3v3 Mode - 40 фрагов за 7 минут.
    /// </summary>
    public class NetworkTDM3v3Mode : NetworkBehaviour
    {
        private static NetworkTDM3v3Mode _instance;
        public static NetworkTDM3v3Mode Instance => _instance;
        
        [Header("TDM 3v3 Settings")]
        [SerializeField] private int _killsToWin = 40;
        #pragma warning disable CS0414 // Field is assigned but never used (зарезервировано для будущего использования)
        [SerializeField] private float _matchDuration = 420f; // 7 минут
        [SerializeField] private int _maxPlayersPerTeam = 3;
        #pragma warning restore CS0414
        
        [Header("Spawn Settings")]
        [SerializeField] private float _respawnDelay = 5f;
        
        // Network Variables
        private NetworkVariable<int> _team1Kills = new NetworkVariable<int>(0);
        private NetworkVariable<int> _team2Kills = new NetworkVariable<int>(0);
        
        // State
        private bool _matchStarted = false;
        private Dictionary<ulong, int> _playerKills = new Dictionary<ulong, int>();
        private Dictionary<ulong, int> _playerDeaths = new Dictionary<ulong, int>();
        
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
                SetupTDMMatch();
            }
            
            // Подписываемся на изменения
            _team1Kills.OnValueChanged += OnTeamKillsChanged;
            _team2Kills.OnValueChanged += OnTeamKillsChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            _team1Kills.OnValueChanged -= OnTeamKillsChanged;
            _team2Kills.OnValueChanged -= OnTeamKillsChanged;
        }
        
        private void SetupTDMMatch()
        {
            if (!IsServer) return;
            
            // Проверяем что у нас есть игроки в обеих командах
            int team1Count = NetworkTeamManager.Instance?.GetTeamPlayerCount(1) ?? 0;
            int team2Count = NetworkTeamManager.Instance?.GetTeamPlayerCount(2) ?? 0;
            
            Debug.Log($"[NetworkTDM3v3Mode] Match setup: Team 1 ({team1Count} players) vs Team 2 ({team2Count} players)");
            
            // Инициализируем статистику для всех игроков
            var allClients = NetworkManager.Singleton.ConnectedClientsIds;
            foreach (var clientId in allClients)
            {
                _playerKills[clientId] = 0;
                _playerDeaths[clientId] = 0;
            }
            
            // AUTO-FILL TEAMS WITH BOTS (Unity Netcode best practice)
            var botSpawner = FlumpGame.Network.AI.NetworkBotSpawner.Instance;
            if (botSpawner != null)
            {
                botSpawner.AutoFillTeams();
                Debug.Log("[NetworkTDM3v3Mode] Auto-filled teams with bots");
            }
            
            _matchStarted = true;
            
            // Start match (прямой вызов на сервере)
            if (NetworkMatchManager.Instance != null)
            {
                NetworkMatchManager.Instance.StartMatch();
            }
        }
        
        /// <summary>
        /// Вызывается когда игрок убивает другого.
        /// </summary>
        public void OnPlayerKilled(ulong killerClientId, ulong victimClientId)
        {
            if (!IsServer) return;
            if (!_matchStarted) return;
            
            // Обновляем статистику убийцы
            if (_playerKills.ContainsKey(killerClientId))
            {
                _playerKills[killerClientId]++;
            }
            else
            {
                _playerKills[killerClientId] = 1;
            }
            
            // Обновляем статистику жертвы
            if (_playerDeaths.ContainsKey(victimClientId))
            {
                _playerDeaths[victimClientId]++;
            }
            else
            {
                _playerDeaths[victimClientId] = 1;
            }
            
            // Получаем команды
            int killerTeam = NetworkTeamManager.Instance?.GetPlayerTeam(killerClientId) ?? 0;
            int victimTeam = NetworkTeamManager.Instance?.GetPlayerTeam(victimClientId) ?? 0;
            
            // Добавляем очко команде убийцы
            if (killerTeam == 1)
            {
                _team1Kills.Value++;
                NetworkMatchManager.Instance?.AddScore(1, 1);
            }
            else if (killerTeam == 2)
            {
                _team2Kills.Value++;
                NetworkMatchManager.Instance?.AddScore(2, 1);
            }
            
            Debug.Log($"[NetworkTDM3v3Mode] Player {killerClientId} (Team {killerTeam}) killed Player {victimClientId} (Team {victimTeam})");
            Debug.Log($"[NetworkTDM3v3Mode] Score: Team 1: {_team1Kills.Value} - Team 2: {_team2Kills.Value}");
            
            // Проверяем условия победы
            CheckWinCondition();
            
            // Respawn victim
            RespawnPlayer(victimClientId);
            
            // Notify all clients
            NotifyKillClientRpc(killerClientId, victimClientId, killerTeam);
        }
        
        private void CheckWinCondition()
        {
            if (!IsServer) return;
            
            // Проверка first to 40 kills
            if (_team1Kills.Value >= _killsToWin)
            {
                EndMatch(1);
            }
            else if (_team2Kills.Value >= _killsToWin)
            {
                EndMatch(2);
            }
            
            // Проверка timeout
            if (NetworkMatchManager.Instance?.MatchTimer <= 0f)
            {
                // Время вышло - побеждает команда с большим счётом
                int winningTeam = _team1Kills.Value > _team2Kills.Value ? 1 : 2;
                if (_team1Kills.Value == _team2Kills.Value)
                {
                    winningTeam = 0; // Draw
                }
                EndMatch(winningTeam);
            }
        }
        
        private void EndMatch(int winningTeam)
        {
            if (!IsServer) return;
            
            _matchStarted = false;
            
            Debug.Log($"[NetworkTDM3v3Mode] Match ended! Winner: Team {winningTeam}");
            Debug.Log($"[NetworkTDM3v3Mode] Final Score: Team 1: {_team1Kills.Value} - Team 2: {_team2Kills.Value}");
            
            // Calculate MVP
            ulong mvpClientId = CalculateMVP();
            
            // Notify clients
            NotifyMatchEndClientRpc(winningTeam, mvpClientId, _team1Kills.Value, _team2Kills.Value);
        }
        
        private ulong CalculateMVP()
        {
            ulong mvpClientId = 0;
            int highestKills = 0;
            
            foreach (var kvp in _playerKills)
            {
                if (kvp.Value > highestKills)
                {
                    highestKills = kvp.Value;
                    mvpClientId = kvp.Key;
                }
            }
            
            Debug.Log($"[NetworkTDM3v3Mode] MVP: Player {mvpClientId} with {highestKills} kills");
            
            return mvpClientId;
        }
        
        private void RespawnPlayer(ulong clientId)
        {
            if (!IsServer) return;
            
            StartCoroutine(RespawnPlayerDelayed(clientId));
        }
        
        private System.Collections.IEnumerator RespawnPlayerDelayed(ulong clientId)
        {
            yield return new UnityEngine.WaitForSeconds(_respawnDelay);
            
            // Получаем команду игрока
            int playerTeam = NetworkTeamManager.Instance?.GetPlayerTeam(clientId) ?? 0;
            
            // Получаем spawn position для команды
            Vector3 spawnPos = FlumpGame.Network.Match.NetworkSpawnManager.Instance.GetSafeSpawnPosition(clientId, playerTeam);
            Quaternion spawnRot = FlumpGame.Network.Match.NetworkSpawnManager.Instance.GetSpawnRotation(clientId, playerTeam);
            
            // Находим player object
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                if (client.PlayerObject != null)
                {
                    // Teleport player
                    client.PlayerObject.transform.position = spawnPos;
                    client.PlayerObject.transform.rotation = spawnRot;
                    
                    Debug.Log($"[NetworkTDM3v3Mode] Player {clientId} (Team {playerTeam}) respawned");
                }
            }
        }
        
        // ============================================
        // CLIENT RPCs
        // ============================================
        
        [ClientRpc]
        private void NotifyKillClientRpc(ulong killerClientId, ulong victimClientId, int killerTeam)
        {
            Debug.Log($"[NetworkTDM3v3Mode] Kill notification: {killerClientId} killed {victimClientId}");
            // Show kill feed, play sound
        }
        
        [ClientRpc]
        private void NotifyMatchEndClientRpc(int winningTeam, ulong mvpClientId, int team1Score, int team2Score)
        {
            Debug.Log($"[NetworkTDM3v3Mode] Match ended! Winner: Team {winningTeam}");
            Debug.Log($"[NetworkTDM3v3Mode] Final Score: {team1Score} - {team2Score}");
            Debug.Log($"[NetworkTDM3v3Mode] MVP: Player {mvpClientId}");
            // Show end screen with stats
        }
        
        // ============================================
        // CALLBACKS
        // ============================================
        
        private void OnTeamKillsChanged(int oldValue, int newValue)
        {
            // Update UI scoreboard
        }
        
        // ============================================
        // PUBLIC API
        // ============================================
        
        public int GetPlayerKills(ulong clientId)
        {
            return _playerKills.TryGetValue(clientId, out int kills) ? kills : 0;
        }
        
        public int GetPlayerDeaths(ulong clientId)
        {
            return _playerDeaths.TryGetValue(clientId, out int deaths) ? deaths : 0;
        }
        
        public float GetPlayerKD(ulong clientId)
        {
            int kills = GetPlayerKills(clientId);
            int deaths = GetPlayerDeaths(clientId);
            
            if (deaths == 0) return kills;
            return (float)kills / deaths;
        }
    }
}
