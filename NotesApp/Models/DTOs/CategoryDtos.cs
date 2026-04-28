using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(100, ErrorMessage = "Максимум 100 символов")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Цвет должен быть в формате HEX: #RRGGBB")]
    public string Color { get; set; } = "#3498db";
}

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(100, ErrorMessage = "Максимум 100 символов")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Цвет должен быть в формате HEX: #RRGGBB")]
    public string Color { get; set; } = "#3498db";
}

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int NotesCount { get; set; }
}