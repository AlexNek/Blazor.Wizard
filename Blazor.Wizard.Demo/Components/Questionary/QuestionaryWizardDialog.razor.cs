using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryWizardDialog : IDisposable
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public QuestionaryModel Model { get; set; } = new();

    [Parameter]
    public EventCallback<QuestionaryModel> OnFinished { get; set; }

    private QuestionaryWizardViewModel? _viewModel;

    private static readonly List<Func<IWizardStep>> _stepFactories = new()
    {
        () => new QuestionaryStep1Logic(),
        () => new QuestionaryStep2Logic(),
        () => new QuestionaryStep3Logic(),
        () => new QuestionaryReportStepLogic()
    };

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new QuestionaryWizardViewModel();
            _viewModel.StateChanged += OnViewModelStateChanged;
            _viewModel.Initialize(_stepFactories);
            await _viewModel.StartAsync();
            StateHasChanged();
        }
        else if (!Visible && _viewModel != null)
        {
            // Clean up when dialog is hidden
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
            _viewModel = null;
        }
    }

    private void OnViewModelStateChanged()
    {
        StateHasChanged();
    }

    private async Task OnNext()
    {
        if (_viewModel != null)
        {
            var result = await _viewModel.NextAsync();
            StateHasChanged();
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
                _viewModel.Reset();
                _viewModel = null;
            }

            StateHasChanged();
        }
    }

    private async Task OnCancel()
    {
        if (_viewModel != null)
        {
            _viewModel.Reset();
            _viewModel = null;
        }

        Visible = false;
        await VisibleChanged.InvokeAsync(false);
    }

    public void Dispose()
    {
        if (_viewModel != null)
        {
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
        }
    }
}
