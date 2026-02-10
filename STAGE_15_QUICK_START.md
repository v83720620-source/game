# 🚀 STAGE 15: QUICK START GUIDE

**Цель:** Реализовать Network Game Modes за 1 неделю

---

## 📋 ПЛАН НА СЕГОДНЯ:

### ДЕНЬ 1: Network Foundation (сегодня!)

**Что создадим:**
1. ✅ NetworkMatchManager - управление матчем
2. ✅ NetworkTeamManager - система команд
3. ✅ NetworkSpawnManager - спавн игроков
4. 🎮 Тестирование базовой системы

**Время:** ~4-5 часов

---

## ⚡ БЫСТРЫЙ СТАРТ:

### ШАГ 1: Создайте папки (1 минута)

```
Assets/_Project/Scripts/Networking/
└── Match/                          ← Создайте эту папку
    ├── NetworkMatchManager.cs      ← Я уже создал!
    ├── NetworkTeamManager.cs       ← TODO
    └── NetworkSpawnManager.cs      ← TODO
```

---

### ШАГ 2: Network Match Manager (УЖЕ СОЗДАН!)

**Файл:** `NetworkMatchManager.cs` ✅

**Что он делает:**
- 🎮 Управляет состоянием матча (Waiting → PreMatch → InProgress → PostMatch)
- ⏱️ Синхронизирует таймер по сети
- 🏆 Отслеживает счёт команд (Team 1 vs Team 2)
- 📡 NetworkVariables для автоматической синхронизации
- 🎯 Server-authoritative (сервер контролирует всё)

**Ключевые методы:**
```csharp
NetworkMatchManager.Instance.StartMatchServerRpc();        // Запустить матч
NetworkMatchManager.Instance.AddScore(teamId, points);     // Добавить очки
NetworkMatchManager.Instance.ResetMatch();                 // Сбросить матч
```

---

### ШАГ 3: Добавьте в сцену Game (2 минуты)

```
Unity → Scenes → Game

1. Hierarchy → Create Empty GameObject
2. Rename: "NetworkMatchManager"
3. Add Component → Network Match Manager (Script)
4. Inspector:
   Match Duration: 600 (10 минут)
   Pre Match Duration: 5
   Post Match Duration: 10
   
5. Сохраните сцену (Ctrl + S)
```

---

### ШАГ 4: Быстрый тест (3 минуты)

**Проверка что всё компилируется:**

```
Unity → Play ▶

Console должен показать:
✅ Нет ошибок компиляции
✅ Нет warnings

Stop Play Mode
```

**Если есть ошибки:**
- Убедитесь что `using Unity.Netcode;` есть в начале файла
- Проверьте что Netcode package установлен

---

## 📝 ЧТО ДАЛЬШЕ:

### Сегодня (День 1):

1. **NetworkTeamManager** (1 час)
   - Автоматическое распределение по командам
   - Team balancing
   - Team switching

2. **NetworkSpawnManager** (1 час)
   - Spawn points для команд
   - Safe spawn (далеко от врагов)
   - Duel spawn logic

3. **Тестирование** (1 час)
   - Проверка team assignment
   - Проверка spawn positions
   - Проверка match flow

---

### Завтра (День 2):

**Duel 1v1 Mode** - полная реализация!
- NetworkDuelMode.cs
- First to 5 kills
- Overtime (Sudden Death)
- Duel-specific spawning

---

## 🎯 ТЕКУЩИЙ СТАТУС:

```
✅ NetworkMatchManager.cs - СОЗДАН
⏳ NetworkTeamManager.cs - TODO
⏳ NetworkSpawnManager.cs - TODO
⏳ Unity Setup - TODO
```

---

## 🐛 TROUBLESHOOTING:

### Ошибка: "NetworkVariable not found"
```
Решение:
using Unity.Netcode; ← Добавьте в начало файла
```

### Ошибка: "NetworkBehaviour not found"
```
Решение:
Window → Package Manager → Netcode for GameObjects → Update
```

### Script не добавляется в GameObject
```
Решение:
Assets → Reimport All
```

---

## ✅ ГОТОВЫ ПРОДОЛЖАТЬ?

**Скажите:**
- **"продолжай"** - создам NetworkTeamManager
- **"тест"** - проверю текущий код
- **"вопрос"** - отвечу на вопросы

---

**СЛЕДУЮЩИЙ ФАЙ: NetworkTeamManager.cs** 🚀
