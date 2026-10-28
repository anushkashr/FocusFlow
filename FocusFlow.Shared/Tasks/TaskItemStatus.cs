using System.Text.Json.Serialization;

namespace FocusFlow.Shared.Tasks;

// Sent over the API as text ("Pending") instead of a number (0)
[JsonConverter(typeof(JsonStringEnumConverter<TaskItemStatus>))]
public enum TaskItemStatus
{
    Pending,
    Completed
}
