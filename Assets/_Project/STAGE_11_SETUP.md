# 📱 ЭТАП 11: MOBILE OPTIMIZATION (ОПТИМИЗАЦИЯ ДЛЯ МОБИЛЬНЫХ)

**Время:** 30-45 минут  
**Сложность:** ⭐⭐⭐  

**🎮 Unity Version:** 6.3 LTS (6000.3.0f1)  
**📦 Render Pipeline:** Universal Render Pipeline (URP) 17.3.0  

---

## ⚠️ ВАЖНО ДЛЯ UNITY 6.3 LTS:

```
В Unity 6 с URP настройки графики разделены:
✅ Quality Settings - базовые настройки (VSync, Anisotropic, Mipmap)
✅ URP Pipeline Asset - графические настройки (Shadows, MSAA, HDR, Post-Processing)

Оба нужно настроить!
```

---

## 🎯 ЧТО БУДЕМ ДЕЛАТЬ:

```
✅ Quality Settings - настройки графики (Low/Medium/High)
✅ Frame Rate Control - контроль FPS
✅ Object Pooling - уже реализован в VFX!
✅ Battery Optimization - экономия батареи
✅ UI Scaling - адаптация под разные экраны
✅ Performance Monitoring - отслеживание производительности
```

---

## 🔧 ШАГ 1: Настройте Quality Settings (15 минут)

**ВАЖНО:** Unity 6.3 LTS использует URP (Universal Render Pipeline)!  
Настройки разделены между Quality Settings и URP Pipeline Assets.

---

### 1.1 Откройте Quality Settings:

```
Unity Menu → Edit → Project Settings → Quality
```

### 1.2 Найдите URP Pipeline Assets:

```
Project → Assets → Settings → 
Ищите файлы типа "UniversalRP-*Quality"

ИЛИ

Project → поиск → "Pipeline Asset"
```

**Должны быть 3 URP Asset файла:**
- `UniversalRP-LowQuality` (или создайте!)
- `UniversalRP-MediumQuality`
- `UniversalRP-HighQuality`

---

### 1.3 Настройте Quality Levels:

Unity уже имеет уровни Low, Medium, High - настроим их!

#### 📱 LOW (для слабых устройств):

**В Quality Settings → Low:**

```
Quality Settings → Low:
├── Render Pipeline Asset: UniversalRP-LowQuality
├── VSync Count: Don't Sync ✓ (ВАЖНО!)
├── Anisotropic Textures: Disabled
├── Global Mipmap Limit: 1 (Half Resolution)
└── Realtime GI CPU Usage: Low
```

**В UniversalRP-LowQuality Asset → Inspector:**

```
General:
├── Depth Texture: ✗
├── Opaque Texture: ✗
├── HDR: ✗
└── MSAA: Disabled

Quality:
├── Main Light: Cast Shadows ✗ (выключить тени!)
├── Additional Lights: Disabled
├── Shadow Resolution: 512
├── Shadow Distance: 20
└── Shadow Cascade Count: 1

Rendering:
├── Render Scale: 0.75 (75%)
└── Upscaling Filter: Bilinear

Post-processing:
└── Post Processing: ✗
```

---

#### 📱 MEDIUM (для средних устройств):

**В Quality Settings → Medium:**

```
Quality Settings → Medium:
├── Render Pipeline Asset: UniversalRP-MediumQuality
├── VSync Count: Don't Sync ✓
├── Anisotropic Textures: Per Texture
├── Global Mipmap Limit: 0 (Full Resolution)
└── Realtime GI CPU Usage: Medium
```

**В UniversalRP-MediumQuality Asset → Inspector:**

```
General:
├── Depth Texture: ✗
├── Opaque Texture: ✗
├── HDR: ✓
└── MSAA: 2x

Quality:
├── Main Light: Cast Shadows ✓
├── Additional Lights: Per Pixel
├── Per Object Limit: 2
├── Shadow Resolution: 1024
├── Shadow Distance: 40
└── Shadow Cascade Count: 2

Rendering:
├── Render Scale: 1.0 (100%)
└── Upscaling Filter: Automatic

Post-processing:
└── Post Processing: ✓
```

---

#### 📱 HIGH (для мощных устройств):

**В Quality Settings → High:**

```
Quality Settings → High:
├── Render Pipeline Asset: UniversalRP-HighQuality
├── VSync Count: Don't Sync ✓
├── Anisotropic Textures: Per Texture
├── Global Mipmap Limit: 0 (Full Resolution)
└── Realtime GI CPU Usage: Unlimited
```

**В UniversalRP-HighQuality Asset → Inspector:**

```
General:
├── Depth Texture: ✓
├── Opaque Texture: ✗
├── HDR: ✓
└── MSAA: 4x

Quality:
├── Main Light: Cast Shadows ✓
├── Additional Lights: Per Pixel
├── Per Object Limit: 4
├── Shadow Resolution: 2048
├── Shadow Distance: 60
└── Shadow Cascade Count: 4

Rendering:
├── Render Scale: 1.0 (100%)
└── Upscaling Filter: FSR 2.0 (если доступно)

Post-processing:
└── Post Processing: ✓
```

---

### 1.4 Установите Default Quality для платформ:

```
Quality Settings → Default:

В списке Levels (вверху):
- Android: Medium (зелёная галочка)
- iOS: Medium (зелёная галочка)
- Standalone: High (зелёная галочка)
```

**Кликните на галочки под каждым уровнем для каждой платформы!**

---

### 1.5 Если URP Assets НЕ СУЩЕСТВУЮТ:

Создайте их:

```
1. Project → правый клик → Create → Rendering → 
   URP Asset (with Universal Renderer)

2. Переименуйте в:
   - UniversalRP-LowQuality
   - UniversalRP-MediumQuality
   - UniversalRP-HighQuality

3. Настройте каждый по инструкциям выше

4. Подключите к Quality Levels:
   Quality Settings → Low → Render Pipeline Asset: UniversalRP-LowQuality
   (повторите для Medium и High)
```

---

## 🎮 ШАГ 2: Создайте Quality Manager (10 минут)

Создайте скрипт для управления качеством графики:

### 2.1 Скрипт уже создан!

```
✅ Scripts/Managers/QualityManager.cs
```

Этот скрипт позволяет:
- Менять качество в рантайме
- Детектировать мощность устройства
- Автоматически устанавливать оптимальные настройки

### 2.2 Создайте GameObject:

```
Hierarchy → Create Empty → "QualityManager"

QualityManager:
1. Add Component → Quality Manager (Script)

Settings:
├── Auto Detect Quality: ✓
├── Target Frame Rate: 60
├── Battery Saver Mode: ✗
└── Default Quality: Medium
```

---

## ⚡ ШАГ 3: Frame Rate Control (5 минут)

### 3.1 Настройте Target Frame Rate:

Для мобильных важно контролировать FPS для экономии батареи:

```
Quality Manager (Script):
├── Target Frame Rate Mobile: 60
├── Target Frame Rate Standalone: 144
└── Enable VSync: ✗ (выключить для мобильных!)
```

### 3.2 Battery Saver Mode:

Опционально - режим экономии батареи:

```
Battery Saver Settings:
├── Reduced Frame Rate: 30 FPS
├── Lower Quality: Low
└── Disable Particles: ✗ (оставить включенными)
```

---

## 🖼️ ШАГ 4: Оптимизация текстур (5 минут)

### 4.1 Настройте импорт текстур:

Для всех текстур в проекте:

```
Project → Материалы → Выберите текстуру → Inspector:

Texture Import Settings:
├── Max Size: 1024 (для UI)
├── Max Size: 512 (для игровых объектов)
├── Compression: ASTC (для Android)
├── Compression: PVRTC (для iOS)
└── Generate Mip Maps: ✓
```

### 4.2 Batch настройка:

```
1. Выберите все текстуры в Project
2. Inspector → Texture Type: Sprite (2D and UI)
3. Max Size: 1024
4. Format: Compressed
5. Apply
```

---

## 📐 ШАГ 5: UI Scaling - Canvas Scaler (5 минут)

### 5.1 Настройте Canvas:

```
MobileUI (Canvas) → Canvas Scaler:

UI Scale Mode: Scale With Screen Size

Reference Resolution:
├── X: 1920
└── Y: 1080

Screen Match Mode: Match Width Or Height
└── Match: 0.5 (баланс между шириной и высотой)
```

### 5.2 Safe Area (для вырезов экрана):

Если хотите поддержку iPhone notch и т.д.:

```
MobileUI → Add Component → Safe Area Helper (опционально)
```

---

## 🔋 ШАГ 6: Battery Optimization (5 минут)

### 6.1 Отключите ненужные Update вызовы:

Проверьте скрипты - если объект неактивен, не вызывайте Update:

```csharp
// ПЛОХО:
void Update() {
    // Код выполняется всегда
}

// ХОРОШО:
void Update() {
    if (!isActiveAndEnabled) return;
    // Код выполняется только если активен
}
```

### 6.2 Оптимизация Audio:

```
AudioManager → Audio Settings:
├── Max Audio Sources: 10 (не больше!)
├── Virtual Voice Count: 10
└── Real Voice Count: 10
```

---

## 📊 ШАГ 7: Performance Monitor (ОПЦИОНАЛЬНО)

Создайте простой FPS счётчик для отладки:

### 7.1 Скрипт уже создан:

```
✅ Scripts/Debug/FPSCounter.cs
```

### 7.2 Добавьте на сцену:

```
Hierarchy → UI → Text → "FPSCounter"

Position: Top Left Corner

FPSCounter:
1. Add Component → FPS Counter (Script)

Settings:
├── Update Interval: 0.5
└── Show Only In Development: ✓
```

---

## 🎨 ШАГ 8: Particle System Optimization (5 минут)

### 8.1 Проверьте VFXManager:

```
VFXManager → Settings:
├── Use Object Pooling: ✓ (должно быть включено)
└── Pool Size: 20 (оптимально)
```

**Если уже настроено - пропустите этот шаг!**

### 8.2 Настройте каждый Particle System Prefab:

Откройте каждый префаб и настройте:

#### **HitSparkEffect:**

```
Project → Prefabs → HitSparkEffect → Inspector:

Particle System:
├── Max Particles: 20
├── Duration: 0.2
├── Start Lifetime: 0.3
└── Prewarm: ✗
```

#### **HitDustEffect:**

```
Project → Prefabs → HitDustEffect → Inspector:

Particle System:
├── Max Particles: 10
├── Duration: 0.5
├── Start Lifetime: 0.5
└── Prewarm: ✗
```

#### **MuzzleFlash:**

```
Project → Prefabs → MuzzleFlash → Inspector:

Если есть Particle System:
├── Max Particles: 5
├── Duration: 0.05
└── Prewarm: ✗
```

---

## 🚀 ШАГ 9: Build Settings для мобильных (Unity 6.3 LTS)

### 9.1 Android Settings:

```
Edit → Project Settings → Player → Android:

Other Settings (прокрутите вверх):
├── Target API Level: API Level 34 (Android 14) или выше
├── Minimum API Level: API Level 24 (Android 7.0) или выше
├── Scripting Backend: IL2CPP (рекомендуется!)
├── API Compatibility Level: .NET Standard 2.1
├── Target Architectures: ARM64 ✓ (обязательно!)
└── Write Permission: External (SD Card) - если нужно

Rendering (прокрутите вниз):
├── Color Space: Linear ✓ (лучше для графики)
├── Multithreaded Rendering: ✓
├── Static Batching: ✓
├── GPU Skinning: GPU (Batched)
└── Lightmap Encoding: Normal Quality

Graphics API (отдельная секция):
├── Auto Graphics API: ✗ (ВЫКЛЮЧИТЕ!)
├── Graphics APIs (настройте вручную):
│   1. Vulkan (переместите наверх - primary!)
│   2. OpenGLES3 (fallback)
└── Multithreaded Rendering: ✓
```

**ВАЖНО:** Default Quality Level устанавливается в **Quality Settings**, а не здесь!

```
Edit → Project Settings → Quality → Таблица с галочками
Установите зелёную галочку на Medium для Android
```

### 9.2 iOS Settings (если делаете):

```
Edit → Project Settings → Player → iOS:

Other Settings (прокрутите вверх):
├── Target Minimum iOS Version: 13.0 или выше
├── Scripting Backend: IL2CPP
├── API Compatibility Level: .NET Standard 2.1
├── Architecture: ARM64
└── Camera Usage Description: "For game features"

Rendering (прокрутите вниз):
├── Color Space: Linear ✓ (должно быть)
├── MSAA Fallback: Downgrade
├── Multithreaded Rendering: ✓
├── Static Batching: ✓
├── GPU Skinning: GPU (Batched)
├── Texture compression format: ASTC
└── Lightmap Encoding: Low Quality (можно оставить)

Graphics API (отдельная секция):
├── Auto Graphics API: ✓ (оставьте включенным)
└── Metal: автоматически (iOS использует только Metal)
```

**ВАЖНО:** Default Quality Level устанавливается в **Quality Settings**, а не здесь!

```
Edit → Project Settings → Quality → Таблица с галочками
Установите зелёную галочку на Medium для iOS
```

### 9.3 Universal (для всех платформ):

```
Edit → Project Settings → Graphics:

Rendering:
├── Scriptable Render Pipeline Settings: UniversalRP-MediumQuality
├── Transparency Sort Mode: Default
└── Camera-Relative Culling: ✓

Shader Stripping:
├── Lightmap Modes: Automatic
└── Instancing Variants: Strip All (оптимизация)
```

---

## ✅ ФИНАЛЬНАЯ СТРУКТУРА:

```
Scene:
├── Player
├── Bot
├── AudioManager
├── VFXManager
├── QualityManager ← НОВОЕ!
├── TeamManager
├── MatchManager
├── GameHUDManager
└── MobileUI (Canvas)
    ├── FPSCounter (опционально) ← НОВОЕ!
    └── GameHUD
```

---

## 🎮 ТЕСТИРОВАНИЕ (Unity 6.3 LTS):

### На компьютере (Editor):

1. **Нажмите Play**
2. **Проверьте FPS** (должно быть 60+ на компьютере)
3. **Смените Quality Level:**
   - В Editor: Game View → Stats → Quality (выпадающее меню)
   - ИЛИ Edit → Project Settings → Quality → Current Active Quality Level
4. **Проверьте каждый уровень:**
   - Low: тени выключены, FPS высокий
   - Medium: тени включены, FPS средний
   - High: всё включено, FPS хороший
5. **Проверьте в URP Debugger:**
   - Window → Analysis → Rendering Debugger
   - Смотрите Frame Stats, Overdraw, Mipmaps

### На мобильном устройстве:

1. **Build and Run** на Android/iOS
2. **Проверьте FPS:**
   - Должно быть 30-60 на слабых устройствах
   - 60 на средних/мощных
3. **Проверьте нагрев:**
   - Телефон не должен сильно греться
   - Если греется - используйте Quality: Low или Battery Saver
4. **Проверьте батарею:**
   - Играйте 10 минут
   - Батарея не должна садиться > 10% за 10 минут
5. **Проверьте разные экраны:**
   - Разные разрешения (720p, 1080p, 1440p)
   - Разные соотношения (16:9, 18:9, 19.5:9, notch)
6. **Unity Remote (опционально):**
   - Для быстрого тестирования без билда
   - Window → General → Device Simulator

---

## 🐛 ВОЗМОЖНЫЕ ПРОБЛЕМЫ:

### Низкий FPS на мобильном:

```
Решения:
- Уменьшите Shadow Distance до 20
- Отключите Anti-Aliasing
- Уменьшите Max Particles в VFX
- Установите Target Frame Rate: 30
- Используйте Quality Level: Low
```

### Игра греет телефон:

```
Решения:
- Установите Target Frame Rate: 30
- Включите Battery Saver Mode
- Уменьшите Pixel Light Count до 0
- Отключите Shadows полностью
```

### UI не подходит под экран:

```
Решения:
- Canvas Scaler → Reference Resolution: 1920x1080
- Match: 0.5 (баланс)
- Добавьте Safe Area Helper для notch
```

### Долгая загрузка:

```
Решения:
- Сожмите текстуры (Max Size: 512)
- Используйте ASTC compression
- Отключите Mip Maps для UI
```

---

## 📋 CHECKLIST:

```
✅ Quality Settings настроены (Low/Medium/High)
✅ QualityManager создан
✅ Target Frame Rate установлен (60 для PC, 30-60 для мобильных)
✅ Текстуры сжаты (ASTC/PVRTC)
✅ Canvas Scaler настроен
✅ Particle Systems оптимизированы (Max Particles)
✅ VSync выключен
✅ Build Settings настроены (IL2CPP, ARM64)
✅ FPS Counter добавлен (опционально)
✅ Протестировано на разных Quality уровнях
```

---

## 🎯 РЕЗУЛЬТАТ:

```
✅ Игра работает на слабых устройствах
✅ FPS стабильный (30-60)
✅ Батарея не садится быстро
✅ UI адаптируется под разные экраны
✅ Быстрая загрузка
✅ Нет перегрева устройства
```

---

## 💡 ДОПОЛНИТЕЛЬНАЯ ОПТИМИЗАЦИЯ (если нужно):

### Occlusion Culling:

```
Window → Rendering → Occlusion Culling
- Bake occlusion data
- Включите на Camera
```

### LOD Groups:

```
Для моделей ботов:
- LOD0: 100% (близко)
- LOD1: 50% (средне)
- LOD2: 25% (далеко)
```

### Lightmapping:

```
Window → Rendering → Lighting
- Generate Lighting
- Use Baked GI for static objects
```

---

## 📊 СТАТИСТИКА ЭТАПА 11:

```
Новые скрипты: 2 (QualityManager, FPSCounter)
Настроенные системы: Quality Settings, Build Settings, UI Scaling
Оптимизация: Текстуры, Particles, Audio, Frame Rate
Время настройки: 30-45 минут
```

---

## ➡️ СЛЕДУЮЩИЙ ЭТАП:

**ЭТАП 12: POLISHING & FINAL TESTING** 🎨

```
✅ UI/UX улучшения
✅ Звуковые эффекты (если есть)
✅ Баланс геймплея
✅ Финальное тестирование
✅ Подготовка к релизу
```

---

**ВАЖНО:** После оптимизации обязательно протестируйте на **реальном устройстве**! 📱

**Когда завершите настройку - напишите "Этап 11 завершён"!** ✅

---

## 🔍 СКРИПТЫ, КОТОРЫЕ УЖЕ СОЗДАНЫ:

```
✅ QualityManager.cs - работает с Unity 6.3 LTS
✅ FPSCounter.cs - счётчик FPS (опционально)
```

**Скрипты полностью совместимы с Unity 6 и URP!** 🚀

---

## 📝 ПОЛЕЗНЫЕ ССЫЛКИ:

- Unity 6 Documentation: https://docs.unity3d.com/6000.3/Documentation/Manual/
- URP Manual: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.3/manual/
- Mobile Optimization: https://docs.unity3d.com/Manual/MobileOptimization.html
