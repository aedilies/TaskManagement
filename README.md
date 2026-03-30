TaskManagement – система управления задачами

Веб-приложение на ASP.NET Core для управления задачами с ролевой моделью доступа (Начальник, Сотрудник, Наблюдатель).
Проект выполнен в рамках тестового задания Backend Developer.

Технологии
- .NET 8 (ASP.NET Core, Entity Framework Core)
- SQL Server (база данных)
- JWT (аутентификация)
- Razor Pages (минимальный фронтенд для демонстрации)
- Swagger (документация API)
- xUnit, Moq (тесты)

Запуск проекта

Требования:
- .NET 8 SDK
- SQL Server (Express, LocalDB или Developer)
- Git (опционально)

1. Клонировать репозиторий
git clone https://github.com/aedilies/TaskManagement.git
cd TaskManagement

2. Настроить строку подключения к БД
Отредактируй src/TaskManagement.API/appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
}

3. Применить миграции и создать БД
dotnet ef database update --project src/TaskManagement.Infrastructure --startup-project src/TaskManagement.API

4. Запустить API
cd src/TaskManagement.API
dotnet run
API будет доступно по адресу http://localhost:5137 (порт может отличаться).
Swagger: http://localhost:5137/swagger

5. Запустить фронтенд 
cd frontend/TaskManagement.Web
dotnet run
Фронтенд запустится на http://localhost:5138 (или другом порту).

Важно: В файлах Login.cshtml и Index.cshtml фронтенда прописан apiBase = 'http://localhost:5137/api'. Если API запустился на другом порту – изменить эту константу.

Тестовые пользователи
Роль: [ Наблюдатель, Сотрудник, Начальник ]
Email: [ observer@example.com, employee@example.com, boss@example.com ]
Пароль: [ 123, 123, 123 ]

Все пользователи уже созданы при инициализации БД.

Возможности
- Аутентификация через JWT (логин/пароль)
- Ролевая модель:
  * Начальник: создание, редактирование, удаление, назначение исполнителя, изменение статуса любой задачи
  * Сотрудник: изменение статуса и удаление только своих задач
  * Наблюдатель: только просмотр
- Управление задачами:
  * CRUD операции через API
  * Фильтрация по статусу, приоритету, отделу исполнителя
  * Сортировка по дате создания
- Фронтенд:
  * Вход по email/паролю
  * Список задач с фильтрацией и кнопками действий (в зависимости от роли)
  * Создание задачи (для начальника)
  * Изменение статуса и назначение исполнителя

API Endpoints (основные)
Метод    URL                         Описание
POST     /api/auth/login             Логин, получение JWT
GET      /api/tasks                  Список задач (с фильтрацией)
POST     /api/tasks                  Создать задачу (только BOSS)
PATCH    /api/tasks/status           Изменить статус
PATCH    /api/tasks/assign           Назначить исполнителя (только BOSS)
DELETE   /api/tasks/{id}             Удалить задачу
GET      /api/users                  Список пользователей
GET      /api/departments            Список отделов

Полная документация – в Swagger.

Примечания
- При первом запуске БД создаётся автоматически, а seed-данные заполняются (роли, разрешения, отделы, пользователи).
- Если фронтенд не видит API – проверь порт и CORS (в Program.cs API уже настроен CORS для http://localhost:5138 и http://localhost:5137).
- Для работы с HTTPS можно добавить сертификаты, но тестовое задание допускает HTTP.
