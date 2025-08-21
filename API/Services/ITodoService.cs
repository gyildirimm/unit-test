using API.DTOs;

namespace API.Services;

public interface ITodoService
{
    Task<List<TodoReadDto>> GetAllAsync(CancellationToken ct = default);
    Task<TodoReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoReadDto> CreateAsync(TodoCreateDto dto, CancellationToken ct = default);
    Task<TodoReadDto?> UpdateAsync(int id, TodoUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

