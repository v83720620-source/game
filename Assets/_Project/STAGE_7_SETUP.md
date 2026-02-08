# 🎮 ЭТАП 7: Team System & Spawn Setup

## 📋 Что создано:
- ✅ `TeamData.cs` - Данные команды (счёт, убийства)
- ✅ `TeamMember.cs` - Компонент члена команды
- ✅ `SpawnPoint.cs` - Точки респавна
- ✅ `SpawnManager.cs` - Система респавна
- ✅ `TeamManager.cs` - Управление командами

---

## 🎯 Система команд:

```
Team 1 (Синие) vs Team 2 (Красные)

✅ Автоматическое присвоение команды
✅ Цветовая индикация (синий/красный)
✅ Счёт команды (kills/deaths)
✅ Система респавна
✅ Командные точки спавна
```

---

## 👥 ШАГ 1: Настройка Player (5 минут)

### 1.1 Добавьте TeamMember на Player:

```
Player:
1. Add Component → Team Member
   
   Settings:
   ├── Team: Team1 (синие)
   ├── Is Player: ✓
   └── Team Color Renderers: Size 0 (пока)
```

---

## 🤖 ШАГ 2: Настройка Bot (5 минут)

### 2.1 Добавьте TeamMember на Bot:

```
Bot:
1. Add Component → Team Member
   
   Settings:
   ├── Team: Team2 (красные)
   ├── Is Player: ☐
   └── Team Color Renderers:
       - Size: 1
       - Element 0: перетащите Mesh Renderer от Bot
```

### 2.2 Создайте второго бота на Team1:

```
1. Duplicate Bot (Ctrl+D)
2. Название: "Bot_Team1"
3. Position: (-10, 1, 0)

Bot_Team1 → Team Member:
- Team: Team1 (синие)  ← ИЗМЕНИТЕ!
```

---

## 📍 ШАГ 3: Создание Spawn Points (10 минут)

### 3.1 Создайте Spawn Manager:

```
Hierarchy → Create Empty
Название: "SpawnManager"
Position: (0, 0, 0)

Add Component → Spawn Manager
Settings:
├── Respawn Delay: 3
└── Auto Respawn: ✓
```

### 3.2 Создайте контейнер для спавнов:

```
Hierarchy → Create Empty
Название: "SpawnPoints"
Position: (0, 0, 0)
```

### 3.3 Создайте спавны для Team1 (синие):

```
SpawnPoints → Create Empty
Название: "Team1_Spawn1"
Position: (-15, 0, -10)

Add Component → Spawn Point
Settings:
├── Team: Team1
├── Is Player Spawn: ✓
└── Spawn Radius: 2

Дублируйте (Ctrl+D) для создания еще спавнов:
- Team1_Spawn2: (-15, 0, 0)
- Team1_Spawn3: (-15, 0, 10)
```

### 3.4 Создайте спавны для Team2 (красные):

```
SpawnPoints → Create Empty
Название: "Team2_Spawn1"
Position: (15, 0, -10)

Add Component → Spawn Point
Settings:
├── Team: Team2
├── Is Player Spawn: ✓
└── Spawn Radius: 2

Дублируйте:
- Team2_Spawn2: (15, 0, 0)
- Team2_Spawn3: (15, 0, 10)
```

---

## 🏆 ШАГ 4: Создание Team Manager (5 минут)

### 4.1 Создайте менеджер:

```
Hierarchy → Create Empty
Название: "TeamManager"
Position: (0, 0, 0)

Add Component → Team Manager
```

### 4.2 Настройте команды:

```
Team Manager (Script):

Team1 Data:
├── Team Id: Team1
├── Team Name: "Blue Team"
├── Team Color: Blue (RGB: 0, 0, 255)
└── Score: 0

Team2 Data:
├── Team Id: Team2
├── Team Name: "Red Team"
├── Team Color: Red (RGB: 255, 0, 0)
└── Score: 0
```

---

## 🔗 ШАГ 5: Интеграция с Health System (10 минут)

### 5.1 Добавьте RespawnHandler на Player:

```
Player:
1. Add Component → Respawn Handler

Settings:
├── Auto Respawn: ✓
└── Respawn Delay: 3
```

**Что это делает:**
- При смерти игрока → ждёт 3 секунды → респавн на Team1 спавне

### 5.2 Добавьте RespawnHandler на ботов:

```
Bot:
1. Add Component → Respawn Handler

Settings:
├── Auto Respawn: ✓
└── Respawn Delay: 3
```

```
Bot_Team1:
1. Add Component → Respawn Handler

Settings:
├── Auto Respawn: ✓
└── Respawn Delay: 3
```

**Теперь боты тоже будут респавниться!**

---

## ✅ ШАГ 6: Финальная структура

```
Scene:
├── Player
│   ├── Player Movement
│   ├── Player Health
│   ├── Team Member (Team1) ← Новое!
│   └── Respawn Handler ← Новое!
│
├── Bot
│   ├── Bot AI
│   ├── Player Health
│   ├── Team Member (Team2) ← Новое!
│   └── Respawn Handler ← Новое!
│
├── Bot_Team1
│   ├── Bot AI
│   ├── Player Health
│   ├── Team Member (Team1) ← Новое!
│   └── Respawn Handler ← Новое!
│
├── SpawnPoints
│   ├── Team1_Spawn1 (SpawnPoint - Team1)
│   ├── Team1_Spawn2 (SpawnPoint - Team1)
│   ├── Team1_Spawn3 (SpawnPoint - Team1)
│   ├── Team2_Spawn1 (SpawnPoint - Team2)
│   ├── Team2_Spawn2 (SpawnPoint - Team2)
│   └── Team2_Spawn3 (SpawnPoint - Team2)
│
├── SpawnManager
│   └── Spawn Manager (Script)
│
└── TeamManager
    └── Team Manager (Script)
```

---

## 🎮 ТЕСТИРОВАНИЕ!

### Проверьте команды:

1. **Нажмите Play**
2. **Посмотрите на ботов:**
   - Bot (Team2) должен быть **КРАСНЫМ**
   - Bot_Team1 (Team1) должен быть **СИНИМ**

### Проверьте респавн:

1. **Убейте бота** (стреляйте пока он не умрёт)
2. **Ждите 3 секунды**
3. **Бот должен РЕСПАВНИТЬСЯ** на своём командном спавне! ✅
4. В Console: **"Bot respawned at Team2_Spawn1"**

### Проверьте свой респавн:

1. **Дайте боту убить вас** (подойдите близко)
2. **Ждите 3 секунды**
3. **Вы респавнитесь** на Team1 спавне! ✅
4. В Console: **"Player respawned at Team1_Spawn1"**

### Проверьте дружественный огонь:

1. **Стреляйте в Bot_Team1** (синий, ваша команда)
2. **Он получает урон** (пока нет проверки команды)
3. **Стреляйте в Bot (Team2)** (красный, враг)
4. **Он получает урон** ✅

**Примечание:** Дружественный огонь пока работает. Отключим в следующем этапе!

### Проверьте Spawn Points (в Scene View):

```
Слева (Team1): 3 синих сферы (спавны)
Справа (Team2): 3 красных сферы (спавны)
```

---

## 🎨 Визуальная индикация команд

### Боты уже окрашены!

Благодаря `TeamMember` компоненту и `Team Color Renderers`:
- Team1 боты = **синие**
- Team2 боты = **красные**

### Для игрока (опционально):

Если хотите окрасить игрока:

```
Player/PlayerModel:
1. Добавьте Mesh Renderer в Team Color Renderers
2. Игрок станет синим (Team1)
```

---

## 🐛 Troubleshooting

### Бот не окрашен:
- ✅ Team Member компонент добавлен?
- ✅ Team Color Renderers заполнен?
- ✅ Mesh Renderer назначен?

### Spawn Points не видны:
- ✅ В Scene View включены Gizmos?
- ✅ Spawn Point компонент добавлен?
- ✅ Team выбран (Team1/Team2)?

### Боты атакуют союзников:
Нужно добавить проверку команды в BotWeapon!
Я создам обновление в следующем шаге.

---

## 🎉 Готово!

Теперь у вас есть:
- ✅ Система команд (Team1 vs Team2)
- ✅ Цветовая индикация
- ✅ Spawn Points для каждой команды
- ✅ Spawn Manager
- ✅ Team Manager для отслеживания счёта

---

## 📸 Что должно получиться:

### В Scene View:
```
         [Синие спавны]     [Красные спавны]
                ⚪              ⚪
              ⚪  ⚪          ⚪  ⚪
                
    Bot_Team1 🔵         Bot 🔴
    (Team1)              (Team2)
```

### В игре:
```
- Боты окрашены (синий/красный)
- Spawn points расставлены
- Готово для режимов игры!
```

---

## 🚀 Следующий этап:

**Напишите "Этап 7 работает"** когда увидите цветных ботов!

### ЭТАП 8: Team Deathmatch Mode
- Игра до 40 убийств
- Таймер 7 минут
- Счётчик счёта
- Win/Lose условия
- MVP система

---

**Начинайте настройку!** 

1. Добавьте TeamMember на Player и Bot
2. Создайте Spawn Points
3. Добавьте TeamManager и SpawnManager
4. Тестируйте!

Если будут вопросы - сразу говорите! 🎮
