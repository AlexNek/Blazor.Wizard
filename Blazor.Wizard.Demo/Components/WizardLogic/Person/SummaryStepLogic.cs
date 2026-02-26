using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public class SummaryStepLogic : IWizardStep
{
    private readonly PersonModelResultBuilder _resultBuilder = new();
    private IWizardData? _cachedData;
    private string? _demoParameter;
    private bool _showPension;
    
    public Type Id => typeof(SummaryStepLogic);
    public bool IsVisible => true;

    public void SetDemoParameter(string demoParameter)
    {
        _demoParameter = demoParameter;
    }

    public void SetShowPension(bool showPension)
    {
        _showPension = showPension;
    }

    public Dictionary<string, object> GetComponentParameters()
    {
        var parameters = new Dictionary<string, object>();
        
        try
        {
            if (_cachedData != null)
            {
                var resultModel = _resultBuilder.Build(_cachedData);
                parameters["Model"] = resultModel;
                parameters["ShowPension"] = _showPension;
                
                if (!string.IsNullOrEmpty(_demoParameter))
                {
                    parameters["DemoParameter"] = _demoParameter;
                }
            }
            else
            {
                // Fallback for initial render before EnterAsync
                parameters["Model"] = new PersonModel();
                parameters["ShowPension"] = false;
            }
        }
        catch (Exception)
        {
            // Defensive: if build fails, return empty model to prevent crash
            parameters["Model"] = new PersonModel();
            parameters["ShowPension"] = false;
        }
        
        return parameters;
    }

    public ValueTask BeforeLeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask EnterAsync(IWizardData data)
    {
        _cachedData = data;
        return ValueTask.CompletedTask;
    }

    public StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = null, StayOnStep = false, CanContinue = true };
    }

    /// <summary>
    ///     Called when the wizard leaves this step. Allows cleanup or saving state.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public ValueTask LeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> ValidateAsync(IWizardData data)
    {
        return ValueTask.FromResult(true);
    }
}