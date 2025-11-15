.PHONY: help build up down restart logs clean init-db init-users migrate deploy stop start ps

# Переменные
COMPOSE = docker compose
API_CONTAINER = cl-backend-api
DB_CONTAINER = cl-backend-postgres
API_PORT = 5000

# Цвета для вывода
GREEN = \033[0;32m
BLUE = \033[0;34m
YELLOW = \033[1;33m
NC = \033[0m # No Color

help: ## Показать справку по командам
	@echo "$(BLUE)Доступные команды:$(NC)"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  $(GREEN)%-15s$(NC) %s\n", $$1, $$2}'

# ============ Основные команды ============

deploy: ## Полное развертывание проекта (build + up + migrate + init-users)
	@echo "$(BLUE)=== Полное развертывание проекта ===$(NC)"
	$(MAKE) build
	$(MAKE) up
	$(MAKE) migrate
	$(MAKE) init-users
	@echo "$(GREEN)=== Развертывание завершено! ===$(NC)"
	@echo "$(BLUE)API доступен по адресу:$(NC) http://localhost:$(API_PORT)"
	@echo "$(BLUE)Swagger UI:$(NC) http://localhost:$(API_PORT)/swagger"

build: ## Собрать Docker образы
	@echo "$(BLUE)Сборка Docker образов...$(NC)"
	$(COMPOSE) build

up: ## Запустить контейнеры
	@echo "$(BLUE)Запуск контейнеров...$(NC)"
	$(COMPOSE) up -d
	@echo "$(GREEN)Контейнеры запущены!$(NC)"

down: ## Остановить и удалить контейнеры
	@echo "$(BLUE)Остановка контейнеров...$(NC)"
	$(COMPOSE) down

stop: ## Остановить контейнеры (без удаления)
	@echo "$(BLUE)Остановка контейнеров...$(NC)"
	$(COMPOSE) stop

start: ## Запустить остановленные контейнеры
	@echo "$(BLUE)Запуск контейнеров...$(NC)"
	$(COMPOSE) start

restart: ## Перезапустить контейнеры
	@echo "$(BLUE)Перезапуск контейнеров...$(NC)"
	$(COMPOSE) restart

ps: ## Показать статус контейнеров
	$(COMPOSE) ps

logs: ## Показать логи всех контейнеров
	$(COMPOSE) logs -f

logs-api: ## Показать логи API
	$(COMPOSE) logs -f $(API_CONTAINER)

logs-db: ## Показать логи БД
	$(COMPOSE) logs -f $(DB_CONTAINER)

# ============ База данных ============

migrate: ## Применить миграции к БД
	@echo "$(BLUE)Ожидание готовности API...$(NC)"
	@sleep 10
	@echo "$(BLUE)Применение миграций...$(NC)"
	docker exec $(API_CONTAINER) dotnet ef database update || true
	@echo "$(GREEN)Миграции применены!$(NC)"

init-db: ## Пересоздать БД с миграциями
	@echo "$(BLUE)Пересоздание базы данных...$(NC)"
	$(COMPOSE) down -v
	$(COMPOSE) up -d postgres
	@sleep 5
	$(COMPOSE) up -d api
	@sleep 10
	$(MAKE) migrate
	@echo "$(GREEN)База данных инициализирована!$(NC)"

db-shell: ## Подключиться к БД через psql
	docker exec -it $(DB_CONTAINER) psql -U postgres -d cl_backend_db

db-backup: ## Создать резервную копию БД
	@echo "$(BLUE)Создание резервной копии...$(NC)"
	docker exec $(DB_CONTAINER) pg_dump -U postgres cl_backend_db > backup_$(shell date +%Y%m%d_%H%M%S).sql
	@echo "$(GREEN)Резервная копия создана!$(NC)"

db-restore: ## Восстановить БД из резервной копии (укажите файл: make db-restore FILE=backup.sql)
	@echo "$(BLUE)Восстановление из резервной копии...$(NC)"
	docker exec -i $(DB_CONTAINER) psql -U postgres -d cl_backend_db < $(FILE)
	@echo "$(GREEN)База данных восстановлена!$(NC)"

# ============ Пользователи ============

init-users: ## Создать тестовых пользователей (admin@admin.admin и test@test.test)
	@echo "$(BLUE)Инициализация пользователей...$(NC)"
	@chmod +x scripts/init-users.sh
	@bash scripts/init-users.sh

# ============ Очистка ============

clean: ## Удалить контейнеры, образы и volumes
	@echo "$(YELLOW)Очистка всех данных...$(NC)"
	$(COMPOSE) down -v --rmi all
	@echo "$(GREEN)Очистка завершена!$(NC)"

clean-volumes: ## Удалить volumes (БД будет потеряна!)
	@echo "$(YELLOW)Удаление volumes...$(NC)"
	$(COMPOSE) down -v
	@echo "$(GREEN)Volumes удалены!$(NC)"

clean-logs: ## Очистить логи Docker
	@echo "$(BLUE)Очистка логов...$(NC)"
	docker system prune -f

# ============ Разработка ============

dev: ## Запустить в режиме разработки (с логами)
	$(COMPOSE) up

rebuild: ## Пересобрать и перезапустить проекты
	@echo "$(BLUE)Пересборка проекта...$(NC)"
	$(COMPOSE) down
	$(COMPOSE) build --no-cache
	$(COMPOSE) up -d
	@echo "$(GREEN)Проект пересобран и запущен!$(NC)"

shell-api: ## Подключиться к контейнеру API
	docker exec -it $(API_CONTAINER) /bin/bash

shell-db: ## Подключиться к контейнеру БД
	docker exec -it $(DB_CONTAINER) /bin/bash

# ============ Тестирование ============

test-api: ## Проверить доступность API
	@echo "$(BLUE)Проверка API...$(NC)"
	@curl -s http://localhost:$(API_PORT)/swagger/index.html > /dev/null && echo "$(GREEN)✓ API доступен$(NC)" || echo "$(RED)✗ API недоступен$(NC)"

test-db: ## Проверить подключение к БД
	@echo "$(BLUE)Проверка БД...$(NC)"
	@docker exec $(DB_CONTAINER) pg_isready -U postgres && echo "$(GREEN)✓ БД доступна$(NC)" || echo "$(RED)✗ БД недоступна$(NC)"

test-all: ## Проверить все сервисы
	$(MAKE) test-api
	$(MAKE) test-db

# ============ Информация ============

info: ## Показать информацию о проекте
	@echo "$(BLUE)=== Информация о проекте ===$(NC)"
	@echo "$(GREEN)API URL:$(NC) http://localhost:$(API_PORT)"
	@echo "$(GREEN)Swagger:$(NC) http://localhost:$(API_PORT)/swagger"
	@echo "$(GREEN)PostgreSQL:$(NC) localhost:54321"
	@echo ""
	@echo "$(BLUE)=== Контейнеры ===$(NC)"
	@$(COMPOSE) ps
	@echo ""
	@echo "$(BLUE)=== Volumes ===$(NC)"
	@docker volume ls | grep cl-backend

env-example: ## Создать .env файл из примера
	@if [ ! -f .env ]; then \
		cp .env.example .env; \
		echo "$(GREEN).env файл создан из .env.example$(NC)"; \
	else \
		echo "$(YELLOW).env файл уже существует$(NC)"; \
	fi
