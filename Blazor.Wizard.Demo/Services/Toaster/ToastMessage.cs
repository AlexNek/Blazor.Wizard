namespace Blazor.Wizard.Demo.Services.Toaster;

public sealed record ToastMessage(Guid Id, string Text, ToastLevel Level, DateTimeOffset CreatedAt);
