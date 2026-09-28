-- Выполняется автоматически PostgreSQL-образом при первом запуске
-- (том с данными пуст) - см. docker-entrypoint-initdb.d в compose.yml.

CREATE TABLE IF NOT EXISTS elements (
    id              BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    attribute_value TEXT,
    html            TEXT NOT NULL
);
