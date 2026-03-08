using Blazor.Wizard.Demo.Components.Person;
using Blazor.Wizard.Demo.Models.Person;
using Blazor.Wizard.Extensions;

namespace Blazor.Wizard.Demo.Pages;

public partial class WizardPersistenceDemo
{
    private const string StateKey = "person-wizard-demo-state";

    private PersonWizardDialog? _dialog;

    private bool _hasSavedState;

    private PersonModel? _result;

    private bool _showWizard = true;

    private string _statusMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var savedState = await Storage.LoadAsync(StateKey);
        _hasSavedState = !string.IsNullOrEmpty(savedState);
        _showWizard = _hasSavedState;
    }

    private async Task ClearSavedState()
    {
        await WizardPersistenceExtensions.ClearStateAsync(StateKey, Storage);
        _hasSavedState = false;
        _statusMessage = "Saved state cleared";
    }

    private async Task HandleComplete(PersonModel result)
    {
        _result = result;
        _showWizard = false;

        // Reset step index to 0 but keep the data for next time
        if (_dialog?.ViewModel != null)
        {
            _dialog.ViewModel.Flow!.Index = 0;
            try
            {
                await _dialog.ViewModel.SaveStateAsync(StateKey, Storage);
                _hasSavedState = true;
            }
            catch
            {
                // Suppress save errors, don't prevent dialog from closing
            }
        }

        _statusMessage = "Wizard completed successfully";
    }

    private async Task StartNewWizard()
    {
        await WizardPersistenceExtensions.ClearStateAsync(StateKey, Storage);
        _result = null;
        _showWizard = true;
        _hasSavedState = false;
        _statusMessage = "Starting new wizard";
        StateHasChanged();
    }
}
