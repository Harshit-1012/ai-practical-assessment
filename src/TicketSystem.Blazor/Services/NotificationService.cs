namespace TicketSystem.Blazor.Services;

public enum NotificationType
{
    Success,
    Error,
    Info
}

public class ToastMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Message { get; init; }
    public NotificationType Type { get; init; }
}

public interface INotificationService
{
    event Action? OnChange;
    IReadOnlyList<ToastMessage> Messages { get; }
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowInfo(string message);
    void Dismiss(Guid id);
}

public class NotificationService : INotificationService
{
    private readonly List<ToastMessage> _messages = [];

    public event Action? OnChange;

    public IReadOnlyList<ToastMessage> Messages => _messages;

    public void ShowSuccess(string message) => Add(NotificationType.Success, message);

    public void ShowError(string message) => Add(NotificationType.Error, message);

    public void ShowInfo(string message) => Add(NotificationType.Info, message);

    public void Dismiss(Guid id)
    {
        _messages.RemoveAll(m => m.Id == id);
        OnChange?.Invoke();
    }

    private void Add(NotificationType type, string message)
    {
        _messages.Add(new ToastMessage { Message = message, Type = type });
        OnChange?.Invoke();
    }
}
