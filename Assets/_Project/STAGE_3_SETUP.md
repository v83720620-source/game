# 🎯 ЭТАП 3: Game HUD Setup

## 📋 Что создано:
- ✅ `HealthBar.cs` - Полоска здоровья
- ✅ `AmmoDisplay.cs` - Счётчик патронов
- ✅ `Crosshair.cs` - Динамический прицел
- ✅ `GameHUD.cs` - Менеджер всего HUD

---

## ❤️ ШАГ 1: Health Bar (10 минут)

### 1.1 Создайте контейнер:
```
MobileUI → Create Empty
Название: "GameHUD"

Rect Transform:
- Anchor: Stretch (занимает весь экран)
- Left: 0, Top: 0, Right: 0, Bottom: 0
```

### 1.2 Создайте HealthBar Background:
```
GameHUD → UI → Image
Название: "HealthBarBackground"

Rect Transform:
├── Anchor Preset: Top Left
├── Position: (20, -20, 0)
├── Width: 200
└── Height: 20

Image:
├── Color: Black (Alpha: 0.5)
└── Source Image: UISprite
```

### 1.3 Создайте HealthBar Fill:
```
HealthBarBackground → UI → Image
Название: "HealthBarFill"

Rect Transform:
├── Anchor: Stretch (заполняет родителя)
├── Left: 0, Top: 0, Right: 0, Bottom: 0

Image:
├── Image Type: Filled
├── Fill Method: Horizontal
├── Fill Origin: Left
├── Fill Amount: 1
├── Color: Green
└── Source Image: UISprite
```

### 1.4 Добавьте HealthBar компонент:
```
HealthBarBackground:
1. Add Component → Health Bar
2. Settings:
   - Fill Image: перетащите HealthBarFill
   - Background Image: перетащите HealthBarBackground
   - Healthy Color: Green (0, 255, 0)
   - Damaged Color: Yellow (255, 255, 0)
   - Critical Color: Red (255, 0, 0)
   - Animation Speed: 5
   - Smooth Transition: ✓
```

---

## 🔫 ШАГ 2: Ammo Display (5 минут)

### 2.1 Создайте AmmoText:
```
GameHUD → UI → Text
Название: "AmmoDisplay"

Rect Transform:
├── Anchor Preset: Bottom Right
├── Position: (-20, 20, 0)
├── Width: 150
└── Height: 50

Text:
├── Text: "999 / ∞"
├── Font Size: 32
├── Alignment: Right/Bottom
├── Color: White
└── Best Fit: ✓ (опционально)
```

### 2.2 Добавьте AmmoDisplay компонент:
```
AmmoDisplay:
1. Add Component → Ammo Display
2. Settings:
   - Ammo Text: перетащите Text компонент
   - Format: "{0} / {1}"
   - Show Infinite Symbol: ✓
   - Normal Color: White
   - Low Ammo Color: Yellow
   - Empty Color: Red
   - Low Ammo Threshold: 0.3
```

---

## 🎯 ШАГ 3: Crosshair (15 минут)

### 3.1 Создайте контейнер:
```
GameHUD → Create Empty
Название: "Crosshair"

Rect Transform:
├── Anchor: Center
├── Position: (0, 0, 0)
├── Width: 100
└── Height: 100
```

### 3.2 Создайте линии прицела:

**Top Line:**
```
Crosshair → UI → Image
Название: "TopLine"

Rect Transform:
├── Anchor: Center
├── Position: (0, 10, 0)
├── Width: 2
└── Height: 10

Image:
├── Color: White (Alpha: 0.8)
└── Source Image: UISprite (белый квадрат)
```

**Bottom Line:**
```
Duplicate TopLine (Ctrl+D)
Название: "BottomLine"
Position: (0, -10, 0)
```

**Left Line:**
```
Duplicate TopLine
Название: "LeftLine"
Position: (-10, 0, 0)
Width: 10
Height: 2
```

**Right Line:**
```
Duplicate LeftLine
Название: "RightLine"
Position: (10, 0, 0)
```

### 3.3 Добавьте Crosshair компонент:
```
Crosshair (родительский объект):
1. Add Component → Crosshair
2. Settings:
   - Top Line: перетащите TopLine (RectTransform)
   - Bottom Line: перетащите BottomLine
   - Left Line: перетащите LeftLine
   - Right Line: перетащите RightLine
   - Base Spread: 10
   - Max Spread: 50
   - Spread Speed: 5
   - Shoot Spread Increase: 5
   - Move Spread Increase: 10
   - Normal Color: White
   - Hit Color: Red
   - Hit Feedback Duration: 0.1
```

---

## 🎮 ШАГ 4: Game HUD Manager (5 минут)

### 4.1 Создайте менеджер:
```
Hierarchy → Create Empty
Название: "GameHUDManager"
Position: (0, 0, 0)
```

### 4.2 Добавьте GameHUD компонент:
```
GameHUDManager:
1. Add Component → Game HUD
2. Settings:

UI References:
├── Health Bar: перетащите HealthBarBackground
├── Ammo Display: перетащите AmmoDisplay
└── Crosshair: перетащите Crosshair

Game Systems:
├── Player Health: перетащите Player
├── Weapon: перетащите Weapon (из Camera/Weapon)
└── Player Movement: перетащите Player
```

---

## ✅ ШАГ 5: Финальная структура

### Должна получиться:
```
Scene:
├── Player
│   └── ...
├── MobileInputManager
├── MobileUI (Canvas)
│   ├── Joystick
│   ├── Buttons
│   └── GameHUD
│       ├── HealthBarBackground (Image + HealthBar)
│       │   └── HealthBarFill (Image)
│       ├── AmmoDisplay (Text + AmmoDisplay)
│       └── Crosshair (Crosshair Script)
│           ├── TopLine (Image)
│           ├── BottomLine (Image)
│           ├── LeftLine (Image)
│           └── RightLine (Image)
└── GameHUDManager
    └── GameHUD (Script)
```

---

## 🎮 ТЕСТИРОВАНИЕ!

### Нажмите Play и проверьте:

#### Health Bar:
- ✅ Видна **слева вверху**
- ✅ **Зелёная** полоска (полное здоровье)
- ✅ Должна быть плавная анимация

**Тест урона:**
1. Выберите `Player` в Hierarchy
2. В Inspector найдите `Player Health (Script)`
3. Нажмите Play
4. Измените `Current Health` (например, с 100 на 50)
5. Полоска должна **плавно** стать короче и **пожелтеть**
6. Установите 20 - должна стать **красной**

#### Ammo Display:
- ✅ Виден **справа внизу**
- ✅ Показывает **"999 / ∞"**
- ✅ Белый цвет

#### Crosshair:
- ✅ **Крестик по центру экрана**
- ✅ При **движении** (WASD) - расширяется
- ✅ При **стрельбе** (ЛКМ) - расширяется
- ✅ Когда стоите - возвращается к базовому размеру

---

## 🎨 Кастомизация

### Изменение цветов Health Bar:
```
HealthBar компонент:
- Healthy Color: Зелёный (здоровье > 60%)
- Damaged Color: Жёлтый (здоровье 30-60%)
- Critical Color: Красный (здоровье < 30%)
```

### Изменение размера Crosshair:
```
Crosshair компонент:
- Base Spread: Начальный размер (меньше = точнее)
- Max Spread: Максимальный размер при движении/стрельбе
```

### Изменение формата Ammo:
```
AmmoDisplay компонент:
- Format: "{0} / {1}" → "Ammo: {0}"
- Show Infinite Symbol: Показывать ∞ или 999
```

---

## 🐛 Troubleshooting

### Health Bar не меняется:
- ✅ В `GameHUD` назначен `Player Health`?
- ✅ У `HealthBar` назначен `Fill Image`?
- ✅ У `HealthBarFill` Image Type = **Filled**?

### Crosshair не двигается:
- ✅ Все 4 линии назначены?
- ✅ У линий правильный `RectTransform` (не Image)?
- ✅ В `GameHUD` назначены `Weapon` и `Player Movement`?

### Ammo не отображается:
- ✅ У `AmmoDisplay` назначен `Text` компонент?
- ✅ Text существует и виден?

### Crosshair слишком большой/маленький:
- ✅ Измените `Base Spread` (10 = средний)
- ✅ Проверьте размер линий (Width/Height)

---

## 🎉 Готово!

Теперь у вас есть:
- ✅ Health Bar с плавной анимацией
- ✅ Ammo Display (готов для системы патронов)
- ✅ Динамический Crosshair
- ✅ Всё интегрировано с игровыми системами!

---

## 📸 Что должно получиться:

```
┌────────────────────────────────┐
│ [====HP====]                   │  ← Health Bar
│                                │
│             +                  │  ← Crosshair
│                                │
│                                │
│                                │
│ (Joystick)          999 / ∞    │  ← Ammo
│                   [Buttons]    │
└────────────────────────────────┘
```

---

## 🚀 Следующие этапы:

После тестирования у вас будет **полноценный HUD**!

**Напишите "Этап 3 работает"** и мы перейдём к:

### ЭТАП 4: Система здоровья и урона
- Полноценная система HP
- Respawn система
- Damage zones
- Shield система (как в документации)

---

**Начинайте настройку!** Следуйте инструкции шаг за шагом! 😊

Если будут вопросы - сразу говорите! 🎮
