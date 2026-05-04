# 🎮 ЭТАП 1: Настройка в Unity

## 📋 Что создано:
- ✅ `PlayerMovement.cs` - FPS движение
- ✅ `FirstPersonCamera.cs` - Камера
- ✅ `SimpleWeapon.cs` - Стрельба

---

## 🗺️ ШАГ 1: Создание карты (5 минут)

### 1.1 Создайте Ground (пол):
```
1. Hierarchy → 3D Object → Plane
2. Rename: "Ground"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (5, 1, 5)  // 50x50 метров
4. Layer: Ground (создайте если нет)
```

### 1.2 Создайте стены (опционально):
```
Wall_North:
1. Hierarchy → 3D Object → Cube
2. Position: (0, 2.5, 25)
3. Scale: (50, 5, 1)

Wall_South:
1. Position: (0, 2.5, -25)
2. Scale: (50, 5, 1)

Wall_East:
1. Position: (25, 2.5, 0)
2. Scale: (1, 5, 50)

Wall_West:
1. Position: (-25, 2.5, 0)
2. Scale: (1, 5, 50)
```

### 1.3 Освещение:
```
Directional Light должен быть в сцене (обычно есть по умолчанию)
Если нет:
1. Hierarchy → Light → Directional Light
2. Rotation: (50, -30, 0)
```

---

## 👤 ШАГ 2: Создание игрока (10 минут)

### 2.1 Создайте Player объект:
```
1. Hierarchy → Create Empty
2. Rename: "Player"
3. Position: (0, 1, 0)
```

### 2.2 Добавьте Character Controller:
```
Player:
1. Add Component → Character Controller
2. Settings:
   - Center: (0, 1, 0)
   - Radius: 0.5
   - Height: 2
   - Skin Width: 0.08
```

### 2.3 Добавьте PlayerMovement:
```
Player:
1. Add Component → Player Movement
2. Settings:
   - Walk Speed: 5
   - Sprint Speed: 8
   - Crouch Speed: 2.5
   - Jump Force: 5
   - Gravity: -15
   - Ground Check Distance: 0.2
   - Ground Mask: Ground (выберите layer)
```

### 2.4 Добавьте визуал (опционально):
```
Player → Create → 3D Object → Capsule
1. Rename: "PlayerModel"
2. Position: (0, 0, 0)  // Локальная
3. УДАЛИТЕ Capsule Collider компонент (важно!)
```

---

## 📷 ШАГ 3: Настройка камеры (5 минут)

### 3.1 Создайте CameraHolder:
```
Player → Create Empty
1. Rename: "CameraHolder"
2. Position: (0, 1.6, 0)  // Уровень глаз
```

### 3.2 Переместите Main Camera:
```
1. Перетащите Main Camera в CameraHolder (drag & drop)
2. Main Camera Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
```

### 3.3 Добавьте FirstPersonCamera:
```
Main Camera:
1. Add Component → First Person Camera
2. Settings:
   - Mouse Sensitivity: 2
   - Smooth Time: 0.1
   - Min Vertical Angle: -90
   - Max Vertical Angle: 90
   - Camera Holder: перетащите CameraHolder из Hierarchy
   - Player Body: перетащите Player из Hierarchy
```

---

## 🔫 ШАГ 4: Добавление оружия (5 минут)

### 4.1 Создайте Weapon объект:
```
Player/CameraHolder/Main Camera → Create Empty
1. Rename: "Weapon"
2. Position: (0.3, -0.2, 0.5)  // Справа внизу от камеры
```

### 4.2 Добавьте SimpleWeapon:
```
Weapon:
1. Add Component → Simple Weapon
2. Settings:
   - Damage: 20
   - Fire Rate: 0.1
   - Range: 100
   - Recoil Amount: 1
   - Recoil Pattern: (0.3, 0.8)
   - Player Camera: перетащите Main Camera
   - Hit Mask: Everything (или создайте маску)
```

### 4.3 Добавьте визуал оружия (опционально):
```
Weapon → Create → 3D Object → Cube
1. Rename: "WeaponModel"
2. Scale: (0.1, 0.1, 0.5)
3. Position: (0, 0, 0.25)
```

---

## ✅ ШАГ 5: Финальная проверка

### Структура должна быть:
```
Scene:
├── Ground (Plane)
├── Walls (4 Cubes - опционально)
├── Directional Light
└── Player
    ├── Character Controller
    ├── PlayerMovement
    ├── PlayerModel (Capsule - опционально)
    └── CameraHolder (Empty)
        └── Main Camera
            ├── FirstPersonCamera
            └── Weapon (Empty)
                ├── SimpleWeapon
                └── WeaponModel (Cube - опционально)
```

---

## 🎮 ТЕСТИРОВАНИЕ!

### Нажмите Play и проверьте:

#### Движение:
- ✅ **WASD** - ходьба
- ✅ **Shift** - бег (быстрее)
- ✅ **Space** - прыжок
- ✅ **Ctrl** - присед (ниже)

#### Камера:
- ✅ **Мышь** - вращение камеры
- ✅ **ESC** - разблокировать курсор

#### Стрельба:
- ✅ **ЛКМ (Mouse1)** - стрельба
- ✅ Отдача камеры
- ✅ Debug линии (красные = попал, жёлтые = промах)

---

## 🐛 Troubleshooting

### Персонаж не двигается:
- ✅ Проверьте что Character Controller добавлен
- ✅ Проверьте Ground Layer в PlayerMovement
- ✅ Проверьте что Ground имеет collider

### Камера не вращается:
- ✅ Проверьте что CameraHolder и PlayerBody назначены
- ✅ Проверьте что курсор заблокирован (должен исчезнуть при Play)

### Стрельба не работает:
- ✅ Проверьте что Player Camera назначена
- ✅ Проверьте Hit Mask (должен включать Default или другие layers)
- ✅ Смотрите в Console на Debug.Log сообщения

### Персонаж проваливается сквозь пол:
- ✅ У Ground должен быть Mesh Collider или Box Collider
- ✅ Ground Layer должен быть в Ground Mask в PlayerMovement

---

## 🎉 Готово!

Если всё работает - **переходим к Этапу 2: Mobile Touch Controls!**

**Напишите "Этап 1 работает" когда протестируете!** ✅

---

## 📸 Что должно получиться:

- Вы можете ходить по карте (WASD)
- Бегать (Shift)
- Прыгать (Space)
- Приседать (Ctrl)
- Вращать камеру (Мышь)
- Стрелять (ЛКМ)
- Видеть debug линии выстрелов

**Это базовый FPS контроллер! 🎮**

Следующий этап: Добавим touch controls для мобильных устройств!
