using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Tasks;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskRequest request)
        => Ok(await _taskService.CreateTaskAsync(request));

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskRequest request)
    {
        await _taskService.UpdateTaskAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/move")]
    public async Task<IActionResult> MoveTask(Guid id, MoveTaskRequest request)
        => Ok(await _taskService.MoveTaskAsync(id, request));
}