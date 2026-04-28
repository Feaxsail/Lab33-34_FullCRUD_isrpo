using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models.DTOs;

public class CreateNoteDto
{
    [Required(ErrorMessage = "Заголовок обязателен")]
    [MaxLength(200, ErrorMessage = "Максимум 200 символов")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(5000, ErrorMessage = "Максимум 5000 символов")]
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 5, ErrorMessage = "Приоритет от 1 до 5")]
    public int Priority { get; set; } = 3;
    
    [Required(ErrorMessage = "Категория обязательна")]
    public int CategoryId { get; set; }
}

public class UpdateNoteDto
{
    [Required(ErrorMessage = "Заголовок обязателен")]
    [MaxLength(200, ErrorMessage = "Максимум 200 символов")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(5000, ErrorMessage = "Максимум 5000 символов")]
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 5, ErrorMessage = "Приоритет от 1 до 5")]
    public int Priority { get; set; } = 3;
    
    [Required(ErrorMessage = "Категория обязательна")]
    public int CategoryId { get; set; }
}

public class NoteResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
}