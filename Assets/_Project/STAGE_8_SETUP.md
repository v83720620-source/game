# 🎮 ЭТАП 8: TEAM DEATHMATCH (TDM) РЕЖИМ ИГРЫ

**Время:** 30-40 минут  
**Сложность:** ⭐⭐⭐  

---

## ✅ СКРИПТЫ УЖЕ СОЗДАНЫ!

```
✅ MatchState.cs - состояния матча
✅ MatchManager.cs - управление матчем
✅ TDMGameMode.cs - логика Team Deathmatch
✅ MatchUI.cs - UI счёта и таймера
✅ KillFeedUI.cs - лента убийств
✅ MatchEndUI.cs - экран победы/поражения
```

**Теперь нужно только настроить их в Unity Editor!**

---

## 🎯 ЧТО СОЗДАДИМ:

```
✅ MatchManager GameObject - управление матчем
✅ Match UI - счёт команд и таймер
✅ Kill Feed - лента убийств
✅ Match End Screen - экран победы/поражения
```

---

## 🔧 ШАГ 1: Создайте MatchManager GameObject (5 минут)

### 1.1 Создайте GameObject:

```
Hierarchy → Create Empty → "MatchManager"

Position: (0, 0, 0)
```

### 1.2 Добавьте компоненты:

```
MatchManager:
1. Add Component → Match Manager (Script)
2. Add Component → TDM Game Mode (Script)
```

### 1.3 Настройте Match Manager:

```
Match Manager (Script):
├── Match Duration: 600 (10 минут)
├── Start Countdown: 3
├── Auto Start Match: ✓
└── Team Manager: [перетащите TeamManager из Hierarchy]
```

**Пока оставьте UI поля пустыми - заполним позже!**

---

## 📊 ШАГ 2: Создайте Match UI (10 минут)

### 2.1 Создайте основной контейнер:

```
MobileUI (Canvas) → Create Empty → "MatchUI"

MatchUI - RectTransform:
├── Anchor Preset: Top-Center (верхний ряд, средняя кнопка)
│   Удерживайте Alt+Shift при клике для установки Pivot и Position
├── Pos X: 0
├── Pos Y: -20
├── Width: 400
└── Height: 100
```

**Как установить Anchor:**
1. Выберите MatchUI в Hierarchy
2. В Inspector → RectTransform → кликните на квадрат с кружками (Anchor)
3. Удерживайте **Alt+Shift** и кликните на **Top-Center** (верхний ряд, средняя кнопка)

**Примечание:** MobileUI - это ваш существующий Canvas из предыдущих этапов!

### 2.2 Создайте ScorePanel:

```
MatchUI → Create Empty → "ScorePanel"

ScorePanel - RectTransform:
├── Anchor Preset: Middle-Center (средний ряд, средняя кнопка)
│   Удерживайте Alt+Shift при клике
├── Pos X: 0
├── Pos Y: 0
├── Width: 300
└── Height: 60

ScorePanel - Horizontal Layout Group:
Add Component → Horizontal Layout Group
├── Padding: Left/Right/Top/Bottom = 0
├── Spacing: 20
├── Child Alignment: Middle Center
├── Child Control Size: Width ✓, Height ✓
└── Child Force Expand: Width ✗, Height ✓
```

**Важно:** Убедитесь что Pos X = 0 и Pos Y = 0 для центрирования!

### 2.3 Создайте текст счёта:

```
ScorePanel → UI → Text - TextMeshPro → "Team1Score"

Team1Score:
├── Text: "0"
├── Font Size: 48
├── Alignment: Center + Middle
├── Color: Blue (0, 128, 255)
└── Best Fit / Auto Size: ON (опционально)
```

```
ScorePanel → UI → Text - TextMeshPro → "VS"

VS:
├── Text: "VS"
├── Font Size: 36
├── Alignment: Center + Middle
└── Color: White
```

```
ScorePanel → UI → Text - TextMeshPro → "Team2Score"

Team2Score:
├── Text: "0"
├── Font Size: 48
├── Alignment: Center + Middle
├── Color: Red (255, 64, 64)
└── Best Fit / Auto Size: ON (опционально)
```

### 2.4 Создайте таймер:

```
MatchUI → UI → Text - TextMeshPro → "TimerText"

TimerText - RectTransform:
├── Anchor Preset: Middle-Center (удерживайте Alt+Shift)
├── Pos X: 0
├── Pos Y: -40
├── Width: 150
└── Height: 40

TimerText - TextMeshProUGUI:
├── Text: "10:00"
├── Font Size: 32
├── Alignment: Center (Horizontal + Vertical)
├── Color: White (255, 255, 255)
└── Font Style: Bold (опционально)
```

**Важно:** TimerText должен быть НИЖЕ ScorePanel, поэтому Pos Y = -40 (отрицательное значение)!

### 2.5 Добавьте MatchUI Script:

```
MatchUI → Add Component → Match UI (Script)

Match UI:
├── Team1 Score Text: [перетащите Team1Score]
├── Team2 Score Text: [перетащите Team2Score]
├── Vs Text: [перетащите VS]
├── Timer Text: [перетащите TimerText]
├── State Text: [оставьте пустым]
├── State Panel: [оставьте пустым]
└── Show State Text: ✗ (выключите)
```

---

## 📰 ШАГ 3: Создайте Kill Feed UI (10 минут)

### 3.1 Создайте контейнер:

```
MobileUI (Canvas) → Create Empty → "KillFeed"

KillFeed:
├── Anchor: Top-Right
├── Pivot: (1, 1)
├── Pos X: -20
├── Pos Y: -120
├── Width: 300
└── Height: 200
```

### 3.2 Создайте Container:

```
KillFeed → Create Empty → "Container"

Container:
├── Anchor: Stretch (удерживайте Alt+Shift)
├── Left/Right/Top/Bottom: 0
├── Add Component → Vertical Layout Group
│   ├── Child Alignment: Upper Right
│   ├── Spacing: 5
│   ├── Child Force Expand: Width ✓, Height ✗
│   └── Child Control Size: Width ✓, Height ✓
└── Add Component → Content Size Fitter
    └── Vertical Fit: Preferred Size
```

### 3.3 Добавьте KillFeedUI Script:

```
KillFeed → Add Component → Kill Feed UI (Script)

Kill Feed UI:
├── Max Entries: 5
├── Entry Lifetime: 5
├── Container: [перетащите Container]
└── Entry Prefab: [оставьте пустым - автосоздастся]
```

**Примечание:** `Entry Prefab` создастся автоматически при старте игры!

---

## 🏆 ШАГ 4: Создайте Match End Screen (10 минут)

### 4.1 Создайте панель:

```
MobileUI (Canvas) → UI → Panel → "MatchEndScreen"

MatchEndScreen:
├── Anchor: Stretch (Alt+Shift)
├── Left/Right/Top/Bottom: 0
└── Color: Black (0, 0, 0, 200) - полупрозрачный
```

**ВАЖНО: ВЫКЛЮЧИТЕ MatchEndScreen!**

```
В Hierarchy найдите MatchEndScreen:
- Слева от имени есть галочка ☑
- КЛИКНИТЕ на неё чтобы УБРАТЬ галочку ☐
- MatchEndScreen станет серым (неактивным)
```

**Зачем?** MatchEndScreen должен быть скрыт во время игры и появляться только когда матч закончится. Скрипт `MatchEndUI` сам активирует его в нужный момент!

**НЕ УДАЛЯЙТЕ объект - просто уберите галочку!**

### 4.2 Создайте ResultText:

```
MatchEndScreen → UI → Text - TextMeshPro → "ResultText"

ResultText:
├── Anchor: Top-Center
├── Pos Y: -100
├── Text: "VICTORY!"
├── Font Size: 72
├── Alignment: Center + Middle
└── Color: Green
```

### 4.3 Создайте ScorePanel:

```
MatchEndScreen → Create Empty → "ScorePanel"

ScorePanel:
├── Anchor: Middle-Center
├── Pos Y: 0
├── Width: 400
└── Height: 150
```

```
ScorePanel → UI → Text - TextMeshPro → "FinalScoreText"

FinalScoreText:
├── Anchor: Stretch
├── Text: "Blue Team: 50\nRed Team: 43"
├── Font Size: 36
├── Alignment: Center + Middle
└── Color: White
```

### 4.4 Создайте ReasonText:

```
MatchEndScreen → UI → Text - TextMeshPro → "ReasonText"

ReasonText:
├── Anchor: Middle-Center
├── Pos Y: -80
├── Text: "Reached 50 kills"
├── Font Size: 24
├── Alignment: Center + Middle
└── Color: Gray (180, 180, 180)
```

### 4.5 Создайте кнопки:

```
MatchEndScreen → UI → Button - TextMeshPro → "PlayAgainButton"

PlayAgainButton:
├── Anchor: Bottom-Center
├── Pos Y: 120
├── Width: 200
├── Height: 60
└── Text (Child): "PLAY AGAIN"
```

```
MatchEndScreen → UI → Button - TextMeshPro → "ExitButton"

ExitButton:
├── Anchor: Bottom-Center
├── Pos Y: 50
├── Width: 200
├── Height: 60
└── Text (Child): "EXIT"
```

### 4.6 Добавьте MatchEndUI Script:

```
MatchEndScreen → Add Component → Match End UI (Script)

Match End UI:
├── Panel: [перетащите MatchEndScreen]
├── Result Text: [перетащите ResultText]
├── Team1 Score Text: [перетащите FinalScoreText]
├── Team2 Score Text: [оставьте пустым - используется тот же текст]
├── Reason Text: [перетащите ReasonText]
├── Play Again Button: [перетащите PlayAgainButton]
├── Exit Button: [перетащите ExitButton]
├── Victory Color: Green (0, 255, 0)
├── Defeat Color: Red (255, 0, 0)
└── Draw Color: Yellow (255, 255, 0)
```

---

## 🔗 ШАГ 5: Подключите UI к MatchManager (5 минут)

### 5.1 Настройте Match Manager:

```
Hierarchy → MatchManager → Match Manager (Script)

Match Manager:
├── Match Duration: 600
├── Start Countdown: 3
├── Auto Start Match: ✓
├── Team Manager: [TeamManager]
├── Match UI: [перетащите MatchUI]
├── Kill Feed UI: [перетащите KillFeed]
└── Match End UI: [перетащите MatchEndScreen]
```

**ВСЁ ГОТОВО!** ✅

---

## ✅ ФИНАЛЬНАЯ СТРУКТУРА:

```
Scene:
├── Player (Team1)
├── Bot (Team2)
├── Bot_Team1 (Team1)
├── SpawnPoints (6 штук)
├── SpawnManager
├── TeamManager
├── MatchManager ← НОВОЕ!
│   ├── Match Manager (Script)
│   └── TDM Game Mode (Script)
├── GameHUDManager
└── MobileUI (Canvas)
    ├── Joystick
    ├── Buttons (Jump, Crouch, Fire, Reload)
    ├── GameHUD (из предыдущих этапов)
    │   ├── HealthBar
    │   ├── AmmoDisplay
    │   └── Crosshair
    ├── MatchUI ← НОВОЕ!
    │   ├── ScorePanel
    │   │   ├── Team1Score
    │   │   ├── VS
    │   │   └── Team2Score
    │   └── TimerText
    ├── KillFeed ← НОВОЕ!
    │   └── Container
    └── MatchEndScreen ← НОВОЕ! (неактивен)
        ├── ResultText
        ├── ScorePanel
        │   └── FinalScoreText
        ├── ReasonText
        ├── PlayAgainButton
        └── ExitButton
```

---

## 🎮 ТЕСТИРОВАНИЕ TDM!

### Проверьте старт матча:

1. **Нажмите Play**
2. **Таймер начинает отсчёт:** 10:00 → 9:59 → ...
3. **Счёт:** 0 VS 0

### Проверьте убийства:

1. **Убейте бота** (Team2)
2. **Счёт обновляется:** 1 VS 0 (Team1: синий, Team2: красный)
3. **Kill Feed (справа вверху):** "Player killed Bot"

### Проверьте респавн:

1. **Убитый бот респавнится через 3 секунды**
2. **Kill Feed продолжает показывать события**

### Проверьте победу (БЫСТРЫЙ ТЕСТ):

```
Для быстрого теста:
MatchManager → TDM Game Mode:
└── Kill Limit: 3 (вместо 50)

MatchManager → Match Manager:
└── Match Duration: 30 (вместо 600)
```

**Теперь:**
1. Убейте 3 ботов
2. **Match End Screen появляется!**
3. **"VICTORY!"** - зелёным
4. **Счёт отображается**

### Проверьте поражение:

1. Дайте ботам убить вас 3 раза
2. **"DEFEAT!"** - красным

### Проверьте кнопки:

1. **Play Again** - перезапускает матч
2. **Exit** - выходит из игры (в Editor - останавливает Play Mode)

---

## 🐛 ВОЗМОЖНЫЕ ПРОБЛЕМЫ:

### Счёт не обновляется:
```
Проверьте:
- MatchManager → Team Manager подключён
- MatchUI → все Text поля заполнены
- В Console нет ошибок
```

### Kill Feed не показывается:
```
Проверьте:
- KillFeed → Container подключён
- KillFeed GameObject активен (галочка слева от имени)
```

### Match End Screen не появляется:
```
Проверьте:
- MatchEndScreen ВЫКЛЮЧЕН в Hierarchy (галочка убрана ☐)
  Как проверить: MatchEndScreen должен быть СЕРЫМ в Hierarchy
- MatchManager → Match End UI подключён
- TDM Game Mode → Kill Limit установлен
```

### Match End Screen виден постоянно:
```
Проблема: MatchEndScreen активен (галочка ☑ стоит)
Решение: УБЕРИТЕ галочку слева от MatchEndScreen в Hierarchy
```

### Таймер не работает:
```
Проверьте:
- MatchManager → Auto Start Match: ✓
- MatchUI → Timer Text подключён
```

---

## 📋 CHECKLIST:

```
✅ MatchManager GameObject создан
✅ Match Manager + TDM Game Mode добавлены
✅ MatchUI создан с счётом и таймером
✅ KillFeed создан с Container
✅ MatchEndScreen создан со всеми элементами
✅ Все UI подключены к MatchManager
✅ TDM Game Mode настроен (Kill Limit, Match Duration)
✅ Протестировано: счёт, таймер, убийства
✅ Протестировано: победа/поражение
✅ Кнопки работают
```

---

## 🎯 РЕЗУЛЬТАТ:

```
✅ Полноценный TDM режим!
✅ Счёт команд отслеживается в реальном времени
✅ Таймер матча работает (10 минут)
✅ Победа при 50 убийствах ИЛИ по таймеру
✅ Kill Feed показывает убийства
✅ Экран победы/поражения с кнопками
```

---

## ➡️ СЛЕДУЮЩИЙ ЭТАП:

**ЭТАП 9: Game Mode - Hardpoint (Захват точки)**

---

**ВАЖНО:** Сохраните сцену после завершения! (Ctrl+S)

**Когда протестируете и всё работает - пишите "Этап 8 работает"!** 🚀
