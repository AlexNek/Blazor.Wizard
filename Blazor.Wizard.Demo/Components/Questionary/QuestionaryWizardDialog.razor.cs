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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            var diagnostics = StartupWizardDiagnostics.Create();
            _viewModel = new QuestionaryWizardViewModel(diagnostics);
            _viewModel.StateChanged += OnViewModelStateChanged;
            _viewModel.Initialize(null);
            await _viewModel.StartAsync();
            StateHasChanged();
        }
        else if (!Visible && _viewModel != null)
        {
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
            _viewModel = null;
        }
    }

    private async Task OnBack()
    {
        if (_viewModel != null)
        {
            await _viewModel.BackAsync();
            StateHasChanged();
        }
    }

    private async Task OnCancel()
    {
        if (_viewModel != null)
        {
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
            _viewModel = null;
        }

        Visible = false;
        await VisibleChanged.InvokeAsync(false);
    }

    private async Task OnNext()
    {
        if (_viewModel != null)
        {
            await _viewModel.NextAsync();
            StateHasChanged();
        }
    }

    private async Task OnOkClick()
    {
        if (_viewModel != null)
        {
            var result = await _viewModel.FinishAsync();
            if (result != null)
            {
                await OnFinished.InvokeAsync(result);
                Visible = false;
                await VisibleChanged.InvokeAsync(false);
                _viewModel.StateChanged -= OnViewModelStateChanged;
                _viewModel.Reset();
                _viewModel = null;
            }

            StateHasChanged();
        }
    }

    private void OnViewModelStateChanged()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        if (_viewModel != null)
        {
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
            _viewModel = null;
        }
    }
}
