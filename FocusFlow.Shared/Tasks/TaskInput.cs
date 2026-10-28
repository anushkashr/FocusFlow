using System.ComponentModel.DataAnnotations;
using FocusFlow.Shared.Validation;

namespace FocusFlow.Shared.Tasks;

// What the client sends to create or edit a task.
// The same rules run in the Blazor form and again in the API.
public class TaskInput
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title must be 100 characters or less")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description must be 500 characters or less")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Due date is required")]
    [FutureDate(ErrorMessage = "Due date can't be in the past")]
    public DateOnly? DueDate { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
}
