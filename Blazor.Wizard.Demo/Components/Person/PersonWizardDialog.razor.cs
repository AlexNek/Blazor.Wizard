using Blazor.Wizard.Demo.Components.WizardLogic.Person;
using Blazor.Wizard.Demo.Models.Person;
using Blazor.Wizard.Demo.Services.Toaster;
using Blazor.Wizard.Extensions;
using Blazor.Wizard.Interfaces;

using Microsoft.AspNetCore.Components;

namespace Blazor.Wizard.Demo.Components.Person;

public partial class PersonWizardDialog : IDisposable
{
    [Inject]
    private IToasterService Toaster { get; set; } = default!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private IWizardStateStorage? Storage { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public PersonModel Model { get; set; } = new();

    [Parameter]
    public EventCallback<PersonModel> OnFinished { get; set; }

    [Parameter]
    public EventCallback OnComplete { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public string? StateKey { get; set; }

    public PersonWizardViewModel ViewModel => _viewModel!;

    private PersonWizardViewModel? _viewModel;

    public async Task ShowAsync()
    {
        Visible = true;
        await OnParametersSetAsync();
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new PersonWizardViewModel(
                new PersonModelMapper(),
                new PersonWizardDefinition(ServiceProvider),
                StartupWizardDiagnostics.Create());
            _viewModel.StateChanged += OnViewModelStateChanged;
            _viewModel.Initialize(null);

            _viewModel.Data.AddService(Toaster);
            
            // Try to load state BEFORE starting wizard
            bool stateLoaded = false;
            if (Storage != null && !string.IsNullOrEmpty(StateKey))
            {
                stateLoaded = await _viewModel.LoadStateAsync(StateKey, Storage);
            }
            
            // If no state loaded, use the Model parameter
            if (!stateLoaded)
            {
                _viewModel.ModelSplitter.Split(Model, _viewModel.Data);
            }

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
        if (Storage != null && !string.IsNullOrEmpty(StateKey) && _viewModel != null)
        {
            _ = _viewModel.SaveStateAsync(StateKey, Storage);
        }
        StateHasChanged();
    }

    private async Task OnCancelInternal()
    {
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
        await OnCancel.InvokeAsync();
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
