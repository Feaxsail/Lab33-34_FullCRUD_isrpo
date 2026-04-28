using Microsoft.AspNetCore.Mvc;
using NotesApp.Helpers;
using NotesApp.Models;
using NotesApp.Models.DTOs;
using NotesApp.Repositories;

namespace NotesApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteRepository _noteRepo;
    private readonly ICategoryRepository _categoryRepo;
    
    public NotesController(INoteRepository noteRepo, ICategoryRepository categoryRepo)
    {
        _noteRepo = noteRepo;
        _categoryRepo = categoryRepo;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteResponseDto>>>> GetAll(
        [FromQuery] int? categoryId = null,
        [FromQuery] bool? isPinned = null,
        [FromQuery] bool archived = false,
        [FromQuery] string? search = null,
        [FromQuery] int? minPriority = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] bool descending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new NoteFilterDto
        {
            CategoryId = categoryId,
            IsPinned = isPinned,
            Archived = archived,
            Search = search,
            MinPriority = minPriority,
            SortBy = sortBy,
            Descending = descending,
            Page = page,
            PageSize = pageSize
        };
        
        var notes = await _noteRepo.GetAllAsync(filter);
        return Ok(ApiResponse<IEnumerable<NoteResponseDto>>.Ok(notes));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> GetById(int id)
    {
        var note = await _noteRepo.GetByIdAsync(id);
        if (note is null)
            return NotFound(ApiError.NotFound($"Заметка с id={id} не найдена"));
        
        return Ok(ApiResponse<NoteResponseDto>.Ok(note));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> Create([FromBody] CreateNoteDto dto)
    {
        
        if (!await _categoryRepo.ExistsAsync(dto.CategoryId))
            return BadRequest(ApiError.BadRequest(
                "Ошибки валидации",
                new List<string> { $"Категория с id={dto.CategoryId} не существует" }));
        
        var note = new Note
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Priority = dto.Priority,
            CategoryId = dto.CategoryId,
            IsPinned = false,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var created = await _noteRepo.CreateAsync(note);
        
        var response = await _noteRepo.GetByIdAsync(created.Id);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            ApiResponse<NoteResponseDto>.Created(response!, "Заметка успешно создана"));
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> Update(int id, [FromBody] UpdateNoteDto dto)
    {
        var note = await _noteRepo.FindAsync(id);
        if (note is null)
            return NotFound(ApiError.NotFound($"Заметка с id={id} не найдена"));
        
        if (!await _categoryRepo.ExistsAsync(dto.CategoryId))
            return BadRequest(ApiError.BadRequest(
                "Ошибки валидации",
                new List<string> { $"Категория с id={dto.CategoryId} не существует" }));
        
        note.Title = dto.Title.Trim();
        note.Content = dto.Content.Trim();
        note.Priority = dto.Priority;
        note.CategoryId = dto.CategoryId;
        
        await _noteRepo.UpdateAsync(note);
        
        var response = await _noteRepo.GetByIdAsync(id);
        return Ok(ApiResponse<NoteResponseDto>.Ok(response!, "Заметка обновлена"));
    }
    
    [HttpPatch("{id}/pin")]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> TogglePin(int id)
    {
        var note = await _noteRepo.FindAsync(id);
        if (note is null)
            return NotFound(ApiError.NotFound($"Заметка с id={id} не найдена"));
        
        note.IsPinned = !note.IsPinned;
        await _noteRepo.UpdateAsync(note);
        
        var response = await _noteRepo.GetByIdAsync(id);
        var message = note.IsPinned ? "Заметка закреплена" : "Заметка откреплена";
        
        return Ok(ApiResponse<NoteResponseDto>.Ok(response!, message));
    }
    
    [HttpPatch("{id}/archive")]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> ToggleArchive(int id)
    {
        var note = await _noteRepo.FindAsync(id);
        if (note is null)
            return NotFound(ApiError.NotFound($"Заметка с id={id} не найдена"));
        
        note.IsArchived = !note.IsArchived;
        
        if (note.IsArchived)
            note.IsPinned = false;
        
        await _noteRepo.UpdateAsync(note);
        
        var response = await _noteRepo.GetByIdAsync(id);
        var message = note.IsArchived ? "Заметка архивирована" : "Заметка восстановлена из архива";
        
        return Ok(ApiResponse<NoteResponseDto>.Ok(response!, message));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var note = await _noteRepo.FindAsync(id);
        if (note is null)
            return NotFound(ApiError.NotFound($"Заметка с id={id} не найдена"));
        
        await _noteRepo.DeleteAsync(note);
        return NoContent();
    }
}