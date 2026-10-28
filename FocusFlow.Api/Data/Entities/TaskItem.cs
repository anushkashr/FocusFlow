using FocusFlow.Shared.Tasks;

namespace FocusFlow.Api.Data.Entities;

// Named TaskItem, not Task, so it doesn't clash with System.Threading.Tasks.Task
public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DueDate { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
}
