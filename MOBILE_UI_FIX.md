# 📱 ОТКЛЮЧЕНИЕ MOBILE UI НА PC

## ❌ ПРОБЛЕМА:

На скриншоте видны мобильные элементы управления:
- Виртуальный джойстик (слева)
- Кнопки RUN, JUMP, CROUCH, FIRE (справа)

Это не должно показываться на PC!

---

## ✅ РЕШЕНИЕ (30 СЕКУНД):

### Шаг 1: Откройте сцену Game

```
Unity → Project → Scenes → Game (двойной клик)
```

---

### Шаг 2: Найдите Mobile UI

В **Hierarchy** найдите один из этих объектов:
- `MobileUI`
- `Canvas` (с джойстиком и кнопками)
- `VirtualJoystick`
- `MobileInputManager`

---

### Шаг 3: ВАРИАНТ A - Отключить GameObject

```
1. Выберите Canvas/MobileUI в Hierarchy
2. Inspector → Снимите галочку слева от имени объекта
```

**Результат:** Mobile UI отключен ✅

---

### Шаг 3: ВАРИАНТ B - Проверить Auto Detect

Если видите **MobileInputManager**:

```
Inspector → MobileInputManager (Script):

Auto Detect Platform: ✅ (должна быть включена!)
Mobile UI: (перетащите сюда Canvas с мобильным UI)
```

Это автоматически скроет UI на PC!

---

### Шаг 3: ВАРИАНТ C - Удалить совсем

Если не нужен мобильный UI вообще:

```
Hierarchy → Canvas/MobileUI → Delete
```

---

## 🎯 ПРОВЕРКА:

После исправления:

```
Play Mode:
✅ НЕТ джойстика слева
✅ НЕТ кнопок справа
✅ Только прицел и HUD (Health, Ammo)
```

---

## 🐛 ЕСЛИ НЕ ПОМОГЛО:

### Проверьте все Canvas объекты

```
1. Hierarchy → найдите ВСЕ объекты типа Canvas
2. Для каждого Canvas:
   - Откройте его детей (▶)
   - Если видите VirtualJoystick или MobileButton
   - Отключите этот Canvas
```

---

## 💡 ДЛЯ БУДУЩЕГО:

Если хотите поддерживать и PC и Mobile:

### Используйте Platform Detection

```csharp
#if UNITY_ANDROID || UNITY_IOS
    // Mobile код
#else
    // PC код
#endif
```

Или используйте `Application.isMobilePlatform` в runtime.

---

**НАЧНИТЕ С ВАРИАНТА A - ПРОСТО ОТКЛЮЧИТЕ CANVAS!**
