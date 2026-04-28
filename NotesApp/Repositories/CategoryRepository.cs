using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Models.DTOs;

namespace NotesApp.Repositories;

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
                Description = c.Description,
                Color = c.Color,
                CreatedAt = c.CreatedAt,
                NotesCount = c.Notes.Count(n => !n.IsArchived)
            })
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
    
    public async Task<Category?> GetByIdAsync(int id)
        => await _db.Categories.FindAsync(id);
    
    public async Task<Category?> GetByIdWithNotesAsync(int id)
    {
        return await _db.Categories
            .Include(c => c.Notes.Where(n => !n.IsArchived))
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<Category> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }
    
    public async Task<Category> UpdateAsync(Category category)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
        return category;
    }
    
    public async Task DeleteAsync(Category category)
    {
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }
    
    public async Task<bool> ExistsAsync(int id)
        => await _db.Categories.AnyAsync(c => c.Id == id);
    
    public async Task<bool> HasNotesAsync(int id)
        => await _db.Notes.AnyAsync(n => n.CategoryId == id);
}