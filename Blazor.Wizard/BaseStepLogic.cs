using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Blazor.Wizard;

/// <summary>
///     Provides core wizard step logic: manages model instance, edit context, lifecycle, and wizard step interface.
///     Use as a base for steps that do not require validation message handling.
/// </summary>
public abstract class BaseStepLogic<TModel> : IWizardStep
{
    private EditContext _context;
    private TModel _model;
    protected ILogger? Logger { get; set; }

    public abstract Type Id { get; }
    public virtual bool IsVisible { get; protected set; } = true;

    protected BaseStepLogic(Func<TModel>? modelFactory = null)
    {
        if (modelFactory != null)
        {
            _model = modelFactory();
        }
        else
        {
            var ctor = typeof(TModel).GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"TModel {typeof(TModel).Name} must have a parameterless constructor or provide a factory.");
            }

            _model = Activator.CreateInstance<TModel>();
        }

        _context = new EditContext(_model);
    }

    public abstract StepResult Evaluate(IWizardData data, ValidationResult validation);

    public virtual ValueTask EnterAsync(IWizardData data)
    {
        Logger?.LogDebug("Entering step {StepId}", Id.Name);
        if (data.TryGet<TModel>(out var existing))
        {
            _model = existing!;
            _context = new EditContext(_model);
            Logger?.LogDebug("Loaded existing model for {StepId}", Id.Name);
        }
        else
        {
            data.Set(_model);
            Logger?.LogDebug("Created new model for {StepId}", Id.Name);
        }

        return ValueTask.CompletedTask;
    }

    public EditContext GetEditContext()
    {
        return _context;
    }

    public TModel GetModel()
    {
        return _model;
    }

    public virtual ValueTask BeforeLeaveAsync(IWizardData data)
    {
        Logger?.LogDebug("Before leaving step {StepId}", Id.Name);
        data.Set(_model);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask LeaveAsync(IWizardData data)
    {
        Logger?.LogDebug("Leaving step {StepId}", Id.Name);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<bool> ValidateAsync(IWizardData data)
    {
        Logger?.LogDebug("Validating step {StepId}", Id.Name);
        var isValid = _context.Validate();
        Logger?.LogDebug("Validation result for {StepId}: {IsValid}", Id.Name, isValid);
        if (!isValid)
        {
            var errors = _context.GetValidationMessages();
            Logger?.LogWarning("Validation failed for {StepId}: {Errors}", Id.Name, string.Join(", ", errors));
        }
        return ValueTask.FromResult(isValid);
    }
}