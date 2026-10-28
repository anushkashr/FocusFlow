using System.Net;
using System.Net.Http.Json;
using FocusFlow.Client.Auth;
using FocusFlow.Shared.Common;
using FocusFlow.Shared.Tasks;

namespace FocusFlow.Client.Services;

// Every call the Blazor pages make to /api/tasks goes through here
public class TaskApiClient(HttpClient http, JwtAuthStateProvider auth)
{
    public Task<PagedResult<TaskDto>> GetTasksAsync(int page, int pageSize, TaskItemStatus? status)
    {
        var url = $"api/tasks?page={page}&pageSize={pageSize}";
        if (status is not null)
            url += $"&status={status}";

        return SendAsync<PagedResult<TaskDto>>(() => http.GetAsync(url));
    }

    public Task<TaskSummary> GetSummaryAsync() =>
        SendAsync<TaskSummary>(() => http.GetAsync("api/tasks/summary"));

    public Task<TaskDto> CreateAsync(TaskInput input) =>
        SendAsync<TaskDto>(() => http.PostAsJsonAsync("api/tasks", input));

    public Task<TaskDto> UpdateAsync(int id, TaskInput input) =>
        SendAsync<TaskDto>(() => http.PutAsJsonAsync($"api/tasks/{id}", input));

    public Task<TaskDto> CompleteAsync(int id) =>
        SendAsync<TaskDto>(() => http.PatchAsync($"api/tasks/{id}/complete", null));

    public async Task DeleteAsync(int id)
    {
        using var response = await CallAsync(() => http.DeleteAsync($"api/tasks/{id}"));
    }

    private async Task<T> SendAsync<T>(Func<Task<HttpResponseMessage>> call)
    {
        using var response = await CallAsync(call);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new ApiException("The server sent an empty response.");
    }

    // Turns network errors and error status codes into an ApiException with a readable message
    private async Task<HttpResponseMessage> CallAsync(Func<Task<HttpResponseMessage>> call)
    {
        HttpResponseMessage response;
        try
        {
            response = await call();
        }
        catch (HttpRequestException)
        {
            throw new ApiException($"Can't reach the API at {http.BaseAddress}. Is FocusFlow.Api running?");
        }

        if (response.IsSuccessStatusCode)
            return response;

        using (response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    await auth.LogoutAsync();
                    throw new ApiException("Your session has expired. Log in again.", isUnauthorized: true);
                case HttpStatusCode.NotFound:
                    throw new ApiException("That task no longer exists.");
                case HttpStatusCode.BadRequest:
                    throw new ApiException("Some fields aren't valid. Check the form and try again.");
                default:
                    throw new ApiException($"Something went wrong ({(int)response.StatusCode}). Try again.");
            }
        }
    }
}
