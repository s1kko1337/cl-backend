#!/bin/bash

# Цвета для вывода
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

API_URL="${API_URL:-http://localhost:5000}"
POSTGRES_CONTAINER="${POSTGRES_CONTAINER:-cl-backend-postgres}"
POSTGRES_DB="${POSTGRES_DB:-cl_backend_db}"
POSTGRES_USER="${POSTGRES_USER:-postgres}"

echo -e "${BLUE}=== Инициализация пользователей ===${NC}"

# Функция для регистрации пользователя через API
register_user() {
    local login=$1
    local password=$2

    echo -e "${BLUE}Регистрация пользователя: ${login}${NC}"

    response=$(curl -s -w "\n%{http_code}" -X POST "${API_URL}/register" \
        -H "Content-Type: application/json" \
        -d "{\"username\":\"${login}\",\"password\":\"${password}\"}")

    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')

    if [ "$http_code" -eq 200 ]; then
        echo -e "${GREEN}✓ Пользователь ${login} успешно зарегистрирован${NC}"
        return 0
    else
        # Проверяем, не существует ли пользователь уже
        if echo "$body" | grep -q "already exists"; then
            echo -e "${YELLOW}⚠ Пользователь ${login} уже существует${NC}"
            return 0
        else
            echo -e "${RED}✗ Ошибка регистрации ${login} (HTTP ${http_code})${NC}"
            echo -e "${RED}  Ответ: ${body}${NC}"
            return 1
        fi
    fi
}

# Функция для обновления роли пользователя в БД
update_user_role() {
    local login=$1
    local role=$2

    echo -e "${BLUE}Обновление роли пользователя ${login} на ${role}${NC}"

    docker exec -i ${POSTGRES_CONTAINER} psql -U ${POSTGRES_USER} -d ${POSTGRES_DB} <<-EOSQL 2>/dev/null
        UPDATE "Users" SET "Role" = '${role}' WHERE "Login" = '${login}';
EOSQL

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Роль пользователя ${login} обновлена на ${role}${NC}"
        return 0
    else
        echo -e "${RED}✗ Ошибка обновления роли для ${login}${NC}"
        return 1
    fi
}

# Проверка доступности API
echo -e "${BLUE}Проверка доступности API на ${API_URL}...${NC}"
for i in {1..30}; do
    if curl -s -f "${API_URL}/swagger/index.html" > /dev/null 2>&1; then
        echo -e "${GREEN}✓ API доступен${NC}"
        break
    fi

    if [ $i -eq 30 ]; then
        echo -e "${RED}✗ API недоступен после 30 попыток${NC}"
        exit 1
    fi

    echo -e "Попытка $i/30... Ожидание..."
    sleep 2
done

# Задержка для полной инициализации
sleep 3

echo ""
echo -e "${BLUE}Создание пользователей...${NC}"

# Регистрация администратора
register_user "admin@admin.admin" "admin@admin.admin"
sleep 1
update_user_role "admin@admin.admin" "admin"

echo ""

# Регистрация тестового пользователя
register_user "test@test.test" "test@test.test"
sleep 1
update_user_role "test@test.test" "user"

echo ""
echo -e "${GREEN}=== Инициализация завершена ===${NC}"
echo -e "${BLUE}Доступные пользователи:${NC}"
echo -e "  ${GREEN}Администратор:${NC} admin@admin.admin / admin@admin.admin"
echo -e "  ${GREEN}Пользователь:${NC}  test@test.test / test@test.test"
echo ""
echo -e "${BLUE}API доступен по адресу:${NC} ${API_URL}"
echo -e "${BLUE}Swagger UI:${NC} ${API_URL}/swagger"
