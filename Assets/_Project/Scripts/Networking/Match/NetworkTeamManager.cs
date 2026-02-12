using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace FlumpGame.Network.Match
{
    /// <summary>
    /// Управляет командами и распределением игроков.
    /// </summary>
    public class NetworkTeamManager : NetworkBehaviour
    {
        private static NetworkTeamManager _instance;
        public static NetworkTeamManager Instance => _instance;
        
        // Списки игроков по командам (хранятся только на сервере)
        private Dictionary<ulong, int> _playerTeams = new Dictionary<ulong, int>();
        
        // Events
        public event Action<ulong, int> OnPlayerAssignedToTeam;
        
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
                // Подписываемся на подключение/отключение игроков
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
        
        private void OnClientConnected(ulong clientId)
        {
            // Автоматически назначаем команду при подключении
            AssignPlayerToTeam(clientId);
        }
        
        private void OnClientDisconnected(ulong clientId)
        {
            // Удаляем игрока из команды
            if (_playerTeams.ContainsKey(clientId))
            {
                int team = _playerTeams[clientId];
                _playerTeams.Remove(clientId);
                
                Debug.Log($"[NetworkTeamManager] Player {clientId} removed from Team {team}");
            }
        }
        
        /// <summary>
        /// Назначает игрока в команду (auto-balance).
        /// </summary>
        private void AssignPlayerToTeam(ulong clientId)
        {
            if (!IsServer) return;
            
            // Подсчитываем игроков в каждой команде
            int team1Count = 0;
            int team2Count = 0;
            
            foreach (var kvp in _playerTeams)
            {
                if (kvp.Value == 1) team1Count++;
                else if (kvp.Value == 2) team2Count++;
            }
            
            // Назначаем в команду с меньшим количеством игроков
            int assignedTeam = team1Count <= team2Count ? 1 : 2;
            
            _playerTeams[clientId] = assignedTeam;
            
            Debug.Log($"[NetworkTeamManager] Player {clientId} assigned to Team {assignedTeam} (T1: {team1Count}, T2: {team2Count})");
            
            // Notify всех клиентов
            NotifyTeamAssignmentClientRpc(clientId, assignedTeam);
            
            OnPlayerAssignedToTeam?.Invoke(clientId, assignedTeam);
        }
        
        /// <summary>
        /// Получить команду игрока.
        /// </summary>
        public int GetPlayerTeam(ulong clientId)
        {
            if (_playerTeams.TryGetValue(clientId, out int team))
            {
                return team;
            }
            return 0; // No team
        }
        
        /// <summary>
        /// Получить количество игроков в команде.
        /// </summary>
        public int GetTeamPlayerCount(int teamId)
        {
            if (!IsServer) return 0;
            
            int count = 0;
            foreach (var kvp in _playerTeams)
            {
                if (kvp.Value == teamId) count++;
            }
            return count;
        }
        
        /// <summary>
        /// Получить всех игроков команды.
        /// </summary>
        public List<ulong> GetTeamPlayers(int teamId)
        {
            List<ulong> teamPlayers = new List<ulong>();
            
            foreach (var kvp in _playerTeams)
            {
                if (kvp.Value == teamId)
                {
                    teamPlayers.Add(kvp.Key);
                }
            }
            
            return teamPlayers;
        }
        
        /// <summary>
        /// Переключить игрока в другую команду (для админов).
        /// </summary>
        [Rpc(SendTo.Server)]
        public void SwitchPlayerTeamServerRpc(ulong clientId)
        {
            if (!_playerTeams.ContainsKey(clientId)) return;
            
            int currentTeam = _playerTeams[clientId];
            int newTeam = currentTeam == 1 ? 2 : 1;
            
            _playerTeams[clientId] = newTeam;
            
            Debug.Log($"[NetworkTeamManager] Player {clientId} switched: Team {currentTeam} → Team {newTeam}");
            
            NotifyTeamAssignmentClientRpc(clientId, newTeam);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyTeamAssignmentClientRpc(ulong clientId, int teamId)
        {
            Debug.Log($"[NetworkTeamManager] Player {clientId} is on Team {teamId} (Client notification)");
            // Update UI, colors, etc.
        }
    }
}
