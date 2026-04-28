using NotesApp.Models;
using NotesApp.Models.DTOs;

namespace NotesApp.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<NoteResponseDto>> GetAllAsync(NoteFilterDto filter);
    Task<NoteResponseDto?> GetByIdAsync(int id);
    Task<Note> CreateAsync(Note note);
    Task<Note> UpdateAsync(Note note);
    Task DeleteAsync(Note note);
    Task<Note?> FindAsync(int id);
    Task<int> GetCountByCategoryAsync(int categoryId);
}