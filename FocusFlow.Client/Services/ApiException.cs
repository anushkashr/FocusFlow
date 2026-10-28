namespace FocusFlow.Client.Services;

// A friendly error message pages can show directly in a toast
public class ApiException(string message, bool isUnauthorized = false) : Exception(message)
{
    public bool IsUnauthorized { get; } = isUnauthorized;
}
