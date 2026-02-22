using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components;

namespace Blazor.Wizard.Demo.Components.Wizard;

public partial class PersonWizardDialog
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
            _viewModel = new PersonWizardViewModel();
            _viewModel.StateChanged += OnViewModelStateChanged;
            _viewModel.Initialize(null);
            await _viewModel.StartAsync();
            StateHasChanged();
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