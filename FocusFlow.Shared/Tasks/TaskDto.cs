namespace FocusFlow.Shared.Tasks;

// What the API sends back for one task
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DueDate { get; set; }
    public TaskItemStatus Status { get; set; }

    public bool IsOverdue =>
        Status == TaskItemStatus.Pending && DueDate < DateOnly.FromDateTime(DateTime.Today);
}
