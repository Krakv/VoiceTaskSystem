# VoiceTaskSystem (VTS)

Информационная система для управления задачами с поддержкой голосового ввода.

## Описание

VoiceTaskSystem — это система для управления задачами. Пользователь может создавать задачи голосовыми командами: аудио или текст передаётся в сервис обработки речи, который распознаёт намерение, извлекает сущности и возвращает структурированный результат. Итог можно подтвердить или отредактировать перед сохранением.

Помимо голосового ввода, система поддерживает ручное управление задачами, уведомления, правила автоматизации, интеграцию с внешними календарями и Telegram-бота.

## Структура репозитория

```
VTS/
├── docker/                         # Docker Compose конфигурации и Prometheus
├── docs/                           # API-спецификация
├── ml/                             # Python ML-модели
│   └── PythonMLModels/
│       ├── EntityExtractor/        # NER-модель на ONNX
│       └── IntentClassifier/       # Классификатор намерений на ONNX
└── src/
    ├── frontend/voice-task/        # React SPA (Vite + TypeScript)
    └── TaskManager/                # Backend .NET-решение
        ├── TaskManager.ApiGateway          # ASP.NET Core Web API, точка входа
        ├── TaskManager.Auth                # Аутентификация, JWT, OAuth (Яндекс), Telegram-связка
        ├── TaskManager.Calendar            # Управление событиями, CalDAV-синхронизация
        ├── TaskManager.Notifications       # Уведомления: Email (SMTP) и Telegram
        ├── TaskManager.RulesEngine         # Движок правил автоматизации задач
        ├── TaskManager.TaskManagement      # Задачи, голосовые команды, фоновая обработка
        ├── TaskManager.Shared              # Общие сущности, события, интерфейсы, пайплайн
        ├── TaskManager.Repository          # EF Core DbContext, миграции PostgreSQL
        ├── TaskManager.IntegrationTests    # Интеграционные тесты (xUnit)
        └── TaskManager.UnitTests           # Юнит-тесты валидаторов и сервисов
    └── SpeechProcessingService/    # .NET-сервис обработки речи
        ├── SpeechProcessingService.API             # ASP.NET Core Web API
        ├── SpeechProcessingService.Application     # CQRS-команды, бизнес-логика
        └── SpeechProcessingService.Infrastructure  # Whisper ASR, ONNX, GigaChat, Duckling
```

## Технологический стек

### Backend (TaskManager)

| Слой | Технологии |
|---|---|
| Web API | ASP.NET Core 9, MediatR (CQRS), FluentValidation |
| База данных | PostgreSQL, Entity Framework Core, Npgsql |
| Аутентификация | ASP.NET Identity, JWT Bearer, OAuth 2.0 (Яндекс) |
| Фоновые задачи | Hangfire + PostgreSQL storage |
| Уведомления | SMTP (MailKit), Telegram Bot API |
| Календарь | CalDAV (Яндекс Календарь) |
| Логирование | Serilog |
| Метрики | prometheus-net |

### Backend (SpeechProcessingService)

| Компонент | Технология |
|---|---|
| Распознавание речи (ASR) | Whisper.NET (GGML-модели) |
| Классификация намерений | ONNX Runtime (собственная модель) |
| Извлечение сущностей (NER) | ONNX Runtime (собственная модель) |
| Нормализация дат | Duckling (HTTP-клиент) |

### ML-модели

Модели обучены на синтетических данных (русский язык) и экспортированы в ONNX.

- `IntentClassifier` — классифицирует намерение команды (создать задачу, обновить, удалить, запросить)
- `EntityExtractor` — извлекает именованные сущности из текста команды (название, дедлайн, приоритет и т.д.)

Скрипты обучения и генерации данных находятся в `ml/PythonMLModels/`.

### Frontend

React 19 + TypeScript, сборка через Vite 7. Стилизация: Tailwind CSS 4 + shadcn/ui. Стейт-менеджмент: Zustand. HTTP-клиент: Axios. Роутинг: React Router 7.

### Инфраструктура

- **Docker Compose** — оркестрация всех сервисов
- **Prometheus** — сбор метрик (`/metrics`)
- **Grafana** — визуализация метрик
- **Serilog** — структурированное логирование

## Архитектура

### Паттерны

- **CQRS + MediatR** — команды и запросы разделены, каждый обработчик изолирован
- **Pipeline Behaviors** — валидация (ValidationBehavior), контроль доступа (TaskAccessBehavior, NotificationAccessBehavior) реализованы как MediatR-пайплайн
- **Clean Architecture** — Application / Infrastructure / API разделены на отдельные проекты
- **Domain Events** — TaskCreatedEvent, TaskUpdatedEvent и другие события публикуются через MediatR для слабой связанности модулей

### Поток обработки голосовой команды

```
Пользователь -> Frontend
Frontend -> TaskManager API (аудио)
TaskManager API -> сохраняет CommandRequestItem со статусом Pending
TaskManager API -> SpeechProcessingService
  SpeechProcessingService:
    1. Whisper ASR (аудио -> текст)
    2. IntentClassifier ONNX (текст -> намерение)
    3. EntityExtractor ONNX (текст -> сущности)
    4. Duckling (нормализация дат)
  -> CommandResponse
TaskManager API -> сохраняет CommandRequestItem со статусом WaitingForConfirmation, Accepted или Failed
Frontend -> показывает результат пользователю
Пользователь просматривает/подтверждает/редактирует -> TaskManager API создаёт/обновляет/удаляет Task
```

## Запуск
 
### 1. Переменные окружения
 
Скопируй `.env.example` в `.env` в корне папки `docker/` и заполни значения:
 
```bash
cp docker/.env.example docker/.env
```
 
| Переменная | Описание | Обязательно |
|---|---|---|
| `JWT_SECRET` | Секрет для подписи JWT-токенов. Любая длинная случайная строка | да |
| `TELEGRAM_BOT_TOKEN` | Токен бота от [@BotFather](https://t.me/BotFather). Нужен для Telegram-уведомлений и связки аккаунта | нет |
| `GIGACHAT_AUTH_TOKEN` | Bearer-токен для GigaChat API. Используется как fallback в SpeechProcessingService | нет |
| `YANDEX_CLIENT_ID` | Client ID OAuth-приложения Яндекса. Нужен для интеграции с Яндекс Календарём | нет |
| `YANDEX_CLIENT_SECRET` | Client Secret OAuth-приложения Яндекса | нет |
| `OAUTH_REDIRECT_URI` | URI редиректа после OAuth. Должен совпадать с настройками приложения в Яндекс ID | нет |
| `SMTP_USERNAME` | Логин SMTP-сервера для отправки email-уведомлений | нет |
| `SMTP_PASSWORD` | Пароль SMTP-сервера | нет |
| `SMTP_FROM` | Адрес отправителя, например `noreply@example.com` | нет |
| `FRONTEND_URL` | URL фронтенда, используется в CORS и ссылках в письмах | нет |
| `CONNECTION_STRING` | Строка подключения к PostgreSQL. По умолчанию ведёт на контейнер `postgres` | заполнено |
| `SPEECH_PROCESSING_SERVICE_URI` | URL эндпоинта SpeechProcessingService. По умолчанию ведёт на контейнер | заполнено |
 
Необязательные переменные можно оставить пустыми — соответствующие функции просто не будут работать (уведомления, Telegram, календарь).
 
### 2. ONNX-модели
 
SpeechProcessingService при запуске ожидает файлы моделей в папке `docker/Models/`. Структура должна быть такой:
 
```
docker/Models/
├── ggml-small.bin      # Whisper GGML-модель для ASR
├── model.onnx          # Классификатор намерений (IntentClassifier)
├── model.onnx.data     # Веса классификатора (часть модели)
├── tokenizer.json      # Токенизатор классификатора
├── vocab.txt           # Словарь классификатора
└── Ner/
    ├── model.onnx      # NER-модель (EntityExtractor)
    └── tokenizer.json  # Токенизатор NER-модели
```
 
Пути к файлам задаются в `appsettings.json` сервиса через секции `GgmlModel`, `IntentOnnxModel`, `NerOnnxModel`.
 
Имена файлов и пути задаются в `appsettings.json` сервиса через секции `GgmlModel`, `IntentOnnxModel`, `NerOnnxModel`.
 
### 3. Запуск
 
Все команды выполняются из корня репозитория.
 
**Минимальный запуск** — только `task-manager`, `postgres` и `frontend` (без SpeechProcessingService, без мониторинга):
 
```bash
docker compose -f docker/docker-compose.yml --profile front up -d --build
```
 
**С SpeechProcessingService** — добавляется сервис обработки речи и Duckling:
 
```bash
docker compose -f docker/docker-compose.yml --profile dev up -d --build
```
 
Профиль `dev` поднимает: `task-manager`, `postgres`, `frontend`, `speech-processing-service`, `duckling`, `prometheus`, `grafana`, `elasticsearch`, `kibana`.
 
**Только бэкенд без фронтенда** (удобно при разработке фронтенда локально):
 
```bash
docker compose -f docker/docker-compose.yml --profile sps up -d --build
```
 
Профиль `sps` поднимает: `task-manager`, `postgres`, `speech-processing-service`, `duckling`.
 
### 4. Адреса сервисов после запуска
 
| Сервис | URL |
|---|---|
| Swagger UI / API | http://localhost:3000 |
| Frontend | http://localhost:3005 |
| SpeechProcessingService | http://localhost:3001 |
| Hangfire Dashboard | http://localhost:3000/hangfire |
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3002 (логин: `admin` / `admin`) |
| Kibana | http://localhost:5601 |
| PostgreSQL | localhost:5432 |
 
Миграции применяются автоматически при старте `task-manager`.

## REST API

Полная спецификация: [docs/API.md](docs/API.md)

API реализует следующие группы эндпоинтов:

- `/api/v1/auth` — регистрация, вход, смена пароля, подтверждение email, Telegram-связка
- `/api/v1/tasks` — CRUD задач с пагинацией и фильтрацией, patch-обновление
- `/api/v1/command-requests` — голосовые команды: создание, статус, подтверждение, редактирование
- `/api/v1/notifications` — CRUD уведомлений с расписанием
- `/api/v1/rules` — правила автоматизации (условия + действия в JSON)
- `/api/v1/calendar` — события и синхронизация с Яндекс Календарём
- `/api/v1/oauth` — OAuth 2.0 flow для Яндекс

## Тесты

```bash
# Юнит-тесты
dotnet test src/TaskManager/TaskManager.UnitTests/

# Интеграционные тесты
dotnet test src/TaskManager/TaskManager.IntegrationTests/
```

Интеграционные тесты поднимают реальный контекст с тестовой БД. Зависимости (email, CalDAV, OAuth) заменяются Fake-реализациями.

## Мониторинг и логирование

- Метрики доступны на эндпоинте `/metrics` (формат Prometheus)
- Swagger UI доступен на `/` (корень API)
- Hangfire Dashboard — на `/hangfire`
- Serilog настраивается через `appsettings.json` (секция `Serilog`)
