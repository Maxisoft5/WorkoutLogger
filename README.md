# WorkoutLogg

> Pet-проект для изучения и демонстрации работы с современным .NET-стеком: модульная архитектура, .NET MAUI, gRPC, Kafka, Redis, OpenSearch, Grafana, .NET Aspire, YooKassa, Stripe.

Приложение для логирования тренировок с полным циклом: онбординг → журнал тренировок → аналитика → Premium-подписка с интегрированной оплатой.

> ⚠️ Проект учебный и активно дорабатывается. В первую очередь это демонстрация подходов и интеграций, а не production-ready продукт.

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
- **EF Core + Npgsql** — доступ к PostgreSQL (несколько `DbContext` в разных схемах)
- **ASP.NET Core Identity** — управление пользователями и ролями
- **JWT Bearer Authentication** — аутентификация и refresh-токены
- **gRPC (Grpc.AspNetCore)** — высокопроизводительный RPC рядом с REST
- **Confluent.Kafka** — продьюсер событий
- **StackExchange.Redis** + HybridCache — многоуровневое кэширование
- **Outbox pattern** — надёжная доставка доменных событий
- **.NET Aspire** — оркестрация локальной инфраструктуры, OpenTelemetry, health-checks
- **YooKassa API** — эквайринг для RU/СНГ (СБП, SberPay, T-Pay, карты)
- **Stripe API** — эквайринг для EN/World (Apple Pay, Google Pay, карты)

### Mobile / Client
- **.NET MAUI** — кросс-платформенный клиент (Android / iOS / Windows)
- **Refit** — типизированный REST-клиент
- **Grpc.Net.Client** — gRPC-клиент с поддержкой server streaming
- **CommunityToolkit.Mvvm** — MVVM с source generators
- **AKSoftware.Localization.MultiLanguages** — локализация (EN / RU)

### Инфраструктура (Docker)
- **PostgreSQL 17** — основная БД (схемы: `users`, `subscriptions`)
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
│  │  Auth / Users    │  Subscriptions    │  Exercises (gRPC)   │  │
│  └────────────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  Modules.Users  │  Modules.Subscriptions  │  Modules.Common│  │
│  └────────────────────────────────────────────────────────────┘  │
└────┬──────────────────┬──────────────────┬──────────────────┬────┘
     │                  │                  │                  │
     ▼                  ▼                  ▼                  ▼
┌──────────┐      ┌──────────┐      ┌──────────┐      ┌─────────────┐
│ Postgres │      │  Redis   │      │  Kafka   │      │  YooKassa   │
│ users    │      │          │      │          │      │  / Stripe   │
│ subscr.  │      └──────────┘      └─────┬────┘      └─────────────┘
└──────────┘                              │
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

- **Модульный монолит** — каждый домен (`Users`, `Subscriptions`) живёт в своих проектах с чёткими границами; межмодульная коммуникация — только через публичные контракты.
- **REST + gRPC сосуществуют** — на одном Kestrel-инстансе через `Http1AndHttp2`. REST используется для стандартных CRUD-операций, gRPC — для тяжёлых справочников и стриминговых сценариев.
- **Event-driven observability** — события аутентификации публикуются в Kafka и отдельно индексируются в OpenSearch. Падение Kafka не ломает основной auth-flow.
- **Multi-level caching** — гибридный кэш с fallback: при недоступности Redis срабатывает circuit breaker и сервис продолжает работать с `IMemoryCache`.
- **Locale-adaptive payments** — бэкенд определяет провайдера по локали запроса: `ru-*` → YooKassa, остальное → Stripe. MAUI-клиент адаптирует UI под методы оплаты региона.

---

## 📦 Структура решения

```
WorkoutLogg/
│
├── Common/
│   ├── Modules.Common.Domain            # Базовые типы, события, Outbox
│   ├── Modules.Common.Grpc.Contracts     # .proto-контракты (shared сервер + клиент)
│   └── Modules.Common.Infrastructure    # Caching, Messaging, Email, конфигурации
│
├── Modules/
│   ├── UsersModule/                     # Пользователи и аутентификация
│   │   ├── Modules.Users.Domain         # User, Role, BodyStats, WorkoutModel
│   │   ├── Modules.Users.DTO            # Контракты для API
│   │   ├── Modules.Users.Infrastructure # EF Core (схема users), JWT, Outbox, Refit API
│   │   └── Tests/
│   │
│   ├── WorkoutsModule/                  # Bounded context: тренировки
│   │   └── Modules.Workouts.DTO         # (в разработке)
│   │
│   └── SubscriptionsModule/             # Подписки и приём платежей
│       └── Modules.Subscriptions.Infrastructure
│           ├── Domain/                  # Subscription, SubscriptionPlan/Status/Provider enums
│           ├── Database/                # SubscriptionsDbContext (схема subscriptions), EF Migrations
│           └── Services/                # IPaymentProvider, YooKassaProvider, StripeProvider, SubscriptionService
│
├── WorkoutLogg/                         # MAUI-клиент
│   ├── Pages/                           # Dashboard, Profile, Workouts, Logger, Premium, Payment, ...
│   ├── PageModels/                      # MVVM ViewModels
│   ├── Services/                        # IAuthApi, IWorkoutsApi, ISubscriptionsApi (Refit), ...
│   └── Resources/Languages/            # en-US.yml, ru-RU.yml (полная локализация)
│
├── WorkoutLogg.AppHost                  # .NET Aspire — оркестрация локальной разработки
├── WorkoutLogg.ServiceDefaults          # OpenTelemetry, health-checks, общие defaults
├── WorkoutLogger.WebApi                 # ASP.NET Core: REST + gRPC + Webhooks (YooKassa/Stripe)
└── WorkoutLogger.EventsConsumer         # Worker Service: Kafka → OpenSearch
```

### Схемы PostgreSQL

| Схема           | DbContext                  | Содержимое                                    |
|-----------------|----------------------------|-----------------------------------------------|
| `users`         | `UsersDbContext`            | ASP.NET Identity tables, Refresh tokens, Workouts, Outbox |
| `subscriptions` | `SubscriptionsDbContext`    | Subscriptions, статусы, external payment IDs  |

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

Миграции применяются **автоматически при старте WebApi** (`MigrateAsync()` вызывается для каждого `DbContext` в `Program.cs`).

Для ручного запуска:

```bash
# Users
cd Modules.Users.Infrastructure
dotnet ef database update --startup-project ..\WorkoutLogger.WebApi\WorkoutLogger.WebApi.csproj

# Subscriptions
cd Modules.Subscriptions.Infrastructure
dotnet ef database update --startup-project ..\WorkoutLogger.WebApi\WorkoutLogger.WebApi.csproj
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

### 5. Конфигурация платёжных провайдеров (опционально)

Добавьте в `appsettings.json` WebApi:

```json
"SubscriptionSettings": {
  "YooKassaShopId": "...",
  "YooKassaSecretKey": "...",
  "YooKassaWebhookSecret": "...",
  "StripeSecretKey": "sk_test_...",
  "StripeWebhookSecret": "whsec_...",
  "StripePriceMonthlyId": "price_...",
  "StripePriceAnnualId": "price_..."
}
```

Без этих ключей модуль подписок загружается, но запросы к провайдерам вернут ошибку.

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

| Топик          | Producer | Consumer        | Содержимое                                               |
|----------------|----------|-----------------|----------------------------------------------------------|
| `auth-events`  | WebApi   | EventsConsumer  | `user.registered`, `user.login`, `user.login_failed`     |

### Webhook-эндпоинты

| URL                    | Провайдер | Событие                                      |
|------------------------|-----------|----------------------------------------------|
| `POST /webhooks/yookassa` | YooKassa | `payment.succeeded` → активация подписки     |
| `POST /webhooks/stripe`   | Stripe   | `checkout.session.completed` → активация     |

---

## 💡 Демонстрируемые подходы

### Аутентификация и авторизация

- JWT Bearer + refresh-tokens с revocation
- ASP.NET Core Identity с кастомным `UserValidator`
- Многошаговый онбординг через `UserRegistrationStep` (Profile → Body → Goals → Finished)

### Платёжная система (`Modules.Subscriptions`)

- `IPaymentProvider` — единый интерфейс поверх YooKassa и Stripe
- **Locale-routing**: бэкенд выбирает провайдера по `locale` в запросе, клиент адаптирует UI
- **PCI DSS compliance**: карточные данные никогда не проходят через наш сервер — только редирект на hosted page провайдера
- **HMAC-SHA256** верификация Stripe webhooks (`Stripe-Signature` header)
- Отдельный `SubscriptionsDbContext` в схеме `subscriptions` — изоляция от Users-модуля
- Webhook-хендлеры атомарно обновляют статус подписки и флаг `User.IsPremium`
- 7-дневный trial: `TrialEndsAt` хранится в БД, реальный платёж инициируется только после

### MAUI Premium UX

- `PremiumPage` — paywall с hero, 5 feature cards, переключатель Annual/Monthly
- `PremiumComparePage` — таблица сравнения Free vs Premium (8 строк)
- `PaymentPage` — адаптивный UI: 4 метода для RU (СБП, SberPay, T-Pay, Карта), 3 для EN (Apple Pay, Google Pay, Карта)
- `[QueryProperty]` — передача плана между страницами без ViewModel
- `Launcher.OpenAsync()` — открытие платёжной ссылки во внешнем браузере/приложении

### Кэширование (`Modules.Common.Infrastructure.Caching`)

- `ICacheService` с `GetOrCreateAsync` (cache-aside pattern)
- `HybridCacheService`: Redis как L2 + `IMemoryCache` как L1 + fallback при недоступности Redis
- **Circuit breaker** для Redis: после N подряд падений сервис на 30 секунд переходит на L1
- Префиксование ключей через `InstanceName = "WorkoutLogger:"`

### gRPC

- Shared contract project (`Modules.Common.Grpc.Contracts`) с `GrpcServices="Both"`
- Unary и server streaming RPC
- Сосуществование REST и gRPC на одном Kestrel (`Http1AndHttp2`)
- Передача JWT через gRPC metadata
- gRPC-клиент в MAUI с bypass dev-сертификата в DEBUG

### Событийная архитектура

- `IEventPublisher` → `KafkaEventPublisher` (идемпотентный продьюсер)
- Воркер-консьюмер с **manual commit** после успешной записи в OpenSearch (at-least-once)
- **Outbox pattern** — `OutboxProcessorService` как `IHostedService`
- **Дневные индексы** в OpenSearch (`auth-events-yyyy.MM.dd`)

### Локализация

- Полная двуязычная поддержка: EN / RU через embedded `.yml`-ресурсы
- `Loc.Get("Key")` — статический accessor для code-behind
- `{loc:Translate KeyName}` — XAML markup extension
- Язык сохраняется в `Preferences`, применяется без перезапуска

### Архитектура решения

- Модульный монолит с чёткими границами (`Domain` / `DTO` / `Infrastructure`)
- `AddXxxModule(configuration)` — extension методы для DI-регистрации модулей
- `IDesignTimeDbContextFactory<T>` для EF Migrations без запуска хоста
- Несколько `DbContext` в разных PostgreSQL-схемах в рамках одной БД

---

## 🗺 Дорожная карта

### Реализовано
- [x] JWT-аутентификация с refresh-токенами
- [x] Модульная структура решения
- [x] gRPC: справочник упражнений (unary + server streaming)
- [x] Kafka: публикация auth-событий
- [x] OpenSearch + Grafana: индексация и визуализация событий
- [x] Redis-кэш с fallback на MemoryCache и circuit breaker
- [x] Outbox pattern для публикации доменных событий
- [x] MAUI-клиент: онбординг, логин, смена пароля
- [x] MAUI: журнал тренировок (CRUD сессий и упражнений)
- [x] MAUI: Dashboard со статистикой и личными рекордами
- [x] MAUI: Профиль — фото, достижения, body stats, стрик
- [x] MAUI: Нормативы пауэрлифтинга (мужчины/женщины)
- [x] MAUI: локализация EN / RU
- [x] Premium paywall (PremiumPage, PremiumComparePage)
- [x] Stripe + YooKassa интеграция (checkout, webhooks, активация)
- [x] `Modules.Subscriptions.Infrastructure` — отдельный модуль, схема БД
- [x] .NET Aspire оркестрация

### В разработке
- [ ] AI-фичи: Coach, Record Forecast, Plan Generator (бэкенд)
- [ ] gRPC `WatchWorkout` — стриминг live-обновлений активной тренировки
- [ ] Rate limiting на `Login` через Redis
- [ ] Восстановление покупки (restore purchase)
- [ ] CI/CD пайплайн

### Идеи на будущее
- [ ] SignalR для real-time нотификаций о новых PR
- [ ] Push-уведомления через Firebase
- [ ] Экспорт PDF/CSV (Premium-фича)
- [ ] Аналитика по мышечным группам (Premium)
- [ ] Социальные функции — share workout, друзья
- [ ] Интеграция с Apple Health / Google Fit

---

## 📝 Заметки

Проект сознательно построен как **демонстрационный** — здесь специально комбинируются технологии, чтобы показать понимание разных подходов:

- gRPC выбран не везде, а только там, где он реально выигрывает у REST (стримы, тяжёлые справочники)
- Kafka не для core-логики, а для событий, где допустима возможная потеря (с логированием)
- Redis с fallback — потому что в проде кэш-слой не должен быть единственной точкой отказа
- Два платёжных провайдера через общий `IPaymentProvider` — потому что для RU и EN нужны принципиально разные решения, но бизнес-логика должна быть одна
- Два `DbContext` в одной БД — демонстрация изоляции модулей без накладных расходов микросервисов

Если что-то выглядит избыточным для подобного приложения — это намеренно. Задача — показать как это **работает вместе**, а не построить минимальный возможный стек.

---

## 📄 Лицензия

MIT
