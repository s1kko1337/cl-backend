# Backend API - E-Commerce Application

RESTful API для интернет-магазина на .NET 9 с PostgreSQL.

## Быстрый старт

```bash
# 1. Создайте .env файл
cp .env.example .env

# 2. Запустите через Docker
docker compose up -d

# 3. Проверьте работу
# Откройте: http://localhost:5000/swagger
```

## Репозитории проекта

- **Backend (этот репозиторий):** https://github.com/s1kko1337/cl-backend
- **Mobile App:** https://github.com/s1kko1337/cl-mobile

## Технологический стек

- **Framework:** ASP.NET Core 9.0
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer
- **API Documentation:** Swagger/OpenAPI
- **Containerization:** Docker + Docker Compose

## Требования

- **Docker** и **Docker Compose** (рекомендуется)
  - Docker Desktop 20.10+
  - Docker Compose 2.0+

**ИЛИ** для локального запуска:

- .NET SDK 9.0+
- PostgreSQL 15+
- Make (опционально)

## Установка и запуск

### Способ 1: Docker Compose (Рекомендуется)

```bash
# Создайте .env из примера
cp .env.example .env

# Запустите контейнеры
docker compose up -d

# Проверьте статус
docker compose ps

# Просмотр логов
docker compose logs -f
```

После запуска API будет доступен:
- **API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

### Способ 2: Makefile (Linux/macOS)

```bash
# Показать все команды
make help

# Полное развертывание
make deploy

# Другие команды
make build          # Собрать Docker образы
make up             # Запустить контейнеры
make down           # Остановить контейнеры
make logs           # Показать логи
make health         # Проверить готовность API
make init-db        # Пересоздать БД
```

### Способ 3: Локальный запуск без Docker

```bash
# Установите PostgreSQL и создайте БД
createdb -U postgres cl_backend_db

# Обновите строку подключения в cl-backend/DbContexts/ApplicationContext.cs

# Запустите проект
cd cl-backend
dotnet ef database update
dotnet run
```

## Конфигурация

### Переменные окружения (.env)

| Переменная | Описание | По умолчанию |
|------------|----------|--------------|
| `POSTGRES_DB` | Имя базы данных | `cl_backend_db` |
| `POSTGRES_USER` | Пользователь PostgreSQL | `postgres` |
| `POSTGRES_PASSWORD` | Пароль PostgreSQL | `55451241` |
| `POSTGRES_PORT` | Порт PostgreSQL (внешний) | `54321` |
| `API_PORT` | Порт API (внешний) | `5000` |

### JWT Configuration

Настройки в файле `AuthOptions.cs`:

```csharp
public const string ISSUER = "MyAuthServer";
public const string AUDIENCE = "MyAuthClient";
const string KEY = "mysupersecret_secretkey!123";
```

**ВАЖНО:** Для production измените ключ!

## API Endpoints

После запуска откройте Swagger UI: http://localhost:5000/swagger

### Основные эндпоинты

- `POST /api/account/login` - Вход пользователя
- `POST /api/account/register` - Регистация
- `GET /api/products` - Список товаров
- `GET /api/products/{id}` - Детали товара
- `GET /api/categories` - Список категорий
- `GET /api/orders` - Заказы пользователя
- `POST /api/orders` - Создать заказ

## Тестовые данные

Backend автоматически создает тестовых пользователей:

**Администратор:**
- Email: `admin@admin.admin`
- Пароль: `admin@admin.admin`

**Пользователь:**
- Email: `test@test.test`
- Пароль: `test@test.test`

## Структура проекта

```
cl-backend/
├── cl-backend/              # Исходный код API
│   ├── Controllers/         # API контроллеры
│   ├── Models/              # Модели данных
│   ├── DbContexts/          # Контекст БД
│   ├── Services/            # Бизнес-логика
│   ├── DTO/                 # Data Transfer Objects
│   ├── Extensions/          # Расширения
│   ├── Utils/               # Утилиты
│   ├── Migrations/          # Миграции БД
│   └── Program.cs           # Точка входа
├── docker-compose.yml       # Docker конфигурация
├── Dockerfile               # Образ API
├── Makefile                 # Команды управления
├── .env.example             # Пример переменных
└── cl-backend.sln           # Solution файл
```

## Устранение неполадок

### Контейнеры не запускаются

```bash
# Проверьте логи
docker compose logs

# Проверьте порты
netstat -ano | findstr :5000  # Windows
lsof -i :5000                 # Linux/macOS
```

### Ошибки миграций

```bash
# Пересоздайте БД
docker compose down -v
docker compose up -d
```

### API недоступен

```bash
# Проверьте статус
docker compose ps

# Перезапустите
docker compose restart api
```

## Разработка

### Полезные команды

```bash
# Запуск в режиме разработки
make dev

# Пересборка
make rebuild

# Подключение к контейнеру
make shell-api

# Подключение к PostgreSQL
make db-shell

# Резервная копия БД
make db-backup

# Восстановление БД
make db-restore FILE=backup.sql
```

## Документация

Полная документация проекта: [README.md в основном репозитории](https://github.com/s1kko1337/cl-backend)

## Контакты

При возникновении проблем создайте Issue: https://github.com/s1kko1337/cl-backend/issues
