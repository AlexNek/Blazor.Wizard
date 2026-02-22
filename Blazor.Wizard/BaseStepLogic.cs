using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard;

/// <summary>
///     Provides core wizard step logic: manages model instance, edit context, lifecycle, and wizard step interface.
///     Use as a base for steps that do not require validation message handling.
/// </summary>
public abstract class BaseStepLogic<TModel> : IWizardStep
{
    private EditContext _context;
    private TModel _model;

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
        if (data.TryGet<TModel>(out var existing))
        {
            _model = existing!;
            _context = new EditContext(_model);
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
        data.Set(_model);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask LeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<bool> ValidateAsync(IWizardData data)
    {
        return ValueTask.FromResult(_context.Validate());
    }
}