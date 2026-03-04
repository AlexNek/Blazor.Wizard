namespace Blazor.Wizard.Demo.Services.Toaster;

public sealed class ToasterService : IToasterService
{
    private readonly object _sync = new();
    private readonly List<ToastMessage> _messages = new();

    public event Action? StateChanged;

    public IReadOnlyList<ToastMessage> Messages
    {
        get
        {
            lock (_sync)
            {
                return _messages.ToList();
            }
        }
    }

    public void ShowInfo(string text, int durationMs = 3000)
    {
        Show(ToastLevel.Info, text, durationMs);
    }

    public void ShowSuccess(string text, int durationMs = 3000)
    {
        Show(ToastLevel.Success, text, durationMs);
    }

    public void ShowWarning(string text, int durationMs = 3000)
    {
        Show(ToastLevel.Warning, text, durationMs);
    }

    public void ShowError(string text, int durationMs = 3000)
    {
        Show(ToastLevel.Error, text, durationMs);
    }

    public void Remove(Guid id)
    {
        lock (_sync)
        {
            _messages.RemoveAll(m => m.Id == id);
        }

        StateChanged?.Invoke();
    }

    private void Show(ToastLevel level, string text, int durationMs)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var toast = new ToastMessage(Guid.NewGuid(), text, level, DateTimeOffset.UtcNow);

        lock (_sync)
        {
            _messages.Add(toast);
        }

        StateChanged?.Invoke();
        _ = RemoveLaterAsync(toast.Id, durationMs);
    }

    private async Task RemoveLaterAsync(Guid id, int durationMs)
    {
        var safeDuration = Math.Max(500, durationMs);
        await Task.Delay(safeDuration);
        Remove(id);
    }
}
