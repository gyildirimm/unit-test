using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class TodoRepository(ApplicationDbContext db) : ITodoRepository
{
    public async Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default)
        => await db.Todos.AsNoTracking().ToListAsync(ct);

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => await db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<TodoItem> AddAsync(TodoItem entity, CancellationToken ct = default)
    {
        db.Todos.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<TodoItem?> UpdateAsync(TodoItem entity, CancellationToken ct = default)
    {
        var existing = await db.Todos.FindAsync(new object?[] { entity.Id }, ct);
        if (existing == null) return null;
        
        existing.Title = entity.Title;
        existing.IsDone = entity.IsDone;
        
        await db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await db.Todos.FindAsync(new object?[] { id }, ct);
        if (existing == null) return false;
        
        db.Todos.Remove(existing);
        
        await db.SaveChangesAsync(ct);
        
        return true;
    }
}

