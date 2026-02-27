using Blazor.Wizard.DemoDevEx.Models;
using Microsoft.AspNetCore.Components;

namespace Blazor.Wizard.DemoDevEx.Components.Wizard;

public partial class PersonWizardDialog : IDisposable
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public PersonModel Model { get; set; } = new();

    [Parameter]
    public EventCallback<PersonModel> OnFinished { get; set; }

    private PersonWizardViewModel? _viewModel;

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new PersonWizardViewModel(
                new PersonModelMapper(),
                StartupWizardDiagnostics.Create());
            _viewModel.StateChanged += OnViewModelStateChanged;
            _viewModel.Initialize(null);
            
            _viewModel.ModelSplitter.Split(Model, _viewModel.Data);
            
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

    private void OnViewModelStateChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private async Task OnCancel()
    {
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
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
