using API.DTOs;
using API.Models;
using API.Repositories;

namespace API.Services;

public class TodoService(ITodoRepository repo) : ITodoService
{
    public async Task<List<TodoReadDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await repo.GetAllAsync(ct);
        return items.Select(t => new TodoReadDto(t.Id, t.Title, t.IsDone)).ToList();
    }

    public async Task<TodoReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await repo.GetByIdAsync(id, ct);
        return item == null ? null : new TodoReadDto(item.Id, item.Title, item.IsDone);
    }

    public async Task<TodoReadDto> CreateAsync(TodoCreateDto dto, CancellationToken ct = default)
    {
        var entity = new TodoItem { Title = dto.Title.Trim(), IsDone = dto.IsDone };
        entity = await repo.AddAsync(entity, ct);
        return new TodoReadDto(entity.Id, entity.Title, entity.IsDone);
    }

    public async Task<TodoReadDto?> UpdateAsync(int id, TodoUpdateDto dto, CancellationToken ct = default)
    {
        var updated = await repo.UpdateAsync(new TodoItem { Id = id, Title = dto.Title.Trim(), IsDone = dto.IsDone }, ct);
        return updated == null ? null : new TodoReadDto(updated.Id, updated.Title, updated.IsDone);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => repo.DeleteAsync(id, ct);
}

