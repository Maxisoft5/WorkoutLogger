# Результат

Настроен сценарий: девять инфраструктурных сервисов в Docker, Web API локально из IDE или CLI.

Ветка: `main`. В начале задачи существовали пользовательские изменения README и `WorkoutLogger.WebApi/appsettings.json`; они сохраняются.

## Изменения

- `docker-compose.yml`: API перенесён в профиль `api`; в контейнерном режиме задано `UseLocalhost=false`.
- `WorkoutLogger.WebApi/Extensions/LocalServiceConfigurationExtensions.cs`: при Development и UseLocalhost читается общий `.env` относительно content root проекта. Используется DotNetEnv 3.1.1 без изменения окружения процесса; источник добавляется с низшим приоритетом.
- `WorkoutLogger.WebApi/Program.cs`: настройка локальных сервисов выполняется до регистрации логирования и сервисов. Переменные окружения и CLI имеют приоритет над необязательным `appsettings.Local.json`.
- `POSTGRES_PASSWORD`, `POSTGRES_DB`, `POSTGRES_USER` задают подключение через `NpgsqlConnectionStringBuilder`; `JWT_SIGNING_KEY` задаёт ключ JWT. Отсутствие пароля или ключа останавливает запуск с понятным сообщением.
- Локальные профили `http` / `https` включают Kafka и OpenSearch; URL согласованы с Kestrel (5000 / 5001).
- README и `.env.example` описывают общий источник настроек и порядок запуска. Создание `appsettings.Local.json` не требуется.

## Проверки

- `dotnet build WorkoutLogger.WebApi/WorkoutLogger.WebApi.csproj`: успешно, 0 ошибок, 35 предупреждений. Предупреждения сборки в рамках этой задачи не исправлялись.
- `docker compose config --quiet` и `docker compose --profile api config --quiet`: успешно.
- `docker compose config --services`: ровно девять сервисов без API.
- `dotnet dev-certs https --check`: найден действительный сертификат разработки.
- Проверочный запуск собранного API с Development, UseLocalhost и включёнными Kafka / OpenSearch: `GET https://localhost:5001/health` вернул `Healthy`, HTTP 200. При проверке curl использовался `-k`, доверие сертификату не проверялось.
- Во время запуска API штатно применил существующие миграции Users, Subscriptions и Trainers к контейнерному PostgreSQL.
- В OpenSearch появился индекс `workoutlogger-logs-2026.09.15` с логами локального API.
- `docker compose ps`: все девять инфраструктурных сервисов работают; контейнера API нет.
- `git diff --check`: успешно. `.env` остаётся исключённым из Git.

## Состояние после проверки и ограничения

- Проверочный процесс API остановлен, чтобы пользователь мог запустить его из IDE. Инфраструктурные контейнеры оставлены работающими.
- На первом подключении Kafka была временная ошибка IPv6 `localhost` и отменённые коротким тайм-аутом curl проверки. Последующая проверка завершилась `Healthy`; постоянная недоступность не наблюдалась.
- Полные бизнес-сценарии регистрации, платежей и отправки почты не проверялись; проверка Redis на уровне бизнес-запроса не выполнялась.
- Пароль существующего тома PostgreSQL автоматически не меняется при редактировании `.env`.

Модульная память в целевом репозитории отсутствует и не обновлялась. Долгоживущая инструкция локальной конфигурации сохранена в README; результаты проверки — в документации задачи.
