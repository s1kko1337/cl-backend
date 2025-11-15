-- Инициализация пользователей
-- Пароли хешируются с использованием BCrypt

-- Удаляем пользователей если они уже существуют
DELETE FROM "Users" WHERE "Login" IN ('admin@admin.admin', 'test@test.test');

-- Вставка администратора
-- Пароль: admin@admin.admin
-- Хеш сгенерирован с помощью BCrypt
INSERT INTO "Users" ("Login", "Password", "Role")
VALUES (
    'admin@admin.admin',
    '$2a$11$YourBCryptHashHereForAdminPassword',  -- Это нужно заменить реальным хешем
    'admin'
);

-- Вставка тестового пользователя
-- Пароль: test@test.test
-- Хеш сгенерирован с помощью BCrypt
INSERT INTO "Users" ("Login", "Password", "Role")
VALUES (
    'test@test.test',
    '$2a$11$YourBCryptHashHereForTestPassword',  -- Это нужно заменить реальным хешем
    'user'
);
