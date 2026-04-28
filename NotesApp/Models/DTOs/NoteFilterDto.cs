namespace NotesApp.Models.DTOs;

public class NoteFilterDto
{
    public int? CategoryId { get; set; }
    public bool? IsPinned { get; set; }
    public bool Archived { get; set; } = false;
    public string? Search { get; set; }
    public int? MinPriority { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public bool Descending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}