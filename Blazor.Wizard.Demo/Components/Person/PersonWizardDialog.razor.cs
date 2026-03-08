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
    private bool _isInitializing;

    private bool _isSavingEnabled = true;

    public async Task ShowAsync()
    {
        Visible = true;
        await OnParametersSetAsync();
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null && !_isInitializing)
        {
            _isInitializing = true;
            
            var viewModel = new PersonWizardViewModel(
                new PersonModelMapper(),
                new PersonWizardDefinition(ServiceProvider),
                StartupWizardDiagnostics.Create());
            viewModel.StateChanged += OnViewModelStateChanged;
            viewModel.Initialize(null);

            viewModel.Data.AddService(Toaster);
            
            // Disable auto-save during initialization
            _isSavingEnabled = false;
            
            // Try to load state BEFORE starting wizard
            int savedIndex = -1;
            if (Storage != null && !string.IsNullOrEmpty(StateKey))
            {
                try
                {
                    savedIndex = await viewModel.LoadStateAsync(StateKey, Storage);
                }
                catch
                {
                    // If loading fails, start fresh
                    savedIndex = -1;
                }
            }
            
            // If no state loaded, use the Model parameter
            if (savedIndex < 0 && Model != null)
            {
                try
                {
                    viewModel.ModelSplitter.Split(Model, viewModel.Data);
                }
                catch (InvalidOperationException)
                {
                    // ModelSplitter not implemented, skip prefilling
                }
            }

            // Start wizard (this will enter step 0)
            await viewModel.StartAsync();
            
            // If we have a saved index > 0, navigate to that step
            if (savedIndex > 0 && viewModel.Flow != null && savedIndex < viewModel.Steps.Count)
            {
                viewModel.Flow.Index = savedIndex;
                await viewModel.Steps[savedIndex].EnterAsync(viewModel.Data);
            }
            
            _viewModel = viewModel;
            _isSavingEnabled = true;
            _isInitializing = false;
            StateHasChanged();
        }
        else if (!Visible && _viewModel != null)
        {
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
            _viewModel = null;
            _isInitializing = false;
        }
    }

    private void OnViewModelStateChanged()
    {
        if (_isSavingEnabled && Storage != null && !string.IsNullOrEmpty(StateKey) && _viewModel != null)
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
