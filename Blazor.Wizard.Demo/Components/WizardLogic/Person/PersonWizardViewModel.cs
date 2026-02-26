using System.Diagnostics;

using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Models;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public class PersonWizardViewModel : ComponentWizardViewModel<PersonModel>
{
    public PersonWizardViewModel(
        IWizardResultBuilder<PersonModel> resultBuilder,
        IWizardDiagnostics? diagnostics = null) 
        : base(resultBuilder, diagnostics)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return PersonStepRegistry.GetByStepIdType(step.Id).ComponentType;
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return PersonStepRegistry.CreateStepFactories();
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        base.Initialize(stepFactories);

        // Update PensionInfoStepLogic visibility based on PersonInfoModel
        var personStep = Steps.OfType<PersonInfoStepLogic>().FirstOrDefault();
        var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
        if (personStep != null && pensionStep != null)
        {
            if (Data.TryGet<PersonInfoModel>(out var personInfo))
            {
                pensionStep.UpdatePersonInfo(personInfo);
            }
        }
    }

    public override async Task<bool> NextAsync()
    {
        UpdatePensionInfoIfNeeded();
        
        // Store data for summary before moving to it
        if (Flow != null && Flow.Index < Steps.Count - 1)
        {
            var personStep = Steps.OfType<PersonInfoStepLogic>().FirstOrDefault();
            var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
            var summaryStep = Steps.OfType<SummaryStepLogic>().FirstOrDefault();
            
            if (summaryStep != null)
            {
                if (personStep != null)
                {
                    summaryStep.SetDemoParameter(personStep.DemoParameter);
                }
                
                if (pensionStep != null)
                {
                    summaryStep.SetShowPension(pensionStep.IsVisible);
                }
            }
        }
        
        return await base.NextAsync();
    }

    /// <summary>
    ///     WARNING: async void is required here because EditContext.OnFieldChanged expects a void event handler.
    ///     All exceptions are caught and logged to avoid unhandled errors.
    /// </summary>
    protected override async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        try
        {
            // Run business logic validation live for PersonInfoStepLogic
            if (Flow != null && Steps.Count > 0 && Flow.Index >= 0 && Flow.Index < Steps.Count)
            {
                var step = Steps[Flow.Index];
                if (step is PersonInfoStepLogic personStep)
                {
                    var validation = new ValidationResult { IsValid = await personStep.ValidateAsync(Data) };
                    personStep.Evaluate(Data, validation);
                }
            }

            // Update CanProceed AFTER business logic validation has run
            // This ensures validation errors are cleared/added before we check the state
            await UpdateCanProceedAsync();
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Exception in OnFieldChanged: {ex}");
        }
    }

    private void UpdatePensionInfoIfNeeded()
    {
        var personStep = Steps.OfType<PersonInfoStepLogic>().FirstOrDefault();
        var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
        if (personStep != null && pensionStep != null)
        {
            if (Data.TryGet<PersonInfoModel>(out var personInfo))
            {
                pensionStep.UpdatePersonInfo(personInfo);
            }
        }
    }
}