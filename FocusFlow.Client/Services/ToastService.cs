namespace FocusFlow.Client.Services;

public enum ToastKind { Success, Error, Info }

public record Toast(Guid Id, string Message, ToastKind Kind);

// Small pop-up messages ("Task created"), shown by the <Toasts> component in the layout
public class ToastService
{
    private readonly List<Toast> _toasts = [];

    public IReadOnlyList<Toast> Toasts => _toasts;
    public event Action? Changed;

    public void Success(string message) => Show(message, ToastKind.Success);
    public void Error(string message) => Show(message, ToastKind.Error);
    public void Info(string message) => Show(message, ToastKind.Info);

    public void Dismiss(Guid id)
    {
        _toasts.RemoveAll(t => t.Id == id);
        Changed?.Invoke();
    }

    private async void Show(string message, ToastKind kind)
    {
        var toast = new Toast(Guid.NewGuid(), message, kind);
        _toasts.Add(toast);
        Changed?.Invoke();

        await Task.Delay(TimeSpan.FromSeconds(4));
        Dismiss(toast.Id);
    }
}
