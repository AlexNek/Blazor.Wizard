using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components;
using System;
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
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;
    private QuestionaryStepFactory? _stepFactory;

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _stepFactory = new QuestionaryStepFactory(ServiceProvider);
            var validator = new QuestionaryValidator();
            var diagnostics = StartupWizardDiagnostics.Create();
            _viewModel = new QuestionaryWizardViewModel(validator, diagnostics, _stepFactory, Model);
        }
        else if (!Visible && _viewModel != null)
        {
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
            _viewModel.MoveNext();
            StateHasChanged();
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

    private async Task OnOkClick()
    {
        if (_viewModel != null)
        {
            var snapshot = _viewModel.CreateSnapshot();
            // Compose the final model from step models
            var builder = new QuestionaryResultBuilder();
            var result = builder.Build(_viewModel.WizardData);
            await OnFinished.InvokeAsync(result);
            Model.Name = result.Name;
            Model.Age = result.Age;
            Model.FavoriteColor = result.FavoriteColor;
            Visible = false;
            await VisibleChanged.InvokeAsync(false);
            _viewModel = null;
            StateHasChanged();
        }
    }

    private async Task OnCancel()
    {
        _viewModel = null;
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
    }

    public void Dispose()
    {
        _viewModel = null;
    }
}
