## Веб-приложение и админка залов

React + TypeScript находится в `WorkoutLogger.Web`. Запуск, настройка доменов,
отдельных PostgreSQL-схем и первого администратора описаны в
[docs/web-platform.md](docs/web-platform.md). Статус переноса функций — в
[docs/web-platform-plan.md](docs/web-platform-plan.md).

## Сборка Android и запуск на USB-устройстве

Для используемого .NET 10 Android SDK нужны **JDK 21** и **Android SDK Platform 36**.
JDK 25 вызывает ошибку `XA0030` даже при подключённом и авторизованном телефоне.

- Установите Microsoft OpenJDK 21:
  `winget install --id Microsoft.OpenJDK.21 --exact --source winget`.
- Укажите каталог установленного JDK 21 в пользовательской переменной `JAVA_HOME`
  и в настройке Java SDK для Android в IDE, если там выбран другой JDK.
  После изменения переменной полностью перезапустите IDE и терминал.
- В Android SDK Manager установите Android SDK Platform 36. Либо выполните
  из папки `WorkoutLogg`:

  ```powershell
  dotnet build WorkoutLogg/WorkoutLogg.csproj -t:InstallAndroidDependencies -f net10.0-android
  dotnet build WorkoutLogg/WorkoutLogg.csproj -f net10.0-android
  ```

На телефоне включите отладку по USB и подтвердите разрешение для компьютера.
Команда `adb devices -l` должна показывать телефон со статусом `device`.

Если сборка сообщает `XARDF7024` / `Access denied` при удалении каталогов внутри
`WorkoutLogg/obj`, сначала проверьте атрибут `ReadOnly` у проблемного каталога.
В локальном окружении он был обнаружен у
`WorkoutLogg/obj/Debug/net10.0-android/android-arm64/stamp`, хотя права разрешали
изменение. После снятия атрибута сборка успешно создала подписанный APK.

Остановите текущую сборку и выполните из папки решения:

```powershell
foreach ($path in @('WorkoutLogg\obj', 'WorkoutLogg\bin')) {
    if (Test-Path -LiteralPath $path) {
        attrib.exe -R "$path"
        attrib.exe -R "$path\*" /S /D
    }
}
dotnet build WorkoutLogg/WorkoutLogg.csproj -f net10.0-android -r android-arm64
```

Команда снимает `ReadOnly` только со сгенерированных каталогов клиента и их
содержимого. Если отказ в доступе остаётся, отдельно проверьте права и блокировки
файлов IDE / синхронизацией. При переносе репозитория не копируйте `bin` и `obj`:
они восстанавливаются сборкой и могут содержать старые пути и атрибуты.

## Локальный Web API и сервисы в Docker

Выполняйте команды из папки `WorkoutLogg`, где находится `docker-compose.yml`.
Нужны запущенный Docker Desktop с Linux-контейнерами и .NET SDK 10.

1. При первом запуске скопируйте `.env.example` в `.env`:

   ```powershell
   Copy-Item .env.example .env
   ```

   Если `.env` уже существует, используйте его без перезаписи. Задайте в нём
   `POSTGRES_PASSWORD`, `GRAFANA_ADMIN_PASSWORD`, `CERT_PASSWORD` и
   `JWT_SIGNING_KEY` (случайный ключ длиной не менее 32 байт).
   `.env` исключён из Git и Docker build context. Compose автоматически читает
   этот файл; `.env.example` служит только образцом. Отсутствующее или пустое
   обязательное значение вызывает ошибку `required variable ... is missing a value`.

2. Проверьте конфигурацию и запустите инфраструктуру:

   ```powershell
   docker compose config --quiet
   docker compose up -d --build
   docker compose ps
   ```

   По умолчанию запускаются PostgreSQL, Redis, Redis Commander, Kafka, Kafka UI,
   OpenSearch, OpenSearch Dashboards, Grafana и `events-consumer`.
   Сервис `api` отнесён к профилю `api` и обычной командой `up` не запускается.
   Если контейнер API был запущен раньше, остановите его командой
   `docker compose stop api`, чтобы освободить порты для локального процесса.

3. Подготовьте доверенный сертификат для локального ASP.NET Core и запустите API:

   ```powershell
   dotnet dev-certs https --trust
   dotnet run --project WorkoutLogger.WebApi --launch-profile https
   ```

   В Rider / Visual Studio выберите проект `WorkoutLogger.WebApi` и профиль
   `https` из `launchSettings.json`. Он включает `Development`, `UseLocalhost`,
   Kafka и OpenSearch. `appsettings.Local.json` для этого сценария не нужен.

При `Development` и `UseLocalhost=true` API автоматически читает `.env` из папки
решения, на один уровень выше каталога проекта `WorkoutLogger.WebApi`.
Путь определяется относительно content root API, а не текущей папки терминала.
Файл читается через DotNetEnv без изменения окружения процесса. Для одинаковых
ключей аргументы CLI и переменные окружения имеют приоритет над локальным JSON,
а `.env` служит источником значений с самым низким приоритетом.

В локальном режиме `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD` формируют
строку подключения к PostgreSQL; `JWT_SIGNING_KEY` задаёт `AuthConfiguration:Key`.
Пустой или отсутствующий пароль / ключ вызывает понятную ошибку при старте.
Резервный пароль `postgres` не используется. При изменении `.env` перезапустите API.
Адреса инфраструктуры в этом режиме принудительно заменяются на локальные:

| Сервис | Адрес для локального API / браузера |
| --- | --- |
| PostgreSQL | `localhost:5432` |
| Redis | `localhost:6379` |
| Kafka | `localhost:9094` |
| OpenSearch | `http://localhost:9200` |
| Redis Commander | `http://localhost:8081` |
| Kafka UI | `http://localhost:8082` |
| OpenSearch Dashboards | `http://localhost:5601` |
| Grafana | `http://localhost:3000` |

Для локального Kafka-клиента оставьте `KAFKA_EXTERNAL_HOST=localhost` в `.env`.
Внутри Docker consumer использует отдельный адрес `kafka:9092`.

Web API слушает `http://localhost:5000` и `https://localhost:5001`.
Проверка готовности: `https://localhost:5001/health` (включает Kafka).
Логин и пароль Grafana задаются в `.env`.
Пароли PostgreSQL и администратора Grafana применяются при первоначальной
инициализации хранилищ: изменение `.env` не меняет пароль в уже существующей БД.

### Android-телефон по USB → локальный API

В `WorkoutLogg/Resources/Raw/appsettings.json` включите `UseLocalhost=true`.
Клиент выбирает адрес так:

- Android-эмулятор: `Api:LocalAndroidUrl`, по умолчанию `https://10.0.2.2:5001`.
- Физический телефон по USB и настольный клиент: `Api:LocalUrl`, по умолчанию
  `https://localhost:5001`.

Адрес `10.0.2.2` относится только к Android-эмулятору. На телефоне `localhost`
означает сам телефон; для доступа к компьютеру настройте ADB reverse.
Выполните команды через `adb` из `platform-tools` Android SDK:

```powershell
adb devices -l
adb -d reverse tcp:5001 tcp:5001
adb -d reverse tcp:5000 tcp:5000
adb -d reverse --list
```

`-d` выбирает единственный физический USB-телефон, даже если также подключён
эмулятор. При нескольких телефонах используйте `-s <serial>` из `adb devices -l`.
После переподключения телефона проверьте проброс и при необходимости повторите.
В Windows Debug-сборка Android и MSBuild Install теперь автоматически запускают
`tools/Connect-LocalApi.ps1`: он восстанавливает проброс для единственного
подключённого авторизованного телефона и проверяет, слушает ли порт API компьютер.
Это выполняется только при `UseLocalhost=true` и loopback-адресе `Api:LocalUrl`.
Если IDE запускает уже установленное приложение без сборки, после переподключения
USB запустите скрипт вручную из папки рядом с решением:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Connect-LocalApi.ps1
```

При нескольких телефонах передайте скрипту `-DeviceSerial <serial>` или сборке
`-p:LocalApiDeviceSerial=<serial>`. Отключить автоматический шаг можно через
`-p:EnableLocalApiReverse=false`. Отсутствие телефона не блокирует сборку.
Скрипт не запускает сервер: Web API должен работать отдельным процессом.

Порт 5001 нужен REST и локальному gRPC по HTTPS; проброс 5000 позволяет проверять
HTTP endpoint и его перенаправление на HTTPS.

Запустите Web API с профилем `https` и оставьте его работающим, затем запустите
клиент на телефоне в конфигурации Debug. Исправления кода и bundled-конфига
вступают в силу после пересборки и установки приложения. В Debug существующий
HTTP handler принимает сертификат разработки; Release требует доверенный
сертификат. Наличие работающих Docker-сервисов само по себе не запускает API.

Запросы авторизации ограничены 30 секундами на HTTP-запрос. При сетевой ошибке,
тайм-ауте или неуспешном ответе страницы регистрации и входа восстанавливают
форму с введёнными значениями и показывают сообщение вместо зависшего
`LoadingPage`. Ошибка загрузки текущего пользователя после входа также
возвращает форму. Полная операция входа может включать несколько HTTP-запросов.

При запуске с сохранённым токеном приложение восстанавливает сессию через CurrentUser.
Если запрос завершается тайм-аутом или сетевой ошибкой, вместо бесконечной анимации
появляется экран «Не удалось восстановить сессию» с кнопками повтора и входа.
После запуска API и восстановления ADB reverse нажмите «Повторить попытку».
Сетевое исключение на старте не удаляет токены. Диагностика доступна в Debug-выводе
по префиксу `[Startup]`.

При тайм-ауте с работающими API и ADB reverse проверьте строку `[ApiEndpoint]` в
Debug-выводе (Android logcat: тег `WorkoutLogger.Api`). На физическом телефоне
ожидаются `DeviceType=Physical` и `Address=https://localhost:5001`.
Проверка эмулятора должна учитывать одновременно Android-платформу и
`DeviceType.Virtual`. После изменения выбора URL полностью перезапустите клиент:
Hot Reload не выполняет `CreateMauiApp` заново и не пересоздаёт HTTP-клиенты.

### Профиль контейнерного API

Для запуска API в контейнере предусмотрена команда
`docker compose --profile api up -d --build`. В контейнере явно задано
`UseLocalhost=false`, поэтому используются адреса сервисов Compose.
Этому профилю нужен экспортированный сертификат:

```powershell
$certPassword = (Get-Content .env | Where-Object { $_ -match '^CERT_PASSWORD=' }) -replace '^CERT_PASSWORD=', ''
New-Item -ItemType Directory -Force "$HOME/.aspnet/https" | Out-Null
dotnet dev-certs https -ep "$HOME/.aspnet/https/aspnetapp.pfx" -p $certPassword
```

Локальный API использует сертификат разработки из хранилища ASP.NET Core,
экспорт PFX для него не требуется. Переменная `CERT_PASSWORD` остаётся обязательной
для разбора общего Compose-файла, даже когда профиль `api` не выбран.

## Переносы текста и отзывчивая навигация клиента

- Текст рядом с иконкой / checkbox должен получать ограниченную ширину через
  Grid `Auto,*`; HorizontalStackLayout для длинного переносимого текста не подходит.
- Составные подписи со ссылками используют один Label с FormattedString и WordWrap.
- Карточки целей онбординга растут по высоте; частота тренировок размещается в сетке
  3×2. При проверке учитывайте русский язык и системное увеличение шрифта.
- Основные вкладки и ссылки профиля переходят через `AppShell.NavigateAsync`:
  индикатор появляется на исходной странице до создания целевой. Оверлей целевой
  страницы закрывается после загрузки её данных.
- Календарь переиспользует ячейки в пределах месяца. CollectionView тренировок и
  журнала является основным вертикальным скроллером, чтобы сохранять виртуализацию.

В Debug-выводе доступны `[Navigation]` (подготовка индикатора и переход Shell) и
`[PageLoad]` (загрузка данных страницы). Значения измеряют разные этапы и не являются
замером полного времени от нажатия до появления последнего пикселя.
Чек-лист проверки на Samsung A51: `docs/onboarding-navigation/manual-checks.md`.

## gRPC

Проект демонстрирует:
- Shared contract project (`WorkoutLogger.Grpc.Contracts`) с `.proto` файлами
- Unary RPC (`GetExercise`) и server streaming (`StreamExercises`, `WatchWorkout`)
- Coexistence с REST на одном Kestrel инстансе (HTTP/1.1 + HTTP/2)
- gRPC клиент в .NET MAUI с поддержкой Android/iOS/Windows
- JWT-аутентификация через gRPC metadata
- Использование `IAsyncEnumerable` для потребления стримов

## Event-driven observability

Auth events (login, registration, failed login) публикуются в Kafka, откуда отдельный consumer-сервис индексирует их в OpenSearch для логов и аналитики.

### Stack
- **Kafka 3.8** (KRaft mode, без Zookeeper) — очередь событий
- **Kafka UI** — мониторинг топиков и сообщений (`:8082`)
- **OpenSearch 2.x** — хранилище и поиск событий
- **OpenSearch Dashboards** — discovery и визуализация (`:5601`)
- **Grafana** — дашборды поверх OpenSearch (`:3000`)

### Endpoints
- `/health` — health-check включает проверку Kafka
- Топик `auth-events` — все события аутентификации

### Что демонстрируется
- Идемпотентный продюсер с `acks=all`
- Manual commit на consumer стороне после успешной записи в OpenSearch
- Дневные индексы (`auth-events-yyyy.MM.dd`) — стандарт для time-series данных
- Резистентность к падению Kafka — auth flow продолжает работать
## EF Core migrations

Package Manager Console (Visual Studio):

```powershell
Add-Migration <Name> -Project Modules.Users.Infrastructure -StartupProject WorkoutLogger.WebApi -Context UsersDbContext
Remove-Migration -Project Modules.Users.Infrastructure -StartupProject WorkoutLogger.WebApi -Context UsersDbContext
```

CLI:

```bash
dotnet ef database update --project Modules.Users.Infrastructure --startup-project WorkoutLogger.WebApi --context UsersDbContext
```
