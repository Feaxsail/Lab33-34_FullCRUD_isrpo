using Microsoft.AspNetCore.Mvc;
using NotesApp.Helpers;
using NotesApp.Models;
using NotesApp.Models.DTOs;
using NotesApp.Repositories;

namespace NotesApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    
    public CategoriesController(ICategoryRepository repo)
    {
        _repo = repo;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponseDto>>>> GetAll()
    {
        var categories = await _repo.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(categories));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Category>>> GetById(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
            return NotFound(ApiError.NotFound($"Категория с id={id} не найдена"));
        
        return Ok(ApiResponse<Category>.Ok(category));
    }
   
    [HttpGet("{id}/notes")]
    public async Task<ActionResult<ApiResponse<object>>> GetWithNotes(int id)
    {
        var category = await _repo.GetByIdWithNotesAsync(id);
        if (category is null)
            return NotFound(ApiError.NotFound($"Категория с id={id} не найдена"));
        
        var response = new
        {
            category.Id,
            category.Name,
            category.Description,
            category.Color,
            category.CreatedAt,
            Notes = category.Notes.Select(n => new
            {
                n.Id,
                n.Title,
                n.IsPinned,
                n.IsArchived,
                n.Priority,
                n.CreatedAt,
                n.UpdatedAt
            })
        };
        
        return Ok(ApiResponse<object>.Ok(response));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Category>>> Create([FromBody] CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Color = dto.Color
        };
        
        var created = await _repo.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, 
            ApiResponse<Category>.Created(created, "Категория успешно создана"));
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Category>>> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
        {
            return NotFound(ApiError.NotFound($"Категория с id={id} не найдена"));
        }
        
        category.Name = dto.Name.Trim();
        category.Description = dto.Description.Trim();
        category.Color = dto.Color;
        
        var updated = await _repo.UpdateAsync(category);
        return Ok(ApiResponse<Category>.Ok(updated, "Категория обновлена"));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
            return NotFound(ApiError.NotFound($"Категория с id={id} не найдена"));
 
        if (await _repo.HasNotesAsync(id))
        {
            return BadRequest(ApiError.BadRequest(
                "Невозможно удалить категорию: в ней есть заметки. " +
                "Сначала удалите или переместите заметки."));
        }
        
        await _repo.DeleteAsync(category);
        return NoContent();
    }
}