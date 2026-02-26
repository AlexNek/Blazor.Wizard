using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;

using Microsoft.AspNetCore.Components;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryWizardDialog : IDisposable
{
    private QuestionaryWizardViewModel? _viewModel;

    [Parameter]
    public QuestionaryModel Model { get; set; } = new();

    [Parameter]
    public EventCallback<QuestionaryModel> OnFinished { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    public void Dispose()
    {
        _viewModel = null;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            var validator = new QuestionaryValidator();
            var diagnostics = StartupWizardDiagnostics.Create();
            _viewModel = new QuestionaryWizardViewModel(
                validator,
                diagnostics,
                Model);
        }
        else if (!Visible && _viewModel != null)
        {
            _viewModel = null;
        }
    }

    private async Task OnBack()
    {
        if (_viewModel != null)
        {
            _viewModel.MoveBack();
            StateHasChanged();
        }
    }

    private async Task OnCancel()
    {
        _viewModel = null;
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
    }

    private async Task OnNext()
    {
        if (_viewModel != null)
        {
            var result = _viewModel.TryProceed();
            StateHasChanged();
        }
    }

    private async Task OnOkClick()
    {
        if (_viewModel != null && _viewModel.TryProceed().CanProceed)
        {
            var resultBuilder = new QuestionaryResultBuilder();
            await OnFinished.InvokeAsync(resultBuilder.Build(_viewModel.WizardData));
            Visible = false;
            await VisibleChanged.InvokeAsync(false);
            _viewModel = null;
            StateHasChanged();
        }
    }

    private void OnViewModelStateChanged()
    {
        StateHasChanged();
    }
}
