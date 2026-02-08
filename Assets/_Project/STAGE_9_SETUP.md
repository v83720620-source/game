# 🎯 ЭТАП 9: HARDPOINT MODE (ЗАХВАТ ТОЧКИ)

**Время:** 40-50 минут  
**Сложность:** ⭐⭐⭐⭐⭐  

---

## ✅ СКРИПТЫ УЖЕ СОЗДАНЫ!

```
✅ CaptureZone.cs - зона захвата
✅ HardpointGameMode.cs - логика режима
✅ ZoneIndicator.cs - визуализация зоны
✅ HardpointUI.cs - UI режима
```

**Теперь нужно только настроить их в Unity Editor!**

---

## 🎯 ЧТО ТАКОЕ HARDPOINT?

```
ЦЕЛЬ: Набрать 150 очков первыми

КАК ИГРАТЬ:
1. Зона появляется на карте (синий/красный цилиндр)
2. Заходишь в зону → твоя команда начинает получать очки
3. +1 очко каждую секунду пока контролируешь
4. Через 60 секунд зона ПЕРЕМЕЩАЕТСЯ в новое место
5. Первая команда до 150 очков ПОБЕЖДАЕТ!

CONTESTED (спор):
- Если ОБЕ команды в зоне → никто не получает очки
- Нужно выбить врагов из зоны!

OVERTIME:
- Если время истекло, но проигрывающая команда на точке
- Матч продолжается пока они не покинут зону
```

---

## 🔧 ШАГ 1: Создайте зоны захвата (15 минут)

### 1.1 Создайте первую зону:

```
Hierarchy → Create Empty → "Zone1"

Position: (0, 0, 0) - центр карты
```

### 1.2 Добавьте компоненты:

```
Zone1:
1. Add Component → Capture Zone (Script)
2. Add Component → Cylinder (Mesh Filter)
3. Add Component → Mesh Renderer
4. Add Component → Sphere Collider (ДА, Sphere - для лучшего детекта)
```

### 1.3 Настройте Capture Zone:

```
Capture Zone (Script):
├── Zone Radius: 6
├── Capture Rate: 1 (1 очко/сек)
└── Indicator: None (Zone Indicator) - заполним позже
```

### 1.4 Настройте Sphere Collider:

```
Sphere Collider:
├── Is Trigger: ✓
├── Radius: 6
└── Center: (0, 1, 0) - немного выше пола
```

### 1.5 Настройте Transform:

```
Zone1 → Transform:
├── Position: (0, 0.1, 0) - чуть выше пола
├── Rotation: (0, 0, 0)
└── Scale: (6, 0.5, 6) - плоский цилиндр
```

---

## 🎨 ШАГ 2: Создайте визуализацию зоны (10 минут)

### 2.1 Создайте материал для зоны:

```
Project → Materials → Create → Material
Название: "ZoneMaterial"

Settings:
├── Shader: Standard
├── Rendering Mode: Transparent
├── Albedo Color: 
│   - Нейтральная: White (255, 255, 255, 100)
│   - Team1 контроль: Blue (0, 128, 255, 150)
│   - Team2 контроль: Red (255, 64, 64, 150)
│   - Contested: Yellow (255, 255, 0, 150)
├── Metallic: 0
└── Smoothness: 0.3
```

### 2.2 Добавьте Zone Indicator:

```
Zone1 → Create Empty Child → "Indicator"

Indicator:
1. Add Component → Zone Indicator (Script)

Zone Indicator:
├── Neutral Color: White (255, 255, 255, 100)
├── Team1 Color: Blue (0, 128, 255, 150)
├── Team2 Color: Red (255, 64, 64, 150)
├── Contested Color: Yellow (255, 255, 0, 150)
└── Pulse Speed: 1
```

### 2.3 Создайте визуальное кольцо:

```
Indicator → 3D Object → Cylinder → "Ring"

Ring - Transform:
├── Position: (0, 0, 0)
├── Rotation: (0, 0, 0)
└── Scale: (1, 0.1, 1) - тонкое кольцо

Ring - Mesh Renderer:
└── Material: ZoneMaterial

Ring - Remove: Capsule Collider (удалите!)
```

---

## 🗺️ ШАГ 3: Создайте дополнительные зоны (10 минут)

### 3.1 Дублируйте Zone1:

```
Hierarchy → Zone1 → ПКМ → Duplicate

Создайте:
├── Zone2 (Position: 15, 0, 10)
├── Zone3 (Position: -15, 0, -10)
└── Zone4 (Position: 0, 0, 20)
```

**Рекомендация:** Разместите зоны в разных частях карты!

### 3.2 Отключите все зоны кроме Zone1:

```
В Hierarchy:
├── Zone1 ✓ (активна)
├── Zone2 ☐ (выключена - убрать галочку)
├── Zone3 ☐ (выключена)
└── Zone4 ☐ (выключена)
```

**Важно:** Только ОДНА зона активна в момент времени! Остальные выключены.

---

## 🎮 ШАГ 4: Создайте Hardpoint Manager (10 минут)

### 4.1 Создайте GameObject:

```
Hierarchy → MatchManager → Add Component → Hardpoint Game Mode (Script)
```

**Важно:** Добавляем на СУЩЕСТВУЮЩИЙ MatchManager!

### 4.2 Настройте Hardpoint Game Mode:

```
Hardpoint Game Mode (Script):
├── Score Limit: 150 (очков для победы)
├── Zone Duration: 60 (секунд на зону)
├── Points Per Second: 1
├── Enable Overtime: ✓
├── Zones (Array):
│   ├── Size: 4
│   ├── [0]: Zone1
│   ├── [1]: Zone2
│   ├── [2]: Zone3
│   └── [3]: Zone4
└── Start Zone Index: 0
```

### 4.3 Отключите TDM Game Mode:

```
MatchManager → TDM Game Mode:
└── Remove Component (ПКМ → Remove Component)

ИЛИ просто выключите галочку слева от компонента
```

**Примечание:** Только ОДИН режим активен! TDM или Hardpoint.

---

## 📊 ШАГ 5: Создайте Hardpoint UI (15 минут)

### 5.1 Обновите MatchUI:

**Hardpoint использует тот же MatchUI**, но с дополнительными элементами!

```
MobileUI → MatchUI → Create Empty → "ZoneInfo"

ZoneInfo - RectTransform:
├── Anchor: Top-Center
├── Pos X: 0
├── Pos Y: -150
├── Width: 400
└── Height: 100
```

### 5.2 Создайте Zone Status Text:

```
ZoneInfo → UI → Text - TextMeshPro → "ZoneStatus"

ZoneStatus:
├── Text: "CAPTURING..."
├── Font Size: 32
├── Alignment: Center + Middle
├── Color: White
└── Outline: Black (0.2)
```

### 5.3 Создайте Zone Timer:

```
ZoneInfo → UI → Text - TextMeshPro → "ZoneTimer"

ZoneTimer - RectTransform:
├── Anchor: Top-Center
├── Pos Y: -40
├── Width: 200
└── Height: 40

TextMeshProUGUI:
├── Text: "Next Zone: 45s"
├── Font Size: 24
├── Alignment: Center + Middle
├── Color: White
└── Outline: Black (0.2)
```

### 5.4 Создайте Progress Bar:

```
ZoneInfo → UI → Slider → "CaptureProgress"

CaptureProgress - RectTransform:
├── Anchor: Bottom-Center
├── Pos Y: 10
├── Width: 300
└── Height: 20

Slider:
├── Min Value: 0
├── Max Value: 100
├── Value: 0
└── Interactable: ✗ (только для показа)

Background (child):
└── Color: Dark Gray (50, 50, 50)

Fill (child):
└── Color: зависит от команды (синий/красный/желтый)
```

### 5.5 Добавьте Hardpoint UI Script:

```
ZoneInfo → Add Component → Hardpoint UI (Script)

Hardpoint UI:
├── Zone Status Text: [ZoneStatus]
├── Zone Timer Text: [ZoneTimer]
├── Capture Progress: [CaptureProgress]
├── Neutral Color: White
├── Team1 Color: Blue
├── Team2 Color: Red
└── Contested Color: Yellow
```

---

## ✅ ШАГ 6: Финальная структура

```
Scene:
├── Player (Team1)
├── Bot (Team2)
├── Bot_Team1 (Team1)
├── SpawnPoints
├── SpawnManager
├── TeamManager
├── MatchManager
│   ├── Match Manager (Script)
│   ├── TDM Game Mode (Script) ← ВЫКЛЮЧЕН!
│   └── Hardpoint Game Mode (Script) ← НОВОЕ! ВКЛЮЧЕН!
├── GameHUDManager
├── Zone1 ← НОВОЕ! (активна)
│   ├── Capture Zone (Script)
│   ├── Sphere Collider
│   └── Indicator
│       └── Ring
├── Zone2 ← НОВОЕ! (выключена)
├── Zone3 ← НОВОЕ! (выключена)
├── Zone4 ← НОВОЕ! (выключена)
└── MobileUI (Canvas)
    ├── GameHUD
    ├── MatchUI
    │   ├── ScorePanel (счёт команд)
    │   ├── TimerText (таймер матча)
    │   └── ZoneInfo ← НОВОЕ!
    │       ├── ZoneStatus
    │       ├── ZoneTimer
    │       └── CaptureProgress
    ├── KillFeed
    └── MatchEndScreen
```

---

## 🎮 ТЕСТИРОВАНИЕ HARDPOINT!

### Проверьте старт матча:

1. **Нажмите Play**
2. **Zone1 должна быть видна** (синий/белый цилиндр на карте)
3. **UI показывает:** "Next Zone: 60s"

### Проверьте захват:

1. **Подойдите к Zone1**
2. **Зона меняет цвет** на синий (Team1)
3. **UI показывает:** "CAPTURING..." или "Team1 Control"
4. **Счёт растёт:** 0 → 1 → 2 → 3... (каждую секунду +1)

### Проверьте Contested:

1. **Пусть бот Team2 зайдёт в зону вместе с вами**
2. **Зона становится ЖЁЛТОЙ**
3. **UI показывает:** "CONTESTED!"
4. **Счёт НЕ растёт** ни у кого!

### Проверьте ротацию:

**Для быстрого теста:**
```
MatchManager → Hardpoint Game Mode:
└── Zone Duration: 10 (вместо 60)
```

1. Ждите 10 секунд
2. **Zone1 ВЫКЛЮЧАЕТСЯ**
3. **Zone2 ВКЛЮЧАЕТСЯ**
4. **UI обновляется:** "Next Zone: 10s"

### Проверьте победу:

**Для быстрого теста:**
```
MatchManager → Hardpoint Game Mode:
└── Score Limit: 10 (вместо 150)
```

1. Захватите зону и удерживайте 10 секунд
2. **Счёт достигает 10**
3. **Match End Screen:** "VICTORY!"

---

## 🐛 ВОЗМОЖНЫЕ ПРОБЛЕМЫ:

### Зона не видна:
```
Проверьте:
- Zone1 активна (галочка ✓)
- Mesh Renderer включён
- Material назначен
- Scale правильный (6, 0.5, 6)
```

### Не захватывается:
```
Проверьте:
- Sphere Collider → Is Trigger: ✓
- Player и Bot имеют Team Member компонент
- Capture Zone → Zone Radius правильный
```

### Счёт не растёт:
```
Проверьте:
- Hardpoint Game Mode включён
- TDM Game Mode ВЫКЛЮЧЕН
- Team Manager подключён
- Capture Zone → Capture Rate = 1
```

### Зона не ротируется:
```
Проверьте:
- В Hardpoint Game Mode → Zones массив заполнен
- Zone Duration > 0
- Остальные зоны ВЫКЛЮЧЕНЫ в Hierarchy
```

### UI не обновляется:
```
Проверьте:
- Hardpoint UI компонент добавлен на ZoneInfo
- Все Text поля подключены
- Capture Progress подключён
```

---

## 📋 CHECKLIST:

```
✅ Создано 4 зоны (Zone1-4)
✅ Каждая зона имеет Capture Zone компонент
✅ Sphere Collider настроен (Is Trigger ✓)
✅ Визуализация работает (цилиндр + материал)
✅ Hardpoint Game Mode добавлен на MatchManager
✅ TDM Game Mode выключен
✅ Zones массив заполнен (4 зоны)
✅ ZoneInfo UI создан
✅ Hardpoint UI компонент настроен
✅ Протестировано: захват работает
✅ Протестировано: счёт растёт
✅ Протестировано: contested работает
✅ Протестировано: ротация работает
✅ Протестировано: победа работает
```

---

## 🎯 РЕЗУЛЬТАТ:

```
✅ Полноценный режим Hardpoint!
✅ Зоны захвата работают
✅ Система контроля работает
✅ Ротация зон каждые 60 сек
✅ Contested state работает
✅ Overtime работает
✅ UI показывает всю информацию
✅ Победа при 150 очках
```

---

## 📊 СТАТИСТИКА ЭТАПА 9:

```
Скрипты созданы: 4
Зоны созданы: 4
UI элементы: 3
Новые механики: Capture, Rotation, Contested, Overtime
Время разработки: 40-50 минут
```

---

## ➡️ СЛЕДУЮЩИЙ ЭТАП:

**ЭТАП 10: VFX & AUDIO**

```
✅ Звуки выстрелов
✅ Звуки попаданий
✅ Визуальные эффекты
✅ Hitmarkers
✅ UI звуки
```

---

**ВАЖНО:** Сохраните сцену после завершения! (Ctrl+S)

**Когда протестируете и всё работает - пишите "Этап 9 работает"!** 🚀
