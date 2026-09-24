using Checklist.Api.DTOs;
using Checklist.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Checklist.Api.Controllers;

/// <summary>
/// Exposes CRUD endpoints for todo items.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private const string GetTodoByIdRouteName = "GetTodoById";
    private readonly ITodoService _todoService;

    /// <summary>
    /// Creates a controller with the todo service dependency.
    /// </summary>
    /// <param name="todoService">Todo service.</param>
    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    /// <summary>
    /// Returns all todo items for the default local user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TodoItemReadDto>>> GetAllAsync()
    {
        var todos = await _todoService.GetAllAsync();
        return Ok(todos);
    }

    /// <summary>
    /// Returns one todo by id.
    /// </summary>
    /// <param name="id">Todo identifier.</param>
    [HttpGet("{id:int}", Name = GetTodoByIdRouteName)]
    public async Task<ActionResult<TodoItemReadDto>> GetByIdAsync([FromRoute] int id)
    {
        var todo = await _todoService.GetByIdAsync(id);
        if (todo is null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="createDto">Todo payload.</param>
    [HttpPost]
    public async Task<ActionResult<TodoItemReadDto>> CreateAsync([FromBody] TodoItemCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _todoService.CreateAsync(createDto);
        return CreatedAtRoute(GetTodoByIdRouteName, new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="id">Todo identifier.</param>
    /// <param name="updateDto">Updated values.</param>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItemReadDto>> UpdateAsync([FromRoute] int id, [FromBody] TodoItemUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _todoService.UpdateAsync(id, updateDto);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Toggles completion state for a todo item.
    /// </summary>
    /// <param name="id">Todo identifier.</param>
    [HttpPatch("{id:int}/toggle")]
    public async Task<ActionResult<TodoItemReadDto>> ToggleAsync([FromRoute] int id)
    {
        var updated = await _todoService.ToggleCompleteAsync(id);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Deletes one todo item.
    /// </summary>
    /// <param name="id">Todo identifier.</param>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var deleted = await _todoService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
