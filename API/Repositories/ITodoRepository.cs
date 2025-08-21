using API.Models;

namespace API.Repositories;

public interface ITodoRepository
{
    Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoItem> AddAsync(TodoItem entity, CancellationToken ct = default);
    Task<TodoItem?> UpdateAsync(TodoItem entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

