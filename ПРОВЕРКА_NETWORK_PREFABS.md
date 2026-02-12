# ✅ ПРОВЕРКА NETWORK PREFABS LIST

**Это критично для работы multiplayer!**

---

## 📋 ЧТО ПРОВЕРИТЬ:

### **В Unity Editor:**

1. **Откройте сцену с NetworkManager:**
   ```
   Scenes:
   - MainMenu.unity?
   - Lobby.unity?
   - Game.unity?
   
   (Ищите GameObject с NetworkManager компонентом)
   ```

2. **Выберите NetworkManager GameObject:**
   ```
   Hierarchy → NetworkManager (или CustomNetworkManager)
   ```

3. **Inspector → Network Prefabs List:**
   ```
   Должны быть в списке:
   ✅ Player.prefab (обязательно!)
   ✅ Bot.prefab (если используется)
   ✅ Любые spawnable objects
   ```

---

## ⚠️ ЕСЛИ ЧЕГО-ТО НЕТ:

### **Добавить Player.prefab:**
```
1. Project → Assets/_Project/Prefabs/
2. Найдите Player.prefab
3. Drag & Drop в Network Prefabs List
```

### **Добавить Bot.prefab:**
```
1. Project → Assets/_Project/Prefabs/
2. Найдите Bot.prefab
3. Drag & Drop в Network Prefabs List
```

---

## 🎯 РЕЗУЛЬТАТ:

После проверки:
- [ ] Player.prefab в Network Prefabs List
- [ ] Bot.prefab в Network Prefabs List (если используется)
- [ ] Нет ошибок в Console

---

## ✅ ГОТОВО!

**Напишите "проверил prefabs" когда закончите!**
