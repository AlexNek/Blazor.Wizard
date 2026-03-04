using Blazor.Wizard.Demo.Components.WizardLogic.Person;
using Blazor.Wizard.Demo.Models;
using Blazor.Wizard.Demo.Services.Toaster;
using Blazor.Wizard.Extensions;

using Microsoft.AspNetCore.Components;

namespace Blazor.Wizard.Demo.Components.Person;

public partial class PersonWizardDialog : IDisposable
{
    [Inject]
    private IToasterService Toaster { get; set; } = default!;

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

            _viewModel.Data.AddService(Toaster);
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
        StateHasChanged();
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
