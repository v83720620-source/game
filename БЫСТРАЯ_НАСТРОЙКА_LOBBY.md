# ⚡ БЫСТРАЯ НАСТРОЙКА LOBBY (10 минут)

---

## 📋 ЧЕКЛИСТ:

### **1. LobbyManager (2 мин)**
```
✓ Откройте Lobby.unity
✓ Выберите LobbyManager GameObject
✓ Add Component → NetworkObject
✓ Add Component → MatchmakingManager
✓ NetworkObject: Don't Destroy With Owner = true
✓ MatchmakingManager: Auto Fill Bots = true
```

---

### **2. Создать кнопки (3 мин)**
```
✓ Canvas → ПКМ → UI → Panel → "Panel_GameModes"
✓ Anchor: Top Center, Pos Y: -200, Width: 800, Height: 400

Создайте 3 кнопки:
✓ Button_Duel1v1    (Pos Y: 100,  Text: "⚔️ DUEL 1v1")
✓ Button_TDM3v3     (Pos Y: 0,    Text: "🎮 TEAM 3v3")
✓ Button_Practice   (Pos Y: -100, Text: "🎯 PRACTICE")
```

---

### **3. Подключить скрипт (5 мин)**
```
✓ Canvas → Add Component → MatchmakingUI

Назначьте ссылки:
✓ Duel 1v1 Button → Button_Duel1v1
✓ TDM 3v3 Button → Button_TDM3v3
✓ Practice Button → Button_Practice
✓ Queue Panel → Panel_SearchStatus
✓ Queue Status Text → Text_Title
✓ Players Count Text → Text_PlayersFound
✓ Timer Text → Text_Timer
✓ Cancel Button → Button_Cancel
✓ Update Interval → 0.5

Настройте панели:
✓ Panel_SearchStatus → SetActive = false (отключить)
✓ Panel_GameModes → SetActive = true (включить)
```

---

### **4. (Опционально) Spinner анимация**
```
✓ Image_LoadingSpinner → Add Component → LoadingSpinner
✓ Rotation Speed: 180
```

---

## 🧪 ТЕСТ:

```
1. Play
2. Выберите "PRACTICE"
3. Ждите 15s
4. Должна загрузиться Game сцена
```

---

## ✅ ГОТОВО!

**Подробная инструкция:** `ИНТЕГРАЦИЯ_MATCHMAKING_В_LOBBY.md`

**Напишите "настроил" когда закончите!**
