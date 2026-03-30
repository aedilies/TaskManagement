using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? status, [FromQuery] int? priority,
                                            [FromQuery] Guid? departmentId, [FromQuery] bool orderByDesc = false)
    {
        var tasks = await _taskService.GetFilteredTasksAsync(status, priority, departmentId, orderByDesc);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var task = await _taskService.CreateTaskAsync(dto, currentUserId);
        return Ok(task);
    }

    [HttpPatch("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _taskService.UpdateStatusAsync(dto, currentUserId);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateTaskDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _taskService.UpdateTaskAsync(dto, currentUserId);
        return NoContent();
    }

    [HttpPatch("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignTaskDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _taskService.AssignTaskAsync(dto, currentUserId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _taskService.DeleteTaskAsync(id, currentUserId);
        return NoContent();
    }
}