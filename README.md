# WorkoutLogg

> Pet-проект для изучения и демонстрации работы с современным .NET-стеком: модульная архитектура, .NET MAUI, gRPC, Kafka, Redis, OpenSearch, Grafana, .NET Aspire.

Приложение для логирования тренировок: онбординг с настройкой целей и параметров тела, дашборд со статистикой, журнал тренировок, профиль пользователя.

> ⚠️ Проект учебный и активно дорабатывается. Часть фич реализована частично — в первую очередь это демонстрация подходов и интеграций, а не production-ready продукт.

---

## 📚 Содержание

- [Стек технологий](#-стек-технологий)
- [Архитектура](#-архитектура)
- [Структура решения](#-структура-решения)
- [Запуск проекта](#-запуск-проекта)
- [Инфраструктура](#-инфраструктура)
- [Демонстрируемые подходы](#-демонстрируемые-подходы)
- [Дорожная карта](#-дорожная-карта)

---

## 🛠 Стек технологий

### Backend
- **.NET 10** — ASP.NET Core Web API
- **EF Core + Npgsql** — доступ к PostgreSQL
- **ASP.NET Core Identity** — управление пользователями и ролями
- **JWT Bearer Authentication** — аутентификация и refresh-токены
- **gRPC (Grpc.AspNetCore)** — высокопроизводительный RPC рядом с REST
- **Confluent.Kafka** — продьюсер событий
- **StackExchange.Redis** + `IDistributedCache` — кэширование
- **.NET Aspire** — оркестрация локальной инфраструктуры, OpenTelemetry, health-checks

### Mobile / Client
- **.NET MAUI** — кросс-платформенный клиент (Android / iOS / Windows)
- **Grpc.Net.Client** — gRPC-клиент с поддержкой server streaming
- **Refit** — типизированный REST-клиент

### Инфраструктура (Docker)
- **PostgreSQL 17** — основная БД
- **Redis 7** — кэш и хранилище распределённых данных
- **Apache Kafka 3.8** (KRaft mode, без Zookeeper) — шина событий
- **OpenSearch 2.x + Dashboards** — хранилище и поиск событий
- **Grafana** — дашборды поверх OpenSearch
- **Kafka UI**, **Redis Commander** — UI для отладки

---

## 🏗 Архитектура

Приложение построено по принципам **модульного монолита**: каждый бизнес-модуль изолирован и состоит из своего домена, инфраструктуры и DTO. Связь между модулями — через общие контракты (`Modules.Common.*`).

```
┌──────────────────────────────────────────────────────────────────┐
│                        WorkoutLogg (MAUI)                        │
│                      Android / iOS / Windows                     │
└─────────────────┬──────────────────────────┬─────────────────────┘
                  │ REST (JSON over HTTPS)   │ gRPC (HTTP/2)
                  ▼                          ▼
┌──────────────────────────────────────────────────────────────────┐
│                     WorkoutLogger.WebApi                         │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  Controllers (REST)         │  Grpc Services               │  │
│  └────────────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  Modules.Users  │  Modules.Workouts  │  Modules.Common      │  │
│  └────────────────────────────────────────────────────────────┘  │
└────┬──────────────────┬──────────────────┬──────────────────┬────┘
     │                  │                  │                  │
     ▼                  ▼                  ▼                  ▼
┌──────────┐      ┌──────────┐      ┌──────────┐      ┌─────────────┐
│ Postgres │      │  Redis   │      │  Kafka   │      │   OpenAPI   │
└──────────┘      └──────────┘      └─────┬────┘      └─────────────┘
                                          │
                                          ▼
                              ┌──────────────────────────┐
                              │ EventsConsumer (Worker)  │
                              └─────────────┬────────────┘
                                            ▼
                                   ┌────────────────┐
                                   │   OpenSearch   │ ◄── Grafana / Dashboards
                                   └────────────────┘
```

### Принципы

- **Модульный монолит** — каждый домен (`Users`, `Workouts`) живёт в своих проектах с чёткими границами; межмодульная коммуникация — только через публичные контракты.
- **REST + gRPC сосуществуют** — на одном Kestrel-инстансе через `Http1AndHttp2`. REST используется для стандартных CRUD-операций, gRPC — для тяжёлых справочников и стриминговых сценариев.
- **Event-driven observability** — события аутентификации публикуются в Kafka и отдельно индексируются в OpenSearch для аналитики и безопасности. Падение Kafka не ломает основной auth-flow.
- **Multi-level caching** — гибридный кэш с fallback: при недоступности Redis срабатывает circuit breaker и сервис продолжает работать с `IMemoryCache`.

---

## 📦 Структура решения

```
WorkoutLogg/
│
├── Common/                              # Общие контракты и инфраструктура
│   ├── Modules.Common.Domain            # Базовые доменные типы, события
│   ├── Modules.Common.Grpc.Contacts     # .proto-контракты (shared между сервером и клиентом)
│   └── Modules.Common.Infrastructure    # Caching, Messaging, общие конфигурации
│
├── Modules/
│   ├── UsersModule/                     # Bounded context: пользователи и аутентификация
│   │   ├── Modules.Users.Domain         # Доменные сущности (User, Role), сервисы
│   │   ├── Modules.Users.DTO            # Контракты для API
│   │   ├── Modules.Users.Infrastructure # EF Core, JWT, репозитории
│   │   └── Tests/                       # Юнит- и интеграционные тесты
│   │
│   └── WorkoutsModule/                  # Bounded context: тренировки
│       └── Modules.Workouts.DTO         # (в разработке)
│
├── WorkoutLogg                          # MAUI-клиент (Android / iOS / Windows)
├── WorkoutLogg.AppHost                  # .NET Aspire — оркестрация локальной разработки
├── WorkoutLogg.ServiceDefaults          # OpenTelemetry, health-checks, общие defaults
├── WorkoutLogger.WebApi                 # ASP.NET Core: REST + gRPC + DI-композиция
└── WorkoutLogger.EventsConsumer         # Worker Service: Kafka → OpenSearch
```

### Зачем отдельный `Common.Grpc.Contacts`

`.proto`-файлы и сгенерированный код вынесены в отдельную сборку с настройкой `<Protobuf GrpcServices="Both" />`. К этой сборке подключаются и сервер, и MAUI-клиент — благодаря этому контракт всегда синхронизирован, а правка `.proto` автоматически перегенерирует обе стороны.

### Зачем `EventsConsumer` отдельным проектом

Чтобы продемонстрировать паттерн consumer/worker и развязать жизненный цикл API от обработки событий. В реальном продукте такой воркер скейлится отдельно от основного API.

---

## 🚀 Запуск проекта

### Требования

- .NET 10 SDK
- Docker Desktop (или Docker + Docker Compose)
- Visual Studio 2022 / Rider / VS Code с MAUI workload

### 1. Поднять инфраструктуру

```bash
docker compose up -d
```

Дождитесь готовности Kafka и OpenSearch (~60 секунд при первом запуске).

### 2. Применить миграции

```bash
dotnet ef database update --project Modules/UsersModule/Modules.Users.Infrastructure --startup-project WorkoutLogger.WebApi
```

### 3. Запустить API

Через Aspire (рекомендуется — поднимет всё разом с дашбордом):

```bash
dotnet run --project WorkoutLogg.AppHost
```

Или напрямую:

```bash
dotnet run --project WorkoutLogger.WebApi
dotnet run --project WorkoutLogger.EventsConsumer
```

### 4. Запустить MAUI-клиент

В Visual Studio: выбрать профиль (Android emulator / Windows) → F5.

---

## 🧰 Инфраструктура

| Сервис                | Порт | URL                              | Назначение                     |
|-----------------------|------|----------------------------------|--------------------------------|
| Web API (REST)        | 5000 | http://localhost:5000            | REST-эндпоинты                 |
| Web API (HTTPS/gRPC)  | 5001 | https://localhost:5001           | gRPC + REST over TLS           |
| PostgreSQL            | 5432 | localhost:5432                   | Основная БД                    |
| Redis                 | 6379 | localhost:6379                   | Кэш                            |
| Redis Commander       | 8081 | http://localhost:8081            | UI для Redis                   |
| Kafka (внешний)       | 9094 | localhost:9094                   | Bootstrap для приложения       |
| Kafka UI              | 8082 | http://localhost:8082            | Просмотр топиков и сообщений   |
| OpenSearch            | 9200 | http://localhost:9200            | REST API хранилища             |
| OpenSearch Dashboards | 5601 | http://localhost:5601            | Discover, визуализация         |
| Grafana               | 3000 | http://localhost:3000            | Дашборды (admin/admin)         |
| Aspire Dashboard      | —    | (открывается автоматически)      | Логи, трейсы, метрики          |

### Топики Kafka

| Топик          | Producer | Consumer        | Содержимое                                      |
|----------------|----------|-----------------|-------------------------------------------------|
| `auth-events`  | WebApi   | EventsConsumer  | `user.registered`, `user.login`, `user.login_failed` |

---

## 💡 Демонстрируемые подходы

Этот раздел — для интервьюеров и читателей кода. Здесь конкретные паттерны и идеи, которые показаны в проекте.

### Аутентификация и авторизация

- JWT Bearer + refresh-tokens
- ASP.NET Core Identity с кастомным `UserValidator` (отключение проверки уникальности username)
- Многошаговый онбординг через `UserRegistrationStep` (Profile → Body → Goals → Finished)

### Кэширование (`Modules.Common.Infrastructure.Caching`)

- Абстракция `ICacheService` с `GetOrCreateAsync` (cache-aside pattern)
- `HybridCacheService`: Redis как L2 + `IMemoryCache` как L1 + fallback при недоступности Redis
- **Circuit breaker** для Redis: после N подряд падений сервис на 30 секунд перестаёт пытаться, чтобы не тратить таймауты
- Префиксование ключей через `InstanceName = "WorkoutLogger:"` для шеринга Redis между приложениями

### gRPC

- Shared contract project (`Modules.Common.Grpc.Contacts`) с `GrpcServices="Both"`
- Unary и server streaming RPC
- Сосуществование REST и gRPC на одном Kestrel (`Http1AndHttp2`)
- Передача JWT через gRPC metadata, переиспользование ASP.NET pipeline для авторизации
- gRPC-клиент в MAUI: учёт особенностей Android (`10.0.2.2` вместо `localhost`), DEBUG-bypass dev-сертификата

### Событийная архитектура

- `IEventPublisher` с реализацией `KafkaEventPublisher`
- Идемпотентный продьюсер (`EnableIdempotence = true`, `Acks = All`)
- Воркер-консьюмер с **manual commit** после успешной записи в OpenSearch — гарантия at-least-once
- **Дневные индексы** в OpenSearch (`auth-events-yyyy.MM.dd`) — стандартный паттерн для time-series данных
- Падение Kafka не ломает регистрацию/логин (логируется, но не пробрасывается)

### Наблюдаемость

- **OpenSearch Dashboards** — discovery событий, ad-hoc анализ
- **Grafana** — постоянные дашборды (логины в минуту, top failed login by IP, регистрации по часам)
- **.NET Aspire Dashboard** — встроенные OpenTelemetry-трейсы, метрики и логи всех проектов
- Health-checks для Kafka и Redis, проброшенные через `MapDefaultEndpoints`

### Архитектура и качество

- Модульный монолит с `Modules.{Domain}` / `Modules.{Domain}.DTO` / `Modules.{Domain}.Infrastructure`
- Общие сервисы и контракты в `Modules.Common.*`
- Расширения `IServiceCollection` для модульной регистрации (`AddAuthModule`, `AddRedisCache`, `AddKafkaMessaging`)
- Юнит- и интеграционные тесты в `Modules/UsersModule/Tests`

---

## 🗺 Дорожная карта

### Реализовано
- [x] JWT-аутентификация с refresh-токенами
- [x] Модульная структура решения
- [x] gRPC: справочник упражнений (unary + server streaming)
- [x] Kafka: публикация auth-событий
- [x] OpenSearch + Grafana: индексация и визуализация событий
- [x] Redis-кэш с fallback на MemoryCache
- [x] MAUI-клиент с онбордингом и логином
- [x] .NET Aspire оркестрация

### В разработке
- [ ] CRUD тренировок (`Modules.Workouts`)
- [ ] gRPC `WatchWorkout` — стриминг live-обновлений активной тренировки
- [ ] Outbox pattern для публикации событий
- [ ] Rate limiting на `Login` через Redis
- [ ] CI/CD пайплайн

### Идеи на будущее
- [ ] SignalR для real-time нотификаций о новых PR (personal records)
- [ ] Push-уведомления через Firebase
- [ ] Социальные функции — share workout, друзья
- [ ] Интеграция с Apple Health / Google Fit

---

## 📝 Заметки

Проект сознательно построен как **демонстрационный** — здесь специально комбинируются технологии, чтобы показать понимание разных подходов:

- gRPC выбран не везде, а только там, где он реально выигрывает у REST (стримы, тяжёлые справочники)
- Kafka не для core-логики, а для событий, где допустима возможная потеря (с логированием)
- Redis с fallback — потому что в проде кэш-слой не должен быть единственной точкой отказа

Если что-то выглядит избыточным для подобного приложения — это намеренно. Задача — показать как это **работает вместе**, а не построить минимальный возможный стек.

---

## 📄 Лицензия

MIT
