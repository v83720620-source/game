# 🌐 ПЛАН РАЗРАБОТКИ: MULTIPLAYER + LOBBY + НОВЫЕ РЕЖИМЫ

**Дата:** 30 января 2026  
**Версия:** Milestone 5 - Multiplayer  
**Unity:** 6.3 LTS + Netcode for GameObjects 2.8.0  

---

## 📋 ТЕКУЩИЙ СТАТУС ПРОЕКТА

### ✅ УЖЕ РЕАЛИЗОВАНО (Single-Player):

```
Player Systems:
✅ Movement (Walk, Sprint, Jump, Crouch)
✅ First Person Camera
✅ Mobile Input (Joystick, Buttons)

Combat Systems:
✅ Advanced Weapon (Recoil, Spread, Magazine)
✅ Hit Detection (Head x2.0, Body x1.0, Limbs x0.75)
✅ Health System (100 HP)
✅ Damage System

Team Systems:
✅ Team1 vs Team2
✅ Team Manager
✅ Team-based spawning
✅ Team scoring

Game Modes (Single-Player):
✅ Team Deathmatch (50 kills, 10 min)
✅ Hardpoint (150 points, zone capture)

AI:
✅ Bot AI (Patrol, Chase, Attack)
✅ Bot Vision (FOV detection)
✅ NavMesh navigation

UI:
✅ HUD (Health, Ammo, Crosshair)
✅ Match UI (Timer, Score)
✅ Kill Feed
✅ Match End Screen

Mobile:
✅ Quality Manager (Low/Medium/High)
✅ Optimization for 30-60 FPS
```

---

## 🎯 ЧТО НУЖНО ДОБАВИТЬ

### 🆕 НОВЫЕ РЕЖИМЫ:

**1. Duel 1v1** ⚔️
```
Формат: 1 vs 1
Цель: Первый до 5 убийств
Время: 3 минуты
Overtime: Sudden Death (60 сек, следующий фраг побеждает)
Спавн: Random, далеко от противника
```

**2. Team Deathmatch 3v3** 🎮
```
Формат: 3 vs 3
Цель: 40 фрагов
Время: 7 минут
Спавн: Dynamic team spawn (рядом с союзниками)
```

### 🌐 MULTIPLAYER SYSTEMS:

**1. Lobby System** 🏠
```
Main Menu:
├── Play Button → Режимы
├── Settings
├── Store (будущее)
├── Friends (будущее)
└── Profile

Режимы выбора:
├── Duel 1v1
├── Team 3v3
├── Team 5v5 (TDM)
├── Hardpoint 5v5
└── Practice (боты)
```

**2. Matchmaking** 🔍
```
Quick Match:
├── Поиск игроков
├── Timeout 15s → добавить ботов
├── Timeout 60s → запустить матч
└── Отображение прогресса поиска

Party System (опционально):
├── Invite friends
├── Party lobby
└── Ready check
```

**3. Network Architecture** 🌐
```
Netcode for GameObjects 2.8.0:
├── Server-Authoritative (критично!)
├── Client-side prediction (движение)
├── Lag compensation (hit detection)
├── NetworkTransform (sync position)
└── RPCs (events)
```

---

## 📅 ПОЭТАПНЫЙ ПЛАН РАЗРАБОТКИ

---

## 🟢 MILESTONE 5: MULTIPLAYER FOUNDATION (4-6 недель)

### ЭТАП 13: Network Setup & Lobby (1 неделя)

**13.1 Установка Netcode (1 час)**
```
1. Window → Package Manager
2. Unity Registry → Netcode for GameObjects
3. Install версию 2.8.0+
4. Добавить NetworkManager в сцену
```

**13.2 Main Menu & Lobby UI (2-3 дня)**
```
Scenes:
├── MainMenu (новая сцена)
├── Lobby (новая сцена)
└── Game (существующая MainScene)

Main Menu UI:
├── Background (Logo, Effects)
├── Play Button → Game Modes
├── Settings Button
├── Quit Button
└── Version Display

Game Modes Menu:
├── Duel 1v1
├── Team 3v3
├── Team 5v5 (TDM)
├── Hardpoint 5v5
└── Practice (боты)

Scripts:
├── MainMenuUI.cs
├── GameModeSelector.cs
├── LobbyManager.cs
└── SceneLoader.cs
```

**13.3 Matchmaking UI (2 дня)**
```
Lobby Scene:
├── "Searching..." animation
├── Players Found: X/Y
├── Timer до старта
├── Cancel button
└── Player list (когда найдены)

Scripts:
├── MatchmakingUI.cs
├── PlayerListItem.cs
└── MatchmakingTimer.cs
```

**13.4 Network Manager Setup (2 дня)**
```
NetworkManager component:
├── Transport: Unity Transport (default)
├── Connection Manager
├── Approval Check
└── Scene Management

Scripts:
├── CustomNetworkManager.cs
├── ServerConnection.cs
└── ClientConnection.cs
```

---

### ЭТАП 14: Player Networking (1 неделя)

**14.1 Network Player Controller (3 дня)**
```
Адаптация PlayerMovement:
├── NetworkObject component
├── NetworkTransform (sync position/rotation)
├── ClientNetworkTransform (client authority для движения)
├── Server validation
└── Interpolation для smooth movement

Scripts:
├── NetworkPlayerController.cs
├── NetworkPlayerMovement.cs
└── NetworkCameraController.cs
```

**14.2 Network Combat (3 дня)**
```
Server-Authoritative Combat:
├── Fire RPC (клиент → сервер)
├── Hit Detection на сервере
├── Damage RPC (сервер → клиент)
├── Lag Compensation (rollback)
└── Hit confirmation

Scripts:
├── NetworkWeapon.cs
├── NetworkDamageSystem.cs
├── ServerHitDetection.cs
└── LagCompensation.cs
```

**14.3 Network Health & Death (1 день)**
```
NetworkVariable<float> Health:
├── Sync на всех клиентов
├── Server authority для изменения
├── Death RPC
├── Respawn logic
└── Kill feed sync

Scripts:
├── NetworkPlayerHealth.cs
├── NetworkRespawnHandler.cs
└── NetworkKillFeed.cs
```

---

### ЭТАП 15: Game Modes Networking (1 неделя)

**15.1 Network Match Manager (2 дня)**
```
Адаптация MatchManager:
├── NetworkVariable<MatchState>
├── NetworkVariable<float> Timer
├── Sync счёта команд
├── Match start/end RPCs
└── State sync на всех клиентов

Scripts:
├── NetworkMatchManager.cs
├── NetworkTeamManager.cs
└── NetworkScoreManager.cs
```

**15.2 Duel 1v1 Mode (2 дня)**
```
Новый режим:
├── DuelGameMode.cs (network)
├── 1v1 spawning logic
├── Score tracking (первый до 5)
├── Overtime (Sudden Death)
├── Far spawn logic
└── UI для дуэли

Components:
├── NetworkDuelManager.cs
├── DuelSpawnSystem.cs
└── DuelUI.cs
```

**15.3 Team 3v3 TDM Mode (2 дня)**
```
Адаптация TDM:
├── 3v3 team setup
├── 40 kills limit
├── Dynamic spawning
├── Backfill (боты если кто-то ливнул)
└── MVP calculation

Scripts:
├── NetworkTDMGameMode.cs
├── Team3v3Setup.cs
└── BackfillManager.cs
```

**15.4 Hardpoint 5v5 Mode (1 день)**
```
Адаптация Hardpoint:
├── Network zone control
├── Sync zone state
├── Points per second sync
├── Zone rotation sync
└── Overtime logic

Scripts:
├── NetworkHardpointMode.cs
├── NetworkCaptureZone.cs
└── ZoneStateSync.cs
```

---

### ЭТАП 16: Matchmaking & Bots (1 неделя)

**16.1 Matchmaking System (3 дня)**
```
Queue System:
├── Join queue for game mode
├── Search for players
├── Timeout 15s → add bots
├── Timeout 60s → start match
├── Cancel search
└── Party system (опционально)

Scripts:
├── MatchmakingManager.cs
├── QueueSystem.cs
├── BotFillSystem.cs
└── PartyManager.cs (опционально)
```

**16.2 Bot Integration (2 дня)**
```
Bot Difficulty Levels:
├── Dummy (манекен, FTUE)
├── Easy (новичок)
├── Normal (средний)
└── Hard (ветеран)

Bot Features:
├── Realistic names (pool 10,000)
├── Fake ping (20-60ms)
├── Random avatars
├── Burst fire (3-5 bullets)
├── Aim smoothing (не aimbot!)
├── Cover usage
├── Grenade throwing (будущее)
└── Headshot cap (max 10%)

Adaptations:
├── NetworkBotController.cs
├── BotDifficultyConfig.cs
├── BotNameGenerator.cs
└── BotBehaviorTree.cs
```

**16.3 Backfill System (1 день)**
```
Replace Leavers:
├── Player disconnect detection
├── Wait 30s for reconnect
├── Spawn bot if not returned
├── Match bot difficulty to player
└── Seamless replacement

Scripts:
├── LeaveDetection.cs
├── ReconnectHandler.cs
└── BotSpawnOnLeave.cs
```

**16.4 Anti-Frustration AI (1 день)**
```
Player Protection:
├── No wallhacks (забывает позицию за стеной)
├── Smoke respect (не стреляет сквозь дым точно)
├── 360 no-scope ban (время на разворот)
├── Mercy rule (умер 3 раза → снижение difficulty)
└── Last known position (а не tracking сквозь стены)

Scripts:
├── BotFairPlay.cs
├── MercyRuleSystem.cs
└── LastKnownPosition.cs
```

---

### ЭТАП 17: Server Hosting & Testing (1-2 недели)

**17.1 Server Setup (3-5 дней)**
```
Варианты хостинга:

A. Listen Server (для начала):
   ├── Один игрок = сервер
   ├── Бесплатно
   ├── Простая настройка
   └── ⚠️ Host advantage

B. Dedicated Server (для релиза):
   ├── Unity Multiplay (платно)
   ├── AWS GameLift
   ├── Google Cloud
   └── Custom VPS

Рекомендация: Начать с Listen Server!
```

**17.2 Network Testing (5-7 дней)**
```
Тестирование:
├── Локальная сеть (2 PC)
├── Парад билдов (2+ устройства)
├── Искусственный lag (100ms, 200ms)
├── Packet loss simulation (1%, 5%)
├── Reconnection testing
└── Bot performance

Tools:
├── Unity Network Profiler
├── Netcode Stats
└── Custom debug tools
```

**17.3 Optimization (3-5 дней)**
```
Network Optimization:
├── Reduce update frequency
├── Compress data
├── Delta compression
├── Interest management (только видимые игроки)
└── Snapshot interpolation

Performance:
├── Target: 60 tickrate
├── Server FPS: stable 60+
├── Client FPS: 30-60 mobile
└── Bandwidth: < 100 KB/s per client
```

---

## 📊 ТЕХНИЧЕСКИЕ ТРЕБОВАНИЯ

### Server-Authoritative Architecture:

```
SERVER отвечает за:
✅ Hit Detection (raycast на сервере)
✅ Damage Calculation
✅ HP/Armor State
✅ Death State
✅ Score/Match State
✅ Spawn/Respawn
✅ Bot AI

CLIENT отвечает за:
✅ Input отправка
✅ Movement prediction
✅ Visual/Audio effects
✅ UI updates (предиктивные)
✅ Camera control

CLIENT НЕ МОЖЕТ:
❌ Решать попал ли он
❌ Изменять свой HP
❌ Изменять счёт
❌ Спавнить объекты
❌ Изменять match state
```

### Lag Compensation:

```
Реализация:
1. Server сохраняет snapshots игроков (каждый tick)
2. Клиент стреляет → отправляет RPC с timestamp
3. Server "откатывает" игроков на timestamp клиента
4. Проверяет попадание (raycast)
5. Применяет урон если попал
6. Отправляет подтверждение клиенту

Результат: Честные попадания даже с пингом 100ms!
```

### Client-Side Prediction:

```
Движение:
1. Клиент двигает персонажа локально (instant)
2. Отправляет input на сервер
3. Сервер симулирует и отправляет real position
4. Клиент сравнивает и корректирует если нужно

Результат: Отзывчивое управление без лага!
```

---

## 🗺️ КАРТЫ ДЛЯ РЕЖИМОВ

### Duel 1v1:
```
Требования:
├── Маленькая (20x20m)
├── Симметричная
├── 4-6 spawn points (far from each other)
├── Укрытия (cover)
├── Вертикальность (levels)
└── Центральный контроль (ammo box каждые 60s)

Примеры:
└── Arena_Duel_01 (создать новую)
```

### Team 3v3:
```
Требования:
├── Средняя (30x30m)
├── 3 lane дизайн (left, center, right)
├── 2x3 spawn zones (по команде)
├── Cover points
└── Flank routes

Примеры:
└── Можно использовать текущую MainScene (уменьшить)
```

### Team 5v5:
```
Требования:
├── Большая (40x40m)
├── Multiple lanes
├── 2x5 spawn zones
├── Cover positions
└── Objective zones (для Hardpoint)

Примеры:
└── Текущая MainScene подходит!
```

---

## 💾 ДАННЫЕ И БАЛАНСИРОВКА

### ScriptableObjects:

**GameModeData.cs:**
```csharp
public class GameModeData : ScriptableObject
{
    public string modeName;
    public int playersPerTeam; // 1, 3, 5
    public int scoreLimit; // 5, 40, 50, 150
    public float matchDuration; // 180s, 420s, 600s
    public bool enableOvertime;
    public GameModeType type; // Duel, TDM, Hardpoint
}
```

**BotDifficultyData.cs:**
```csharp
public class BotDifficultyData : ScriptableObject
{
    public string difficultyName; // Dummy, Easy, Normal, Hard
    public float accuracyMult; // 2.0, 1.5, 1.0, 0.7
    public float reactionTime; // 1.0s, 0.7s, 0.5s, 0.2s
    public float headshotCap; // 0%, 5%, 10%, 10%
    public float aggressionFactor; // 0.2, 0.5, 0.7, 0.9
    public int healthThreshold; // 20%, 30%, 40%, 50%
}
```

---

## 🎨 UI ДИЗАЙН

### Main Menu:
```
Layout:
├── Background (3D scene с персонажем)
├── Logo (вверху)
├── Play Button (центр, большая)
├── Settings, Store, Friends (внизу)
└── Version + Ping (правый нижний угол)
```

### Game Mode Selection:
```
Grid Layout (2x2 или 1x4):
├── [Duel 1v1] (иконка скрещенных мечей)
├── [Team 3v3] (иконка 3 человека)
├── [Team 5v5 TDM] (иконка череп)
├── [Hardpoint 5v5] (иконка флаг)
└── [Practice] (иконка мишень + бот)

Каждая карточка:
├── Название режима
├── Краткое описание
├── Онлайн игроков (если есть)
└── Play кнопка
```

### Matchmaking Screen:
```
Center:
├── "Searching for players..."
├── Animated loading spinner
├── Players found: 4/6
├── Timer: 00:23
└── Cancel button

Bottom:
├── Selected mode: "Team 3v3"
└── Expected wait: ~30s
```

---

## 🔧 ТЕХНОЛОГИЧЕСКИЙ СТЕК

```
Core:
├── Unity 6.3 LTS
├── URP 17.3.0
├── Netcode for GameObjects 2.8.0
├── Unity Transport (default)
└── C# (.NET Standard 2.1)

Multiplayer:
├── Server-Authoritative
├── Client-Side Prediction
├── Lag Compensation
├── Snapshot Interpolation
└── Delta Compression

Architecture:
├── NetworkBehaviours
├── RPCs (ServerRpc, ClientRpc)
├── NetworkVariables
├── NetworkObject
└── SceneManagement
```

---

## 📈 МЕТРИКИ И АНАЛИТИКА

### Отслеживаем:

**Player Metrics:**
```
├── K/D Ratio
├── Win Rate (по режимам)
├── Average Match Duration
├── Most Played Mode
└── Skill Level (MMR)
```

**Bot Metrics:**
```
├── Bot K/D Ratio (цель: 0.8-0.9)
├── Bot Win Rate
├── Bot Lifetime Average
└── Reports на ботов за "читы"
```

**Network Metrics:**
```
├── Average Ping
├── Packet Loss %
├── Tickrate stability
├── Desync events
└── Reconnection success rate
```

**Matchmaking Metrics:**
```
├── Average Queue Time
├── Match Fill Rate (% без ботов)
├── Leaver Rate
└── Match Completion Rate
```

---

## 🎯 ПРИОРИТЕТЫ

### MUST HAVE (MVP):
```
1. ✅ Main Menu + Lobby UI
2. ✅ Network Player Controller
3. ✅ Server-Authoritative Combat
4. ✅ Duel 1v1 Mode
5. ✅ Team 3v3 TDM Mode
6. ✅ Matchmaking с ботами
7. ✅ Listen Server hosting
```

### NICE TO HAVE:
```
1. 🔧 Team 5v5 modes (уже есть single-player)
2. 🔧 Party System (друзья)
3. 🔧 Voice Chat
4. 🔧 Dedicated Server
5. 🔧 Anti-cheat
6. 🔧 Ranked Mode
```

### FUTURE:
```
1. 📦 Store (скины, оружие)
2. 📦 Battle Pass
3. 📦 Seasonal Content
4. 📦 Tournaments
5. 📦 Clans/Guilds
```

---

## ⏰ ВРЕМЕННАЯ ШКАЛА

```
Milestone 5: MULTIPLAYER

Неделя 1:  Network Setup + Lobby UI
Неделя 2:  Player Networking
Неделя 3:  Game Modes Networking
Неделя 4:  Matchmaking + Bots
Неделя 5:  Server Hosting + Testing
Неделя 6:  Optimization + Polish

ИТОГО: 6 недель full-time разработки
```

---

## 📚 РЕСУРСЫ

### Документация:
```
Unity Netcode:
└── docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.8/

Tutorials:
└── learn.unity.com/tutorial/get-started-with-netcode-for-gameobjects

Lag Compensation:
└── docs.unity3d.com/.../dealing-with-latency.html
```

### Примеры:
```
Boss Room Sample:
└── github.com/Unity-Technologies/com.unity.multiplayer.samples.coop

Unity Multiplayer Samples:
└── github.com/Unity-Technologies/com.unity.multiplayer.samples
```

---

## 🎊 ИТОГО

**Что получим:**

```
✅ Main Menu + Lobby
✅ Matchmaking System
✅ Duel 1v1 (новый режим!)
✅ Team 3v3 TDM (новый режим!)
✅ Team 5v5 TDM (network версия)
✅ Hardpoint 5v5 (network версия)
✅ Smart Bots (4 уровня сложности)
✅ Server-Authoritative Combat
✅ Lag Compensation
✅ Client-Side Prediction
✅ Backfill System
✅ Anti-Frustration AI
```

**Готово к релизу Online PvP игры!** 🚀

---

**Дата:** 30 января 2026  
**Статус:** План готов к реализации  
**Следующий шаг:** Этап 13 - Network Setup & Lobby
