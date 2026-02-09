# 🎮 ЭТАП 13: ИНСТРУКЦИИ ПО НАСТРОЙКЕ В UNITY EDITOR

**Статус кода:** ✅ ВСЕ СКРИПТЫ СОЗДАНЫ  
**Netcode:** ✅ УЖЕ УСТАНОВЛЕН (v2.9.1)  
**Что осталось:** Настройка в Unity Editor

---

## ✅ ЧТО УЖЕ СДЕЛАНО АВТОМАТИЧЕСКИ:

### Созданные скрипты:
```
✅ CustomNetworkManager.cs - управление сетью
✅ NetworkSceneManager.cs - загрузка сцен по сети
✅ SceneLoader.cs - загрузка сцен (singleton)
✅ GameModeData.cs - ScriptableObject для режимов
✅ GameModeManager.cs - singleton для выбранного режима
✅ MainMenuUI.cs - UI главного меню
✅ GameModeSelector.cs - выбор режима игры
✅ MatchmakingUI.cs - поиск игроков
✅ PlayerListItem.cs - элемент списка игроков
```

---

## 📋 ЧТО НУЖНО СДЕЛАТЬ В UNITY (ПОШАГОВО)

---

## ЧАСТЬ 1: СОЗДАНИЕ SCRIPTABLEOBJECTS (5-10 минут)

### Шаг 1: Создайте папку для ScriptableObjects

```
1. В Project окне:
   Assets/_Project/ScriptableObjects/GameModes/ (создайте если нет)
```

### Шаг 2: Создайте 5 Game Mode Assets

**Важно:** Нажмите правой кнопкой в папке GameModes → Create → Flump → Game Mode Data

#### 1️⃣ Duel 1v1
```
Имя файла: GameMode_Duel1v1
━━━━━━━━━━━━━━━━━━━━━━━━━━
Mode Info:
  Mode Name: Duel 1v1
  Description: 1v1 battle. First to 5 kills wins!
  Mode Type: Duel1v1
  Icon: (оставьте пустым пока)

Match Settings:
  Players Per Team: 1
  Score Limit: 5
  Match Duration Seconds: 180
  Enable Overtime: ✅

Scene:
  Game Scene Name: Game

Display:
  Show In Menu: ✅
  Sort Order: 0
```

#### 2️⃣ Team 3v3 TDM
```
Имя файла: GameMode_Team3v3TDM
━━━━━━━━━━━━━━━━━━━━━━━━━━
Mode Info:
  Mode Name: Team 3v3
  Description: 3v3 team battle. Reach 40 kills to win!
  Mode Type: Team3v3TDM
  Icon: (оставьте пустым пока)

Match Settings:
  Players Per Team: 3
  Score Limit: 40
  Match Duration Seconds: 420
  Enable Overtime: ✅

Scene:
  Game Scene Name: Game

Display:
  Show In Menu: ✅
  Sort Order: 1
```

#### 3️⃣ Team 5v5 TDM
```
Имя файла: GameMode_Team5v5TDM
━━━━━━━━━━━━━━━━━━━━━━━━━━
Mode Info:
  Mode Name: Team Deathmatch 5v5
  Description: Classic 5v5 battle. First team to 50 kills!
  Mode Type: Team5v5TDM
  Icon: (оставьте пустым пока)

Match Settings:
  Players Per Team: 5
  Score Limit: 50
  Match Duration Seconds: 600
  Enable Overtime: ✅

Scene:
  Game Scene Name: Game

Display:
  Show In Menu: ✅
  Sort Order: 2
```

#### 4️⃣ Hardpoint 5v5
```
Имя файла: GameMode_Hardpoint5v5
━━━━━━━━━━━━━━━━━━━━━━━━━━
Mode Info:
  Mode Name: Hardpoint 5v5
  Description: Capture and hold the zone to reach 150 points!
  Mode Type: Hardpoint5v5
  Icon: (оставьте пустым пока)

Match Settings:
  Players Per Team: 5
  Score Limit: 150
  Match Duration Seconds: 600
  Enable Overtime: ✅

Scene:
  Game Scene Name: Game

Display:
  Show In Menu: ✅
  Sort Order: 3
```

#### 5️⃣ Practice (Bots)
```
Имя файла: GameMode_Practice
━━━━━━━━━━━━━━━━━━━━━━━━━━
Mode Info:
  Mode Name: Practice vs Bots
  Description: Train your skills against AI bots
  Mode Type: Practice
  Icon: (оставьте пустым пока)

Match Settings:
  Players Per Team: 1
  Score Limit: 999
  Match Duration Seconds: 999
  Enable Overtime: ❌

Scene:
  Game Scene Name: Game

Display:
  Show In Menu: ✅
  Sort Order: 4
```

---

## ЧАСТЬ 2: НАСТРОЙКА MAINMENU СЦЕНЫ (15-20 минут)

### Шаг 1: Откройте сцену MainMenu

```
Assets/Scenes/MainMenu.unity (двойной клик)
```

### Шаг 2: Создайте базовую UI структуру

#### 2.1 Создайте Canvas (если нет)
```
Hierarchy → Right Click → UI → Canvas

Canvas (настройки):
├── Render Mode: Screen Space - Overlay
├── Canvas Scaler:
│   ├── UI Scale Mode: Scale With Screen Size
│   ├── Reference Resolution: 1920 x 1080
│   └── Match: 0.5
└── Graphic Raycaster: ✅
```

#### 2.2 Создайте EventSystem (если нет)
```
Hierarchy → Right Click → UI → Event System
(должен быть только один в сцене)
```

#### 2.3 Создайте фон
```
Canvas → Right Click → UI → Image
Имя: Panel_Background

Inspector:
├── Anchor: Stretch / Stretch (Alt+Shift+Click на anchor)
├── Left, Right, Top, Bottom: 0
├── Color: (0, 0, 0, 200) - полупрозрачный чёрный
```

#### 2.4 Создайте Panel главного меню
```
Canvas → Right Click → UI → Panel
Имя: Panel_MainMenu

Inspector:
├── Anchor Preset: Center
├── Width: 400
├── Height: 600
├── Add Component → Vertical Layout Group:
│   ├── Padding: 20, 20, 20, 20
│   ├── Spacing: 20
│   ├── Child Alignment: Upper Center
│   ├── Child Force Expand: Width ✅, Height ❌
│   └── Child Controls Size: Width ✅, Height ✅
```

#### 2.5 Создайте элементы меню внутри Panel_MainMenu

**Заголовок:**
```
Panel_MainMenu → Right Click → UI → Text - TextMeshPro
Имя: Text_Title

Inspector:
├── Text: "FLUMP GAME"
├── Font Size: 72
├── Alignment: Center/Middle
├── Font Style: Bold
├── Color: White
├── Add Component → Layout Element:
    └── Min Height: 100
```

**Spacer (отступ):**
```
Panel_MainMenu → Right Click → UI → Image
Имя: Spacer
Alpha: 0 (прозрачный)
Layout Element → Min Height: 50
```

**Кнопка Play:**
```
Panel_MainMenu → Right Click → UI → Button - TextMeshPro
Имя: Button_Play

Inspector (Button):
├── Layout Element → Min Height: 80
└── Text (внутри Button):
    ├── Text: "PLAY"
    ├── Font Size: 36
    └── Font Style: Bold
```

**Кнопка Settings:**
```
Panel_MainMenu → Right Click → UI → Button - TextMeshPro
Имя: Button_Settings

Inspector (Button):
├── Layout Element → Min Height: 60
└── Text: "Settings"
    └── Font Size: 24
```

**Кнопка Quit:**
```
Panel_MainMenu → Right Click → UI → Button - TextMeshPro
Имя: Button_Quit

Inspector (Button):
├── Layout Element → Min Height: 60
└── Text: "Quit"
    └── Font Size: 24
```

**Версия (внизу справа):**
```
Canvas → Right Click → UI → Text - TextMeshPro
Имя: Text_Version

Inspector:
├── Anchor: Bottom Right
├── Position: X: -10, Y: 10
├── Text: "v0.5.0 Alpha"
├── Font Size: 18
├── Alignment: Right/Bottom
├── Color: Gray (128, 128, 128)
```

#### 2.6 Создайте Panel выбора режимов
```
Canvas → Right Click → UI → Panel
Имя: Panel_GameModeSelection
Active: ❌ (выключен по умолчанию!)

Inspector:
├── Anchor: Stretch/Stretch
├── Left, Right, Top, Bottom: 0
```

**Внутри Panel_GameModeSelection создайте:**

**Заголовок:**
```
Panel_GameModeSelection → UI → Text - TextMeshPro
Имя: Text_Header
├── Anchor: Top Center
├── Position: Y: -50
├── Width: 600, Height: 80
├── Text: "SELECT GAME MODE"
├── Font Size: 48
├── Alignment: Center
```

**Кнопка Back:**
```
Panel_GameModeSelection → UI → Button - TextMeshPro
Имя: Button_Back
├── Anchor: Top Left
├── Position: X: 100, Y: -50
├── Width: 150, Height: 60
├── Text: "← Back"
```

**Scroll View для режимов:**
```
Panel_GameModeSelection → UI → Scroll View
Имя: ScrollView_GameModes
├── Anchor: Center
├── Width: 800, Height: 700
├── Scroll Rect:
│   └── Vertical: ✅, Horizontal: ❌
└── Content (внутри Viewport):
    ├── Add Component → Vertical Layout Group
    │   ├── Padding: 10
    │   ├── Spacing: 20
    │   └── Child Force Expand: Width ✅, Height ❌
    └── Content Size Fitter:
        └── Vertical Fit: Preferred Size
```

#### 2.7 Создайте MainMenuManager GameObject
```
Hierarchy → Create Empty
Имя: MainMenuManager

Inspector:
├── Add Component → Main Menu UI (script)
└── Заполните поля:
    Main Menu Panels:
      Main Menu Panel: Panel_MainMenu
      Game Mode Selection Panel: Panel_GameModeSelection
    
    Main Menu Buttons:
      Play Button: Button_Play
      Settings Button: Button_Settings
      Quit Button: Button_Quit
    
    Version Display:
      Version Text: Text_Version
```

#### 2.8 Настройте GameModeSelector
```
Panel_GameModeSelection → Add Component → Game Mode Selector

Заполните поля:
References:
  Game Mode Container: Content (из ScrollView)
  Game Mode Card Prefab: (создадим в следующем шаге)
  Back Button: Button_Back

Game Modes:
  Size: 5
  Element 0: GameMode_Duel1v1
  Element 1: GameMode_Team3v3TDM
  Element 2: GameMode_Team5v5TDM
  Element 3: GameMode_Hardpoint5v5
  Element 4: GameMode_Practice
```

### Шаг 3: Создайте Prefab для Game Mode Card

#### 3.1 Создайте временный GameObject
```
Hierarchy → UI → Panel
Имя: GameModeCard
```

#### 3.2 Настройте структуру карточки
```
GameModeCard (Panel):
├── Width: 750, Height: 150
├── Add Component → Button
├── Add Component → Horizontal Layout Group
│   ├── Padding: 20
│   ├── Spacing: 20
│   └── Child Force Expand: Height ✅
│
├── Image_Icon (UI → Image):
│   ├── Width: 100, Height: 100
│   └── Layout Element → Preferred Width: 100
│
└── InfoContainer (UI → Panel):
    ├── Layout Element → Flexible Width: 1
    ├── Add Component → Vertical Layout Group
    │   ├── Spacing: 10
    │   └── Child Alignment: Upper Left
    │
    ├── Text_ModeName (UI → Text - TextMeshPro):
    │   ├── Text: "Team Deathmatch"
    │   ├── Font Size: 28
    │   └── Font Style: Bold
    │
    ├── Text_Description (UI → Text - TextMeshPro):
    │   ├── Text: "Description here"
    │   ├── Font Size: 18
    │   └── Color: Light Gray
    │
    └── Text_PlayersOnline (UI → Text - TextMeshPro):
        ├── Text: "🟢 0 players online"
        ├── Font Size: 16
        └── Color: Green
```

#### 3.3 Сохраните как префаб
```
Перетащите GameModeCard из Hierarchy в папку:
Assets/_Project/Prefabs/UI/

Затем удалите GameModeCard из Hierarchy
```

#### 3.4 Назначьте префаб в GameModeSelector
```
Panel_GameModeSelection → Inspector → Game Mode Selector:
  Game Mode Card Prefab: GameModeCard (префаб)
```

### Шаг 4: Сохраните сцену
```
Ctrl+S или File → Save
```

---

## ЧАСТЬ 3: НАСТРОЙКА LOBBY СЦЕНЫ (15-20 минут)

### Шаг 1: Откройте сцену Lobby
```
Assets/Scenes/Lobby.unity (двойной клик)
```

### Шаг 2: Создайте UI структуру

#### 2.1 Canvas и EventSystem (если нет)
```
Создайте так же как в MainMenu
```

#### 2.2 Фон
```
Canvas → UI → Image
Имя: Panel_Background
Anchor: Stretch/Stretch
Color: (0, 0, 0, 200)
```

#### 2.3 Panel_SearchStatus (экран поиска)
```
Canvas → UI → Panel
Имя: Panel_SearchStatus

Inspector:
├── Anchor: Center
├── Width: 600, Height: 500
```

**Внутри Panel_SearchStatus создайте:**

**Loading Spinner:**
```
UI → Image
Имя: Image_LoadingSpinner
├── Width: 100, Height: 100
├── Position: Y: 150
├── Sprite: любой круглый спрайт
└── (будет вращаться через код)
```

**Текст статуса:**
```
UI → Text - TextMeshPro
Имя: Text_Status
├── Position: Y: 50
├── Width: 500, Height: 60
├── Text: "Searching for players..."
├── Font Size: 28
├── Alignment: Center
```

**Игроки найдены:**
```
UI → Text - TextMeshPro
Имя: Text_PlayersFound
├── Position: Y: -20
├── Width: 400, Height: 50
├── Text: "Players found: 0/10"
├── Font Size: 24
├── Alignment: Center
```

**Таймер:**
```
UI → Text - TextMeshPro
Имя: Text_Timer
├── Position: Y: -80
├── Width: 200, Height: 50
├── Text: "00:00"
├── Font Size: 32
├── Alignment: Center
```

**Кнопка Cancel:**
```
UI → Button - TextMeshPro
Имя: Button_Cancel
├── Position: Y: -180
├── Width: 200, Height: 60
├── Text: "Cancel"
├── Font Size: 24
```

#### 2.4 Panel_PlayerList (список игроков)
```
Canvas → UI → Panel
Имя: Panel_PlayerList
Active: ❌ (выключен!)

Inspector:
├── Anchor: Stretch/Stretch
├── Margins: 50
```

**Внутри Panel_PlayerList:**

**Заголовок:**
```
UI → Text - TextMeshPro
Имя: Text_Title
├── Anchor: Top Center
├── Position: Y: -30
├── Text: "LOBBY"
├── Font Size: 48
```

**Scroll View:**
```
UI → Scroll View
Имя: ScrollView_Players
├── Anchor: Center
├── Width: 800, Height: 600
├── Content:
    ├── Vertical Layout Group
    ├── Spacing: 10
    └── Content Size Fitter: Vertical Fit = Preferred Size
```

**Кнопка Back:**
```
UI → Button - TextMeshPro
Имя: Button_Back
├── Anchor: Top Left
├── Position: X: 100, Y: -30
├── Text: "← Back"
```

### Шаг 3: Создайте префаб PlayerListItem

#### 3.1 Создайте временный GameObject
```
Hierarchy → UI → Panel
Имя: PlayerListItem
```

#### 3.2 Настройте структуру
```
PlayerListItem (Panel):
├── Width: 750, Height: 80
├── Background Color: (20, 20, 20)
├── Add Component → Horizontal Layout Group
│   ├── Padding: 15
│   ├── Spacing: 20
│   └── Child Alignment: Middle Left
│
├── Image_Avatar (UI → Image):
│   ├── Width: 50, Height: 50
│   ├── Color: Gray
│   └── Sprite: круглый спрайт (или None)
│
├── InfoContainer (UI → Panel):
│   ├── Layout Element → Flexible Width: 1
│   ├── Vertical Layout Group
│   │
│   ├── Text_PlayerName (UI → Text - TMP):
│   │   ├── Text: "PlayerName"
│   │   └── Font Size: 20
│   │
│   └── StatsContainer (UI → Panel):
│       ├── Horizontal Layout Group, Spacing: 20
│       │
│       ├── Text_Level (UI → Text - TMP):
│       │   ├── Text: "Level 15"
│       │   └── Font Size: 16
│       │
│       └── Text_Ping (UI → Text - TMP):
│           ├── Text: "25ms"
│           └── Font Size: 16
│
└── Image_TeamColor (UI → Image):
    ├── Width: 20, Height: 80
    └── Color: Blue или Red
```

#### 3.3 Добавьте скрипт и сохраните
```
PlayerListItem → Add Component → Player List Item (script)

Заполните поля (перетащите из иерархии):
  Avatar Image: Image_Avatar
  Player Name Text: Text_PlayerName
  Level Text: Text_Level
  Ping Text: Text_Ping
  Team Color Image: Image_TeamColor

Сохраните как префаб:
  Assets/_Project/Prefabs/UI/PlayerListItem.prefab

Удалите из Hierarchy
```

### Шаг 4: Создайте LobbyManager и настройте
```
Hierarchy → Create Empty
Имя: LobbyManager

Add Component → Matchmaking UI

Заполните поля:
Panels:
  Search Panel: Panel_SearchStatus
  Player List Panel: Panel_PlayerList

Search Status:
  Status Text: Text_Status
  Players Found Text: Text_PlayersFound
  Timer Text: Text_Timer
  Loading Spinner: Image_LoadingSpinner

Buttons:
  Cancel Button: Button_Cancel
  Back Button: Button_Back

Player List:
  Player List Container: Content (из ScrollView_Players)
  Player List Item Prefab: PlayerListItem (префаб)

Settings:
  Spinner Rotation Speed: 180
```

### Шаг 5: Сохраните сцену
```
Ctrl+S
```

---

## ЧАСТЬ 4: НАСТРОЙКА GAME СЦЕНЫ (10 минут)

### Шаг 1: Откройте Game сцену
```
Assets/Scenes/Game.unity
```

### Шаг 2: Создайте NetworkManager GameObject
```
Hierarchy → Create Empty
Имя: NetworkManager

Inspector:
├── Add Component → Network Manager
├── Add Component → Unity Transport (если нет)
├── Add Component → Custom Network Manager
└── Add Component → Network Scene Manager
```

### Шаг 3: Настройте NetworkManager компонент
```
Network Manager:
┌─────────────────────────────────────────┐
│ General Settings                        │
├─────────────────────────────────────────┤
│ □ Run In Background: ✅                 │
│                                         │
│ Player Prefab: (оставьте None пока)    │
│                                         │
│ Network Prefabs List:                   │
│   □ Default: (создадим позже)           │
│                                         │
├─────────────────────────────────────────┤
│ Connection Settings                     │
├─────────────────────────────────────────┤
│ ☑ Enable Connection Approval: ✅        │
│ Client Connection Buffer Timeout: 10    │
│                                         │
├─────────────────────────────────────────┤
│ Scene Management                        │
├─────────────────────────────────────────┤
│ ☑ Enable Scene Management: ✅           │
│                                         │
├─────────────────────────────────────────┤
│ Transport                               │
├─────────────────────────────────────────┤
│ Network Transport: Unity Transport      │
└─────────────────────────────────────────┘
```

### Шаг 4: Настройте Unity Transport
```
Unity Transport:
┌─────────────────────────────────────────┐
│ Connection Data                         │
├─────────────────────────────────────────┤
│ Address: 127.0.0.1                      │
│ Port: 7777                              │
│ Server Listen Address: 0.0.0.0          │
│                                         │
│ Max Connect Attempts: 60                │
│ Connect Timeout MS: 1000                │
│ Disconnect Timeout MS: 30000            │
└─────────────────────────────────────────┘
```

### Шаг 5: Настройте Custom Network Manager
```
Custom Network Manager:
┌─────────────────────────────────────────┐
│ Server Settings                         │
├─────────────────────────────────────────┤
│ Max Connections: 10                     │
│                                         │
├─────────────────────────────────────────┤
│ Debug (для тестирования)                │
├─────────────────────────────────────────┤
│ □ Start Server On Awake: ❌             │
│ ☑ Start Host On Awake: ✅ (для теста)  │
│ □ Start Client On Awake: ❌             │
└─────────────────────────────────────────┘
```

### Шаг 6: Сохраните сцену
```
Ctrl+S
```

---

## ЧАСТЬ 5: BUILD SETTINGS (5 минут)

### Настройте Build Settings
```
File → Build Settings

Scenes In Build:
┌─────────────────────────────────────────┐
│ [0] ☑ Scenes/MainMenu                   │
│ [1] ☑ Scenes/Lobby                      │
│ [2] ☑ Scenes/Game                       │
└─────────────────────────────────────────┘

ВАЖНО: MainMenu должен быть первым (index 0)!

Если сцена не в списке:
  → Нажмите "Add Open Scenes"
  
Чтобы изменить порядок:
  → Перетащите сцену вверх/вниз
```

---

## ✅ ФИНАЛЬНАЯ ПРОВЕРКА

### Шаг 1: Проверьте консоль на ошибки
```
Window → General → Console

Не должно быть красных ошибок!

Если есть ошибки компиляции:
  → Проверьте что все using правильные
  → Проверьте что Netcode установлен
  → Перезапустите Unity
```

### Шаг 2: Тест MainMenu
```
1. Откройте MainMenu сцену
2. Нажмите Play ▶
3. Проверьте:
   ✅ Меню отображается
   ✅ Кнопка Play работает
   ✅ Панель выбора режимов появляется
   ✅ Режимы отображаются
   ✅ При выборе режима загружается Lobby
```

### Шаг 3: Тест Lobby
```
1. Нажмите Play в MainMenu → выберите режим
2. Должна загрузиться Lobby сцена
3. Проверьте:
   ✅ Появляется "Searching for players..."
   ✅ Спиннер вращается
   ✅ Таймер работает
   ✅ Находит игроков (симуляция)
   ✅ Показывает список игроков
   ✅ Запускает Game сцену
```

### Шаг 4: Тест Network
```
1. Откройте Game сцену
2. Нажмите Play ▶
3. Проверьте консоль:
   ✅ "[NetworkManager] Starting host..."
   ✅ "[NetworkManager] ✅ Server started successfully!"
   ✅ "[NetworkManager] ✅ Client connected: 0"

Если всё работает - ВЫ МОЛОДЕЦ! ✅
```

---

## 🐛 TROUBLESHOOTING

### Ошибка: "NetworkManager is null"
```
Решение:
1. Убедитесь что NetworkManager GameObject существует в Game сцене
2. Убедитесь что компонент NetworkManager добавлен
3. Проверьте что Start Host On Awake включен для теста
```

### Ошибка: "Scene not found"
```
Решение:
1. File → Build Settings
2. Добавьте все 3 сцены в список
3. Проверьте правильность имён сцен в GameModeData
```

### Ошибка: "Missing TMP_Text"
```
Решение:
1. Window → TextMeshPro → Import TMP Essential Resources
2. Перезапустите Unity
```

### UI не отображается
```
Решение:
1. Проверьте что Canvas Scaler настроен
2. Проверьте что EventSystem существует
3. Проверьте что объекты Active в Inspector
```

### Game Mode Cards не появляются
```
Решение:
1. Проверьте что префаб назначен в GameModeSelector
2. Проверьте что ScriptableObjects созданы
3. Проверьте что они добавлены в список Game Modes
```

---

## 📊 СТАТУС ЗАВЕРШЕНИЯ

После выполнения всех шагов отметьте:

```
✅ ЧАСТЬ 1: ScriptableObjects созданы (5 штук)
✅ ЧАСТЬ 2: MainMenu настроен
✅ ЧАСТЬ 3: Lobby настроен
✅ ЧАСТЬ 4: Game сцена с NetworkManager
✅ ЧАСТЬ 5: Build Settings настроены
✅ ФИНАЛЬНАЯ ПРОВЕРКА: Все тесты пройдены
```

---

## 🎉 ПОЗДРАВЛЯЮ!

Если все тесты пройдены - **ЭТАП 13 ЗАВЕРШЁН!**

### Что получилось:
- ✅ Main Menu с выбором режимов
- ✅ Lobby с поиском игроков
- ✅ Network инфраструктура настроена
- ✅ 5 игровых режимов созданы
- ✅ Навигация между сценами работает

### Следующий этап:
**ЭТАП 14: Player Networking**
- Сделать Player префаб сетевым
- Синхронизировать движение
- Реализовать network combat

---

**Время выполнения:** ~60-90 минут  
**Сложность:** Средняя  
**Готовность к Этапу 14:** ✅

Удачи! 🚀
