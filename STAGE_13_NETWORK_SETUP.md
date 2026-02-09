# 🌐 ЭТАП 13: NETWORK SETUP & LOBBY

**Milestone:** 5 - Multiplayer Foundation  
**Длительность:** 1 неделя (7 дней)  
**Приоритет:** CRITICAL - Фундамент для всей мультиплеер системы  
**Дата начала:** _указать при старте_  

---

## 📋 ОБЗОР ЭТАПА

На этом этапе мы закладываем фундамент мультиплеер системы:
- ✅ Устанавливаем Netcode for GameObjects
- ✅ Создаём Main Menu и систему навигации между сценами
- ✅ Разрабатываем UI для выбора игровых режимов
- ✅ Настраиваем базовую сетевую инфраструктуру
- ✅ Создаём систему поиска игроков (UI часть)

**Результат:** Готовая система главного меню и базовая сетевая настройка.

---

## 🎯 ПОДЭТАП 13.1: УСТАНОВКА NETCODE (1 ЧАС)

### Цель
Установить и настроить Netcode for GameObjects в проекте Unity.

### Шаги установки

#### Шаг 1: Откройте Package Manager
```
1. Unity Editor → Window → Package Manager
2. Дождитесь загрузки списка пакетов
```

#### Шаг 2: Найдите Netcode for GameObjects
```
1. В Package Manager выберите "Unity Registry" (dropdown вверху слева)
2. В поиске введите: "Netcode for GameObjects"
3. Найдите пакет "Netcode for GameObjects"
```

#### Шаг 3: Установите пакет
```
1. Нажмите на пакет "Netcode for GameObjects"
2. Справа нажмите кнопку "Install"
3. Дождитесь окончания установки (1-2 минуты)
4. Проверьте версию: должна быть 2.8.0 или выше
```

#### Шаг 4: Проверка установки
```
✅ В Package Manager появился пакет "Netcode for GameObjects"
✅ В Project → Packages видна папка "Netcode for GameObjects"
✅ В консоли нет ошибок
✅ Unity не требует рестарта (если требует - перезапустите)
```

### Дополнительные пакеты (опционально)
```
Если планируете dedicated server:
1. Unity Transport (обычно устанавливается автоматически)
2. Collections (dependency, автоматически)
```

### Проверка работоспособности
Создайте тестовую сцену для проверки:
```
1. Создайте новую сцену: File → New Scene
2. Создайте пустой GameObject: GameObject → Create Empty
3. Переименуйте в "NetworkManager"
4. Add Component → NetworkManager
5. Если компонент добавился - установка успешна! ✅
```

### ⚠️ Возможные проблемы

**Проблема:** Package Manager не загружается
```
Решение:
1. Edit → Preferences → Network
2. Проверьте "Enable Unity Package Manager cache"
3. Перезапустите Unity
```

**Проблема:** Ошибки компиляции после установки
```
Решение:
1. Закройте Unity
2. Удалите папки: Library/, Temp/
3. Откройте Unity заново
4. Дождитесь полной компиляции
```

**Проблема:** Версия Netcode ниже 2.8.0
```
Решение:
1. В Package Manager выберите пакет
2. Справа нажмите "Update to X.X.X"
3. Или удалите и установите заново
```

### ✅ Чеклист завершения 13.1
- [ ] Netcode for GameObjects установлен (версия 2.8.0+)
- [ ] Unity Transport установлен (dependency)
- [ ] Консоль без ошибок
- [ ] NetworkManager компонент доступен
- [ ] Проект компилируется успешно

---

## 🎨 ПОДЭТАП 13.2: MAIN MENU & LOBBY UI (2-3 ДНЯ)

### Цель
Создать систему главного меню с навигацией и UI для выбора игровых режимов.

---

### 📁 ДЕНЬ 1: СОЗДАНИЕ СЦЕН И БАЗОВОЙ СТРУКТУРЫ

#### Шаг 1: Создание новых сцен

**1.1 Создайте сцену MainMenu**
```
1. File → New Scene → Empty (URP)
2. Сохраните: File → Save As → Assets/_Project/Scenes/MainMenu.unity
3. В Build Settings добавьте сцену (index 0)
```

**1.2 Настройка MainMenu сцены**
```
Создайте базовую структуру:

[MainMenu Scene]
├── Main Camera
│   └── Position: (0, 1, -10)
├── Directional Light
│   └── Rotation: (50, -30, 0)
├── Canvas (UI)
│   ├── Canvas Scaler
│   │   ├── UI Scale Mode: Scale With Screen Size
│   │   └── Reference Resolution: 1920x1080
│   └── RenderMode: Screen Space - Overlay
└── EventSystem
```

**1.3 Создайте сцену Lobby**
```
1. File → New Scene → Empty (URP)
2. Сохраните: Assets/_Project/Scenes/Lobby.unity
3. В Build Settings добавьте сцену (index 1)
4. Скопируйте Canvas и EventSystem из MainMenu
```

**1.4 Обновите Build Settings**
```
File → Build Settings:
[0] MainMenu (стартовая)
[1] Lobby
[2] MainScene (ваша игровая сцена)

✅ MainMenu должна быть index 0!
```

#### Шаг 2: Создание UI Manager систем

**2.1 Создайте папку для UI скриптов**
```
Assets/_Project/Scripts/UI/
├── MainMenu/
├── Lobby/
└── Shared/
```

**2.2 Создайте SceneLoader.cs**
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlumpGame.UI
{
    /// <summary>
    /// Управляет загрузкой сцен в игре.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;
        
        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SceneLoader");
                    _instance = go.AddComponent<SceneLoader>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Загружает сцену по имени.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// Загружает сцену по индексу.
        /// </summary>
        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        
        /// <summary>
        /// Загружает сцену асинхронно с прогрессом.
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
        
        private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                float progress = asyncLoad.progress;
                // TODO: Обновить UI loading bar
                yield return null;
            }
        }
        
        /// <summary>
        /// Перезагружает текущую сцену.
        /// </summary>
        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
        
        /// <summary>
        /// Выход из игры.
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/UI/Shared/SceneLoader.cs`

#### Шаг 3: Базовая структура Main Menu

**3.1 Создайте UI элементы в MainMenu сцене**
```
Canvas
├── Panel_Background (Image - темный фон)
│   ├── Anchor: Stretch/Stretch
│   └── Color: (0, 0, 0, 200)
│
├── Panel_MainMenu (VerticalLayoutGroup)
│   ├── Anchor: Center
│   ├── Size: 400x600
│   │
│   ├── Text_Title
│   │   ├── Text: "FLUMP GAME"
│   │   ├── Font Size: 72
│   │   ├── Alignment: Center
│   │   └── Style: Bold
│   │
│   ├── Spacer (LayoutElement - 50px)
│   │
│   ├── Button_Play
│   │   ├── Size: 350x80
│   │   ├── Text: "PLAY"
│   │   └── Font Size: 36
│   │
│   ├── Button_Settings
│   │   ├── Size: 350x60
│   │   └── Text: "Settings"
│   │
│   ├── Button_Quit
│   │   ├── Size: 350x60
│   │   └── Text: "Quit"
│   │
│   └── Text_Version (Bottom Right)
│       ├── Text: "v0.5.0 Alpha"
│       └── Font Size: 18
```

**3.2 Настройка VerticalLayoutGroup**
```
Panel_MainMenu:
├── VerticalLayoutGroup:
│   ├── Padding: 20 (all sides)
│   ├── Spacing: 20
│   ├── Child Alignment: Upper Center
│   ├── Child Force Expand: Width = On, Height = Off
│   └── Child Controls Size: Width = On, Height = On
```

#### Шаг 4: Создание MainMenuUI.cs

**4.1 Создайте скрипт MainMenuUI.cs**
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Если используете TextMeshPro

namespace FlumpGame.UI.MainMenu
{
    /// <summary>
    /// Управляет UI главного меню.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Menu Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _gameModeSelectionPanel;
        
        [Header("Main Menu Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;
        
        [Header("Version Display")]
        [SerializeField] private TextMeshProUGUI _versionText;
        
        private void Awake()
        {
            SetupButtons();
            UpdateVersionText();
        }
        
        private void OnEnable()
        {
            ShowMainMenu();
        }
        
        private void SetupButtons()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlayButtonClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        
        private void OnDisable()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
        
        private void UpdateVersionText()
        {
            if (_versionText != null)
            {
                _versionText.text = $"v{Application.version} Alpha";
            }
        }
        
        private void ShowMainMenu()
        {
            if (_mainMenuPanel != null)
                _mainMenuPanel.SetActive(true);
            
            if (_gameModeSelectionPanel != null)
                _gameModeSelectionPanel.SetActive(false);
        }
        
        private void OnPlayButtonClicked()
        {
            Debug.Log("Play button clicked");
            
            // Показываем меню выбора режима игры
            if (_mainMenuPanel != null)
                _mainMenuPanel.SetActive(false);
            
            if (_gameModeSelectionPanel != null)
                _gameModeSelectionPanel.SetActive(true);
        }
        
        private void OnSettingsButtonClicked()
        {
            Debug.Log("Settings button clicked");
            // TODO: Открыть меню настроек
        }
        
        private void OnQuitButtonClicked()
        {
            Debug.Log("Quit button clicked");
            SceneLoader.Instance.QuitGame();
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/UI/MainMenu/MainMenuUI.cs`

**4.2 Прикрепите скрипт к сцене**
```
1. В сцене MainMenu создайте пустой GameObject: "MainMenuManager"
2. Add Component → MainMenuUI
3. Перетащите UI элементы в соответствующие поля:
   - Main Menu Panel → Panel_MainMenu
   - Play Button → Button_Play
   - Settings Button → Button_Settings
   - Quit Button → Button_Quit
   - Version Text → Text_Version
```

#### ✅ Чеклист завершения Дня 1
- [ ] Созданы сцены MainMenu и Lobby
- [ ] Build Settings настроен правильно
- [ ] SceneLoader.cs создан и работает
- [ ] Базовый UI главного меню создан
- [ ] MainMenuUI.cs создан и подключен
- [ ] Кнопки работают (проверьте в Play Mode)
- [ ] Quit работает корректно

---

### 📁 ДЕНЬ 2: МЕНЮ ВЫБОРА ИГРОВЫХ РЕЖИМОВ

#### Шаг 1: Создание Game Mode Selection UI

**1.1 Добавьте панель выбора режимов**
```
Canvas
└── Panel_GameModeSelection (скрыт по умолчанию)
    ├── Anchor: Stretch/Stretch
    │
    ├── Panel_Header
    │   ├── Text_Title: "SELECT GAME MODE"
    │   └── Button_Back (← назад)
    │
    ├── ScrollView_GameModes
    │   ├── Viewport
    │   │   └── Content (VerticalLayoutGroup)
    │   │       ├── GameModeCard_Duel1v1
    │   │       ├── GameModeCard_Team3v3
    │   │       ├── GameModeCard_Team5v5_TDM
    │   │       ├── GameModeCard_Hardpoint5v5
    │   │       └── GameModeCard_Practice
```

**1.2 Создайте префаб GameModeCard**
```
GameModeCard (Prefab):
├── Button (300x150)
│   ├── Image_Background
│   │   └── Color: (30, 30, 30, 255)
│   │
│   ├── Image_Icon (50x50, left)
│   │   └── Sprite: иконка режима
│   │
│   ├── VerticalLayoutGroup (right side)
│   │   ├── Text_ModeName
│   │   │   ├── Font Size: 28
│   │   │   └── Style: Bold
│   │   │
│   │   ├── Text_Description
│   │   │   ├── Font Size: 16
│   │   │   └── Color: Gray
│   │   │
│   │   └── Text_PlayersOnline
│   │       ├── Text: "🟢 24 players online"
│   │       └── Font Size: 14
```

**Сохраните префаб:** `Assets/_Project/Prefabs/UI/GameModeCard.prefab`

#### Шаг 2: Создание GameModeData ScriptableObject

**2.1 Создайте GameModeData.cs**
```csharp
using UnityEngine;

namespace FlumpGame.Data
{
    /// <summary>
    /// Типы игровых режимов.
    /// </summary>
    public enum GameModeType
    {
        Duel1v1,
        Team3v3TDM,
        Team5v5TDM,
        Hardpoint5v5,
        Practice
    }
    
    /// <summary>
    /// Данные игрового режима.
    /// </summary>
    [CreateAssetMenu(fileName = "GameMode_", menuName = "Flump/Game Mode Data")]
    public class GameModeData : ScriptableObject
    {
        [Header("Mode Info")]
        public string modeName = "Team Deathmatch";
        public string description = "Eliminate enemies to reach the score limit.";
        public GameModeType modeType = GameModeType.Team5v5TDM;
        public Sprite icon;
        
        [Header("Match Settings")]
        public int playersPerTeam = 5;
        public int scoreLimit = 50;
        public float matchDurationSeconds = 600f; // 10 minutes
        public bool enableOvertime = true;
        
        [Header("Scene")]
        public string gameSceneName = "MainScene";
        
        [Header("Display")]
        public bool showInMenu = true;
        public int sortOrder = 0;
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/Data/GameModeData.cs`

**2.2 Создайте ScriptableObject ассеты**
```
Assets/_Project/ScriptableObjects/GameModes/

Создайте 5 файлов:

1. GameMode_Duel1v1.asset
   - Mode Name: "Duel 1v1"
   - Description: "1v1 battle. First to 5 kills wins!"
   - Players Per Team: 1
   - Score Limit: 5
   - Duration: 180s (3 min)
   - Sort Order: 0

2. GameMode_Team3v3TDM.asset
   - Mode Name: "Team 3v3"
   - Description: "3v3 team battle. Reach 40 kills!"
   - Players Per Team: 3
   - Score Limit: 40
   - Duration: 420s (7 min)
   - Sort Order: 1

3. GameMode_Team5v5TDM.asset
   - Mode Name: "Team Deathmatch 5v5"
   - Description: "Classic 5v5. First team to 50 kills!"
   - Players Per Team: 5
   - Score Limit: 50
   - Duration: 600s (10 min)
   - Sort Order: 2

4. GameMode_Hardpoint5v5.asset
   - Mode Name: "Hardpoint 5v5"
   - Description: "Capture and hold the zone!"
   - Players Per Team: 5
   - Score Limit: 150
   - Duration: 600s (10 min)
   - Sort Order: 3

5. GameMode_Practice.asset
   - Mode Name: "Practice vs Bots"
   - Description: "Train against AI bots"
   - Players Per Team: 1
   - Score Limit: 999
   - Duration: 999s
   - Sort Order: 4
```

#### Шаг 3: Создание GameModeSelector.cs

**3.1 Создайте скрипт GameModeSelector.cs**
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace FlumpGame.UI.MainMenu
{
    /// <summary>
    /// Управляет выбором игрового режима.
    /// </summary>
    public class GameModeSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _gameModeContainer;
        [SerializeField] private GameObject _gameModeCardPrefab;
        [SerializeField] private Button _backButton;
        
        [Header("Game Modes")]
        [SerializeField] private List<Data.GameModeData> _gameModes;
        
        private List<GameObject> _spawnedCards = new List<GameObject>();
        private Data.GameModeData _selectedGameMode;
        
        private void Awake()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void OnEnable()
        {
            PopulateGameModes();
        }
        
        private void OnDisable()
        {
            ClearGameModes();
            
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
        
        private void PopulateGameModes()
        {
            ClearGameModes();
            
            if (_gameModes == null || _gameModes.Count == 0)
            {
                Debug.LogWarning("No game modes assigned!");
                return;
            }
            
            // Сортируем режимы по sortOrder
            var sortedModes = _gameModes
                .Where(mode => mode != null && mode.showInMenu)
                .OrderBy(mode => mode.sortOrder)
                .ToList();
            
            foreach (var gameMode in sortedModes)
            {
                CreateGameModeCard(gameMode);
            }
        }
        
        private void CreateGameModeCard(Data.GameModeData gameMode)
        {
            if (_gameModeCardPrefab == null || _gameModeContainer == null)
                return;
            
            GameObject card = Instantiate(_gameModeCardPrefab, _gameModeContainer);
            _spawnedCards.Add(card);
            
            // Настраиваем карточку
            SetupCard(card, gameMode);
        }
        
        private void SetupCard(GameObject card, Data.GameModeData gameMode)
        {
            // Находим компоненты
            TextMeshProUGUI modeNameText = card.transform.Find("Text_ModeName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = card.transform.Find("Text_Description")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playersOnlineText = card.transform.Find("Text_PlayersOnline")?.GetComponent<TextMeshProUGUI>();
            Image iconImage = card.transform.Find("Image_Icon")?.GetComponent<Image>();
            Button button = card.GetComponent<Button>();
            
            // Заполняем данными
            if (modeNameText != null)
                modeNameText.text = gameMode.modeName;
            
            if (descriptionText != null)
                descriptionText.text = gameMode.description;
            
            if (playersOnlineText != null)
            {
                // TODO: Получить реальное количество игроков онлайн
                int playersOnline = Random.Range(10, 50);
                playersOnlineText.text = $"🟢 {playersOnline} players online";
            }
            
            if (iconImage != null && gameMode.icon != null)
                iconImage.sprite = gameMode.icon;
            
            // Подписываемся на клик
            if (button != null)
            {
                button.onClick.AddListener(() => OnGameModeSelected(gameMode));
            }
        }
        
        private void ClearGameModes()
        {
            foreach (var card in _spawnedCards)
            {
                if (card != null)
                    Destroy(card);
            }
            
            _spawnedCards.Clear();
        }
        
        private void OnGameModeSelected(Data.GameModeData gameMode)
        {
            Debug.Log($"Selected game mode: {gameMode.modeName}");
            _selectedGameMode = gameMode;
            
            // Переходим в Lobby
            SceneLoader.Instance.LoadScene("Lobby");
            
            // Сохраняем выбранный режим (для передачи в Lobby)
            GameModeManager.Instance.SetSelectedGameMode(gameMode);
        }
        
        private void OnBackButtonClicked()
        {
            // Возвращаемся в главное меню
            gameObject.SetActive(false);
            
            // Показываем главное меню
            MainMenuUI mainMenu = FindObjectOfType<MainMenuUI>();
            if (mainMenu != null)
            {
                // mainMenu.ShowMainMenu(); // Вызовется автоматически через OnEnable
            }
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/UI/MainMenu/GameModeSelector.cs`

#### Шаг 4: Создание GameModeManager Singleton

**4.1 Создайте GameModeManager.cs**
```csharp
using UnityEngine;

namespace FlumpGame
{
    /// <summary>
    /// Singleton для управления текущим выбранным игровым режимом.
    /// Сохраняется между сценами.
    /// </summary>
    public class GameModeManager : MonoBehaviour
    {
        private static GameModeManager _instance;
        
        public static GameModeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameModeManager");
                    _instance = go.AddComponent<GameModeManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private Data.GameModeData _selectedGameMode;
        
        public Data.GameModeData SelectedGameMode => _selectedGameMode;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Устанавливает выбранный игровой режим.
        /// </summary>
        public void SetSelectedGameMode(Data.GameModeData gameMode)
        {
            _selectedGameMode = gameMode;
            Debug.Log($"Game mode set to: {gameMode.modeName}");
        }
        
        /// <summary>
        /// Очищает выбранный режим.
        /// </summary>
        public void ClearSelectedGameMode()
        {
            _selectedGameMode = null;
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/Managers/GameModeManager.cs`

#### Шаг 5: Подключение всех компонентов

**5.1 Настройка в MainMenu сцене**
```
1. Создайте Panel_GameModeSelection (если ещё не создан)
2. Установите Active = false
3. Добавьте GameModeSelector component на Panel_GameModeSelection
4. Заполните поля:
   - Game Mode Container → Content (внутри ScrollView)
   - Game Mode Card Prefab → ваш префаб
   - Game Modes → добавьте все 5 ScriptableObject
   - Back Button → кнопка возврата
```

**5.2 Обновите MainMenuUI.cs**
```csharp
// В методе OnPlayButtonClicked добавьте:
private void OnPlayButtonClicked()
{
    _mainMenuPanel.SetActive(false);
    _gameModeSelectionPanel.SetActive(true);
    
    // Активация панели автоматически вызовет PopulateGameModes()
}
```

#### ✅ Чеклист завершения Дня 2
- [ ] Панель выбора режимов создана
- [ ] Префаб GameModeCard создан и настроен
- [ ] GameModeData ScriptableObject создан
- [ ] Все 5 игровых режимов созданы как ассеты
- [ ] GameModeSelector.cs работает
- [ ] GameModeManager singleton создан
- [ ] Навигация Main Menu → Game Mode Selection работает
- [ ] Кнопка Back возвращает в главное меню
- [ ] При выборе режима загружается Lobby

---

### 📁 ДЕНЬ 3: ПОЛИРОВКА И ВИЗУАЛЬНОЕ УЛУЧШЕНИЕ

#### Задачи на День 3

**1. Добавление анимаций**
```
- Button hover effects (Scale: 1.0 → 1.05)
- Panel fade in/out transitions
- Card появление с задержкой (stagger)
```

**2. Улучшение UI дизайна**
```
- Красивый фон (градиент или изображение)
- Иконки для каждого режима
- Цветовая схема (primary, secondary, accent colors)
```

**3. Sound Effects**
```
- Button click sound
- Menu transition sound
- Hover sound (опционально)
```

**4. Адаптация под разные разрешения**
```
- Тестирование на 1920x1080
- Тестирование на 1280x720
- Тестирование на мобильных (16:9, 18:9)
```

#### ✅ Чеклист завершения Дня 3
- [ ] UI выглядит привлекательно
- [ ] Анимации работают плавно
- [ ] Звуки добавлены (если есть)
- [ ] UI адаптируется под разные разрешения
- [ ] Протестировано на нескольких разрешениях

---

## 🔍 ПОДЭТАП 13.3: MATCHMAKING UI (2 ДНЯ)

### Цель
Создать UI для экрана поиска игроков (Lobby/Matchmaking).

---

### 📁 ДЕНЬ 1: СОЗДАНИЕ LOBBY UI

#### Шаг 1: Настройка Lobby сцены

**1.1 Структура Lobby UI**
```
Canvas
├── Panel_Lobby
│   │
│   ├── Panel_Header
│   │   ├── Text_SelectedMode
│   │   │   └── Text: "TEAM DEATHMATCH 5v5"
│   │   └── Button_Back
│   │       └── Text: "← Back"
│   │
│   ├── Panel_SearchStatus (Center)
│   │   ├── Image_LoadingSpinner
│   │   │   └── Rotation animation
│   │   │
│   │   ├── Text_Status
│   │   │   └── Text: "Searching for players..."
│   │   │
│   │   ├── Text_PlayersFound
│   │   │   └── Text: "Players found: 4/10"
│   │   │
│   │   ├── Text_Timer
│   │   │   └── Text: "00:23"
│   │   │
│   │   └── Button_Cancel
│   │       ├── Size: 200x60
│   │       └── Text: "Cancel"
│   │
│   └── Panel_PlayerList (Bottom, hidden initially)
│       ├── Text_Title: "LOBBY"
│       │
│       ├── ScrollView_Players
│       │   └── Content (VerticalLayoutGroup)
│       │       ├── PlayerListItem (Team 1)
│       │       ├── PlayerListItem (Team 1)
│       │       ├── ...
│       │       ├── Divider
│       │       ├── PlayerListItem (Team 2)
│       │       └── ...
```

#### Шаг 2: Создание PlayerListItem префаба

**2.1 Создайте префаб PlayerListItem**
```
PlayerListItem (Prefab 400x60):
├── Image_Background
│   └── Color: (20, 20, 20, 255)
│
├── HorizontalLayoutGroup
│   ├── Image_Avatar (50x50)
│   │   └── Circle sprite
│   │
│   ├── VerticalLayoutGroup (name & stats)
│   │   ├── Text_PlayerName
│   │   │   ├── Font Size: 20
│   │   │   └── Text: "PlayerName"
│   │   │
│   │   └── HorizontalLayoutGroup (stats)
│   │       ├── Text_Level
│   │       │   └── Text: "Level 15"
│   │       │
│   │       └── Text_Ping
│   │           └── Text: "25ms"
│   │
│   └── Image_TeamColor (20x60, right)
│       └── Color: Red or Blue
```

**Сохраните:** `Assets/_Project/Prefabs/UI/PlayerListItem.prefab`

#### Шаг 3: Создание MatchmakingUI.cs

**3.1 Создайте скрипт MatchmakingUI.cs**
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FlumpGame.UI.Lobby
{
    /// <summary>
    /// Управляет UI matchmaking (поиск игроков).
    /// </summary>
    public class MatchmakingUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _searchPanel;
        [SerializeField] private GameObject _playerListPanel;
        
        [Header("Search Status")]
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _playersFoundText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private RectTransform _loadingSpinner;
        
        [Header("Buttons")]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _backButton;
        
        [Header("Player List")]
        [SerializeField] private Transform _playerListContainer;
        [SerializeField] private GameObject _playerListItemPrefab;
        
        [Header("Settings")]
        [SerializeField] private float _spinnerRotationSpeed = 180f; // degrees per second
        
        private bool _isSearching = false;
        private float _searchTimer = 0f;
        private int _playersFound = 0;
        private int _totalPlayersNeeded = 10;
        
        private void Awake()
        {
            SetupButtons();
        }
        
        private void OnEnable()
        {
            ShowSearchPanel();
            StartSearch();
        }
        
        private void OnDisable()
        {
            StopSearch();
        }
        
        private void Update()
        {
            if (_isSearching)
            {
                UpdateSearchTimer();
                RotateLoadingSpinner();
            }
        }
        
        private void SetupButtons()
        {
            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(OnCancelButtonClicked);
            
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void ShowSearchPanel()
        {
            if (_searchPanel != null)
                _searchPanel.SetActive(true);
            
            if (_playerListPanel != null)
                _playerListPanel.SetActive(false);
        }
        
        private void ShowPlayerListPanel()
        {
            if (_searchPanel != null)
                _searchPanel.SetActive(false);
            
            if (_playerListPanel != null)
                _playerListPanel.SetActive(true);
        }
        
        private void StartSearch()
        {
            _isSearching = true;
            _searchTimer = 0f;
            _playersFound = 0;
            
            // Определяем количество игроков
            var selectedMode = GameModeManager.Instance.SelectedGameMode;
            if (selectedMode != null)
            {
                _totalPlayersNeeded = selectedMode.playersPerTeam * 2;
            }
            
            UpdateStatusText("Searching for players...");
            UpdatePlayersFoundText();
            
            // TODO: Начать реальный поиск игроков
            // Пока симулируем
            StartCoroutine(SimulateMatchmaking());
        }
        
        private void StopSearch()
        {
            _isSearching = false;
            StopAllCoroutines();
        }
        
        private void UpdateSearchTimer()
        {
            _searchTimer += Time.deltaTime;
            
            if (_timerText != null)
            {
                int minutes = Mathf.FloorToInt(_searchTimer / 60f);
                int seconds = Mathf.FloorToInt(_searchTimer % 60f);
                _timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }
        
        private void RotateLoadingSpinner()
        {
            if (_loadingSpinner != null)
            {
                float rotation = _loadingSpinner.localEulerAngles.z;
                rotation -= _spinnerRotationSpeed * Time.deltaTime;
                _loadingSpinner.localEulerAngles = new Vector3(0, 0, rotation);
            }
        }
        
        private void UpdateStatusText(string status)
        {
            if (_statusText != null)
                _statusText.text = status;
        }
        
        private void UpdatePlayersFoundText()
        {
            if (_playersFoundText != null)
            {
                _playersFoundText.text = $"Players found: {_playersFound}/{_totalPlayersNeeded}";
            }
        }
        
        private void OnCancelButtonClicked()
        {
            Debug.Log("Cancel matchmaking");
            StopSearch();
            
            // Возвращаемся в главное меню
            UI.SceneLoader.Instance.LoadScene("MainMenu");
        }
        
        private void OnBackButtonClicked()
        {
            OnCancelButtonClicked();
        }
        
        // ============================================
        // ВРЕМЕННАЯ СИМУЛЯЦИЯ MATCHMAKING (для теста)
        // TODO: Заменить на реальный matchmaking в Этапе 16
        // ============================================
        
        private IEnumerator SimulateMatchmaking()
        {
            yield return new WaitForSeconds(1f);
            
            // Симулируем поиск игроков
            for (int i = 1; i <= _totalPlayersNeeded; i++)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));
                
                _playersFound = i;
                UpdatePlayersFoundText();
                
                if (_playersFound >= _totalPlayersNeeded)
                {
                    UpdateStatusText("Match found!");
                    yield return new WaitForSeconds(1f);
                    
                    // Показываем список игроков
                    ShowPlayerList();
                    yield break;
                }
            }
        }
        
        private void ShowPlayerList()
        {
            ShowPlayerListPanel();
            PopulatePlayerList();
            
            // Через 5 секунд запускаем матч
            StartCoroutine(StartMatchCountdown());
        }
        
        private void PopulatePlayerList()
        {
            ClearPlayerList();
            
            var selectedMode = GameModeManager.Instance.SelectedGameMode;
            if (selectedMode == null) return;
            
            int playersPerTeam = selectedMode.playersPerTeam;
            
            // Team 1
            for (int i = 0; i < playersPerTeam; i++)
            {
                CreatePlayerListItem($"Player{i + 1}", Random.Range(1, 50), Random.Range(10, 80), 1);
            }
            
            // Team 2
            for (int i = 0; i < playersPerTeam; i++)
            {
                CreatePlayerListItem($"Enemy{i + 1}", Random.Range(1, 50), Random.Range(10, 80), 2);
            }
        }
        
        private void CreatePlayerListItem(string playerName, int level, int ping, int team)
        {
            if (_playerListItemPrefab == null || _playerListContainer == null)
                return;
            
            GameObject item = Instantiate(_playerListItemPrefab, _playerListContainer);
            
            // TODO: Настроить item с данными игрока
            // Пока просто создаём
        }
        
        private void ClearPlayerList()
        {
            if (_playerListContainer == null) return;
            
            foreach (Transform child in _playerListContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        private IEnumerator StartMatchCountdown()
        {
            for (int i = 5; i > 0; i--)
            {
                UpdateStatusText($"Starting in {i}...");
                yield return new WaitForSeconds(1f);
            }
            
            // Загружаем игровую сцену
            var selectedMode = GameModeManager.Instance.SelectedGameMode;
            if (selectedMode != null)
            {
                UI.SceneLoader.Instance.LoadScene(selectedMode.gameSceneName);
            }
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/UI/Lobby/MatchmakingUI.cs`

#### Шаг 4: Подключение компонентов

**4.1 Настройка в Lobby сцене**
```
1. Создайте GameObject "LobbyManager"
2. Add Component → MatchmakingUI
3. Заполните все поля в Inspector:
   - Search Panel → Panel_SearchStatus
   - Player List Panel → Panel_PlayerList
   - Status Text → Text_Status
   - Players Found Text → Text_PlayersFound
   - Timer Text → Text_Timer
   - Loading Spinner → Image_LoadingSpinner
   - Cancel Button → Button_Cancel
   - Back Button → Button_Back
   - Player List Container → Content (ScrollView)
   - Player List Item Prefab → PlayerListItem prefab
```

#### ✅ Чеклист завершения Дня 1 (13.3)
- [ ] Lobby UI создан
- [ ] Loading spinner вращается
- [ ] Таймер работает
- [ ] Симуляция поиска работает
- [ ] Кнопка Cancel возвращает в меню
- [ ] PlayerListItem префаб создан

---

### 📁 ДЕНЬ 2: УЛУЧШЕНИЕ LOBBY UI

#### Задачи на День 2

**1. Создание PlayerListItem.cs**
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlumpGame.UI.Lobby
{
    /// <summary>
    /// Представляет одного игрока в списке лобби.
    /// </summary>
    public class PlayerListItem : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _pingText;
        [SerializeField] private Image _teamColorImage;
        
        public void Setup(string playerName, int level, int ping, int team)
        {
            if (_playerNameText != null)
                _playerNameText.text = playerName;
            
            if (_levelText != null)
                _levelText.text = $"Lvl {level}";
            
            if (_pingText != null)
            {
                _pingText.text = $"{ping}ms";
                
                // Цвет пинга (зелёный = хороший, жёлтый = средний, красный = плохой)
                if (ping < 50)
                    _pingText.color = Color.green;
                else if (ping < 100)
                    _pingText.color = Color.yellow;
                else
                    _pingText.color = Color.red;
            }
            
            if (_teamColorImage != null)
            {
                _teamColorImage.color = team == 1 ? Color.blue : Color.red;
            }
            
            // TODO: Загрузить аватар игрока
        }
    }
}
```

**2. Добавление звуков**
```
- Player joined sound
- Match found sound
- Countdown tick sound
```

**3. Анимации**
```
- Player list items появляются с fade in
- Countdown с scale animation
```

**4. Тестирование всех сценариев**
```
- Поиск → находит игроков → запускает матч
- Поиск → отмена → возврат в меню
- Разные игровые режимы (1v1, 3v3, 5v5)
```

#### ✅ Чеклист завершения Дня 2 (13.3)
- [ ] PlayerListItem.cs создан и работает
- [ ] Игроки отображаются корректно
- [ ] Цвета команд показываются правильно
- [ ] Пинг окрашивается по качеству
- [ ] Звуки добавлены
- [ ] Анимации работают
- [ ] Протестированы все режимы

---

## 🌐 ПОДЭТАП 13.4: NETWORK MANAGER SETUP (2 ДНЯ)

### Цель
Настроить базовую сетевую инфраструктуру с Netcode for GameObjects.

---

### 📁 ДЕНЬ 1: БАЗОВАЯ НАСТРОЙКА NETWORK MANAGER

#### Шаг 1: Создание NetworkManager GameObject

**1.1 Создайте NetworkManager в игровой сцене**
```
1. Откройте сцену MainScene (ваша игровая сцена)
2. GameObject → Create Empty
3. Переименуйте в "NetworkManager"
4. Add Component → NetworkManager
```

**1.2 Настройка NetworkManager компонента**
```
NetworkManager:
├── Transport: Unity Transport
│   ├── Protocol Type: UnityTransport
│   ├── Connection Data:
│   │   ├── Address: 127.0.0.1 (localhost для тестов)
│   │   ├── Port: 7777
│   │   └── Server Listen Address: 0.0.0.0
│   │
│   └── Timeout: 10 seconds
│
├── Connection Approval: ✅ Enable
├── Scene Management: ✅ Enable
│
└── Network Prefabs: (пока пусто, добавим позже)
```

#### Шаг 2: Создание CustomNetworkManager.cs

**2.1 Создайте скрипт CustomNetworkManager.cs**
```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network
{
    /// <summary>
    /// Кастомный Network Manager для игры.
    /// Управляет подключением, авторизацией и сессиями.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class CustomNetworkManager : MonoBehaviour
    {
        private NetworkManager _networkManager;
        
        [Header("Server Settings")]
        [SerializeField] private int _maxConnections = 10;
        
        [Header("Debug")]
        [SerializeField] private bool _startServerOnAwake = false;
        [SerializeField] private bool _startHostOnAwake = false;
        [SerializeField] private bool _startClientOnAwake = false;
        
        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
            
            // Подписываемся на события
            _networkManager.OnServerStarted += OnServerStarted;
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
            
            // Connection Approval
            _networkManager.ConnectionApprovalCallback = ApprovalCheck;
        }
        
        private void Start()
        {
            // Автозапуск для тестирования
            if (_startServerOnAwake)
                StartServer();
            else if (_startHostOnAwake)
                StartHost();
            else if (_startClientOnAwake)
                StartClient();
        }
        
        private void OnDestroy()
        {
            if (_networkManager != null)
            {
                _networkManager.OnServerStarted -= OnServerStarted;
                _networkManager.OnClientConnectedCallback -= OnClientConnected;
                _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
        
        // ============================================
        // PUBLIC API
        // ============================================
        
        /// <summary>
        /// Запускает сервер.
        /// </summary>
        public bool StartServer()
        {
            Debug.Log("Starting server...");
            return _networkManager.StartServer();
        }
        
        /// <summary>
        /// Запускает хост (сервер + клиент).
        /// </summary>
        public bool StartHost()
        {
            Debug.Log("Starting host...");
            return _networkManager.StartHost();
        }
        
        /// <summary>
        /// Запускает клиент и подключается к серверу.
        /// </summary>
        public bool StartClient()
        {
            Debug.Log("Starting client...");
            return _networkManager.StartClient();
        }
        
        /// <summary>
        /// Отключается от сети.
        /// </summary>
        public void Shutdown()
        {
            Debug.Log("Shutting down network...");
            _networkManager.Shutdown();
        }
        
        // ============================================
        // CONNECTION APPROVAL
        // ============================================
        
        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // Проверяем количество подключений
            if (_networkManager.ConnectedClientsIds.Count >= _maxConnections)
            {
                response.Approved = false;
                response.Reason = "Server is full";
                Debug.LogWarning($"Connection rejected: Server is full ({_maxConnections}/{_maxConnections})");
                return;
            }
            
            // TODO: Дополнительные проверки:
            // - Версия клиента
            // - Бан лист
            // - Пароль комнаты
            
            // Принимаем подключение
            response.Approved = true;
            response.CreatePlayerObject = true; // Создаём player object автоматически
            
            Debug.Log($"Connection approved for client {request.ClientNetworkId}");
        }
        
        // ============================================
        // NETWORK CALLBACKS
        // ============================================
        
        private void OnServerStarted()
        {
            Debug.Log("✅ Server started successfully!");
        }
        
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"✅ Client connected: {clientId}");
            
            if (_networkManager.IsServer)
            {
                Debug.Log($"Total clients connected: {_networkManager.ConnectedClientsIds.Count}");
            }
        }
        
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"❌ Client disconnected: {clientId}");
            
            if (_networkManager.IsServer)
            {
                Debug.Log($"Remaining clients: {_networkManager.ConnectedClientsIds.Count}");
                
                // TODO: Handle player leaving
                // - Remove player from match
                // - Notify other players
                // - Spawn bot replacement (Этап 16)
            }
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/Network/CustomNetworkManager.cs`

**2.2 Прикрепите скрипт к NetworkManager**
```
1. Выберите NetworkManager GameObject
2. Add Component → CustomNetworkManager
3. Настройте:
   - Max Connections: 10
   - Start Host On Awake: ✅ (для тестов)
```

#### Шаг 3: Создание ServerConnection.cs

**3.1 Создайте ServerConnection.cs**
```csharp
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace FlumpGame.Network
{
    /// <summary>
    /// Управляет подключением к серверу.
    /// </summary>
    public class ServerConnection : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField] private string _serverAddress = "127.0.0.1";
        [SerializeField] private ushort _serverPort = 7777;
        
        private UnityTransport _transport;
        private NetworkManager _networkManager;
        
        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            if (_networkManager != null)
            {
                _transport = _networkManager.GetComponent<UnityTransport>();
            }
        }
        
        /// <summary>
        /// Подключается к серверу по указанному адресу и порту.
        /// </summary>
        public bool ConnectToServer(string address, ushort port)
        {
            if (_transport == null)
            {
                Debug.LogError("UnityTransport not found!");
                return false;
            }
            
            Debug.Log($"Connecting to server: {address}:{port}");
            
            _transport.ConnectionData.Address = address;
            _transport.ConnectionData.Port = port;
            
            return NetworkManager.Singleton.StartClient();
        }
        
        /// <summary>
        /// Подключается к серверу с настройками по умолчанию.
        /// </summary>
        public bool ConnectToServer()
        {
            return ConnectToServer(_serverAddress, _serverPort);
        }
        
        /// <summary>
        /// Отключается от сервера.
        /// </summary>
        public void Disconnect()
        {
            if (_networkManager != null && _networkManager.IsClient)
            {
                Debug.Log("Disconnecting from server...");
                _networkManager.Shutdown();
            }
        }
        
        /// <summary>
        /// Проверяет подключение к серверу.
        /// </summary>
        public bool IsConnected()
        {
            return _networkManager != null && _networkManager.IsConnectedClient;
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/Network/ServerConnection.cs`

#### Шаг 4: Создание ClientConnection.cs

**4.1 Создайте ClientConnection.cs**
```csharp
using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network
{
    /// <summary>
    /// Управляет состоянием клиента и его данными.
    /// </summary>
    public class ClientConnection : MonoBehaviour
    {
        private NetworkManager _networkManager;
        
        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
        }
        
        /// <summary>
        /// Возвращает ID текущего клиента.
        /// </summary>
        public ulong GetClientId()
        {
            if (_networkManager != null && _networkManager.IsClient)
            {
                return _networkManager.LocalClientId;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Проверяет, является ли текущий клиент хостом.
        /// </summary>
        public bool IsHost()
        {
            return _networkManager != null && _networkManager.IsHost;
        }
        
        /// <summary>
        /// Проверяет, является ли текущий клиент сервером.
        /// </summary>
        public bool IsServer()
        {
            return _networkManager != null && _networkManager.IsServer;
        }
        
        /// <summary>
        /// Проверяет, подключен ли клиент.
        /// </summary>
        public bool IsConnected()
        {
            return _networkManager != null && _networkManager.IsConnectedClient;
        }
        
        /// <summary>
        /// Возвращает количество подключенных клиентов.
        /// </summary>
        public int GetConnectedClientsCount()
        {
            if (_networkManager != null && _networkManager.IsServer)
            {
                return _networkManager.ConnectedClientsIds.Count;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Возвращает RTT (ping) к серверу в миллисекундах.
        /// </summary>
        public int GetPing()
        {
            if (_networkManager != null && _networkManager.IsClient)
            {
                // RTT (Round Trip Time) в секундах
                float rtt = _networkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(_networkManager.ServerClientId);
                return Mathf.RoundToInt(rtt * 1000f);
            }
            
            return 0;
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/Network/ClientConnection.cs`

#### Шаг 5: Тестирование базового подключения

**5.1 Создайте тестовый UI для подключения**
```
Временный UI в игровой сцене:
├── Canvas
    └── Panel_NetworkTest
        ├── Button_StartHost
        ├── Button_StartClient
        ├── Button_Disconnect
        └── Text_Status
```

**5.2 Создайте NetworkTestUI.cs (временный)**
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlumpGame.Network.Debug
{
    /// <summary>
    /// ВРЕМЕННЫЙ UI для тестирования сети.
    /// Удалить после завершения Этапа 13!
    /// </summary>
    public class NetworkTestUI : MonoBehaviour
    {
        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _startClientButton;
        [SerializeField] private Button _disconnectButton;
        [SerializeField] private TextMeshProUGUI _statusText;
        
        private CustomNetworkManager _networkManager;
        
        private void Awake()
        {
            _networkManager = FindObjectOfType<CustomNetworkManager>();
            
            if (_startHostButton != null)
                _startHostButton.onClick.AddListener(OnStartHost);
            
            if (_startClientButton != null)
                _startClientButton.onClick.AddListener(OnStartClient);
            
            if (_disconnectButton != null)
                _disconnectButton.onClick.AddListener(OnDisconnect);
        }
        
        private void Update()
        {
            UpdateStatus();
        }
        
        private void OnStartHost()
        {
            if (_networkManager != null)
            {
                _networkManager.StartHost();
            }
        }
        
        private void OnStartClient()
        {
            if (_networkManager != null)
            {
                _networkManager.StartClient();
            }
        }
        
        private void OnDisconnect()
        {
            if (_networkManager != null)
            {
                _networkManager.Shutdown();
            }
        }
        
        private void UpdateStatus()
        {
            if (_statusText == null) return;
            
            var nm = Unity.Netcode.NetworkManager.Singleton;
            if (nm == null)
            {
                _statusText.text = "Network Manager: NULL";
                return;
            }
            
            string status = "Status: ";
            
            if (nm.IsHost)
                status += "HOST";
            else if (nm.IsServer)
                status += "SERVER";
            else if (nm.IsClient)
                status += "CLIENT";
            else
                status += "OFFLINE";
            
            if (nm.IsServer)
            {
                status += $"\nClients: {nm.ConnectedClientsIds.Count}";
            }
            
            _statusText.text = status;
        }
    }
}
```

**5.3 Тестирование**
```
1. Нажмите Play в Unity
2. Нажмите "Start Host" → должно появиться "Status: HOST"
3. Консоль должна показать: "✅ Server started successfully!"
4. Нажмите "Disconnect" → должно отключиться
```

#### ✅ Чеклист завершения Дня 1 (13.4)
- [ ] NetworkManager настроен в игровой сцене
- [ ] CustomNetworkManager.cs создан и работает
- [ ] ServerConnection.cs создан
- [ ] ClientConnection.cs создан
- [ ] Тестовый UI создан
- [ ] Host запускается успешно
- [ ] Connection approval работает
- [ ] Отключение работает

---

### 📁 ДЕНЬ 2: SCENE MANAGEMENT И ИНТЕГРАЦИЯ

#### Шаг 1: Настройка Network Scene Management

**1.1 Настройте NetworkManager для управления сценами**
```
NetworkManager:
└── Scene Management: ✅ Enable
```

**1.2 Создайте NetworkSceneManager.cs**
```csharp
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlumpGame.Network
{
    /// <summary>
    /// Управляет загрузкой сцен по сети.
    /// </summary>
    public class NetworkSceneManager : MonoBehaviour
    {
        private NetworkManager _networkManager;
        
        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            
            if (_networkManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
                _networkManager.SceneManager.OnUnloadEventCompleted += OnUnloadEventCompleted;
            }
        }
        
        private void OnDestroy()
        {
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
                _networkManager.SceneManager.OnUnloadEventCompleted -= OnUnloadEventCompleted;
            }
        }
        
        /// <summary>
        /// Загружает сцену для всех подключенных клиентов (только сервер).
        /// </summary>
        public void LoadSceneNetwork(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (_networkManager == null || !_networkManager.IsServer)
            {
                Debug.LogError("Only server can load network scenes!");
                return;
            }
            
            Debug.Log($"Loading scene: {sceneName}");
            
            var status = _networkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogError($"Failed to load scene: {sceneName}. Status: {status}");
            }
        }
        
        private void OnLoadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            System.Collections.Generic.List<ulong> clientsCompleted,
            System.Collections.Generic.List<ulong> clientsTimedOut)
        {
            Debug.Log($"✅ Scene loaded: {sceneName}");
            Debug.Log($"Clients completed: {clientsCompleted.Count}");
            
            if (clientsTimedOut.Count > 0)
            {
                Debug.LogWarning($"Clients timed out: {clientsTimedOut.Count}");
            }
        }
        
        private void OnUnloadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            System.Collections.Generic.List<ulong> clientsCompleted,
            System.Collections.Generic.List<ulong> clientsTimedOut)
        {
            Debug.Log($"Scene unloaded: {sceneName}");
        }
    }
}
```

**Где сохранить:** `Assets/_Project/Scripts/Network/NetworkSceneManager.cs`

#### Шаг 2: Интеграция с Matchmaking

**2.1 Обновите MatchmakingUI.cs**
```csharp
// Добавьте в StartMatchCountdown():

private IEnumerator StartMatchCountdown()
{
    // Запускаем хост (сервер + клиент)
    var networkManager = FindObjectOfType<CustomNetworkManager>();
    if (networkManager != null)
    {
        networkManager.StartHost();
        yield return new WaitForSeconds(1f); // Ждём запуска
    }
    
    for (int i = 5; i > 0; i--)
    {
        UpdateStatusText($"Starting in {i}...");
        yield return new WaitForSeconds(1f);
    }
    
    // Загружаем игровую сцену через network
    var selectedMode = GameModeManager.Instance.SelectedGameMode;
    if (selectedMode != null)
    {
        var networkSceneManager = FindObjectOfType<NetworkSceneManager>();
        if (networkSceneManager != null)
        {
            networkSceneManager.LoadSceneNetwork(selectedMode.gameSceneName);
        }
        else
        {
            // Fallback: обычная загрузка
            UI.SceneLoader.Instance.LoadScene(selectedMode.gameSceneName);
        }
    }
}
```

#### Шаг 3: Создание Network Prefabs списка

**3.1 Создайте папку для network префабов**
```
Assets/_Project/Prefabs/Network/
```

**3.2 Обновите NetworkManager**
```
NetworkManager:
└── Network Prefabs:
    └── (пока пусто, добавим в Этапе 14)
```

#### Шаг 4: Финальное тестирование

**4.1 Протестируйте весь flow**
```
Тест 1: Main Menu → Game Mode → Lobby → Match (Host)
1. Запустите игру
2. Main Menu → нажмите Play
3. Выберите любой режим
4. Дождитесь matchmaking
5. Проверьте загрузку игровой сцены
6. Проверьте консоль: должно быть "✅ Server started"

Тест 2: Build Testing (2 клиента)
1. File → Build Settings → Build
2. Запустите build → Start Host
3. Запустите Unity Editor → Start Client
4. Оба должны подключиться к одной сессии
```

#### Шаг 5: Документация и cleanup

**5.1 Создайте README для Этапа 13**
```markdown
# Network Setup - Completed ✅

## Что реализовано:
- ✅ Netcode for GameObjects установлен
- ✅ Main Menu с выбором режимов
- ✅ Lobby UI с matchmaking
- ✅ Network Manager настроен
- ✅ Connection approval работает
- ✅ Scene management настроен

## Следующие шаги:
- Этап 14: Player Networking
- Добавить NetworkObject к игроку
- Реализовать NetworkTransform
- Создать network combat

## Известные проблемы:
- [ ] Matchmaking пока симулируется
- [ ] Player prefab ещё не сетевой
- [ ] Combat не работает по сети

## Тестирование:
- Протестировано локально (Host)
- Build testing: В процессе
```

**5.2 Удалите тестовый код**
```
- NetworkTestUI.cs → удалить после тестов
- Тестовые UI элементы → удалить
```

#### ✅ Чеклист завершения Дня 2 (13.4)
- [ ] NetworkSceneManager создан и работает
- [ ] Интеграция с Matchmaking завершена
- [ ] Сцены загружаются через network
- [ ] Протестирован full flow: Menu → Lobby → Game
- [ ] Build testing пройден (2+ клиента)
- [ ] Документация создана
- [ ] Тестовый код удалён

---

## 🎉 ЗАВЕРШЕНИЕ ЭТАПА 13

### Финальный Чеклист

#### ✅ Функциональность
- [ ] Netcode for GameObjects установлен (v2.8.0+)
- [ ] Main Menu создано и работает
- [ ] Game Mode Selection функционирует
- [ ] Lobby UI с matchmaking работает
- [ ] NetworkManager настроен корректно
- [ ] Connection approval работает
- [ ] Scene management работает
- [ ] Навигация между сценами работает

#### ✅ Код
- [ ] SceneLoader.cs
- [ ] MainMenuUI.cs
- [ ] GameModeSelector.cs
- [ ] GameModeManager.cs
- [ ] GameModeData.cs + 5 ScriptableObjects
- [ ] MatchmakingUI.cs
- [ ] PlayerListItem.cs
- [ ] CustomNetworkManager.cs
- [ ] ServerConnection.cs
- [ ] ClientConnection.cs
- [ ] NetworkSceneManager.cs

#### ✅ UI/UX
- [ ] Main Menu выглядит привлекательно
- [ ] Game Mode Selection интуитивен
- [ ] Lobby показывает статус поиска
- [ ] Анимации работают плавно
- [ ] Кнопки реагируют на клик
- [ ] Навигация логична

#### ✅ Тестирование
- [ ] Main Menu → Game Mode Selection работает
- [ ] Game Mode Selection → Lobby работает
- [ ] Matchmaking симуляция работает
- [ ] Network connection работает (Host)
- [ ] Scene loading работает
- [ ] Build test пройден (2 клиента)

#### ✅ Оптимизация
- [ ] Нет утечек памяти (проверено Profiler)
- [ ] UI не лагает
- [ ] Переходы между сценами плавные
- [ ] Console без ошибок

---

## 📊 РЕЗУЛЬТАТЫ ЭТАПА 13

### Что получили:
```
✅ Готовая система главного меню
✅ Выбор из 5 игровых режимов
✅ Lobby с поиском игроков (UI)
✅ Базовая сетевая инфраструктура
✅ NetworkManager с approval
✅ Scene management по сети
✅ Навигация между 3 сценами (Menu, Lobby, Game)
```

### Метрики:
```
Сцены: 3 (MainMenu, Lobby, MainScene)
Скрипты: 11 новых
UI Prefabs: 2 (GameModeCard, PlayerListItem)
ScriptableObjects: 5 (Game Modes)
Время разработки: 7 дней
```

---

## 🚀 СЛЕДУЮЩИЙ ЭТАП

**Этап 14: Player Networking**
```
Задачи:
1. Сделать Player prefab сетевым (NetworkObject)
2. Добавить NetworkTransform
3. Синхронизировать движение
4. Реализовать network combat
5. Синхронизировать здоровье
6. Реализовать respawn по сети
```

**Когда начинать Этап 14:**
- После успешного завершения всех пунктов Этапа 13
- После прохождения всех тестов
- После code review (если есть команда)

---

## 📝 ЗАМЕТКИ

### Что нужно помнить:
```
⚠️ NetworkManager должен быть в игровой сцене (MainScene)
⚠️ NetworkManager НЕ ДОЛЖЕН быть DontDestroyOnLoad
⚠️ Matchmaking пока симулируется (реальный в Этапе 16)
⚠️ Боты пока не работают (будет в Этапе 16)
⚠️ Сохраните тестовый NetworkTestUI для дебага
```

### Полезные ресурсы:
```
Netcode Docs: https://docs-multiplayer.unity3d.com/
Boss Room Sample: github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
Community Discord: discord.gg/unity
```

---

**Статус:** В разработке  
**Прогресс:** 0% → 100%  
**Следующий этап:** Этап 14 - Player Networking

**Удачи в разработке!** 🚀
