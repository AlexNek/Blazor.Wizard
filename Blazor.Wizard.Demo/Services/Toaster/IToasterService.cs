namespace Blazor.Wizard.Demo.Services.Toaster;

public interface IToasterService
{
    event Action? StateChanged;

    IReadOnlyList<ToastMessage> Messages { get; }

    void ShowInfo(string text, int durationMs = 3000);
    void ShowSuccess(string text, int durationMs = 3000);
    void ShowWarning(string text, int durationMs = 3000);
    void ShowError(string text, int durationMs = 3000);
    void Remove(Guid id);
}
