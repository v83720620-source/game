# 🔧 ИНТЕГРАЦИЯ MATCHMAKING В СУЩЕСТВУЮЩУЮ LOBBY СЦЕНУ

**Дата:** 12.02.2026  
**Цель:** Подключить Matchmaking System к вашей готовой Lobby сцене

---

## ✅ ЧТО УЖЕ ЕСТЬ:

### **В сцене Lobby.unity:**
```
✓ Canvas
✓ Panel_SearchStatus (наша Queue Panel!)
✓ Panel_PlayerList
✓ ScrollView_Players
✓ Button_Cancel
✓ Button_Back
✓ Text_Title
✓ Text_PlayersFound
✓ Text_Timer
✓ Image_LoadingSpinner
✓ EventSystem
✓ LobbyManager GameObject (пустой)
```

**Вывод:** UI уже готов! Нужно только подключить скрипты!

---

## 📋 БЫСТРАЯ ИНТЕГРАЦИЯ (10 минут):

### **ШАГ 1: Добавить MatchmakingManager (2 мин)**

1. **Откройте сцену Lobby:**
   ```
   Assets/Scenes/Lobby.unity (двойной клик)
   ```

2. **Выберите GameObject "LobbyManager" в Hierarchy**

3. **Добавьте компоненты:**
   ```
   Inspector → Add Component → NetworkObject
   Inspector → Add Component → MatchmakingManager
   ```

4. **Настройте NetworkObject:**
   ```
   ✓ Don't Destroy With Owner = true
   ✓ Destroy With Scene = false
   ```

5. **Настройте MatchmakingManager:**
   ```
   ✓ Auto Fill Bots = true
   ```

---

### **ШАГ 2: Создать кнопки Game Mode (3 мин)**

**Нужно добавить 3 кнопки для выбора режима:**

1. **Найдите Canvas в Hierarchy**

2. **Создайте панель для кнопок:**
   ```
   Canvas → ПКМ → UI → Panel
   Имя: "Panel_GameModes"
   
   Rect Transform:
   - Anchor: Top Center
   - Pos Y: -200
   - Width: 800
   - Height: 400
   ```

3. **Создайте 3 кнопки:**
   
   **Кнопка A: Duel 1v1**
   ```
   Panel_GameModes → ПКМ → UI → Button
   Имя: "Button_Duel1v1"
   
   Rect Transform:
   - Pos Y: 100
   - Width: 300
   - Height: 80
   
   Text (TMP):
   - Текст: "⚔️ DUEL 1v1"
   - Font Size: 32
   ```
   
   **Кнопка B: TDM 3v3**
   ```
   Panel_GameModes → ПКМ → UI → Button
   Имя: "Button_TDM3v3"
   
   Rect Transform:
   - Pos Y: 0
   - Width: 300
   - Height: 80
   
   Text (TMP):
   - Текст: "🎮 TEAM 3v3"
   - Font Size: 32
   ```
   
   **Кнопка C: Practice**
   ```
   Panel_GameModes → ПКМ → UI → Button
   Имя: "Button_Practice"
   
   Rect Transform:
   - Pos Y: -100
   - Width: 300
   - Height: 80
   
   Text (TMP):
   - Текст: "🎯 PRACTICE"
   - Font Size: 32
   ```

---

### **ШАГ 3: Адаптировать MatchmakingUI (5 мин)**

Мне нужно создать **адаптированную версию** MatchmakingUI под ваш UI!

1. **Выберите Canvas в Hierarchy**

2. **Добавьте компонент:**
   ```
   Inspector → Add Component → MatchmakingUI
   ```

3. **Назначьте ссылки в Inspector:**
   
   **Game Mode Selection:**
   ```
   Duel 1v1 Button: [перетащите Button_Duel1v1]
   TDM 3v3 Button: [перетащите Button_TDM3v3]
   Practice Button: [перетащите Button_Practice]
   ```
   
   **Queue Panel:**
   ```
   Queue Panel: [перетащите Panel_SearchStatus]
   Queue Status Text: [перетащите Text_Title]
   Players Count Text: [перетащите Text_PlayersFound]
   Timer Text: [перетащите Text_Timer]
   Cancel Button: [перетащите Button_Cancel]
   ```
   
   **Settings:**
   ```
   Update Interval: 0.5
   ```

4. **Настройте видимость панелей:**
   ```
   Panel_SearchStatus → Inspector → ✓ Отключите (SetActive = false)
   Panel_GameModes → Inspector → ✓ Включите (SetActive = true)
   ```

---

## 🎮 ЛОГИКА РАБОТЫ:

### **Когда игрок открывает Lobby:**
```
1. Видит Panel_GameModes с 3 кнопками
2. Выбирает режим (Duel/TDM/Practice)
3. Panel_GameModes скрывается
4. Panel_SearchStatus появляется с:
   - Text_Title: "Searching for players..."
   - Text_PlayersFound: "Players: 1/2"
   - Text_Timer: "Time: 15s"
   - Image_LoadingSpinner вращается
   - Button_Cancel активна
5. Через 15s → добавляются боты
6. Match starts!
```

---

## 📊 МАППИНГ ВАШЕГО UI → МОЙ СКРИПТ:

| **Ваш UI** | **Мой скрипт** | **Назначение** |
|-------------|----------------|----------------|
| Panel_SearchStatus | Queue Panel | Панель поиска игры |
| Text_Title | Queue Status Text | "Searching..." / "Adding bots..." |
| Text_PlayersFound | Players Count Text | "Players: 1/2" |
| Text_Timer | Timer Text | "Time: 15s" |
| Button_Cancel | Cancel Button | Отмена поиска |
| Panel_GameModes | (новая) | Выбор режима |
| Button_Duel1v1 | (новая) | Кнопка Duel |
| Button_TDM3v3 | (новая) | Кнопка TDM |
| Button_Practice | (новая) | Кнопка Practice |

---

## ⚡ ДОПОЛНИТЕЛЬНО (опционально):

### **Image_LoadingSpinner - Анимация вращения:**

Если хотите чтобы спиннер вращался:

1. **Создайте скрипт `LoadingSpinner.cs`:**
   ```csharp
   using UnityEngine;

   public class LoadingSpinner : MonoBehaviour
   {
       [SerializeField] private float _rotationSpeed = 180f;
       
       private void Update()
       {
           transform.Rotate(0, 0, -_rotationSpeed * Time.deltaTime);
       }
   }
   ```

2. **Добавьте на Image_LoadingSpinner:**
   ```
   Image_LoadingSpinner → Add Component → LoadingSpinner
   Rotation Speed: 180
   ```

---

### **Panel_PlayerList - Список игроков (будущее):**

Этот панель можно использовать для отображения списка игроков в очереди:
- Пока оставьте пустым
- В будущем (Этап 16.2+) добавим динамическое отображение

---

## 🔍 ПРОВЕРКА НАСТРОЙКИ:

### **Чеклист перед тестом:**

- [ ] LobbyManager существует в сцене
- [ ] NetworkObject добавлен на LobbyManager
- [ ] MatchmakingManager добавлен на LobbyManager
- [ ] Panel_GameModes создана с 3 кнопками
- [ ] MatchmakingUI добавлен на Canvas
- [ ] Все ссылки назначены в Inspector
- [ ] Panel_SearchStatus изначально отключена (SetActive = false)
- [ ] Panel_GameModes изначально включена (SetActive = true)

---

## 🧪 ТЕСТ:

### **Тест 1: Solo (Practice Mode)**
```
1. Play в Unity Editor
2. Выберите "PRACTICE"
3. Наблюдайте:
   - Panel_GameModes скрывается
   - Panel_SearchStatus появляется
   - Text_Title: "Searching for players..."
   - Text_Timer: идёт отсчёт
   - Через 15s: "Adding bots..."
   - Загружается Game сцена
```

### **Тест 2: Duel 1v1 (с ParrelSync)**
```
Clone 1:
1. Start Host
2. Выберите "⚔️ DUEL 1v1"
3. Ждёте...

Clone 2:
1. Start Client
2. Выберите "⚔️ DUEL 1v1"
3. Оба клиента видят "Match starting!"
4. Загружается Game сцена
```

### **Тест 3: Cancel**
```
1. Выберите любой режим
2. Нажмите [Cancel]
3. Наблюдайте:
   - Panel_SearchStatus скрывается
   - Panel_GameModes появляется
   - Можно снова выбрать режим
```

---

## ⚠️ ВОЗМОЖНЫЕ ПРОБЛЕМЫ:

### **Проблема: "MatchmakingManager not found!"**
```
Решение:
- Убедитесь что LobbyManager в сцене
- Проверьте что MatchmakingManager Awake() создаёт Singleton
- Проверьте Console на ошибки инициализации
```

### **Проблема: "Queue Panel не появляется"**
```
Решение:
- Проверьте что Panel_SearchStatus назначена в MatchmakingUI
- Убедитесь что SetActive = false изначально
- Проверьте что кнопка вызывает OnGameModeSelected()
```

### **Проблема: "Scene не загружается"**
```
Решение:
- File → Build Settings
- Добавьте Lobby.unity и Game.unity
- Проверьте что NetworkManager может загружать сцены
```

---

## ✅ ГОТОВО К ИНТЕГРАЦИИ!

**Следуйте шагам 1-3, это займёт 10 минут!**

После настройки напишите:
- ✅ **"настроил"** - продолжим с тестированием
- ❓ **"не понятно [что]"** - объясню подробнее
- 🐛 **"ошибка [текст]"** - исправим

---

**НАЧИНАЙТЕ С ШАГА 1!** 🚀
