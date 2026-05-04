# 🎯 ФИНАЛЬНЫЕ ИНСТРУКЦИИ: Завершение Android Билда

**Дата:** 30 января 2026  
**Статус:** Готов к финальному билду  
**Время выполнения:** 20-40 минут  

---

## 📋 ВАШ ТЕКУЩИЙ СТАТУС:

```
✅ 11 этапов завершены (91.7%)
✅ Все скрипты написаны и исправлены
✅ Old Input закомментирован
✅ Все системы работают
🟡 Остановились на Android билде
```

---

## 🎯 ФИНАЛЬНЫЙ ПЛАН ДЕЙСТВИЙ (4 ШАГА)

---

## ШАГ 1: Измените Active Input Handling (5 минут)

### 1.1 Откройте Player Settings:
```
Unity Editor:
1. Edit → Project Settings
2. Кликните "Player" (левая панель)
3. Найдите иконку Android (вкладка вверху)
4. Кликните на Android
```

### 1.2 Найдите Active Input Handling:
```
Прокрутите вниз до:
→ Other Settings (раскройте если свёрнуто)
→ Configuration
→ Active Input Handling

Сейчас там: "Both"
```

### 1.3 Измените настройку:
```
Active Input Handling:
"Both" → Измените на → "Input System Package (New)"

Нажмите: Apply (если есть кнопка)
```

### 1.4 Перезапустите Unity:
```
Unity покажет диалог:
"You must restart Unity for this change to take effect"

Нажмите: "Yes" или "Restart"
Дождитесь перезапуска (1-2 минуты)
```

**✅ РЕЗУЛЬТАТ:** Input System настроен правильно для Android!

---

## ШАГ 2: Выберите Scripting Backend (5 минут)

### У вас 2 ВАРИАНТА:

### ВАРИАНТ A: MONO (БЫСТРО) - Рекомендуется для теста!

**Преимущества:**
- ✅ Работает БЕЗ проблем CMake
- ✅ Быстрая сборка (5-10 минут)
- ✅ Можно сразу протестировать

**Недостатки:**
- ⚠️ Только для тестирования
- ⚠️ Не для финального релиза в Google Play

**Как настроить:**
```
1. Edit → Project Settings → Player → Android
2. Other Settings → Configuration
3. Scripting Backend: IL2CPP
4. Измените на: "Mono"
5. Закройте Project Settings
```

---

### ВАРИАНТ B: IL2CPP (ПРАВИЛЬНО) - Для финального релиза

**Преимущества:**
- ✅ Требуется для Google Play
- ✅ ARM64 поддержка
- ✅ Лучшая производительность

**Недостатки:**
- ⚠️ Долгая сборка (15-30 минут)
- ⚠️ Может быть ошибка CMake

**Как настроить:**
```
1. Edit → Project Settings → Player → Android
2. Other Settings → Configuration
3. Scripting Backend: IL2CPP (оставьте как есть)
4. Target Architectures: ARM64 ✓
5. Закройте Project Settings
```

**Если ошибка CMake:**
```
1. Закройте Unity
2. Найдите: c:\UnityProjects\Flump\DELETE_LIBRARY.bat
3. Запустите от администратора (правый клик → Run as Administrator)
4. Дождитесь удаления Library (2-5 минут)
5. Откройте проект через Unity Hub
6. Дождитесь импорта (5-10 минут)
7. Попробуйте билд снова
```

---

## ШАГ 3: Создайте APK билд (10-30 минут)

### 3.1 Откройте Build Profiles:
```
Unity Editor:
File → Build Profiles
(или Window → General → Build Profiles)
```

### 3.2 Выберите Android:
```
Build Profiles → Platforms (левая панель):
1. Найдите "Android"
2. ДВОЙНОЙ КЛИК на "Android"
3. Дождитесь активации (если не активен)
4. Проверьте зелёную метку "Active"
```

### 3.3 Настройте Platform Settings:
```
Build Profiles → Platform Settings (правая панель):

Texture Compression: ASTC ✓
Export Project: ✗
Symlink Sources: ✗
Build App Bundle (Google Play): ✗ ← ВАЖНО! (для APK)
Development Build: ✓ (для теста можно включить)
Script Debugging: ✗
```

### 3.4 Проверьте сцену:
```
Build Profiles → Scene List:
✓ Scenes/MainScene (должна быть добавлена)

Если нет:
1. Откройте MainScene (двойной клик в Project)
2. Build Profiles → "+ Add Open Scenes"
```

### 3.5 Создайте билд:
```
Build Profiles → "Build" (кнопка внизу)

В диалоге:
1. Выберите папку: C:\UnityProjects\Flump\Builds\Android\
   (создайте если нет)
2. Имя файла: FlumpGame_TEST.apk
3. Нажмите "Save"
4. Дождитесь завершения

Прогресс показывается внизу справа Unity!
```

### 3.6 Время ожидания:
```
С MONO:
└── 5-10 минут (первый раз)
└── 3-5 минут (последующие)

С IL2CPP:
└── 15-30 минут (первый раз!)
└── 10-20 минут (последующие)

⏳ Не закрывайте Unity во время билда!
```

### 3.7 Проверка успешности:
```
Успешный билд:
✅ Console: "Build completed successfully"
✅ Файл создан: Builds/Android/FlumpGame_TEST.apk
✅ Размер: ~60-150 MB

Ошибка:
❌ Console: красные ошибки
❌ Диалог "Build failure"
→ Читайте ошибку и исправляйте
→ Или попробуйте Mono вместо IL2CPP
```

---

## ШАГ 4: Установите на Android устройство (5-10 минут)

### ВАРИАНТ A: Build And Run (автоматически)

**Подготовка:**
```
1. Подключите Android телефон через USB
2. На телефоне:
   - Settings → About Phone
   - Нажмите "Build Number" 7 раз (Developer mode)
   - Settings → Developer Options
   - Включите "USB Debugging"
3. Подтвердите на телефоне USB debugging
```

**Билд:**
```
Build Profiles → "Build And Run" (вместо "Build")
Unity:
1. Создаст APK
2. Установит на телефон автоматически
3. Запустит игру
```

---

### ВАРИАНТ B: Ручная установка

**Шаг 1: Скопируйте APK на телефон**
```
1. Подключите телефон к PC (USB или WiFi)
2. Откройте: C:\UnityProjects\Flump\Builds\Android\
3. Найдите: FlumpGame_TEST.apk
4. Скопируйте на телефон (в любую папку)
```

**Шаг 2: Разрешите установку**
```
На телефоне:
1. Settings → Security (или Apps)
2. "Install unknown apps" или "Unknown sources"
3. Найдите ваш File Manager (например, Files, Downloads)
4. Разрешите установку из этого источника
```

**Шаг 3: Установите APK**
```
1. Откройте File Manager на телефоне
2. Найдите FlumpGame_TEST.apk
3. Нажмите на файл
4. Нажмите "Install"
5. Дождитесь установки
6. Нажмите "Open"
```

---

## ШАГ 5: Протестируйте игру (10-20 минут)

### Что тестировать:

**Загрузка:**
```
✓ Игра запускается?
✓ Загрузка < 10 секунд?
✓ Нет crash на старте?
```

**Управление:**
```
✓ Joystick работает? (движение персонажа)
✓ Fire кнопка работает? (стрельба)
✓ Reload кнопка работает? (перезарядка)
✓ Jump кнопка работает? (прыжок)
✓ Run кнопка работает? (бег)
✓ Crouch кнопка работает? (присед)
✓ Camera rotation работает? (свайп по экрану)
```

**UI:**
```
✓ Health Bar видно и обновляется?
✓ Ammo Display показывает патроны?
✓ Crosshair виден?
✓ Match Timer работает?
✓ Team Scores обновляются?
✓ Kill Feed показывает убийства?
```

**Gameplay:**
```
✓ Боты двигаются и атакуют?
✓ Попадания регистрируются?
✓ Урон работает?
✓ Смерть и респавн работают?
✓ Match заканчивается правильно?
✓ Victory/Defeat экран показывается?
```

**Performance:**
```
✓ FPS стабильный? (30-60)
✓ Нет лагов?
✓ Нет фризов?
✓ Телефон не греется сильно?
✓ Батарея садится умеренно?
```

---

## 🐛 TROUBLESHOOTING

### Проблема: "Build failed" с CMake ошибкой

**Решение 1: Попробуйте Mono**
```
Player Settings → Scripting Backend → Mono
Попробуйте билд снова
```

**Решение 2: Очистите Library**
```
1. Запустите DELETE_LIBRARY.bat от администратора
2. Переоткройте проект
3. Попробуйте билд
```

---

### Проблема: "Unsupported Input Handling" диалог

**Решение: Нажмите "No" и измените настройки**
```
1. Нажмите "No" в диалоге
2. Edit → Project Settings → Player → Android
3. Active Input Handling → "Input System Package (New)"
4. Apply и перезапустите Unity
5. Попробуйте билд снова
```

---

### Проблема: APK не устанавливается на телефон

**Решение 1: Проверьте Unknown Sources**
```
Settings → Security → Install unknown apps
Разрешите для File Manager
```

**Решение 2: Проверьте API Level**
```
Player Settings → Minimum API Level
Должен быть не выше чем Android версия на телефоне

Рекомендуется: API Level 24 (Android 7.0)
```

---

### Проблема: Игра вылетает на старте

**Решение: Проверьте logcat**
```
Windows:
1. Подключите телефон через USB
2. Откройте cmd
3. cd C:\Program Files\Unity\Hub\Editor\6000.3.3f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools
4. adb logcat | findstr Unity

Читайте ошибки и исправляйте
```

---

### Проблема: Управление не работает

**Проверьте:**
```
1. Active Input Handling = "Input System Package (New)"?
2. Old Input код закомментирован?
3. MobileInputManager включён в сцене?
4. EventSystem есть в сцене?
```

---

## 📊 ПОСЛЕ ТЕСТИРОВАНИЯ

### Если ВСЁ РАБОТАЕТ ✅

**Поздравляю! Игра готова!** 🎉

**Следующие шаги:**
```
1. Собрать feedback от друзей/тестеров
2. Исправить найденные баги
3. Добавить звуки (опционально)
4. Финальный polish
5. Создать AAB для Google Play:
   - Настроить Keystore
   - Build App Bundle: ✓
   - Загрузить в Google Play Console
```

---

### Если ЕСТЬ ПРОБЛЕМЫ ❌

**Сообщите мне:**
```
1. Какая именно проблема?
2. На каком шаге возникла?
3. Текст ошибки (screenshot)
4. Что уже пробовали?

Я помогу исправить!
```

---

## 📋 КРАТКИЙ ЧЕКЛИСТ

```
□ Шаг 1: Active Input Handling → "Input System Package (New)"
□ Шаг 1: Unity перезапущен
□ Шаг 2: Scripting Backend выбран (Mono или IL2CPP)
□ Шаг 3: Build Profiles → Android активен
□ Шаг 3: Platform Settings настроены
□ Шаг 3: Build App Bundle = ✗ (для APK)
□ Шаг 3: "Build" нажата
□ Шаг 3: APK создан успешно
□ Шаг 4: APK установлен на телефон
□ Шаг 5: Игра протестирована
□ Шаг 5: Всё работает!
```

---

## 🎯 ИТОГО

**Время выполнения:**
- Быстрый путь (Mono): 20-30 минут
- Полный путь (IL2CPP): 40-60 минут

**Результат:**
- ✅ Рабочий APK файл
- ✅ Протестированная игра на телефоне
- ✅ Готовность к финальному polish

---

## 📞 ЕСЛИ НУЖНА ПОМОЩЬ

**Покажите:**
1. Screenshot ошибки (если есть)
2. На каком шаге застряли
3. Что уже пробовали

**Я помогу решить проблему!** 🔧

---

**УДАЧИ С БИЛДОМ! ВЫ ПОЧТИ У ЦЕЛИ!** 🚀

**Дата:** 30 января 2026  
**Следующий шаг:** Шаг 1 - Измените Active Input Handling
