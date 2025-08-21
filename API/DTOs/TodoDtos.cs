namespace API.DTOs;

public record TodoReadDto(int Id, string Title, bool IsDone);

public record TodoCreateDto
{
    public string Title { get; init; } = string.Empty;
    public bool IsDone { get; init; }
}

public record TodoUpdateDto
{
    public string Title { get; init; } = string.Empty;
    public bool IsDone { get; init; }
}

