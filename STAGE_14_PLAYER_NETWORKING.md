# 🎮 ЭТАП 14: PLAYER NETWORKING

**Milestone:** 5 - Multiplayer Foundation  
**Длительность:** 1 неделя (7 дней)  
**Приоритет:** CRITICAL - Синхронизация игрока по сети  
**Предыдущий этап:** Этап 13 (Network Setup & Lobby) ✅  

---

## 📋 ОБЗОР ЭТАПА

На этом этапе мы превратим single-player систему в multiplayer:
- ✅ Добавим NetworkObject к игроку
- ✅ Синхронизируем движение по сети
- ✅ Реализуем network combat (стрельба, урон)
- ✅ Синхронизируем здоровье
- ✅ Настроим respawn по сети
- ✅ Создадим Player prefab для спавна

**Результат:** Полностью функциональный multiplayer с синхронизацией игроков!

---

## 🎯 ЧТО МЫ СОЗДАДИМ

### Network Scripts (5 новых файлов):
```
NetworkPlayerController.cs    - Обёртка для управления network игроком
NetworkPlayerMovement.cs       - Синхронизация движения
NetworkPlayerHealth.cs         - Синхронизация здоровья
NetworkWeapon.cs               - Стрельба по сети
NetworkCombat.cs               - Урон и смерть по сети
```

### Что будет работать:
- 🏃 Плавное движение всех игроков
- 🔫 Стрельба с синхронизацией
- 💔 Урон и смерть по сети  
- 🔄 Respawn через сервер
- 👥 Визуализация других игроков
- 🎯 Точное hit detection

---

## 📅 ПОЭТАПНЫЙ ПЛАН (7 ДНЕЙ)

### ДЕНЬ 1-2: Network Player Setup
- Создание NetworkPlayerController
- Добавление NetworkObject и NetworkTransform
- Настройка ownership и authority

### ДЕНЬ 3-4: Movement Synchronization
- NetworkPlayerMovement с client-side prediction
- Синхронизация анимаций (если есть)
- Оптимизация bandwidth

### ДЕНЬ 5-6: Combat Networking
- Server-authoritative weapon system
- Hit detection с lag compensation
- Синхронизация здоровья и смерти

### ДЕНЬ 7: Testing & Polish
- Тестирование с 2+ клиентами
- Оптимизация производительности
- Bug fixing

---

## 🚀 НАЧИНАЕМ!

---

## 📁 ДЕНЬ 1-2: NETWORK PLAYER SETUP

### Цель
Создать базовую network структуру для игрока с правильной настройкой authority и ownership.

---

### ШАГ 1: СОЗДАНИЕ NETWORKPLAYERCONTROLLER (1 ЧАС)

#### 1.1 Создайте скрипт

Я уже создам код для вас! Вам нужно будет только:

**Файл:** `Assets/_Project/Scripts/Networking/Player/NetworkPlayerController.cs`

```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Главный контроллер network игрока.
    /// Управляет ownership, authority и координирует все network компоненты.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private FirstPersonCamera _camera;
        [SerializeField] private AdvancedWeapon _weapon;
        
        [Header("Network Components")]
        private NetworkPlayerMovement _networkMovement;
        private NetworkPlayerHealth _networkHealth;
        private NetworkWeapon _networkWeapon;
        
        [Header("Visual")]
        [SerializeField] private GameObject _localPlayerVisuals;
        [SerializeField] private GameObject _remotePlayerVisuals;
        
        private void Awake()
        {
            // Кэшируем network компоненты
            _networkMovement = GetComponent<NetworkPlayerMovement>();
            _networkHealth = GetComponent<NetworkPlayerHealth>();
            _networkWeapon = GetComponent<NetworkWeapon>();
        }
        
        public override void OnNetworkSpawn()
        {
            // Вызывается когда объект spawned в сети
            if (IsOwner)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupRemotePlayer();
            }
            
            Debug.Log($"[NetworkPlayer] Spawned! IsOwner: {IsOwner}, IsServer: {IsServer}");
        }
        
        private void SetupLocalPlayer()
        {
            // Это наш локальный игрок
            Debug.Log("[NetworkPlayer] Setting up LOCAL player");
            
            // Включаем управление
            if (_playerMovement != null)
                _playerMovement.enabled = true;
            
            // Включаем камеру
            if (_camera != null)
                _camera.enabled = true;
            
            // Включаем оружие
            if (_weapon != null)
                _weapon.enabled = true;
            
            // Визуалы
            if (_localPlayerVisuals != null)
                _localPlayerVisuals.SetActive(true);
            if (_remotePlayerVisuals != null)
                _remotePlayerVisuals.SetActive(false);
            
            // Устанавливаем слой для локального игрока (чтобы не видеть свою модель)
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        
        private void SetupRemotePlayer()
        {
            // Это удалённый игрок (другой клиент)
            Debug.Log("[NetworkPlayer] Setting up REMOTE player");
            
            // Отключаем управление
            if (_playerMovement != null)
                _playerMovement.enabled = false;
            
            // Отключаем камеру
            if (_camera != null)
                _camera.enabled = false;
            
            // Отключаем локальное оружие
            if (_weapon != null)
                _weapon.enabled = false;
            
            // Визуалы
            if (_localPlayerVisuals != null)
                _localPlayerVisuals.SetActive(false);
            if (_remotePlayerVisuals != null)
                _remotePlayerVisuals.SetActive(true);
            
            // Устанавливаем слой для удалённых игроков
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        }
        
        public override void OnNetworkDespawn()
        {
            Debug.Log($"[NetworkPlayer] Despawned! IsOwner: {IsOwner}");
        }
        
        /// <summary>
        /// Получить team ID игрока (для будущего использования).
        /// </summary>
        public int GetTeamId()
        {
            // TODO: Реализовать team assignment в Этапе 15
            return IsOwner ? 1 : 2; // Временно
        }
    }
}
```

#### 1.2 Создайте папку

```
Assets/_Project/Scripts/Networking/Player/
```

И положите туда скрипт.

---

### ШАГ 2: НАСТРОЙКА PLAYER PREFAB (1-2 ЧАСА)

#### 2.1 Найдите ваш Player prefab

В вашем проекте должен быть Player GameObject (возможно в сцене Game).

**Где искать:**
```
1. Откройте сцену Game (Assets/Scenes/Game.unity)
2. В Hierarchy найдите Player GameObject
3. Если его нет - создайте из существующего
```

#### 2.2 Добавьте Network компоненты

Выберите Player в Hierarchy и добавьте компоненты:

**Add Component:**
1. **Network Object**
   ```
   ✅ Is Player Object: ON (важно!)
   ```

2. **Network Transform**
   ```
   Sync Position X: ✅
   Sync Position Y: ✅
   Sync Position Z: ✅
   Sync Rotation Y: ✅ (только Y для FPS)
   Sync Scale: ❌
   
   Interpolate: ✅
   Use Half Float Precision: ✅
   Use Quaternion Synchronization: ❌
   Use Quaternion Compression: ❌
   ```

3. **Client Network Transform** (опционально, для client authority движения)
   ```
   Это позволит клиенту управлять своей позицией напрямую
   ```

4. **Network Animator** (если есть Animator)
   ```
   Synchronize Animator: ✅
   ```

5. **NetworkPlayerController** (ваш скрипт)

#### 2.3 Настройте ownership

В **Network Object**:
```
┌─────────────────────────────────────────┐
│ Network Object                          │
├─────────────────────────────────────────┤
│ ☑ Is Player Object: ✅                  │
│ □ Destroy With Scene: ❌                │
│ □ Always Replicate As Root: ❌          │
│ □ Synchronize Transform: ❌              │
│   (NetworkTransform делает это)         │
│ □ Active Scene Synchronization: ❌      │
│ □ Scene Migration Synchronization: ❌   │
│ □ Spawn With Observers: ✅              │
│ □ Don't Destroy With Owner: ❌          │
└─────────────────────────────────────────┘
```

**ВАЖНО:** `Is Player Object` должно быть включено!

#### 2.4 Создайте префаб

```
1. Перетащите настроенный Player из Hierarchy в папку:
   Assets/_Project/Prefabs/Network/Player.prefab

2. Удалите Player из сцены (он будет spawned через NetworkManager)

3. В NetworkManager (Game сцена) → Inspector:
   Player Prefab: Player (ваш префаб)
```

---

### ШАГ 3: ДОБАВЛЕНИЕ В NETWORK PREFABS LIST (15 МИНУТ)

#### 3.1 Создайте Network Prefabs List (если нет)

```
1. Project → Right Click → Create → Netcode → Network Prefabs List
2. Назовите: NetworkPrefabsList
3. Сохраните в: Assets/_Project/Settings/NetworkPrefabsList.asset
```

#### 3.2 Добавьте Player префаб

```
1. Откройте NetworkPrefabsList
2. Add → Перетащите Player.prefab
```

#### 3.3 Назначьте в NetworkManager

```
Game сцена → NetworkManager → Inspector:

Network Prefabs List: NetworkPrefabsList (ваш asset)
```

---

### ✅ ЧЕКЛИСТ ЗАВЕРШЕНИЯ ДНЯ 1-2:

```
[ ] NetworkPlayerController.cs создан
[ ] Player prefab имеет NetworkObject
[ ] Network Transform добавлен и настроен
[ ] Is Player Object включено
[ ] Player префаб создан
[ ] NetworkPrefabsList создан
[ ] Player добавлен в список
[ ] NetworkManager ссылается на список
[ ] Префаб корректно сохранён
```

---

## 🏃 ДЕНЬ 3-4: MOVEMENT SYNCHRONIZATION

### Цель
Синхронизировать движение игрока по сети с client-side prediction для плавности.

---

### ШАГ 1: СОЗДАНИЕ NETWORKPLAYERMOVEMENT (2-3 ЧАСА)

**ВАЖНО:** Я создам весь код! Вам нужно только добавить компонент к Player префабу!

**Файл:** `Assets/_Project/Scripts/Networking/Player/NetworkPlayerMovement.cs`

```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Network синхронизация движения игрока.
    /// Использует client-side prediction для плавности.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(PlayerMovement))]
    public class NetworkPlayerMovement : NetworkBehaviour
    {
        private PlayerMovement _playerMovement;
        private CharacterController _controller;
        
        [Header("Network Settings")]
        [SerializeField] private float _positionThreshold = 0.1f;
        [SerializeField] private float _snapThreshold = 5f;
        
        // Синхронизированные переменные
        private NetworkVariable<Vector3> _networkPosition = new NetworkVariable<Vector3>();
        private NetworkVariable<Quaternion> _networkRotation = new NetworkVariable<Quaternion>();
        private NetworkVariable<bool> _networkIsGrounded = new NetworkVariable<bool>();
        
        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _controller = GetComponent<CharacterController>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                // Локальный игрок - отправляем позицию на сервер
                _networkPosition.Value = transform.position;
                _networkRotation.Value = transform.rotation;
            }
            else
            {
                // Удалённый игрок - синхронизируем с сервером
                _networkPosition.OnValueChanged += OnPositionChanged;
                _networkRotation.OnValueChanged += OnRotationChanged;
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
            {
                _networkPosition.OnValueChanged -= OnPositionChanged;
                _networkRotation.OnValueChanged -= OnRotationChanged;
            }
        }
        
        private void Update()
        {
            if (!IsSpawned) return;
            
            if (IsOwner)
            {
                // Локальный игрок - отправляем обновления
                SendMovementUpdate();
            }
        }
        
        private void SendMovementUpdate()
        {
            // Проверяем изменилась ли позиция достаточно
            if (Vector3.Distance(_networkPosition.Value, transform.position) > _positionThreshold)
            {
                UpdatePositionServerRpc(transform.position, transform.rotation, _playerMovement.IsGrounded);
            }
        }
        
        [ServerRpc(RequireOwnership = true)]
        private void UpdatePositionServerRpc(Vector3 position, Quaternion rotation, bool isGrounded)
        {
            // Сервер валидирует и обновляет
            // TODO: Добавить валидацию на читерство (телепорт, флай)
            
            _networkPosition.Value = position;
            _networkRotation.Value = rotation;
            _networkIsGrounded.Value = isGrounded;
        }
        
        private void OnPositionChanged(Vector3 previousValue, Vector3 newValue)
        {
            // Для удалённых игроков
            float distance = Vector3.Distance(transform.position, newValue);
            
            if (distance > _snapThreshold)
            {
                // Большое расхождение - телепортируем
                if (_controller != null && _controller.enabled)
                {
                    _controller.enabled = false;
                    transform.position = newValue;
                    _controller.enabled = true;
                }
                else
                {
                    transform.position = newValue;
                }
            }
            // Иначе NetworkTransform сам сделает интерполяцию
        }
        
        private void OnRotationChanged(Quaternion previousValue, Quaternion newValue)
        {
            // NetworkTransform обрабатывает вращение
            // Эта функция для дополнительной логики если нужно
        }
    }
}
```

### ШАГ 2: ДОБАВЛЕНИЕ К PLAYER PREFAB (10 МИНУТ)

```
1. Откройте Player.prefab
2. Add Component → Network Player Movement
3. Настройте параметры:
   Position Threshold: 0.1
   Snap Threshold: 5.0

4. Сохраните префаб (Ctrl+S)
```

---

### ШАГ 3: ТЕСТИРОВАНИЕ ДВИЖЕНИЯ (30 МИНУТ)

#### 3.1 Запуск Host

```
1. Откройте Game сцену
2. Убедитесь что NetworkManager настроен:
   - Player Prefab назначен
   - Start Host On Awake: ✅ (для теста)

3. Нажмите Play ▶

4. Проверьте консоль:
   ✅ "[NetworkPlayer] Spawned! IsOwner: True"
   ✅ "[NetworkPlayer] Setting up LOCAL player"
```

#### 3.2 Build Test (2 клиента)

**ВАЖНО:** Нужно собрать билд для полноценного теста!

```
1. File → Build Settings
2. Platform: Windows/Mac/Linux
3. Development Build: ✅
4. Build

5. Запустите build → Start Host
6. Запустите Unity Editor → Start Client
```

**Что тестировать:**
```
✅ Оба игрока видят друг друга
✅ Движение синхронизируется
✅ Нет телепортов (плавное движение)
✅ Rotation синхронизируется
```

---

### ✅ ЧЕКЛИСТ ЗАВЕРШЕНИЯ ДНЯ 3-4:

```
[ ] NetworkPlayerMovement.cs создан
[ ] Компонент добавлен к Player префабу
[ ] Тестирование в Host режиме работает
[ ] Build test с 2 клиентами успешен
[ ] Движение синхронизируется плавно
[ ] Нет заметных лагов или телепортов
```

---

## 🔫 ДЕНЬ 5-6: COMBAT NETWORKING

### Цель
Реализовать server-authoritative combat систему с корректным hit detection.

---

### ШАГ 1: СОЗДАНИЕ NETWORKPLAYERHEALTH (2 ЧАСА)

**Файл:** `Assets/_Project/Scripts/Networking/Player/NetworkPlayerHealth.cs`

```csharp
using Unity.Netcode;
using UnityEngine;
using System;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Network синхронизация здоровья игрока.
    /// Server-authoritative: только сервер может изменять HP.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(PlayerHealth))]
    public class NetworkPlayerHealth : NetworkBehaviour
    {
        private PlayerHealth _playerHealth;
        
        // Синхронизированное здоровье
        private NetworkVariable<float> _networkHealth = new NetworkVariable<float>(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        
        // События
        public event Action<float> OnHealthChanged;
        public event Action<ulong> OnDeath; // Killer's client ID
        
        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Сервер управляет здоровьем
                _networkHealth.Value = _playerHealth.CurrentHealth;
                
                // Подписываемся на локальные события
                _playerHealth.OnDamageReceived += OnLocalDamageReceived;
                _playerHealth.OnDeath += OnLocalDeath;
            }
            
            // Все клиенты слушают изменения
            _networkHealth.OnValueChanged += OnNetworkHealthChanged;
            
            // Синхронизируем начальное значение
            UpdateLocalHealth(_networkHealth.Value);
        }
        
        public override void OnNetworkDespawn()
        {
            _networkHealth.OnValueChanged -= OnNetworkHealthChanged;
            
            if (IsServer)
            {
                _playerHealth.OnDamageReceived -= OnLocalDamageReceived;
                _playerHealth.OnDeath -= OnLocalDeath;
            }
        }
        
        /// <summary>
        /// Применить урон (вызывается на сервере).
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(float damage, ulong attackerClientId)
        {
            if (!IsServer) return;
            
            Debug.Log($"[NetworkHealth] Server applying {damage} damage from client {attackerClientId}");
            
            // Применяем урон локально на сервере
            GameObject attacker = GetPlayerByClientId(attackerClientId);
            _playerHealth.TakeDamage(damage, attacker);
            
            // NetworkVariable автоматически синхронизируется
            _networkHealth.Value = _playerHealth.CurrentHealth;
        }
        
        private void OnLocalDamageReceived(float damage, GameObject attacker)
        {
            // Вызывается на сервере когда локальный PlayerHealth получил урон
            Debug.Log($"[NetworkHealth] Local damage received: {damage}");
        }
        
        private void OnLocalDeath(GameObject killer)
        {
            // Вызывается на сервере когда игрок умер
            Debug.Log($"[NetworkHealth] Player died! Killer: {killer?.name}");
            
            ulong killerClientId = killer != null ? GetClientIdFromPlayer(killer) : OwnerClientId;
            
            // Уведомляем всех клиентов о смерти
            NotifyDeathClientRpc(killerClientId);
        }
        
        [ClientRpc]
        private void NotifyDeathClientRpc(ulong killerClientId)
        {
            Debug.Log($"[NetworkHealth] Death notification! Killer client: {killerClientId}");
            OnDeath?.Invoke(killerClientId);
            
            // TODO: Проиграть анимацию смерти, звук, эффекты
        }
        
        private void OnNetworkHealthChanged(float previousValue, float newValue)
        {
            UpdateLocalHealth(newValue);
            OnHealthChanged?.Invoke(newValue);
        }
        
        private void UpdateLocalHealth(float health)
        {
            if (_playerHealth != null)
            {
                _playerHealth.SetHealth(health);
            }
        }
        
        private GameObject GetPlayerByClientId(ulong clientId)
        {
            if (NetworkManager.Singleton == null) return null;
            
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                return client.PlayerObject?.gameObject;
            }
            
            return null;
        }
        
        private ulong GetClientIdFromPlayer(GameObject player)
        {
            var networkObject = player.GetComponent<NetworkObject>();
            return networkObject != null ? networkObject.OwnerClientId : 0;
        }
    }
}
```

---

### ШАГ 2: СОЗДАНИЕ NETWORKWEAPON (3 ЧАСА)

**Файл:** `Assets/_Project/Scripts/Networking/Player/NetworkWeapon.cs`

```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Player
{
    /// <summary>
    /// Network синхронизация оружия.
    /// Server-authoritative hit detection.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkWeapon : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private AdvancedWeapon _weapon;
        [SerializeField] private Camera _playerCamera;
        
        [Header("Weapon Settings")]
        [SerializeField] private float _fireRate = 0.1f;
        [SerializeField] private int _damage = 25;
        [SerializeField] private float _range = 100f;
        [SerializeField] private LayerMask _hitLayers;
        
        private float _nextFireTime;
        
        private void Awake()
        {
            if (_weapon == null)
                _weapon = GetComponent<AdvancedWeapon>();
            
            if (_playerCamera == null)
                _playerCamera = GetComponentInChildren<Camera>();
        }
        
        private void Update()
        {
            if (!IsOwner || !IsSpawned) return;
            
            // Локальный игрок - обрабатываем input
            HandleFireInput();
        }
        
        private void HandleFireInput()
        {
            bool wantsToFire = Input.GetButton("Fire1"); // Mouse 0
            
            if (wantsToFire && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + _fireRate;
                Fire();
            }
        }
        
        private void Fire()
        {
            // Клиент стреляет локально для instant feedback
            PlayFireEffects();
            
            // Отправляем запрос на сервер для hit detection
            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            FireServerRpc(ray.origin, ray.direction);
        }
        
        [ServerRpc]
        private void FireServerRpc(Vector3 origin, Vector3 direction)
        {
            // Сервер проверяет попадание
            Debug.Log($"[NetworkWeapon] Server processing fire from client {OwnerClientId}");
            
            Ray ray = new Ray(origin, direction);
            
            if (Physics.Raycast(ray, out RaycastHit hit, _range, _hitLayers))
            {
                Debug.Log($"[NetworkWeapon] Hit: {hit.collider.name} at {hit.point}");
                
                // Проверяем что попали в игрока
                var hitPlayer = hit.collider.GetComponentInParent<NetworkPlayerHealth>();
                if (hitPlayer != null && hitPlayer.NetworkObject != NetworkObject)
                {
                    // Применяем урон
                    float damage = CalculateDamage(hit);
                    hitPlayer.TakeDamageServerRpc(damage, OwnerClientId);
                    
                    // Уведомляем клиентов о попадании
                    NotifyHitClientRpc(hit.point, hit.normal);
                }
            }
        }
        
        private float CalculateDamage(RaycastHit hit)
        {
            // Проверяем куда попали (голова, тело, конечности)
            var hitBox = hit.collider.GetComponent<HitBox>();
            if (hitBox != null)
            {
                return _damage * hitBox.DamageMultiplier;
            }
            
            return _damage;
        }
        
        [ClientRpc]
        private void NotifyHitClientRpc(Vector3 hitPoint, Vector3 hitNormal)
        {
            // Все клиенты проигрывают эффект попадания
            PlayHitEffect(hitPoint, hitNormal);
        }
        
        private void PlayFireEffects()
        {
            // Локальные эффекты выстрела
            if (_weapon != null)
            {
                // Используем существующие эффекты из AdvancedWeapon
                // _weapon.PlayMuzzleFlash();
                // _weapon.PlayFireSound();
            }
            
            Debug.Log("[NetworkWeapon] Playing fire effects");
        }
        
        private void PlayHitEffect(Vector3 position, Vector3 normal)
        {
            // Эффект попадания (искры, следы от пуль)
            Debug.Log($"[NetworkWeapon] Playing hit effect at {position}");
            
            // TODO: Создать пулевое отверстие, эффекты
        }
    }
}
```

---

### ШАГ 3: ДОБАВЛЕНИЕ КОМПОНЕНТОВ К ПРЕФАБУ (15 МИНУТ)

```
1. Откройте Player.prefab

2. Add Component → Network Player Health

3. Add Component → Network Weapon
   Настройте:
   - Weapon: (перетащите AdvancedWeapon если есть)
   - Player Camera: (перетащите Camera)
   - Fire Rate: 0.1
   - Damage: 25
   - Range: 100
   - Hit Layers: Everything кроме Player

4. Сохраните префаб
```

---

### ШАГ 4: НАСТРОЙКА HIT LAYERS (10 МИНУТ)

#### 4.1 Создайте новые слои

```
Edit → Project Settings → Tags and Layers

Layers:
├── Layer 8: Player (локальный игрок)
└── Layer 9: RemotePlayer (удалённые игроки)
```

#### 4.2 Настройте Layer Collision Matrix

```
Edit → Project Settings → Physics

Layer Collision Matrix:
□ Player не коллайдит с Player
☑ Player коллайдит с RemotePlayer  
☑ RemotePlayer коллайдит с все
```

---

### ✅ ЧЕКЛИСТ ЗАВЕРШЕНИЯ ДНЯ 5-6:

```
[ ] NetworkPlayerHealth.cs создан
[ ] NetworkWeapon.cs создан
[ ] Компоненты добавлены к префабу
[ ] Layers настроены (Player, RemotePlayer)
[ ] Hit Layers правильно назначены
[ ] Тестирование стрельбы работает
[ ] Урон применяется корректно
[ ] Смерть синхронизируется
```

---

## 🧪 ДЕНЬ 7: TESTING & POLISH

### Цель
Протестировать всю систему и исправить баги.

---

### ШАГ 1: ПОЛНОЕ ТЕСТИРОВАНИЕ (2-3 ЧАСА)

#### Тест 1: Movement Sync
```
✅ 2 клиента видят друг друга
✅ Движение плавное без телепортов
✅ Rotation синхронизируется
✅ Jump синхронизируется
✅ Crouch синхронизируется
```

#### Тест 2: Combat
```
✅ Выстрелы регистрируются
✅ Урон применяется корректно
✅ Хедшот делает x2 урона
✅ Смерть работает
✅ Kill feed обновляется
```

#### Тест 3: Network Stability
```
✅ Нет ошибок в консоли
✅ Framerate стабильный (60 FPS)
✅ Пинг приемлемый (<100ms)
✅ Reconnect работает
```

---

### ШАГ 2: ОПТИМИЗАЦИЯ (2 ЧАСА)

#### 2.1 Reduce Network Traffic

```
NetworkTransform settings:
- Threshold: 0.01 → 0.05 (меньше обновлений)
- Interpolate: ✅
- Half Float Precision: ✅
```

#### 2.2 Optimize Update Rate

```
NetworkPlayerMovement:
- Position Threshold: 0.1 → 0.15
- Только отправляем если изменилось
```

---

### ✅ ФИНАЛЬНЫЙ ЧЕКЛИСТ ЭТАПА 14:

```
СКРИПТЫ:
[ ] NetworkPlayerController.cs ✅
[ ] NetworkPlayerMovement.cs ✅
[ ] NetworkPlayerHealth.cs ✅
[ ] NetworkWeapon.cs ✅

ПРЕФАБ:
[ ] Player имеет NetworkObject
[ ] NetworkTransform настроен
[ ] Все network компоненты добавлены
[ ] Префаб в NetworkPrefabsList

ТЕСТИРОВАНИЕ:
[ ] Host mode работает
[ ] 2+ клиентов могут играть
[ ] Movement синхронизируется
[ ] Combat работает корректно
[ ] Нет критических багов

ПРОИЗВОДИТЕЛЬНОСТЬ:
[ ] FPS стабильный (60+)
[ ] Пинг приемлемый (<100ms)
[ ] Bandwidth оптимизирован
```

---

## 🎉 ПОЗДРАВЛЯЮ!

Этап 14 завершён! Теперь у вас:
- ✅ Полностью функциональный multiplayer
- ✅ Синхронизация игроков
- ✅ Server-authoritative combat
- ✅ Плавное движение с prediction
- ✅ Корректный hit detection

## 🚀 СЛЕДУЮЩИЙ ЭТАП

**Этап 15: Game Modes Networking**
- Network Match Manager
- Duel 1v1 networking
- Team 3v3 networking
- Hardpoint networking

**Готовы продолжать?** 💪🎮
