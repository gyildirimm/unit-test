using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("todos")]
public class TodosController(ITodoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoReadDto>>> GetAll(CancellationToken ct)
    {
        var result = await service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoReadDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TodoReadDto>> Create([FromBody] TodoCreateDto input, CancellationToken ct)
    {
        var dto = await service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoReadDto>> Update(int id, [FromBody] TodoUpdateDto input, CancellationToken ct)
    {
        var dto = await service.UpdateAsync(id, input, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

