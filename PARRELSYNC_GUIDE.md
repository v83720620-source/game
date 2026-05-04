# 🎮 PARRELSYNC: ТЕСТИРОВАНИЕ MULTIPLAYER

**Что это:** Инструмент для создания клонов Unity Editor  
**Зачем:** Тестировать multiplayer на одном компьютере  
**Время установки:** 5 минут  

---

## 📦 ШАГ 1: УСТАНОВКА (2 МИНУТЫ)

### 1.1 Откройте Package Manager

```
Unity → Window → Package Manager
```

### 1.2 Добавьте пакет через Git URL

```
1. В левом верхнем углу Package Manager нажмите: + (плюс)
2. Выберите: "Add package from git URL..."
3. Вставьте URL:
   https://github.com/VeriorPies/ParrelSync.git?path=/ParrelSync
4. Нажмите "Add"
5. Дождитесь установки (~1-2 минуты)
```

### 1.3 Проверка установки

```
✅ В Package Manager появился "ParrelSync"
✅ В Menu Bar появился: Window → ParrelSync
✅ Консоль без ошибок
```

---

## 🔧 ШАГ 2: СОЗДАНИЕ КЛОНА (3 МИНУТЫ)

### 2.1 Откройте Clones Manager

```
Window → ParrelSync → Clones Manager
```

### 2.2 Создайте клон

```
1. В окне Clones Manager нажмите:
   "Create new clone"

2. Выберите папку для клона:
   Рекомендую: c:\UnityProjects\Flump\FlumpGame_Clone1

3. Дождитесь создания (~2-3 минуты)
   Unity создаёт symbolic links на большинство файлов
```

### 2.3 Откройте клон

```
1. В Clones Manager появится кнопка "Open in New Editor"
2. Нажмите на неё
3. Откроется второй Unity Editor с клоном проекта
```

---

## 🎮 ШАГ 3: НАСТРОЙКА ДЛЯ ТЕСТА (2 МИНУТЫ)

### 3.1 В ОСНОВНОМ Editor (Host)

```
Откройте сцену Game
NetworkManager → Inspector → Custom Network Manager:

Debug:
  ☑ Start Host On Awake: ✅
  □ Start Client On Awake: ❌
```

### 3.2 В КЛОНЕ Editor (Client)

```
Откройте сцену Game
NetworkManager → Inspector → Custom Network Manager:

Debug:
  □ Start Host On Awake: ❌
  ☑ Start Client On Awake: ✅
```

---

## 🎯 ШАГ 4: ТЕСТИРОВАНИЕ! (МОМЕНТ ИСТИНЫ)

### 4.1 Запуск

```
1. ОСНОВНОЙ Editor → Play ▶ (Host стартует первым!)
2. Дождитесь сообщения в Console:
   "[NetworkManager] ✅ Server started successfully!"

3. КЛОН Editor → Play ▶ (Client подключается)
```

### 4.2 Что должно произойти

**В ОСНОВНОМ Editor (Host):**
```
Console:
✅ [NetworkManager] Starting host...
✅ [NetworkManager] ✅ Server started successfully!
✅ [NetworkManager] ✅ Client connected: 0 (вы)
✅ [NetworkPlayer] Spawned! IsOwner: True, ClientId: 0
✅ [NetworkManager] ✅ Client connected: 1 (клон!)
✅ [NetworkPlayer] Spawned! IsOwner: False, ClientId: 1 (второй игрок!)

Game View:
🎮 Вы управляете своим Player
👤 Видите второго игрока (синхронизируется)
```

**В КЛОНЕ Editor (Client):**
```
Console:
✅ [NetworkManager] Starting client...
✅ [NetworkManager] Connected to server!
✅ [NetworkPlayer] Spawned! IsOwner: True, ClientId: 1
✅ [NetworkPlayer] Setting up LOCAL player

Game View:
🎮 Вы управляете своим Player
👤 Видите первого игрока (Host)
```

---

## ✅ ЧТО ТЕСТИРОВАТЬ:

### Тест 1: Movement Sync
```
Двигайтесь в одном Editor
→ В другом Editor должен двигаться второй игрок

✅ Движение синхронизируется
✅ Rotation синхронизируется
✅ Нет телепортов
✅ Плавное движение
```

### Тест 2: Combat (пока может не работать полностью)
```
Стреляйте в другого игрока
→ Проверьте Console на сообщения о попаданиях

✅ Выстрелы регистрируются
✅ Hit detection работает
```

---

## 🐛 TROUBLESHOOTING

### Client не подключается
```
Проблема: "Connection failed" в клоне

Решение:
1. Проверьте что Host запущен ПЕРВЫМ
2. NetworkManager → Unity Transport:
   Address: 127.0.0.1
   Port: 7777
3. Firewall не блокирует Unity
```

### Два игрока spawned в одном месте
```
Это нормально для первого теста!
В Этапе 15 добавим правильную spawn систему.
```

### Видно только одного игрока
```
Решение:
1. Проверьте что оба Editor в Play mode
2. Проверьте Console обоих - должно быть "Spawned!"
3. Попробуйте подвигаться - может игроки в разных местах
```

### Лаги / телепорты
```
Это нормально для локального теста
NetworkTransform нужно оптимизировать (будет позже)
```

---

## 📊 ОЖИДАЕМЫЙ РЕЗУЛЬТАТ:

### В Host Editor:
```
🎮 Ваш игрок (управление работает)
👤 Второй игрок (двигается когда Client двигается)
📊 Счёт: 0 vs 0
💯 HP: 100
```

### В Client Editor:
```
🎮 Ваш игрок (управление работает)
👤 Первый игрок (двигается когда Host двигается)
📊 Счёт: 0 vs 0
💯 HP: 100
```

---

## 🎉 ЕСЛИ ВСЁ РАБОТАЕТ:

### ПОЗДРАВЛЯЮ! ВЫ СДЕЛАЛИ MULTIPLAYER! 🎊

**Это огромное достижение!**

Теперь у вас:
- ✅ Работающий multiplayer
- ✅ Синхронизация игроков
- ✅ Network infrastructure
- ✅ Server-authoritative система

---

## 💾 СОХРАНИМ ПРОГРЕСС В GIT?

Хотите сохранить текущее состояние:
```
git add .
git commit -m "[Feature] Stage 14 Day 1-2 Complete: Player Networking Basic Setup"
```

---

## 🚀 СЛЕДУЮЩИЕ ШАГИ:

### День 3-4: Movement Optimization
- Оптимизация bandwidth
- Улучшение синхронизации
- Тестирование с задержкой (lag simulation)

### День 5-6: Combat Networking
- Полная реализация стрельбы
- Урон по сети
- Kill feed по сети
- Respawn по сети

---

**Запускайте ParrelSync и покажите скриншот с двумя игроками!** 🎮✨

Или нужна помощь с установкой ParrelSync?