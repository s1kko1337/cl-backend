# ⚡ Быстрый старт

## Одна команда для запуска всего:

```bash
make deploy
```

Эта команда:
1. ✅ Соберет Docker образы
2. ✅ Запустит PostgreSQL и API
3. ✅ Применит миграции к БД
4. ✅ Создаст тестовых пользователей

---

## Доступ к API

После выполнения команды:

- **API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

---

## Тестовые пользователи

| Роль | Логин | Пароль |
|------|-------|--------|
| **Админ** | admin@admin.admin | admin@admin.admin |
| **User** | test@test.test | test@test.test |

---

## Основные команды

```bash
# Запуск
make up

# Остановка
make down

# Перезапуск
make restart

# Логи
make logs

# Справка
make help
```

---

## Проверка работы

```bash
# Проверить API
curl http://localhost:5000/swagger/index.html

# Логин (получить токен)
curl -X POST http://localhost:5000/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin@admin.admin","password":"admin@admin.admin"}'
```

---

## Если что-то пошло не так

```bash
# Полная очистка и пересоздание
make clean
make deploy

# Только пересоздать БД
make init-db

# Только создать пользователей заново
make init-users
```

---

## Дополнительно

- [Полная документация по развертыванию](DEPLOYMENT.md)
- [API документация для разработчиков](API_DOCUMENTATION_ADMIN.md)
- [Краткий справочник endpoints](ADMIN_ENDPOINTS_QUICK_REFERENCE.md)
