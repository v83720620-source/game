# 💥 ЭТАП 4: Hit Zones System Setup

## 📋 Что создано:
- ✅ `HitZone.cs` - Enum зон урона и DamageInfo
- ✅ `HitBox.cs` - Компонент для частей тела
- ✅ `SimpleWeapon.cs` - Обновлён для поддержки зон

---

## 🎯 Система зон урона:

```
Голова (Head)      → x2.0 урона (критический урон)
Тело (Body)        → x1.0 урона (базовый урон)
Конечности (Limbs) → x0.75 урона (пониженный урон)
```

---

## 👤 ШАГ 1: Создание HitBox для головы (10 минут)

### 1.1 Создайте Sphere для головы:

```
Player/PlayerModel → 3D Object → Sphere
Название: "HeadHitBox"

Transform:
├── Position: (0, 0.65, 0)  // Выше тела
├── Scale: (0.3, 0.3, 0.3)  // Размер головы
```

### 1.2 Настройте коллайдер:

```
HeadHitBox:
1. Удалите Mesh Renderer (правый клик → Remove Component)
   Мы не хотим видеть сферу!

2. Sphere Collider:
   - Is Trigger: ✓ (ВАЖНО!)
   - Radius: 0.5
```

### 1.3 Добавьте HitBox компонент:

```
HeadHitBox:
1. Add Component → Hit Box
2. Settings:
   - Hit Zone: Head
   - Player Health: перетащите Player (или оставьте пустым - найдёт автоматически)
```

---

## 💪 ШАГ 2: Создание HitBox для тела (5 минут)

### 2.1 Создайте Capsule для тела:

```
Player/PlayerModel → 3D Object → Capsule
Название: "BodyHitBox"

Transform:
├── Position: (0, 0, 0)     // Центр тела
├── Scale: (0.4, 0.5, 0.4)  // Размер туловища
```

### 2.2 Настройте коллайдер:

```
BodyHitBox:
1. Удалите Mesh Renderer

2. Capsule Collider:
   - Is Trigger: ✓
   - Radius: 0.5
   - Height: 2
   - Direction: Y-Axis
```

### 2.3 Добавьте HitBox компонент:

```
BodyHitBox:
1. Add Component → Hit Box
2. Settings:
   - Hit Zone: Body
   - Player Health: перетащите Player
```

---

## 🦵 ШАГ 3: Создание HitBox для конечностей (опционально, 5 минут)

### Если хотите точные зоны для рук/ног:

```
Player/PlayerModel → 3D Object → Capsule
Название: "LeftArmHitBox"

Transform:
├── Position: (-0.4, 0, 0)  // Слева от тела
├── Rotation: (0, 0, 90)    // Горизонтально
├── Scale: (0.15, 0.4, 0.15)

Capsule Collider:
- Is Trigger: ✓

Hit Box:
- Hit Zone: Limbs
```

**Повторите для правой руки, левой/правой ноги**

---

## ⚠️ ВАЖНО: Отключите Character Controller для HitBox!

### Проблема:
Character Controller блокирует триггеры!

### Решение 1: Layer Mask (рекомендуется)

1. **Создайте Layer "HitBox":**
   ```
   Edit → Project Settings → Tags and Layers
   User Layer 8: "HitBox"
   ```

2. **Назначьте всем HitBox объектам:**
   ```
   HeadHitBox → Layer: HitBox
   BodyHitBox → Layer: HitBox
   (и остальные)
   ```

3. **Обновите Weapon Hit Mask:**
   ```
   Weapon → Simple Weapon (Script)
   Hit Mask: включите "HitBox" layer
   ```

### Решение 2: Упрощённый (быстрый)

Просто создайте HitBox **НЕ внутри PlayerModel**, а рядом:

```
Player
├── Character Controller
├── Player Movement
├── Player Health
├── CameraHolder
├── PlayerModel (визуал)
└── HitBoxes (Empty Object)
    ├── HeadHitBox
    ├── BodyHitBox
    └── LimbsHitBox
```

---

## ✅ ШАГ 4: Финальная структура

### Вариант 1: Минимальный (быстрый)

```
Player
├── Character Controller
├── Player Movement
├── Player Health
└── PlayerModel
    ├── HeadHitBox (Sphere + HitBox - Head)
    └── BodyHitBox (Capsule + HitBox - Body)
```

### Вариант 2: Полный (точный)

```
Player
└── PlayerModel
    ├── HeadHitBox (Head)
    ├── BodyHitBox (Body)
    ├── LeftArmHitBox (Limbs)
    ├── RightArmHitBox (Limbs)
    ├── LeftLegHitBox (Limbs)
    └── RightLegHitBox (Limbs)
```

---

## 🎮 ТЕСТИРОВАНИЕ!

### 1. Создайте dummy Target:

```
Hierarchy → 3D Object → Capsule
Название: "TestTarget"
Position: (5, 1, 0)  // Перед игроком

Add Component:
- Player Health (Max Health: 100)

Внутри TestTarget создайте:
- HeadHitBox (сверху)
- BodyHitBox (центр)
```

### 2. Запустите игру:

```
Нажмите Play
Стреляйте в Target:
```

**Проверьте в Console:**
```
Выстрел в голову:
→ "Hit Head: 20 x 2.0 = 40"
→ Красная debug линия

Выстрел в тело:
→ "Hit Body: 20 x 1.0 = 20"
→ Жёлтая debug линия

Выстрел в конечности:
→ "Hit Limbs: 20 x 0.75 = 15"
→ Голубая debug линия
```

---

## 🎨 Визуализация HitBox (опционально)

### Чтобы видеть HitBox в игре:

1. **Включите Gizmos в Game View:**
   ```
   Game View → Gizmos (справа сверху)
   ```

2. **Или добавьте OnDrawGizmos в HitBox.cs:**
   Для этого скажите мне и я добавлю!

---

## 🐛 Troubleshooting

### HitBox не регистрирует попадания:
- ✅ Is Trigger включен?
- ✅ Layer "HitBox" создан и назначен?
- ✅ В Weapon → Hit Mask включен "HitBox"?
- ✅ У HitBox есть Collider?

### Урон не применяется:
- ✅ Player Health компонент на Player?
- ✅ В HitBox → Player Health назначен?
- ✅ Current Health > 0?

### Debug линии одного цвета:
- ✅ HitBox компонент добавлен?
- ✅ Hit Zone правильно выбран (Head/Body/Limbs)?

### Character Controller блокирует попадания:
- ✅ Используйте Layer Mask решение выше
- ✅ Или создайте HitBox вне PlayerModel

---

## 🎉 Готово!

Теперь у вас есть:
- ✅ Система зон урона (Head x2, Body x1, Limbs x0.75)
- ✅ HitBox компоненты
- ✅ Цветные debug линии
- ✅ Интеграция с оружием

---

## 📸 Что должно получиться:

### В Scene View:
```
Player с множеством коллайдеров:
- Маленькая сфера сверху (голова)
- Большая капсула в центре (тело)
- Опционально: капсулы для рук/ног
```

### При стрельбе:
```
Голова   → Красная линия   → 40 урона
Тело     → Жёлтая линия    → 20 урона
Конечности → Голубая линия → 15 урона
```

---

## 🚀 Следующие этапы:

**Напишите "Этап 4 работает"** когда увидите разный урон!

### ЭТАП 5: Продвинутое оружие
- Система магазинов
- Перезарядка
- Множественное оружие
- Switching

---

**Начинайте настройку!** 

Создайте минимум 2 HitBox (голова + тело) и протестируйте! 🎯

Если будут вопросы - сразу говорите! 😊
