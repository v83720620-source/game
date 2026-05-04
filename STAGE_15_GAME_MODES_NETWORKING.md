# 🏆 STAGE 15: GAME MODES NETWORKING

**Milestone:** 5 - Multiplayer Foundation  
**Длительность:** 1 неделя (7 дней)  
**Приоритет:** HIGH - Multiplayer игровые режимы  
**Предыдущий этап:** Stage 14 (Player Networking) ✅  

---

## 📋 ОБЗОР ЭТАПА

На этом этапе мы превратим single-player режимы в полноценный multiplayer:
- ✅ Создадим Network Match Manager
- ✅ Реализуем Network Team System
- ✅ Добавим Duel 1v1 Mode (НОВЫЙ!)
- ✅ Адаптируем Team 3v3 TDM
- ✅ Настроим Network Spawning
- ✅ Синхронизируем Score и Timer

**Результат:** Полностью функциональные multiplayer режимы!

---

## 🎯 ЧТО МЫ СОЗДАДИМ

### Network Scripts (8 новых файлов):

```
📁 Scripts/Networking/Match/
├── NetworkMatchManager.cs      - Управление матчем по сети
├── NetworkTeamManager.cs       - Система команд по сети
├── NetworkScoreManager.cs      - Подсчёт очков по сети
├── NetworkSpawnManager.cs      - Spawn игроков по сети
└── NetworkMatchState.cs        - Состояние матча (enum)

📁 Scripts/GameModes/Network/
├── NetworkDuelMode.cs          - Duel 1v1 режим
├── NetworkTDM3v3Mode.cs        - Team 3v3 TDM режим
└── NetworkHardpointMode.cs     - Hardpoint 5v5 режим
```

### Что будет работать:
- 🎮 Duel 1v1: Первый до 5 фрагов
- 👥 Team 3v3: 40 фрагов за 7 минут
- 🎯 Hardpoint 5v5: 150 очков
- ⏱️ Match Timer синхронизация
- 🏆 Score синхронизация
- 👥 Team assignment
- 📍 Network spawning

---

## 📅 ПОЭТАПНЫЙ ПЛАН (7 ДНЕЙ)

### ДЕНЬ 1-2: Network Match Foundation
- NetworkMatchManager с NetworkVariables
- NetworkTeamManager для team assignment
- NetworkScoreManager для очков
- Match State машина

### ДЕНЬ 3-4: Duel 1v1 Mode
- NetworkDuelMode создание
- 1v1 spawning logic
- First to 5 kills система
- Overtime (Sudden Death)

### ДЕНЬ 5-6: Team Modes Networking
- NetworkTDM3v3Mode (3v3)
- Team spawning система
- Team score tracking
- Backfill logic (боты вместо ливнувших)

### ДЕНЬ 7: Testing & Polish
- Тестирование всех режимов
- Bug fixing
- Балансировка

---

## 🚀 НАЧИНАЕМ!

---

# 📁 ДЕНЬ 1-2: NETWORK MATCH FOUNDATION

## Цель
Создать базовую network архитектуру для управления матчами, командами и очками.

---

## ШАГ 1: NETWORK MATCH MANAGER (2 часа)

### Создайте файл: `NetworkMatchManager.cs`

```csharp
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
        /// Запускает матч (только сервер).
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void StartMatchServerRpc()
        {
            if (CurrentState != MatchState.WaitingForPlayers)
            {
                Debug.LogWarning("[NetworkMatchManager] Cannot start - not in WaitingForPlayers state");
                return;
            }
            
            StartCoroutine(StartMatchSequence());
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
```

---

## ШАГ 2: NETWORK TEAM MANAGER (1 час)

### Создайте файл: `NetworkTeamManager.cs`

```csharp
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
        /// Переключить игрока в другую команду (для админов).
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void SwitchPlayerTeamServerRpc(ulong clientId)
        {
            if (!_playerTeams.ContainsKey(clientId)) return;
            
            int currentTeam = _playerTeams[clientId];
            int newTeam = currentTeam == 1 ? 2 : 1;
            
            _playerTeams[clientId] = newTeam;
            
            Debug.Log($"[NetworkTeamManager] Player {clientId} switched: Team {currentTeam} → Team {newTeam}");
            
            NotifyTeamAssignmentClientRpc(clientId, newTeam);
        }
        
        [ClientRpc]
        private void NotifyTeamAssignmentClientRpc(ulong clientId, int teamId)
        {
            Debug.Log($"[NetworkTeamManager] Player {clientId} is on Team {teamId} (Client notification)");
            // Update UI, colors, etc.
        }
    }
}
```

---

## ШАГ 3: NETWORK SPAWN MANAGER (1 час)

### Создайте файл: `NetworkSpawnManager.cs`

```csharp
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FlumpGame.Network.Match
{
    /// <summary>
    /// Управляет спавном игроков по сети.
    /// </summary>
    public class NetworkSpawnManager : NetworkBehaviour
    {
        private static NetworkSpawnManager _instance;
        public static NetworkSpawnManager Instance => _instance;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] _team1SpawnPoints;
        [SerializeField] private Transform[] _team2SpawnPoints;
        [SerializeField] private Transform[] _neutralSpawnPoints; // Для Duel
        
        [Header("Spawn Settings")]
        [SerializeField] private float _minSpawnDistance = 10f; // Минимальная дистанция от врагов
        [SerializeField] private bool _randomSpawn = true;
        
        private Dictionary<ulong, Vector3> _lastSpawnPositions = new Dictionary<ulong, Vector3>();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        
        /// <summary>
        /// Получить позицию спавна для игрока.
        /// </summary>
        public Vector3 GetSpawnPosition(ulong clientId, int teamId)
        {
            Transform[] spawnPoints = GetSpawnPointsForTeam(teamId);
            
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning($"[NetworkSpawnManager] No spawn points for team {teamId}!");
                return Vector3.zero;
            }
            
            // Random spawn
            if (_randomSpawn)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Vector3 position = spawnPoints[randomIndex].position;
                
                _lastSpawnPositions[clientId] = position;
                return position;
            }
            
            // Sequential spawn
            int index = (int)(clientId % spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[index].position;
            
            _lastSpawnPositions[clientId] = spawnPos;
            return spawnPos;
        }
        
        /// <summary>
        /// Получить безопасную позицию спавна (далеко от врагов).
        /// </summary>
        public Vector3 GetSafeSpawnPosition(ulong clientId, int teamId)
        {
            Transform[] spawnPoints = GetSpawnPointsForTeam(teamId);
            
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return Vector3.zero;
            }
            
            // Найти все точки спавна отсортированные по безопасности
            var safeSpawns = spawnPoints
                .OrderByDescending(sp => GetSpawnSafetyScore(sp.position, teamId))
                .ToArray();
            
            Vector3 position = safeSpawns[0].position;
            _lastSpawnPositions[clientId] = position;
            
            return position;
        }
        
        private float GetSpawnSafetyScore(Vector3 position, int teamId)
        {
            float score = 100f;
            
            // Найти всех игроков врагов
            var enemies = GameObject.FindGameObjectsWithTag("Player"); // TODO: Filter by team
            
            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                
                if (distance < _minSpawnDistance)
                {
                    score -= (1f - distance / _minSpawnDistance) * 50f;
                }
            }
            
            return score;
        }
        
        private Transform[] GetSpawnPointsForTeam(int teamId)
        {
            return teamId switch
            {
                1 => _team1SpawnPoints,
                2 => _team2SpawnPoints,
                0 => _neutralSpawnPoints, // Neutral (for Duel)
                _ => null
            };
        }
        
        /// <summary>
        /// Setup spawn points from scene (auto-find).
        /// </summary>
        public void AutoSetupSpawnPoints()
        {
            // Find all spawn points in scene
            GameObject team1Spawns = GameObject.Find("Team1Spawns");
            GameObject team2Spawns = GameObject.Find("Team2Spawns");
            GameObject neutralSpawns = GameObject.Find("NeutralSpawns");
            
            if (team1Spawns != null)
            {
                _team1SpawnPoints = team1Spawns.GetComponentsInChildren<Transform>()
                    .Where(t => t != team1Spawns.transform).ToArray();
                Debug.Log($"[NetworkSpawnManager] Found {_team1SpawnPoints.Length} Team 1 spawn points");
            }
            
            if (team2Spawns != null)
            {
                _team2SpawnPoints = team2Spawns.GetComponentsInChildren<Transform>()
                    .Where(t => t != team2Spawns.transform).ToArray();
                Debug.Log($"[NetworkSpawnManager] Found {_team2SpawnPoints.Length} Team 2 spawn points");
            }
            
            if (neutralSpawns != null)
            {
                _neutralSpawnPoints = neutralSpawns.GetComponentsInChildren<Transform>()
                    .Where(t => t != neutralSpawns.transform).ToArray();
                Debug.Log($"[NetworkSpawnManager] Found {_neutralSpawnPoints.Length} Neutral spawn points");
            }
        }
    }
}
```

---

# ПРОДОЛЖЕНИЕ В СЛЕДУЮЩИХ ДНЯХ...

**Что дальше:**
- День 3-4: Duel 1v1 Mode полная реализация
- День 5-6: Team 3v3 TDM Mode
- День 7: Testing & Polish

**Хотите продолжить сейчас или сначала реализуем первые 3 скрипта?**
