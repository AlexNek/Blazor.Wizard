using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Core;

/// <summary>
///     Extends BaseStepLogic with validation message handling for wizard steps.
///     Use as a base for steps that require field-level validation and error management.
/// </summary>
public abstract class GeneralStepLogic<TModel> : BaseStepLogic<TModel>
{
    protected ValidationMessageStore? ValidationMessageStore { get; private set; }
    private EditContext? _lastEditContext;

    protected void AddValidationError(EditContext? editContext, string fieldName, string errorMessage)
    {
        if (ValidationMessageStore != null && editContext != null)
        {
            ValidationMessageStore.Add(editContext.Field(fieldName), errorMessage);
            editContext.NotifyValidationStateChanged();
        }
    }

    protected void ClearValidation(EditContext? editContext, string fieldName)
    {
        if (ValidationMessageStore != null && editContext != null)
        {
            ValidationMessageStore.Clear(editContext.Field(fieldName));
        }
    }

    protected void EnsureValidationMessageStore(EditContext? editContext)
    {
        if (editContext != null)
        {
            // Recreate ValidationMessageStore if EditContext changed
            if (ValidationMessageStore == null || _lastEditContext != editContext)
            {
                ValidationMessageStore = new ValidationMessageStore(editContext);
                _lastEditContext = editContext;
            }
        }
    }

    protected void NotifyValidation(EditContext? editContext)
    {
        if (editContext != null)
        {
            editContext.NotifyValidationStateChanged();
        }
    }
}