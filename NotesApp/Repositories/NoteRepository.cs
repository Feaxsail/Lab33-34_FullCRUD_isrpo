using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Models.DTOs;

namespace NotesApp.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _db;
    
    public NoteRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<NoteResponseDto>> GetAllAsync(NoteFilterDto filter)
    {
        var query = _db.Notes
            .Include(n => n.Category)
            .AsQueryable();
        
       
        query = query.Where(n => n.IsArchived == filter.Archived);
        
        if (filter.CategoryId.HasValue)
            query = query.Where(n => n.CategoryId == filter.CategoryId.Value);
        
        if (filter.IsPinned.HasValue)
            query = query.Where(n => n.IsPinned == filter.IsPinned.Value);
        
        if (filter.MinPriority.HasValue)
            query = query.Where(n => n.Priority >= filter.MinPriority.Value);
        
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(n =>
                n.Title.ToLower().Contains(search) ||
                n.Content.ToLower().Contains(search));
        }
        
        
        query = filter.SortBy.ToLower() switch
        {
            "title" => filter.Descending ? query.OrderByDescending(n => n.Title) : query.OrderBy(n => n.Title),
            "priority" => filter.Descending ? query.OrderByDescending(n => n.Priority) : query.OrderBy(n => n.Priority),
            "updatedat" => filter.Descending ? query.OrderByDescending(n => n.UpdatedAt) : query.OrderBy(n => n.UpdatedAt),
            _ => filter.Descending ? query.OrderByDescending(n => n.CreatedAt) : query.OrderBy(n => n.CreatedAt),
        };
        
        
        query = query.OrderByDescending(n => n.IsPinned);
        
        
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);
        
        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NoteResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                IsPinned = n.IsPinned,
                IsArchived = n.IsArchived,
                Priority = n.Priority,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CategoryId = n.CategoryId,
                CategoryName = n.Category.Name,
                CategoryColor = n.Category.Color
            })
            .ToListAsync();
    }
    
    public async Task<NoteResponseDto?> GetByIdAsync(int id)
    {
        return await _db.Notes
            .Include(n => n.Category)
            .Where(n => n.Id == id)
            .Select(n => new NoteResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                IsPinned = n.IsPinned,
                IsArchived = n.IsArchived,
                Priority = n.Priority,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CategoryId = n.CategoryId,
                CategoryName = n.Category.Name,
                CategoryColor = n.Category.Color
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<Note> CreateAsync(Note note)
    {
        _db.Notes.Add(note);
        await _db.SaveChangesAsync();
        return note;
    }
    
    public async Task<Note> UpdateAsync(Note note)
    {
        note.UpdatedAt = DateTime.UtcNow;
        _db.Notes.Update(note);
        await _db.SaveChangesAsync();
        return note;
    }
    
    public async Task DeleteAsync(Note note)
    {
        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
    }
    
    public async Task<Note?> FindAsync(int id)
        => await _db.Notes.FindAsync(id);
    
    public async Task<int> GetCountByCategoryAsync(int categoryId)
        => await _db.Notes.CountAsync(n => n.CategoryId == categoryId);
}