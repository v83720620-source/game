# 🔫 ЭТАП 5: Advanced Weapon System Setup

## 📋 Что создано:
- ✅ `WeaponData.cs` - ScriptableObject для данных оружия
- ✅ `Magazine.cs` - Система магазина и патронов
- ✅ `AdvancedWeapon.cs` - Продвинутый контроллер оружия
- ✅ `GameHUD.cs` - Обновлён для отображения патронов

---

## 🎯 Новые возможности:

```
✅ Магазин и патроны (30/90)
✅ Перезарядка (R)
✅ Авто/полуавто режимы
✅ Система разброса (accuracy)
✅ Улучшенная отдача
✅ Интеграция с UI
```

---

## 📦 ШАГ 1: Создание WeaponData (5 минут)

### 1.1 Создайте ScriptableObject:

```
Project → Assets/_Project/ScriptableObjects
(Если нет папки - создайте: правый клик → Create → Folder)

Правый клик → Create → Flump → Weapon Data
Название: "AssaultRifle_Data"
```

### 1.2 Настройте данные:

```
AssaultRifle_Data:

Weapon Info:
├── Weapon Name: "Assault Rifle"

Damage:
├── Base Damage: 20
└── Range: 100

Fire Rate:
├── Fire Rate: 0.1 (10 выстрелов/сек)
└── Is Automatic: ✓

Magazine:
├── Magazine Size: 30
├── Reserve Ammo: 90
└── Reload Time: 2

Recoil:
├── Recoil Amount: 1
├── Recoil Pattern: (0.3, 0.8)
└── Recoil Recovery Speed: 5

Spread:
├── Base Spread: 0.01
├── Max Spread: 0.1
├── Spread Increase Per Shot: 0.01
└── Spread Decrease Speed: 5
```

---

## 🔫 ШАГ 2: Замена SimpleWeapon на AdvancedWeapon (10 минут)

### 2.1 Удалите старый SimpleWeapon:

```
Weapon объект (Main Camera/Weapon):
1. Найдите "Simple Weapon (Script)"
2. Правый клик → Remove Component
```

### 2.2 Добавьте новые компоненты:

```
Weapon объект:
1. Add Component → Magazine
2. Add Component → Advanced Weapon
```

### 2.3 Настройте Magazine:

```
Magazine (Script):
├── Magazine Size: 30
├── Current Ammo: 30
├── Reserve Ammo: 90
├── Infinite Ammo: ☐
└── Reload Time: 2
```

### 2.4 Настройте AdvancedWeapon:

```
Advanced Weapon (Script):

Weapon Data:
└── Weapon Data: перетащите AssaultRifle_Data

References:
├── Muzzle Point: (оставьте пустым - найдёт автоматически)
├── Player Camera: (оставьте пустым)
└── Hit Mask: Everything (или выберите нужные layers)

Effects:
├── Muzzle Flash Prefab: None (пока)
└── Audio Source: None (пока)
```

---

## 🎮 ШАГ 3: Обновление GameHUDManager (5 минут)

### 3.1 Обновите ссылки:

```
GameHUDManager → Game HUD (Script):

Game Systems:
├── Player Health: Player ✓
├── Simple Weapon: None (удалите)
├── Advanced Weapon: перетащите Weapon
└── Player Movement: Player ✓
```

---

## 🔌 ШАГ 4: Обновление MobileInputManager (опционально)

### Если используете mobile controls:

```
MobileInputManager → Mobile Input Manager (Script):

Target Systems:
├── Weapon: удалите старую ссылку
└── (Advanced Weapon использует ту же систему)
```

**ИЛИ**

Оставьте как есть - `SimpleWeapon` и `AdvancedWeapon` совместимы по API.

---

## ✅ ШАГ 5: Финальная структура

```
Player/CameraHolder/Main Camera/Weapon:
├── Transform
├── Magazine (Script)
│   ├── Magazine Size: 30
│   ├── Current Ammo: 30
│   ├── Reserve Ammo: 90
│   └── Reload Time: 2
├── Advanced Weapon (Script)
│   ├── Weapon Data: AssaultRifle_Data
│   ├── Player Camera: Auto
│   └── Hit Mask: Everything
└── WeaponModel (Optional visual)

GameHUDManager:
└── Game HUD (Script)
    ├── Health Bar: HealthBarBackground
    ├── Ammo Display: AmmoDisplay
    ├── Crosshair: Crosshair
    ├── Player Health: Player
    ├── Advanced Weapon: Weapon  ← Обновлено!
    └── Player Movement: Player
```

---

## 🎮 ТЕСТИРОВАНИЕ!

### Нажмите Play и проверьте:

#### Стрельба:
- ✅ **ЛКМ** - стреляет (автоматический огонь)
- ✅ Счётчик патронов уменьшается: **30 → 29 → 28...**
- ✅ При 0 патронов - автоматическая перезарядка

#### Перезарядка:
- ✅ Нажмите **R** → Перезарядка начинается
- ✅ В Console: **"Reloading... (2s)"**
- ✅ Через 2 секунды: **"Reload complete! 30/60"**
- ✅ Патроны обновились: **30/60** (30 в магазине, 60 в запасе)

#### Ammo Display:
- ✅ Показывает **30 / 90** в начале
- ✅ При стрельбе: **29 / 90** → **28 / 90**...
- ✅ При перезарядке: **0 / 90** → **30 / 60**
- ✅ Когда мало патронов: **3 / 60** → текст **жёлтый**
- ✅ Когда 0: **0 / 60** → текст **красный**

#### Spread (разброс):
- ✅ Стреляйте очередью - пули летят с разбросом
- ✅ Подождите - разброс уменьшается

---

## 🔧 Создание второго оружия (опционально)

### Создайте снайперскую винтовку:

```
Create → Flump → Weapon Data
Название: "Sniper_Data"

Settings:
├── Weapon Name: "Sniper Rifle"
├── Base Damage: 80  ← Высокий урон
├── Fire Rate: 0.8   ← Медленная стрельба
├── Is Automatic: ☐  ← Полуавтоматическая!
├── Magazine Size: 5 ← Маленький магазин
├── Reserve Ammo: 20
├── Base Spread: 0.001 ← Очень точная
└── Recoil Amount: 3  ← Сильная отдача
```

---

## 🎨 Настройка параметров оружия

### Fire Rate (скорострельность):
```
0.05 = 20 выстрелов/сек (пулемёт)
0.1  = 10 выстрелов/сек (автомат)
0.3  = 3 выстрела/сек (полуавто)
1.0  = 1 выстрел/сек (снайперка)
```

### Spread (точность):
```
0.001 = Идеальная точность (снайперка)
0.01  = Хорошая точность (автомат)
0.05  = Средняя точность (пистолет)
0.1   = Плохая точность (дробовик)
```

### Recoil (отдача):
```
0.5 = Слабая
1.0 = Средняя
2.0 = Сильная
5.0 = Очень сильная
```

---

## 🐛 Troubleshooting

### Патроны не уменьшаются:
- ✅ Magazine компонент добавлен на Weapon?
- ✅ В Magazine → Current Ammo не 0?
- ✅ Advanced Weapon видит Magazine?

### Не перезаряжается:
- ✅ Reserve Ammo > 0?
- ✅ Infinite Ammo выключен?
- ✅ Нажимаете R?

### Ammo Display показывает 999/∞:
- ✅ В GameHUD → Advanced Weapon назначен?
- ✅ Magazine компонент существует?
- ✅ Magazine.OnAmmoChanged срабатывает?

### Оружие не стреляет:
- ✅ Weapon Data назначен?
- ✅ Hit Mask настроен?
- ✅ Current Ammo > 0?

### Infinite Ammo не работает:
- ✅ Magazine → Infinite Ammo: ✓
- ✅ Reserve Ammo можно оставить любым

---

## 🎉 Готово!

Теперь у вас есть:
- ✅ Полноценная система магазинов
- ✅ Перезарядка (R)
- ✅ Счётчик патронов в UI
- ✅ Авто/полуавто режимы
- ✅ Система разброса
- ✅ ScriptableObject для оружия
- ✅ Готово для добавления нового оружия!

---

## 📸 Что должно получиться:

### В UI:
```
┌─────────────────────────────┐
│ [====HP====]                │
│                             │
│            +                │
│                             │
│                             │
│                   30 / 90   │  ← Патроны!
│               [Buttons]     │
└─────────────────────────────┘
```

### При стрельбе:
```
30 / 90  → *Пиф* → 29 / 90 → *Пиф* → 28 / 90...
```

### При перезарядке:
```
0 / 90  → Нажал R → "Reloading..." → 30 / 60
```

---

## 🚀 Следующие этапы:

**Напишите "Этап 5 работает"** когда протестируете!

### ЭТАП 6: AI Боты
- Базовый AI
- Патрулирование
- Стрельба по игроку
- Использование Hit Zones
- Difficulty levels

---

**Начинайте настройку!** 

1. Создайте WeaponData
2. Замените SimpleWeapon на AdvancedWeapon
3. Обновите GameHUD
4. Тестируйте!

Если будут вопросы - сразу говорите! 🔫
