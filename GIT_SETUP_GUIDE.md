# 📦 ПОШАГОВАЯ ИНСТРУКЦИЯ: Сохранение Проекта в Git/GitHub

**Дата:** 30 января 2026  
**Проект:** Flump Game  
**Время выполнения:** 20-30 минут (первый раз)  

---

## 🎯 ЧТО ПОЛУЧИТЕ

После выполнения этих шагов:
- ✅ Полный backup проекта на GitHub
- ✅ История всех изменений
- ✅ Защита от потери данных
- ✅ Возможность работать с разных компьютеров

---

## 📋 ШАГ 1: Установка Git (5 минут)

### 1.1 Скачайте Git:
```
Ссылка: https://git-scm.com/download/win

1. Откройте ссылку в браузере
2. Скачается автоматически: Git-2.xx.x-64-bit.exe
3. Дождитесь завершения загрузки
```

### 1.2 Установите Git:
```
1. Откройте скачанный файл Git-2.xx.x-64-bit.exe
2. Нажмите "Next"
3. Оставьте всё по умолчанию (просто Next → Next → Next)
4. Нажмите "Install"
5. Дождитесь установки (1-2 минуты)
6. Нажмите "Finish"
```

### 1.3 Проверьте установку:
```
1. Нажмите Win + R
2. Введите: cmd
3. Нажмите Enter
4. В командной строке введите: git --version
5. Нажмите Enter

Должно показать:
git version 2.xx.x

Если показало - Git установлен! ✅
Если ошибка - перезагрузите компьютер и попробуйте снова
```

---

## 📋 ШАГ 2: Настройка Git (2 минуты)

### 2.1 Откройте PowerShell в папке проекта:
```
1. Откройте проводник Windows
2. Перейдите в: C:\UnityProjects\Flump\FlumpGame
3. В адресной строке введите: powershell
4. Нажмите Enter
5. Откроется PowerShell в этой папке
```

### 2.2 Настройте имя:
```powershell
git config --global user.name "Ваше Имя"
```
**Замените "Ваше Имя" на своё имя или nickname**

Например:
```powershell
git config --global user.name "Alex Developer"
```

### 2.3 Настройте email:
```powershell
git config --global user.email "your.email@example.com"
```
**Замените на свой email**

Например:
```powershell
git config --global user.email "alex@example.com"
```

### 2.4 Проверьте настройки:
```powershell
git config --global --list
```

Должно показать:
```
user.name=Ваше Имя
user.email=your.email@example.com
```

✅ Если показало - настройка завершена!

---

## 📋 ШАГ 3: Инициализация Git (1 минута)

### 3.1 Убедитесь что находитесь в папке проекта:
```powershell
pwd
```
Должно показать: `C:\UnityProjects\Flump\FlumpGame`

Если нет - перейдите:
```powershell
cd C:\UnityProjects\Flump\FlumpGame
```

### 3.2 Создайте Git репозиторий:
```powershell
git init
```

Должно показать:
```
Initialized empty Git repository in C:/UnityProjects/Flump/FlumpGame/.git/
```

✅ Репозиторий создан!

---

## 📋 ШАГ 4: Первый Commit (5 минут)

### 4.1 Проверьте что будет добавлено:
```powershell
git status
```

Покажет МНОГО файлов красным цветом - это нормально!

### 4.2 Добавьте ВСЕ файлы:
```powershell
git add .
```

**Точка в конце важна!**

Эта команда добавит все файлы (кроме .gitignore)

⏳ Подождите 10-20 секунд

### 4.3 Проверьте что добавилось:
```powershell
git status
```

Теперь файлы должны быть зелёными - готовы к commit!

### 4.4 Создайте первый commit:
```powershell
git commit -m "Initial commit: Flump Game - 11 stages completed, single-player ready"
```

⏳ **ВАЖНО: Подождите 1-3 минуты!**
Первый commit большой, это нормально!

Должно показать что-то типа:
```
[master (root-commit) abc1234] Initial commit: Flump Game - 11 stages completed
 XXX files changed, XXXXX insertions(+)
 create mode 100644 Assets/...
 ...
```

✅ Первый commit создан!

---

## 📋 ШАГ 5: Регистрация на GitHub (5 минут)

**Если у вас уже есть аккаунт - пропустите этот шаг!**

### 5.1 Откройте GitHub:
```
https://github.com
```

### 5.2 Зарегистрируйтесь:
```
1. Нажмите "Sign up" (вверху справа)
2. Введите email
3. Создайте пароль
4. Придумайте username (например: alexgamedev)
5. Нажмите "Continue"
6. Пройдите проверку (solve puzzle)
7. Подтвердите email (зайдите в почту и кликните ссылку)
```

### 5.3 Войдите в аккаунт:
```
1. Откройте https://github.com
2. Нажмите "Sign in"
3. Введите email и пароль
```

✅ Аккаунт готов!

---

## 📋 ШАГ 6: Создание Репозитория на GitHub (3 минуты)

### 6.1 Создайте новый репозиторий:
```
1. На GitHub нажмите "+" (вверху справа)
2. Выберите "New repository"
```

### 6.2 Заполните форму:

**Repository name:**
```
flump-game
```
(или любое другое имя, без пробелов!)

**Description (опционально):**
```
5v5 Mobile Shooter Game for iOS & Android - Unity 6.3 LTS
```

**Public или Private:**
```
Выберите: Private (рекомендую!)
```
*Private = только вы видите код*
*Public = все видят код*

**НЕ СТАВЬТЕ галочки:**
```
❌ Add a README file (у нас уже есть)
❌ Add .gitignore (у нас уже есть)
❌ Choose a license (пока не нужно)
```

### 6.3 Нажмите:
```
"Create repository" (зелёная кнопка)
```

✅ Репозиторий создан на GitHub!

---

## 📋 ШАГ 7: Подключение к GitHub (5 минут)

### 7.1 Скопируйте URL репозитория:

На странице вашего нового репозитория найдите:
```
Quick setup — if you've done this kind of thing before
```

Под этим текстом будет URL вида:
```
https://github.com/ВАШ_USERNAME/flump-game.git
```

**Скопируйте этот URL!** (Ctrl+C)

### 7.2 Добавьте remote в PowerShell:
```powershell
git remote add origin https://github.com/ВАШ_USERNAME/flump-game.git
```

**Замените URL на ваш скопированный!**

Например:
```powershell
git remote add origin https://github.com/alexgamedev/flump-game.git
```

Нажмите Enter.

### 7.3 Проверьте remote:
```powershell
git remote -v
```

Должно показать:
```
origin  https://github.com/ВАШ_USERNAME/flump-game.git (fetch)
origin  https://github.com/ВАШ_USERNAME/flump-game.git (push)
```

✅ Remote добавлен!

---

## 📋 ШАГ 8: Отправка на GitHub (5-10 минут)

### 8.1 Отправьте код на GitHub:
```powershell
git push -u origin master
```

⏳ **ВАЖНО: Это займёт 5-10 минут при первой отправке!**

### 8.2 Авторизация:

**Вариант A: Окно авторизации появится**
```
1. Откроется окно браузера
2. Нажмите "Authorize"
3. Вернитесь в PowerShell
4. Отправка начнётся автоматически
```

**Вариант B: Запросит username/password**
```
Username: ваш_github_username
Password: ИСПОЛЬЗУЙТЕ PERSONAL ACCESS TOKEN!

Как создать token:
1. GitHub → Settings (вверху справа, ваш профиль)
2. Developer settings (внизу слева)
3. Personal access tokens → Tokens (classic)
4. Generate new token (classic)
5. Note: "Flump Game"
6. Expiration: 90 days (или No expiration)
7. Scope: ✓ repo (поставьте галочку)
8. Generate token
9. СКОПИРУЙТЕ TOKEN! (он больше не покажется!)
10. Вставьте вместо пароля
```

### 8.3 Дождитесь завершения:

Увидите прогресс:
```
Enumerating objects: XXXX, done.
Counting objects: 100% (XXXX/XXXX), done.
Delta compression using up to X threads
Compressing objects: 100% (XXXX/XXXX), done.
Writing objects: 100% (XXXX/XXXX), XXX.XX MiB | X.XX MiB/s, done.
Total XXXX (delta XXX), reused XXXX (delta XXX)
remote: Resolving deltas: 100% (XXX/XXX), done.
To https://github.com/ВАШ_USERNAME/flump-game.git
 * [new branch]      master -> master
Branch 'master' set up to track remote branch 'master' from 'origin'.
```

✅ Код отправлен на GitHub!

---

## 📋 ШАГ 9: Проверка (2 минуты)

### 9.1 Откройте ваш репозиторий:
```
https://github.com/ВАШ_USERNAME/flump-game
```

### 9.2 Проверьте что загрузилось:

Должны увидеть:
```
✅ Assets/
✅ Packages/
✅ ProjectSettings/
✅ .gitignore
✅ README.md
✅ MULTIPLAYER_PLAN.md
✅ FINAL_INSTRUCTIONS.md
✅ и другие .md файлы
```

**НЕ должны видеть:**
```
❌ Library/
❌ Temp/
❌ Builds/
❌ .vs/
❌ .vscode/
```

Если Library/ или Temp/ есть - что-то пошло не так!

### 9.3 Проверьте количество файлов:

На GitHub покажет что-то вроде:
```
XXX commits
1 branch
XXX files
```

✅ Если файлов 100+ - всё отлично!

---

## 🔄 ЕЖЕДНЕВНОЕ ИСПОЛЬЗОВАНИЕ

### После работы над проектом (каждый день):

**Откройте PowerShell в папке проекта:**
```powershell
cd C:\UnityProjects\Flump\FlumpGame
```

**Шаг 1: Проверьте что изменилось:**
```powershell
git status
```
Покажет красным - что изменено, зелёным - что добавлено

**Шаг 2: Добавьте изменения:**
```powershell
git add .
```

**Шаг 3: Создайте commit:**
```powershell
git commit -m "Описание что сделали"
```

Примеры описаний:
```powershell
git commit -m "[Feature] Added main menu UI"
git commit -m "[Fix] Fixed Android build error"
git commit -m "[Optimize] Improved bot AI"
git commit -m "[Docs] Updated multiplayer plan"
```

**Шаг 4: Отправьте на GitHub:**
```powershell
git push
```
⏳ Подождите 10-30 секунд

✅ Готово! Изменения на GitHub!

---

## 📝 ПРИМЕРЫ COMMIT СООБЩЕНИЙ

### Формат:
```
[Тип] Краткое описание (50 символов максимум)
```

### Типы:

**[Feature]** - Новая функция
```powershell
git commit -m "[Feature] Added multiplayer lobby"
git commit -m "[Feature] Implemented Duel 1v1 mode"
git commit -m "[Feature] Created matchmaking system"
```

**[Fix]** - Исправление бага
```powershell
git commit -m "[Fix] Fixed input system error on Android"
git commit -m "[Fix] Resolved bot spawn issue"
git commit -m "[Fix] Corrected health bar display"
```

**[Optimize]** - Оптимизация
```powershell
git commit -m "[Optimize] Improved network performance"
git commit -m "[Optimize] Reduced memory usage"
git commit -m "[Optimize] Enhanced bot AI efficiency"
```

**[Refactor]** - Рефакторинг кода
```powershell
git commit -m "[Refactor] Reorganized UI scripts"
git commit -m "[Refactor] Cleaned up weapon system"
git commit -m "[Refactor] Simplified match manager"
```

**[Docs]** - Документация
```powershell
git commit -m "[Docs] Updated README"
git commit -m "[Docs] Added multiplayer guide"
git commit -m "[Docs] Improved stage instructions"
```

**[Art]** - Графика/Ассеты
```powershell
git commit -m "[Art] Added new weapon models"
git commit -m "[Art] Updated UI textures"
git commit -m "[Art] Improved map lighting"
```

**[Audio]** - Звуки
```powershell
git commit -m "[Audio] Integrated weapon sounds"
git commit -m "[Audio] Added footstep effects"
git commit -m "[Audio] Implemented UI sounds"
```

**[Test]** - Тестирование
```powershell
git commit -m "[Test] Added unit tests for damage"
git commit -m "[Test] Tested Android build"
git commit -m "[Test] Verified multiplayer sync"
```

---

## ⚠️ ВАЖНЫЕ ПРАВИЛА

### ❌ НИКОГДА не коммитьте:

```
❌ Library/ (большая, временная, Unity пересоздаст)
❌ Temp/ (временная)
❌ Builds/ (APK, AAB файлы - слишком большие)
❌ .vs/ .vscode/ .idea/ (IDE кэш)
❌ Keystore файлы (.keystore, .jks) - СЕКРЕТНЫЕ!
❌ Пароли и API ключи
```

*Эти файлы уже в .gitignore - Git их игнорирует автоматически!*

### ✅ ВСЕГДА коммитьте:

```
✅ Assets/ (все скрипты, prefabs, сцены)
✅ Packages/ (manifest.json, packages-lock.json)
✅ ProjectSettings/ (настройки проекта)
✅ .meta файлы (КРИТИЧНО для Unity!)
✅ .md файлы (документация)
✅ .gitignore
✅ README.md
```

---

## 🆘 ЧАСТЫЕ ПРОБЛЕМЫ

### Проблема: "git is not recognized"
```
Решение:
1. Перезагрузите компьютер
2. Попробуйте снова
3. Если не помогло - переустановите Git
```

### Проблема: "Permission denied (publickey)"
```
Решение:
Используйте HTTPS URL, не SSH!
Правильно: https://github.com/username/repo.git
Неправильно: git@github.com:username/repo.git
```

### Проблема: "Authentication failed"
```
Решение:
Используйте Personal Access Token вместо пароля!
(см. Шаг 8.2 - Вариант B)
```

### Проблема: "Library/ загрузилась на GitHub"
```
Решение:
1. Удалите её:
   git rm -r --cached Library
2. Commit:
   git commit -m "Remove Library folder"
3. Push:
   git push
```

---

## 🎯 БЫСТРАЯ ШПАРГАЛКА

### Первая настройка (один раз):
```powershell
git init
git add .
git commit -m "Initial commit"
git remote add origin ВАША_ССЫЛКА_С_GITHUB
git push -u origin master
```

### Каждый день:
```powershell
git add .
git commit -m "[Feature] Что сделали"
git push
```

### Полезные команды:
```powershell
git status           # Что изменилось
git log              # История коммитов
git diff             # Детали изменений
git branch           # Список веток
git pull             # Скачать изменения с GitHub
```

---

## ✅ CHECKLIST

Отметьте выполненное:

```
□ Шаг 1: Git установлен
□ Шаг 2: Имя и email настроены
□ Шаг 3: Репозиторий инициализирован (git init)
□ Шаг 4: Первый commit создан
□ Шаг 5: Аккаунт на GitHub создан
□ Шаг 6: Репозиторий на GitHub создан
□ Шаг 7: Remote добавлен
□ Шаг 8: Код отправлен на GitHub (git push)
□ Шаг 9: Проверил на GitHub - файлы есть
```

Если все галочки стоят - **ВЫ МОЛОДЕЦ!** 🎉

---

## 🎊 ПОЗДРАВЛЯЮ!

Теперь ваш проект:
- ✅ В безопасности на GitHub
- ✅ Имеет backup в облаке
- ✅ Отслеживает все изменения
- ✅ Готов к командной работе
- ✅ Выглядит профессионально

---

## 📞 НУЖНА ПОМОЩЬ?

Если что-то не получилось:
1. Прочитайте секцию "Частые проблемы"
2. Проверьте что выполнили все шаги
3. Перечитайте инструкцию внимательно
4. Спросите меня - покажите screenshot ошибки!

---

**Дата создания:** 30 января 2026  
**Версия:** 1.0  
**Статус:** Готов к использованию ✅
