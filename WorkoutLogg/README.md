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