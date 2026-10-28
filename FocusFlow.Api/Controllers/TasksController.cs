using FocusFlow.Api.Services;
using FocusFlow.Shared.Common;
using FocusFlow.Shared.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FocusFlow.Api.Controllers;

// Reading is public. Every change needs a valid JWT ([Authorize]).
[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    // GET api/tasks?page=1&pageSize=5&status=Pending   (leave out status for all)
    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
        int page = 1, int pageSize = 5, TaskItemStatus? status = null) =>
        await taskService.GetTasksAsync(page, pageSize, status);

    // GET api/tasks/summary
    [HttpGet("summary")]
    public async Task<ActionResult<TaskSummary>> GetSummary() =>
        await taskService.GetSummaryAsync();

    // GET api/tasks/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await taskService.GetByIdAsync(id);
        return task is null ? NotFound() : task;
    }

    // POST api/tasks
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(TaskInput input)
    {
        var task = await taskService.CreateAsync(input);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    // PUT api/tasks/5
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> Update(int id, TaskInput input)
    {
        var task = await taskService.UpdateAsync(id, input);
        return task is null ? NotFound() : task;
    }

    // PATCH api/tasks/5/complete
    [Authorize]
    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<TaskDto>> Complete(int id)
    {
        var task = await taskService.CompleteAsync(id);
        return task is null ? NotFound() : task;
    }

    // DELETE api/tasks/5
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await taskService.DeleteAsync(id) ? NoContent() : NotFound();
}
