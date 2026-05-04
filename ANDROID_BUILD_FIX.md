# 🔧 ИСПРАВЛЕНИЕ: Android Build Failed (CMake/IL2CPP Error)

## Ошибка:
```
[CXX1429] error when building with cmake
C++ build system [prefab] failed while executing
```

---

## ✅ РЕШЕНИЕ 1: Установите Android Build Support (ГЛАВНОЕ!)

```
1. Откройте Unity Hub
2. Installs → Unity 6.3.0f1 → ⚙️ (Settings icon)
3. "Add Modules"
4. Отметьте ВСЕ галочки:
   ☑ Android Build Support
   ☑ Android SDK & NDK Tools
   ☑ OpenJDK
   ☑ Android Logcat (опционально)
5. Нажмите "Install" / "Continue"
6. Дождитесь установки (5-15 минут, ~3-5 GB)
7. Перезапустите Unity Editor
```

**ВАЖНО:** Без Android SDK & NDK Tools IL2CPP билд НЕВОЗМОЖЕН!

---

## ✅ РЕШЕНИЕ 2: Проверьте External Tools

После установки модулей:

```
Unity Editor:
1. Edit → Preferences
2. External Tools (левая панель)
3. Проверьте пути:

Android:
├── JDK: [должен быть путь к OpenJDK]
├── Android SDK: [должен быть путь к SDK]
├── Android NDK: [должен быть путь к NDK]
└── Gradle: [должен быть путь к Gradle]

Если пути пустые или красные:
→ Нажмите кнопки справа для автоматического поиска
→ Или установите модули через Unity Hub (см. выше)
```

---

## ✅ РЕШЕНИЕ 3: Очистите кэш (если уже установлено всё)

```
1. Закройте Unity
2. Удалите папки:
   - C:\UnityProjects\Flump\FlumpGame\Library\
   - C:\UnityProjects\Flump\FlumpGame\Temp\
3. Откройте проект снова
4. Дождитесь импорта (5-10 минут)
5. Попробуйте билд снова
```

---

## ✅ РЕШЕНИЕ 4: Проверьте Player Settings

```
Build Profiles → Player Settings → Android:

Other Settings:
├── Scripting Backend: IL2CPP ✓
├── API Compatibility: .NET Standard 2.1 ✓
├── Target Architectures: ARM64 ✓
└── Strip Engine Code: ✓

Если что-то не так - исправьте!
```

---

## ⚠️ ВРЕМЕННОЕ РЕШЕНИЕ: Используйте Mono вместо IL2CPP

**Только для тестирования! Google Play требует IL2CPP + ARM64!**

```
Build Profiles → Player Settings → Android:
1. Other Settings → Configuration
2. Scripting Backend: IL2CPP → измените на "Mono"
3. Попробуйте билд

Это создаст APK для теста, но НЕ подходит для Google Play!
```

---

## 📋 ПОРЯДОК ДЕЙСТВИЙ:

```
1. ✅ Установите Android Build Support через Unity Hub
2. ✅ Проверьте External Tools пути
3. ✅ Перезапустите Unity
4. ✅ Попробуйте билд снова
5. ❌ Если не помогло - очистите Library/Temp
6. ❌ Если всё равно ошибка - попробуйте Mono (только для теста)
```

---

## 🎯 ПОСЛЕ ИСПРАВЛЕНИЯ:

```
Android билд должен работать!
Время сборки: 15-30 минут (первый раз)
```
