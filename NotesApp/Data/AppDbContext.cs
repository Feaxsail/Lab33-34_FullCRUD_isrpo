using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Note> Notes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Category)
            .WithMany(c => c.Notes)
            .HasForeignKey(n => n.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        modelBuilder.Entity<Note>()
            .HasIndex(n => n.CategoryId);
        
        modelBuilder.Entity<Note>()
            .HasIndex(n => n.CreatedAt);
        
        
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Работа",
                Description = "Рабочие задачи и проекты",
                Color = "#e74c3c",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 2,
                Name = "Учёба",
                Description = "Конспекты и материалы для обучения",
                Color = "#3498db",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 3,
                Name = "Личное",
                Description = "Личные заметки и идеи",
                Color = "#2ecc71",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        
        
        modelBuilder.Entity<Note>().HasData(
            new Note
            {
                Id = 1,
                Title = "Изучить ASP.NET Core",
                Content = "Пройти лабораторные работы 27-36.",
                Priority = 5,
                IsPinned = true,
                IsArchived = false,
                CategoryId = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Note
            {
                Id = 2,
                Title = "Подготовить отчёт",
                Content = "Написать отчёт по лабораторной работе №30 до конца недели.",
                Priority = 4,
                IsPinned = false,
                IsArchived = false,
                CategoryId = 1,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Note
            {
                Id = 3,
                Title = "Идеи для проекта",
                Content = "Сделать приложение для отслеживания расходов.",
                Priority = 3,
                IsPinned = false,
                IsArchived = false,
                CategoryId = 3,
                CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new Note
            {
                Id = 4,
                Title = "Конспект по LINQ",
                Content = "Основные методы для работы с коллекциями и EF Core.",
                Priority = 4,
                IsPinned = true,
                IsArchived = false,
                CategoryId = 2,
                CreatedAt = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc)
            },
            new Note
            {
                Id = 5,
                Title = "Старый черновик",
                Content = "Этот черновик больше не актуален, отправлен в архив.",
                Priority = 1,
                IsPinned = false,
                IsArchived = true,
                CategoryId = 3,
                CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}