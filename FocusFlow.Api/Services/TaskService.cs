using FocusFlow.Api.Data;
using FocusFlow.Api.Data.Entities;
using FocusFlow.Shared.Common;
using FocusFlow.Shared.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FocusFlow.Api.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public const int MaxPageSize = 50;

    public async Task<PagedResult<TaskDto>> GetTasksAsync(int page, int pageSize, TaskItemStatus? status)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = db.Tasks.AsNoTracking();
        if (status is not null)
            query = query.Where(t => t.Status == status);

        var totalCount = await query.CountAsync();

        // Pending first, then soonest due date first
        var items = await query
            .OrderBy(t => t.Status == TaskItemStatus.Completed)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => ToDto(t))
            .ToListAsync();

        return new PagedResult<TaskDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TaskSummary> GetSummaryAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var pending = await db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Pending);
        var completed = await db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Completed);
        var overdue = await db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Pending && t.DueDate < today);
        return new TaskSummary(pending, completed, overdue);
    }

    public async Task<TaskDto?> GetByIdAsync(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        return task is null ? null : ToDto(task);
    }

    public async Task<TaskDto> CreateAsync(TaskInput input)
    {
        var task = new TaskItem();
        Apply(input, task);

        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return ToDto(task);
    }

    public async Task<TaskDto?> UpdateAsync(int id, TaskInput input)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return null;

        Apply(input, task);
        task.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return ToDto(task);
    }

    public async Task<TaskDto?> CompleteAsync(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return null;

        task.Status = TaskItemStatus.Completed;
        task.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return ToDto(task);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return false;

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }

    private static void Apply(TaskInput input, TaskItem task)
    {
        task.Title = input.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
        task.DueDate = input.DueDate!.Value;
        task.Status = input.Status;
    }

    private static TaskDto ToDto(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        DueDate = task.DueDate,
        Status = task.Status
    };
}
