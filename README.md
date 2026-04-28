# Лабораторная работа №33-34: Полноценный CRUD с базой данных

## Информация о студенте

- **ФИО:** Щербаков Данил Николаевич
- **Группа:** ИСП-232
- **Дата выполнения:** 28.04.2026

## Описание проекта

**NotesApp** — это RESTful API для управления заметками с категориями. Приложение позволяет:

- Создавать, читать, обновлять и удалять категории
- Создавать, читать, обновлять и удалять заметки
- Фильтровать заметки по категории, приоритету, поиску и статусу (архивные/активные)
- Закреплять и архивировать заметки
- Получать количество заметок в каждой категории

### Технологии

- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger/OpenAPI
- Repository Pattern

## Структура проекта

NotesApp/
├── Controllers/
│ ├── CategoriesController.cs # Контроллер категорий
│ └── NotesController.cs # Контроллер заметок
├── Data/
│ └── AppDbContext.cs # Контекст базы данных
├── Helpers/
│ └── ApiResponse.cs # Единый формат ответов
├── Models/
│ ├── Category.cs # Модель категории
│ ├── Note.cs # Модель заметки
│ └── DTOs/
│ ├── CategoryDto.cs # DTO для категорий
│ ├── NoteDto.cs # DTO для заметок
│ └── NoteFilterDto.cs # DTO для фильтрации
├── Repositories/
│ ├── ICategoryRepository.cs # Интерфейс репозитория категорий
│ ├── CategoryRepository.cs # Реализация репозитория категорий
│ ├── INoteRepository.cs # Интерфейс репозитория заметок
│ └── NoteRepository.cs # Реализация репозитория заметок
├── Program.cs # Точка входа и конфигурация
├── appsettings.json # Настройки приложения
├── appsettings.Development.json # Настройки для разработки
└── notesapp.db # База данных SQLite


## Маршруты API

### Категории

| Метод | URL | Описание | Коды ответа |
|-------|-----|----------|-------------|
| GET | /api/categories | Получить все категории с количеством заметок | 200 |
| GET | /api/categories/{id} | Получить категорию по ID | 200, 404 |
| GET | /api/categories/{id}/notes | Получить категорию со списком заметок | 200, 404 |
| POST | /api/categories | Создать новую категорию | 201, 400 |
| PUT | /api/categories/{id} | Обновить существующую категорию | 200, 400, 404 |
| DELETE | /api/categories/{id} | Удалить категорию | 204, 400, 404 |

### Заметки

| Метод | URL | Описание | Коды ответа |
|-------|-----|----------|-------------|
| GET | /api/notes | Получить все заметки с фильтрацией | 200 |
| GET | /api/notes/{id} | Получить заметку по ID | 200, 404 |
| POST | /api/notes | Создать новую заметку | 201, 400 |
| PUT | /api/notes/{id} | Обновить существующую заметку | 200, 400, 404 |
| PATCH | /api/notes/{id}/pin | Закрепить/открепить заметку | 200, 404 |
| PATCH | /api/notes/{id}/archive | Архивировать/восстановить заметку | 200, 404 |
| DELETE | /api/notes/{id} | Удалить заметку | 204, 404 |

### Параметры фильтрации заметок (GET /api/notes)

| Параметр | Тип | Описание | Пример |
|----------|-----|----------|--------|
| categoryId | int? | Фильтр по категории | ?categoryId=2 |
| isPinned | bool? | Только закреплённые | ?isPinned=true |
| archived | bool | Включать архивные (по умолчанию false) | ?archived=true |
| search | string | Поиск по заголовку и содержимому | ?search=LINQ |
| minPriority | int? | Минимальный приоритет (1-5) | ?minPriority=3 |
| sortBy | string | Поле сортировки (title, priority, updatedAt, createdAt) | ?sortBy=priority |
| descending | bool | Направление сортировки (по умолчанию true) | ?descending=false |
| page | int | Номер страницы (по умолчанию 1) | ?page=2 |
| pageSize | int | Размер страницы (1-50, по умолчанию 10) | ?pageSize=20 |

## Паттерн Repository

### Что такое Repository?

**Repository** — это паттерн проектирования, который изолирует логику работы с данными от логики контроллера.

### Архитектура без Repository:

Контроллер → DbContext → База данных


### Архитектура с Repository:

Контроллер → Repository → DbContext → База данных


### Зачем нужен Repository?

| Проблема без Repository | Решение с Repository |
|------------------------|---------------------|
| Логика запросов к БД размазана по контроллерам | Все запросы в одном месте |
| Трудно тестировать контроллер отдельно от БД | Можно подменить репозиторий на тестовый |
| При смене БД нужно менять все контроллеры | Меняется только репозиторий |
| Дублирование одинаковых запросов | Переиспользуемые методы |

### Пример из реального мира

Представьте склад (база данных) и кладовщика (Repository). Продавцы (контроллеры) не ходят на склад сами — они отправляют запрос кладовщику. Кладовщик знает, где всё лежит и как правильно работать со складом.

### Реализация в проекте

```csharp
// Интерфейс репозитория
public interface ICategoryRepository
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task<bool> ExistsAsync(int id);
    Task<bool> HasNotesAsync(int id);
}

// Реализация репозитория
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;
    
    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        return await _db.Categories
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                NotesCount = c.Notes.Count(n => !n.IsArchived)
            })
            .ToListAsync();
    }
}

// Использование в контроллере
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    
    public CategoriesController(ICategoryRepository repo)
    {
        _repo = repo;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var categories = await _repo.GetAllAsync();
        return Ok(categories);
    }
}

```

