using FocusFlow.Shared.Common;
using FocusFlow.Shared.Tasks;

namespace FocusFlow.Api.Services;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetTasksAsync(int page, int pageSize, TaskItemStatus? status);
    Task<TaskSummary> GetSummaryAsync();
    Task<TaskDto?> GetByIdAsync(int id);
    Task<TaskDto> CreateAsync(TaskInput input);
    Task<TaskDto?> UpdateAsync(int id, TaskInput input);
    Task<TaskDto?> CompleteAsync(int id);
    Task<bool> DeleteAsync(int id);
}
