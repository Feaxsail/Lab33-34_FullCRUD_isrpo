using NotesApp.Models;
using NotesApp.Models.DTOs;

namespace NotesApp.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByIdWithNotesAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task<bool> ExistsAsync(int id);
    Task<bool> HasNotesAsync(int id);
}