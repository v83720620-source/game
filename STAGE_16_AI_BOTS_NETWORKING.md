# 🤖 STAGE 16: AI BOTS NETWORKING - ПОЛНЫЙ ПЛАН

**Milestone:** 5 - Multiplayer Foundation  
**Длительность:** 4-5 дней  
**Приоритет:** HIGH - AI для multiplayer  
**Предыдущий этап:** Stage 15 (Game Modes Networking) ✅  

---

## 📋 КРАТКОЕ СОДЕРЖАНИЕ

**Что адаптируем:**
- 🤖 Существующий BotAI → Network-aware
- 👁️ BotVision → работа с NetworkPlayers
- 🔫 BotWeapon → Network damage sync
- 📍 Bot spawning через NetworkSpawnManager
- 🎯 Интеграция с game modes

**Результат:** Боты работают в multiplayer, заполняют пустые слоты команд!

---

## 🎯 ЧТО У НАС УЖЕ ЕСТЬ:

### **Single-player AI система:**

```
✅ BotAI.cs - FSM (Idle, Patrol, Chase, Attack)
✅ BotVision.cs - обнаружение врагов
✅ BotWeapon.cs - стрельба
✅ Bot.prefab - готовый префаб
✅ PlayerHealth integration
```

**НО:** Всё работает только локально, нужна network синхронизация!

---

## 📅 ПЛАН ПО ДНЯМ

```
День 1: NetworkBot Foundation (3-4 часа)
День 2: Bot Spawning System (3 часа)
День 3: Game Mode Integration (3 часа)
День 4: Testing + Polish (2-3 часа)
```

---

# 📁 ДЕНЬ 1: NETWORK BOT FOUNDATION

## Цель
Адаптировать существующую AI систему для работы по сети.

---

## ЧАСТЬ 1: NETWORK BOT CONTROLLER (1.5 часа)

### ШАГ 1.1: Создайте структуру папок

```
Assets/_Project/Scripts/
├── AI/                          ← Уже существует
│   ├── BotAI.cs                 ✅ (модифицируем)
│   ├── BotVision.cs             ✅ (модифицируем)
│   └── BotWeapon.cs             ✅ (модифицируем)
└── Networking/
    └── AI/                      ← СОЗДАЙТЕ
        ├── NetworkBotController.cs    ← Создадим
        ├── NetworkBotHealth.cs        ← Создадим
        └── NetworkBotSpawner.cs       ← День 2
```

---

### ШАГ 1.2: Код NetworkBotController.cs

**Файл:** `Assets/_Project/Scripts/Networking/AI/NetworkBotController.cs`

```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.AI
{
    /// <summary>
    /// Network контроллер для AI бота.
    /// Server-authoritative - вся AI логика на сервере.
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
        
        // Network Variables
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
            // Сервер управляет AI
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
            // Клиенты не управляют AI (только визуал)
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
            
            // Schedule respawn or remove
            StartCoroutine(RespawnOrRemoveCoroutine());
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
            var duelMode = FlumpGame.GameModes.Network.NetworkDuelMode.Instance;
            if (duelMode != null)
            {
                // Bots can't be in Duel mode (1v1)
                return;
            }
            
            var tdmMode = FlumpGame.GameModes.Network.NetworkTDM3v3Mode.Instance;
            if (tdmMode != null)
            {
                tdmMode.OnPlayerKilled(killerClientId, BotId);
            }
        }
        
        private System.Collections.IEnumerator RespawnOrRemoveCoroutine()
        {
            // Деактивируем бота
            _isActive.Value = false;
            
            // Ждём 3 секунды
            yield return new WaitForSeconds(3f);
            
            // Респавн
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
                _health.ResetHealth();
            }
            
            // Активируем
            _isActive.Value = true;
            
            Debug.Log($"[NetworkBot] {BotName} respawned at {spawnPos}");
        }
        
        /// <summary>
        /// Установить команду бота (Server only).
        /// </summary>
        public void SetTeam(int teamId)
        {
            if (!IsServer) return;
            
            _teamId.Value = teamId;
            Debug.Log($"[NetworkBot] {BotName} assigned to Team {teamId}");
        }
        
        /// <summary>
        /// Установить имя бота (Server only).
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
```

---

## ЧАСТЬ 2: NETWORK BOT HEALTH (30 мин)

### ШАГ 2.1: Код NetworkBotHealth.cs

**Файл:** `Assets/_Project/Scripts/Networking/AI/NetworkBotHealth.cs`

```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.AI
{
    /// <summary>
    /// Network синхронизация здоровья бота.
    /// Server-authoritative.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkBotHealth : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth _health;
        
        // Network Variable для синхронизации HP
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
            
            // Синхронизация на клиентах
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
        
        private void OnHealthChanged(float newHealth)
        {
            if (!IsServer) return;
            
            // Обновляем network variable
            _networkHealth.Value = newHealth;
        }
        
        private void OnNetworkHealthChanged(float oldHealth, float newHealth)
        {
            // Клиенты применяют изменение здоровья
            if (IsServer) return;
            
            if (_health != null)
            {
                _health.SetHealth(newHealth);
            }
        }
        
        /// <summary>
        /// Нанести урон боту (вызывается с сервера).
        /// </summary>
        [Rpc(SendTo.Server)]
        public void TakeDamageServerRpc(float damage, ulong attackerClientId)
        {
            if (_health != null)
            {
                // PlayerHealth сам вызовет OnHealthChanged
                // который обновит _networkHealth
                _health.TakeDamage(damage);
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
```

---

## ЧАСТЬ 3: МОДИФИКАЦИЯ СУЩЕСТВУЮЩИХ AI СКРИПТОВ (1.5 часа)

### ШАГ 3.1: Обновить BotVision.cs

Добавьте поддержку NetworkPlayers:

```csharp
// В BotVision.cs, метод FindTarget()

private Transform FindTarget()
{
    Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _targetMask);
    
    Transform closestTarget = null;
    float closestDistance = Mathf.Infinity;
    
    foreach (Collider hit in hits)
    {
        // Пропускаем себя
        if (hit.transform == transform) continue;
        
        // Проверяем команду (не атакуем союзников)
        if (!IsEnemy(hit.gameObject)) continue;
        
        // ... остальная логика ...
    }
    
    return closestTarget;
}

private bool IsEnemy(GameObject target)
{
    // Проверяем NetworkPlayerController
    var networkPlayer = target.GetComponent<FlumpGame.Network.Player.NetworkPlayerController>();
    if (networkPlayer != null)
    {
        int playerTeam = networkPlayer.GetTeamId();
        int myTeam = GetComponent<NetworkBotController>()?.TeamId ?? 0;
        return playerTeam != myTeam;
    }
    
    // Проверяем NetworkBotController
    var networkBot = target.GetComponent<NetworkBotController>();
    if (networkBot != null)
    {
        int botTeam = networkBot.TeamId;
        int myTeam = GetComponent<NetworkBotController>()?.TeamId ?? 0;
        return botTeam != myTeam;
    }
    
    // Fallback для старой системы
    var teamMember = target.GetComponent<TeamMember>();
    if (teamMember != null)
    {
        int targetTeam = teamMember.TeamId;
        int myTeam = GetComponent<TeamMember>()?.TeamId ?? 0;
        return targetTeam != myTeam;
    }
    
    return false; // Не враг если неизвестен
}
```

---

### ШАГ 3.2: Обновить BotWeapon.cs

Добавьте network damage:

```csharp
// В BotWeapon.cs, метод Fire()

private void Fire()
{
    // ... существующая логика прицеливания ...
    
    if (Physics.Raycast(ray, out RaycastHit hit, _range, _hitMask))
    {
        Debug.Log($"[BotWeapon] Hit: {hit.collider.name}");
        
        // Network damage для игроков
        var networkPlayerHealth = hit.collider.GetComponentInParent<FlumpGame.Network.Player.NetworkPlayerHealth>();
        if (networkPlayerHealth != null)
        {
            float damage = CalculateDamage(hit);
            
            // ВАЖНО: Используем BotId как attackerClientId
            var botController = GetComponent<NetworkBotController>();
            ulong botId = botController != null ? botController.BotId : 0;
            
            networkPlayerHealth.TakeDamageServerRpc(damage, botId);
            return;
        }
        
        // Network damage для ботов
        var networkBotHealth = hit.collider.GetComponentInParent<NetworkBotHealth>();
        if (networkBotHealth != null)
        {
            float damage = CalculateDamage(hit);
            
            var botController = GetComponent<NetworkBotController>();
            ulong botId = botController != null ? botController.BotId : 0;
            
            networkBotHealth.TakeDamageServerRpc(damage, botId);
            return;
        }
        
        // Fallback для старой системы
        var playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            float damage = CalculateDamage(hit);
            playerHealth.TakeDamage(damage);
        }
    }
}
```

---

# 📁 ДЕНЬ 2: BOT SPAWNING SYSTEM

## ЧАСТЬ 1: NETWORK BOT SPAWNER (2 часа)

### ШАГ 1.1: Код NetworkBotSpawner.cs

**Файл:** `Assets/_Project/Scripts/Networking/AI/NetworkBotSpawner.cs`

```csharp
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace FlumpGame.Network.AI
{
    /// <summary>
    /// Spawns AI bots in multiplayer matches.
    /// Server-only.
    /// </summary>
    public class NetworkBotSpawner : NetworkBehaviour
    {
        private static NetworkBotSpawner _instance;
        public static NetworkBotSpawner Instance => _instance;
        
        [Header("Bot Settings")]
        [SerializeField] private GameObject _botPrefab;
        [SerializeField] private int _maxBotsPerTeam = 3;
        [SerializeField] private bool _autoFillTeams = true;
        
        [Header("Bot Names")]
        [SerializeField] private string[] _botNames = new string[]
        {
            "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot"
        };
        
        // Spawned bots
        private List<NetworkBotController> _spawnedBots = new List<NetworkBotController>();
        private int _botNameIndex = 0;
        
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
        /// </summary>
        public NetworkBotController SpawnBot(int teamId)
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
            
            // Use a high client ID for bots (1000+)
            ulong fakeBotClientId = 1000 + (ulong)_spawnedBots.Count;
            
            Vector3 spawnPos = spawnManager.GetSpawnPosition(fakeBotClientId, teamId);
            Quaternion spawnRot = spawnManager.GetSpawnRotation(fakeBotClientId, teamId);
            
            // Instantiate
            GameObject botObj = Instantiate(_botPrefab, spawnPos, spawnRot);
            
            // Get NetworkObject
            var networkObject = botObj.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("[NetworkBotSpawner] Bot prefab must have NetworkObject!");
                Destroy(botObj);
                return null;
            }
            
            // Spawn on network
            networkObject.Spawn();
            
            // Setup bot
            var botController = botObj.GetComponent<NetworkBotController>();
            if (botController != null)
            {
                botController.SetTeam(teamId);
                botController.SetBotName(GetNextBotName());
                _spawnedBots.Add(botController);
            }
            
            Debug.Log($"[NetworkBotSpawner] Spawned bot on Team {teamId} at {spawnPos}");
            
            return botController;
        }
        
        /// <summary>
        /// Auto-fill teams with bots (Server only).
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
                SpawnBot(1);
            }
            
            // Fill Team 2
            int team2Needed = Mathf.Max(0, _maxBotsPerTeam - team2PlayerCount - team2BotCount);
            for (int i = 0; i < team2Needed; i++)
            {
                SpawnBot(2);
            }
            
            Debug.Log($"[NetworkBotSpawner] Auto-filled teams: T1={team1Needed} bots, T2={team2Needed} bots");
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
        
        private string GetNextBotName()
        {
            if (_botNames.Length == 0) return $"Bot{_botNameIndex}";
            
            string name = _botNames[_botNameIndex % _botNames.Length];
            _botNameIndex++;
            return name;
        }
        
        /// <summary>
        /// Remove all bots (Server only).
        /// </summary>
        public void DespawnAllBots()
        {
            if (!IsServer) return;
            
            foreach (var bot in _spawnedBots)
            {
                if (bot != null && bot.NetworkObject != null)
                {
                    bot.NetworkObject.Despawn();
                    Destroy(bot.gameObject);
                }
            }
            
            _spawnedBots.Clear();
            _botNameIndex = 0;
            
            Debug.Log("[NetworkBotSpawner] All bots despawned");
        }
    }
}
```

---

# 📁 ДЕНЬ 3: GAME MODE INTEGRATION

## ЧАСТЬ 1: ИНТЕГРАЦИЯ С TDM MODE (2 часа)

### ШАГ 1.1: Обновить NetworkTDM3v3Mode.cs

```csharp
// В NetworkTDM3v3Mode.cs

private void SetupTDMMatch()
{
    if (!IsServer) return;
    
    Debug.Log("[NetworkTDM3v3Mode] Setting up TDM 3v3 match...");
    
    // ... существующая логика спавна игроков ...
    
    // AUTO-FILL TEAMS С БОТАМИ
    var botSpawner = FlumpGame.Network.AI.NetworkBotSpawner.Instance;
    if (botSpawner != null)
    {
        botSpawner.AutoFillTeams();
    }
    
    // Start match
    if (NetworkMatchManager.Instance != null)
    {
        NetworkMatchManager.Instance.StartMatch();
    }
    
    _matchStarted = true;
}
```

---

# 📁 ДЕНЬ 4: UNITY SETUP & TESTING

## Unity Editor Setup

### 1. Bot Prefab Setup

```
1. Откройте Bot.prefab
2. Add Component → Network Object
3. Add Component → Network Transform
4. Add Component → Network Bot Controller
5. Add Component → Network Bot Health
6. Assign references в Inspector
7. Save prefab
```

### 2. Network Prefabs

```
1. Откройте NetworkManager в сцене
2. Network Prefabs List → Add
3. Перетащите Bot.prefab
4. Save scene
```

### 3. NetworkBotSpawner Setup

```
1. Создайте пустой GameObject в сцене Game
2. Назовите "NetworkBotSpawner"
3. Add Component → Network Bot Spawner
4. Assign Bot Prefab
5. Max Bots Per Team = 3
6. Auto Fill Teams = true
7. Save scene
```

---

## 🧪 ТЕСТИРОВАНИЕ

### Solo Test
```
1. Play ▶
2. Start Host
3. Проверить что боты заспавнились
4. Проверить что боты видят игрока
5. Проверить что боты стреляют
```

### ParrelSync Test
```
1. Основной: Start Host
2. Клон: Start Client
3. Проверить что боты видны на обоих клиентах
4. Проверить урон от ботов
5. Проверить что боты заполняют команды
```

---

## ✅ ГОТОВО!

После завершения Stage 16:
- ✅ Боты работают в multiplayer
- ✅ Auto-fill команд ботами
- ✅ Network damage sync
- ✅ Respawn ботов
- ✅ Интеграция с TDM 3v3

---

## 🚀 СЛЕДУЮЩИЙ ЭТАП: Stage 17

- UI для game modes
- Matchmaking
- Polish & Testing
