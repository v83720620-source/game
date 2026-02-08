# 🤖 ЭТАП 6: AI Bots System Setup

## 📋 Что создано:
- ✅ `BotVision.cs` - Система зрения (обнаружение игрока)
- ✅ `BotWeapon.cs` - Стрельба бота
- ✅ `BotAI.cs` - Основной AI контроллер

---

## 🎯 AI возможности:

```
✅ Патрулирование (случайное или по точкам)
✅ Обнаружение игрока (поле зрения 60°)
✅ Преследование
✅ Атака (стрельба)
✅ Система здоровья
```

---

## 🗺️ ШАГ 1: Настройка NavMesh (10 минут)

### 1.1 Подготовка Ground:

```
Выберите Ground в Hierarchy

Inspector:
1. Сверху справа нажмите "Static" ▼
2. Выберите "Navigation Static" ✓
```

### 1.2 Настройка стен (если есть):

```
Выберите Wall_North, Wall_South, Wall_East, Wall_West

Для каждой стены:
1. Static ▼ → "Navigation Static" ✓
```

### 1.3 Bake NavMesh:

```
Window → AI → Navigation (устаревший метод) 
ИЛИ
Window → AI → Navigation (новый метод)

Во вкладке "Bake":
├── Agent Radius: 0.5
├── Agent Height: 2
├── Max Slope: 45
└── Step Height: 0.4

Нажмите "Bake" внизу!
```

**Результат:** Синяя сетка на полу = NavMesh готов! ✅

---

## 🤖 ШАГ 2: Создание бота (15 минут)

### 2.1 Создайте базовый объект:

```
Hierarchy → 3D Object → Capsule
Название: "Bot"
Position: (10, 1, 0)  // Где-то на карте
```

### 2.2 Добавьте визуал (опционально):

```
Bot:
- Capsule уже есть (будет телом)
- Можете добавить другой цвет:
  
Bot → Mesh Renderer → Materials:
- Создайте новый Material
- Назовите "BotMaterial"
- Измените цвет на красный
```

### 2.3 Добавьте компоненты здоровья:

```
Bot:
1. Add Component → Player Health
   Settings:
   - Max Health: 100
   - Current Health: 100

2. Add Component → Character Controller
   Settings:
   - Center: (0, 1, 0)
   - Radius: 0.5
   - Height: 2
```

### 2.4 Создайте HitBoxes:

```
Bot → 3D Object → Sphere
Название: "HeadHitBox"
Position: (0, 0.65, 0)
Scale: (0.3, 0.3, 0.3)

1. Удалите Mesh Renderer
2. Sphere Collider → Is Trigger: ✓
3. Add Component → Hit Box
   - Hit Zone: Head
   - Player Health: перетащите Bot

Bot → 3D Object → Capsule
Название: "BodyHitBox"
Position: (0, 0, 0)
Scale: (0.4, 0.5, 0.4)

1. Удалите Mesh Renderer
2. Capsule Collider → Is Trigger: ✓
3. Add Component → Hit Box
   - Hit Zone: Body
   - Player Health: перетащите Bot
```

---

## 🧠 ШАГ 3: Добавление AI (10 минут)

### 3.1 NavMesh Agent:

```
Bot:
1. Add Component → Nav Mesh Agent
   Settings:
   - Speed: 3.5
   - Angular Speed: 120
   - Acceleration: 8
   - Stopping Distance: 1
   - Auto Braking: ✓
   - Radius: 0.5
   - Height: 2
```

### 3.2 Bot Vision:

```
Bot:
1. Add Component → Bot Vision
   Settings:
   
   Vision Settings:
   ├── Sight Range: 30
   ├── Sight Angle: 60
   ├── Target Mask: Default (слой игрока)
   └── Obstacle Mask: Default (стены)
   
   Detection:
   └── Detection Time: 0.5
```

### 3.3 Bot Weapon:

```
Bot → Create Empty
Название: "WeaponPoint"
Position: (0, 1.5, 0.5)  // Перед ботом на уровне груди

WeaponPoint:
1. Add Component → Bot Weapon
   Settings:
   
   Weapon Settings:
   ├── Damage: 15
   ├── Fire Rate: 0.3
   ├── Range: 50
   └── Accuracy: 0.8
   
   References:
   ├── Weapon Muzzle: оставьте пустым (сам объект)
   └── Hit Mask: Default
```

### 3.4 Bot AI:

```
Bot:
1. Add Component → Bot AI
   Settings:
   
   AI Settings:
   ├── Patrol Radius: 20
   ├── Patrol Wait Time: 2
   ├── Chase Distance: 30
   ├── Attack Distance: 20
   └── Stop Distance: 10
   
   Patrol Points:
   └── Size: 0 (оставьте пустым для случайного патруля)
```

---

## ✅ ШАГ 4: Настройка Player Layer (ВАЖНО!)

### 4.1 Создайте Layer для игрока:

```
Edit → Project Settings → Tags and Layers

User Layer 9: "Player"
```

### 4.2 Назначьте Player'у Layer:

```
Выберите Player в Hierarchy
Inspector → Layer (сверху) → "Player"

Если спросит про children:
→ "Yes, change children"
```

### 4.3 Обновите Bot Vision:

```
Bot → Bot Vision (Script)
Target Mask: выберите ТОЛЬКО "Player" layer
Obstacle Mask: выберите "Default" (стены)
```

---

## ✅ ШАГ 5: Финальная структура

```
Scene:
├── Ground (Navigation Static ✓)
├── Walls (Navigation Static ✓)
├── Player (Layer: Player)
│   └── ... (все как было)
└── Bot
    ├── Capsule (визуал)
    ├── Character Controller
    ├── Player Health
    ├── Nav Mesh Agent
    ├── Bot Vision
    ├── Bot AI
    ├── HeadHitBox (HitBox - Head)
    ├── BodyHitBox (HitBox - Body)
    └── WeaponPoint
        └── Bot Weapon
```

---

## 🎮 ТЕСТИРОВАНИЕ!

### Нажмите Play и проверьте:

#### Патрулирование:
- ✅ Бот **ходит** по карте случайным образом
- ✅ **Останавливается** на 2 секунды
- ✅ Идёт к новой точке

#### Обнаружение:
- ✅ Подойдите к боту спереди
- ✅ Бот должен **заметить** вас (начнёт идти к вам)
- ✅ В Scene View видна **красная линия** от бота к игроку

#### Атака:
- ✅ Когда бот близко - начинает **стрелять**
- ✅ Вы получаете **урон**
- ✅ Health Bar уменьшается

#### Смерть бота:
- ✅ Стреляйте в бота
- ✅ Когда HP = 0 → **останавливается**
- ✅ В Console: **"Bot Bot died!"**

---

## 🎯 Создание нескольких ботов

### Способ 1: Дублирование

```
1. Выберите Bot в Hierarchy
2. Ctrl+D (дублировать)
3. Переименуйте: Bot_2
4. Переместите на другую позицию (15, 1, 5)
5. Повторите для Bot_3, Bot_4...
```

### Способ 2: Prefab (рекомендуется)

```
1. Перетащите Bot из Hierarchy в папку Prefabs
2. Теперь можете создавать ботов перетаскиванием из Prefabs
3. Все изменения в Prefab применятся ко всем ботам!
```

---

## 🛠️ Настройка патрульных точек (опционально)

### Если хотите точный маршрут патрулирования:

```
1. Hierarchy → Create Empty
   Название: "PatrolPoint1"
   Position: (5, 0, 5)

2. Создайте еще точки:
   - PatrolPoint2: (15, 0, 5)
   - PatrolPoint3: (15, 0, 15)
   - PatrolPoint4: (5, 0, 15)

3. Bot → Bot AI (Script):
   Patrol Points:
   - Size: 4
   - Element 0: PatrolPoint1
   - Element 1: PatrolPoint2
   - Element 2: PatrolPoint3
   - Element 3: PatrolPoint4

Бот будет ходить по точкам по кругу!
```

---

## 🎨 Изменение поведения бота

### Агрессивный бот (атакует с дальней дистанции):

```
Bot AI:
├── Chase Distance: 50  ← Увеличено
├── Attack Distance: 40 ← Увеличено
└── Stop Distance: 20   ← Увеличено
```

### Оборонительный бот (патрулирует небольшую зону):

```
Bot AI:
├── Patrol Radius: 5    ← Уменьшено
├── Chase Distance: 15  ← Уменьшено
└── Attack Distance: 10 ← Уменьшено
```

### Снайпер бот (стоит на месте, стреляет издалека):

```
Bot AI:
├── Patrol Radius: 0    ← Не патрулирует!
├── Chase Distance: 60
├── Attack Distance: 50
└── Stop Distance: 50   ← Не приближается

Bot Weapon:
├── Damage: 40          ← Высокий урон
├── Fire Rate: 1        ← Медленная стрельба
└── Accuracy: 0.95      ← Высокая точность
```

---

## 🐛 Troubleshooting

### Бот не двигается:
- ✅ NavMesh забейкан? (синяя сетка на полу)
- ✅ Nav Mesh Agent добавлен?
- ✅ Ground имеет Navigation Static?

### Бот не видит игрока:
- ✅ Player Layer создан и назначен?
- ✅ Target Mask = Player layer?
- ✅ Sight Range достаточно большой (30)?
- ✅ Вы в поле зрения бота (60° спереди)?

### Бот не стреляет:
- ✅ Bot Weapon добавлен на WeaponPoint?
- ✅ Attack Distance настроен правильно?
- ✅ Hit Mask включает слой игрока?
- ✅ Weapon Muzzle (если пустой - использует свой Transform)?

### Бот стреляет сквозь стены:
- ✅ Obstacle Mask в Bot Vision = Default
- ✅ У стен есть коллайдеры

### Урон не применяется к боту:
- ✅ Player Health добавлен на Bot?
- ✅ HitBox'ы созданы и настроены?
- ✅ HitBox → Player Health назначен?

---

## 🎉 Готово!

Теперь у вас есть:
- ✅ Базовый AI с патрулированием
- ✅ Система обнаружения
- ✅ Стрельба по игроку
- ✅ NavMesh навигация
- ✅ Hit Zones для бота
- ✅ Готово для Multiplayer!

---

## 📸 Что должно получиться:

### В игре:
```
1. Бот патрулирует карту
2. Замечает игрока в радиусе 30м
3. Бежит к игроку
4. Начинает стрелять с 20м
5. Игрок может убить бота (100 HP)
```

### В Scene View:
```
- Синяя сетка на полу (NavMesh)
- Жёлтая сфера вокруг бота (Sight Range)
- Голубые линии от бота (Field of View)
- Красная линия к игроку (обнаружен)
```

---

## 🚀 Следующие этапы:

**Напишите "Этап 6 работает"** когда бот вас атакует!

### ЭТАП 7: Улучшенные боты
- 4 уровня сложности (Dummy, Easy, Normal, Hard)
- Укрытия (Cover system)
- Burst fire (очереди)
- Улучшенная точность
- Реакция на звуки

---

**Начинайте настройку!** 

1. Забейкайте NavMesh
2. Создайте бота
3. Добавьте AI компоненты
4. Тестируйте!

Если будут вопросы - сразу говорите! 🤖
