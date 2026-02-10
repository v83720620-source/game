using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections;

namespace FlumpGame.Network.Match
{
    /// <summary>
    /// Управляет матчем по сети.
    /// Server-authoritative - только сервер может изменять состояние.
    /// </summary>
    public class NetworkMatchManager : NetworkBehaviour
    {
        private static NetworkMatchManager _instance;
        public static NetworkMatchManager Instance => _instance;
        
        [Header("Match Settings")]
        [SerializeField] private float _matchDuration = 600f; // 10 минут
        [SerializeField] private float _preMatchDuration = 5f; // 5 секунд до старта
        [SerializeField] private float _postMatchDuration = 10f; // 10 секунд после конца
        
        // Network Variables - синхронизируются автоматически
        private NetworkVariable<MatchState> _matchState = new NetworkVariable<MatchState>(
            MatchState.WaitingForPlayers,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private NetworkVariable<float> _matchTimer = new NetworkVariable<float>(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private NetworkVariable<int> _team1Score = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private NetworkVariable<int> _team2Score = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        // Events для UI
        public event Action<MatchState> OnMatchStateChanged;
        public event Action<float> OnMatchTimerUpdated;
        public event Action<int, int> OnScoreUpdated;
        public event Action<int> OnMatchEnded; // WinningTeamId
        
        // Properties
        public MatchState CurrentState => _matchState.Value;
        public float MatchTimer => _matchTimer.Value;
        public int Team1Score => _team1Score.Value;
        public int Team2Score => _team2Score.Value;
        
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
            // Подписываемся на изменения NetworkVariables
            _matchState.OnValueChanged += OnMatchStateChangedCallback;
            _matchTimer.OnValueChanged += OnMatchTimerChangedCallback;
            _team1Score.OnValueChanged += OnTeamScoreChangedCallback;
            _team2Score.OnValueChanged += OnTeamScoreChangedCallback;
            
            if (IsServer)
            {
                Debug.Log("[NetworkMatchManager] Server spawned - ready to start match");
            }
        }
        
        public override void OnNetworkDespawn()
        {
            _matchState.OnValueChanged -= OnMatchStateChangedCallback;
            _matchTimer.OnValueChanged -= OnMatchTimerChangedCallback;
            _team1Score.OnValueChanged -= OnTeamScoreChangedCallback;
            _team2Score.OnValueChanged -= OnTeamScoreChangedCallback;
        }
        
        private void Update()
        {
            if (!IsServer) return;
            
            // Update timer на сервере
            UpdateMatchTimer();
        }
        
        /// <summary>
        /// Запускает матч напрямую на сервере.
        /// </summary>
        public void StartMatch()
        {
            if (!IsServer) return;
            
            if (CurrentState != MatchState.WaitingForPlayers)
            {
                Debug.LogWarning("[NetworkMatchManager] Cannot start - not in WaitingForPlayers state");
                return;
            }
            
            StartCoroutine(StartMatchSequence());
        }
        
        /// <summary>
        /// Запускает матч через RPC (для клиентов).
        /// </summary>
        [Rpc(SendTo.Server)]
        public void StartMatchServerRpc()
        {
            StartMatch();
        }
        
        private IEnumerator StartMatchSequence()
        {
            // Pre-match countdown
            _matchState.Value = MatchState.PreMatch;
            _matchTimer.Value = _preMatchDuration;
            
            Debug.Log("[NetworkMatchManager] Pre-match countdown started");
            
            yield return new WaitForSeconds(_preMatchDuration);
            
            // Start match
            _matchState.Value = MatchState.InProgress;
            _matchTimer.Value = _matchDuration;
            
            Debug.Log("[NetworkMatchManager] Match started!");
            
            // RPC для всех клиентов
            NotifyMatchStartClientRpc();
        }
        
        private void UpdateMatchTimer()
        {
            if (CurrentState == MatchState.InProgress)
            {
                _matchTimer.Value -= Time.deltaTime;
                
                if (_matchTimer.Value <= 0f)
                {
                    EndMatch();
                }
            }
            else if (CurrentState == MatchState.PreMatch)
            {
                _matchTimer.Value -= Time.deltaTime;
            }
        }
        
        /// <summary>
        /// Добавляет очко команде (только сервер).
        /// </summary>
        public void AddScore(int teamId, int points = 1)
        {
            if (!IsServer) return;
            
            if (CurrentState != MatchState.InProgress) return;
            
            if (teamId == 1)
            {
                _team1Score.Value += points;
                Debug.Log($"[NetworkMatchManager] Team 1 score: {_team1Score.Value}");
            }
            else if (teamId == 2)
            {
                _team2Score.Value += points;
                Debug.Log($"[NetworkMatchManager] Team 2 score: {_team2Score.Value}");
            }
            
            // Проверка условий победы (переопределяется в game modes)
            CheckWinConditions();
        }
        
        protected virtual void CheckWinConditions()
        {
            // Базовая проверка - переопределяется в конкретных режимах
        }
        
        private void EndMatch()
        {
            if (CurrentState != MatchState.InProgress) return;
            
            _matchState.Value = MatchState.PostMatch;
            _matchTimer.Value = _postMatchDuration;
            
            // Определяем победителя
            int winningTeam = _team1Score.Value > _team2Score.Value ? 1 : 2;
            
            if (_team1Score.Value == _team2Score.Value)
            {
                winningTeam = 0; // Ничья
            }
            
            Debug.Log($"[NetworkMatchManager] Match ended! Winner: Team {winningTeam}");
            
            // Notify clients
            NotifyMatchEndClientRpc(winningTeam);
            
            OnMatchEnded?.Invoke(winningTeam);
            
            // Return to lobby after post-match
            StartCoroutine(ReturnToLobbyAfterDelay());
        }
        
        private IEnumerator ReturnToLobbyAfterDelay()
        {
            yield return new WaitForSeconds(_postMatchDuration);
            
            // TODO: Load lobby scene
            Debug.Log("[NetworkMatchManager] Returning to lobby...");
        }
        
        /// <summary>
        /// Сброс матча для новой игры (только сервер).
        /// </summary>
        public void ResetMatch()
        {
            if (!IsServer) return;
            
            _matchState.Value = MatchState.WaitingForPlayers;
            _matchTimer.Value = 0f;
            _team1Score.Value = 0;
            _team2Score.Value = 0;
            
            Debug.Log("[NetworkMatchManager] Match reset");
        }
        
        // ============================================
        // CLIENT RPCs
        // ============================================
        
        [ClientRpc]
        private void NotifyMatchStartClientRpc()
        {
            Debug.Log("[NetworkMatchManager] Match started! (Client notification)");
            // Play sound, show notification, etc.
        }
        
        [ClientRpc]
        private void NotifyMatchEndClientRpc(int winningTeam)
        {
            Debug.Log($"[NetworkMatchManager] Match ended! Winner: Team {winningTeam} (Client notification)");
            // Show end screen, play sound, etc.
        }
        
        // ============================================
        // CALLBACKS
        // ============================================
        
        private void OnMatchStateChangedCallback(MatchState oldState, MatchState newState)
        {
            Debug.Log($"[NetworkMatchManager] State changed: {oldState} → {newState}");
            OnMatchStateChanged?.Invoke(newState);
        }
        
        private void OnMatchTimerChangedCallback(float oldTimer, float newTimer)
        {
            OnMatchTimerUpdated?.Invoke(newTimer);
        }
        
        private void OnTeamScoreChangedCallback(int oldScore, int newScore)
        {
            OnScoreUpdated?.Invoke(_team1Score.Value, _team2Score.Value);
        }
    }
    
    /// <summary>
    /// Состояния матча.
    /// </summary>
    public enum MatchState
    {
        WaitingForPlayers,  // Ожидание игроков
        PreMatch,           // Countdown перед стартом
        InProgress,         // Матч идёт
        PostMatch,          // Матч закончен, показываем результаты
        Overtime            // Дополнительное время (для некоторых режимов)
    }
}
