using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NotesApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsPinned = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "#e74c3c", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Рабочие задачи и проекты", "Работа" },
                    { 2, "#3498db", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Конспекты и материалы для обучения", "Учёба" },
                    { 3, "#2ecc71", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Личные заметки и идеи", "Личное" }
                });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "Id", "CategoryId", "Content", "CreatedAt", "IsArchived", "IsPinned", "Priority", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 2, "Пройти лабораторные работы 27-36.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 5, "Изучить ASP.NET Core", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, "Написать отчёт по лабораторной работе №30 до конца недели.", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), false, false, 4, "Подготовить отчёт", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, "Сделать приложение для отслеживания расходов.", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), false, false, 3, "Идеи для проекта", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, "Основные методы для работы с коллекциями и EF Core.", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 4, "Конспект по LINQ", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 3, "Этот черновик больше не актуален, отправлен в архив.", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 1, "Старый черновик", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CategoryId",
                table: "Notes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CreatedAt",
                table: "Notes",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
