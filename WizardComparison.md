# Wizard Implementation Comparison

This document compares the active wizard examples in the Blazor.Wizard.Demo project: **Inline Fun Wizard**, **Questionary Wizard**, and **Person Wizard**.

---

## Overview

| Feature | Inline Fun Wizard | Questionary Wizard | Person Wizard |
|---------|-------------------|-------------------|--------------|
| **Route** | `/inline-fun-wizard` | `/` | `/` |
| **Complexity** | Minimal starter example | Simple form-based | Advanced with business rules |
| **Host UI** | Inline page | Modal dialog | Modal dialog |
| **Step Registration** | Inline ViewModel factories | Static Registry pattern | DI-based Definition pattern |
| **Service Injection** | No service injection | No service injection | Full DI support in steps |
| **Conditional Logic** | Linear flow | Linear flow | Age-based dynamic routing |
| **Step Visibility** | All steps always visible | All steps always visible | Dynamic (PensionInfo shows/hides) |
| **Live Validation** | Standard form validation | Standard form validation | Real-time field validation |
| **Result Building** | One-way mapper | One-way result builder | Bidirectional mapper (edit mode) |

---

## 1. Simplest Starting Point

### Inline Fun Wizard: Minimal Inline Pattern

**Files:** `Pages/InlineFunWizard.razor`, `Components/InlineFun/*`, `Components/WizardLogic/Fun/*`

This is the smallest practical `Blazor.Wizard` example in the repository. It uses:

- two `FormStepLogic<TModel>` steps
- one `ResultStepLogic<TResultModel>` summary step
- one `ComponentWizardViewModel<TResult>` subclass
- one inline page host with `DynamicComponent`

Use it when you want the fastest path from a plain page to a working wizard without dialog hosting, DI-based step creation, or complex business rules.

---

## 2. Step Registration Patterns

### Person Wizard: Definition Pattern with DI

**File:** `PersonWizardDefinition.cs`

```csharp
public sealed class PersonWizardDefinition
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<PersonStepDefinition> _steps;

    public PersonWizardDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _steps =
            [
                new(
                    EPersonStepId.PersonInfo,
                    typeof(PersonInfoStepLogic),
                    typeof(PersonInfoForm),
                    sp => ActivatorUtilities.CreateInstance<PersonInfoStepLogic>(sp)),
                new(
                    EPersonStepId.Address,
                    typeof(AddressStepLogic),
                    typeof(AddressForm),
                    sp => ActivatorUtilities.CreateInstance<AddressStepLogic>(sp)),
                new(
                    EPersonStepId.PensionInfo,
                    typeof(PensionInfoStepLogic),
                    typeof(PensionInfoForm),
                    sp => ActivatorUtilities.CreateInstance<PensionInfoStepLogic>(sp)),
                new(
                    EPersonStepId.Summary,
                    typeof(SummaryStepLogic),
                    typeof(SummaryView),
                    sp => ActivatorUtilities.CreateInstance<SummaryStepLogic>(sp))
            ];

        ValidateRegistrations();
    }

    public IReadOnlyList<Func<IWizardStep>> CreateStepFactories()
    {
        return _steps
            .Select(step => new Func<IWizardStep>(() => step.StepFactory(_serviceProvider)))
            .ToList();
    }
}
```

**Step Definition Record:**
```csharp
public sealed record PersonStepDefinition(
    EPersonStepId Id,
    Type StepIdType,
    Type ComponentType,
    Func<IServiceProvider, IWizardStep> StepFactory);
```

**Key Features:**
- ✅ Full dependency injection support via `ActivatorUtilities.CreateInstance`
- ✅ Service provider passed to factory
- ✅ Supports constructor injection in step logic classes
- ✅ Enum validation at startup

---

### Questionary Wizard: Static Registry Pattern

**File:** `QuestionaryStepRegistry.cs`

```csharp
public static class QuestionaryStepRegistry
{
    private static readonly QuestionaryResultBuilder _resultBuilder = new();

    private static readonly List<StepRegistration> _steps = new()
    {
        new(
            EQuestionaryStepId.Step1,
            typeof(QuestionaryStep1Model),
            () => new FormStepLogic<QuestionaryStep1Model>(typeof(QuestionaryStep1Model)),
            typeof(QuestionaryStep1)),
        new(
            EQuestionaryStepId.Step2,
            typeof(QuestionaryStep2Model),
            () => new FormStepLogic<QuestionaryStep2Model>(typeof(QuestionaryStep2Model)),
            typeof(QuestionaryStep2)),
        new(
            EQuestionaryStepId.Step3,
            typeof(QuestionaryStep3Model),
            () => new FormStepLogic<QuestionaryStep3Model>(typeof(QuestionaryStep3Model)),
            typeof(QuestionaryStep3)),
        new(
            EQuestionaryStepId.Report,
            typeof(QuestionaryModel),
            () => new ResultStepLogic<QuestionaryModel>(typeof(QuestionaryModel), data => _resultBuilder.Build(data)),
            typeof(QuestionaryReportStep))
    };

    public static IReadOnlyList<Func<IWizardStep>> CreateStepFactories()
    {
        return _steps.Select(step => step.StepFactory).ToList();
    }
}
```

**Step Registration Record:**
```csharp
public sealed record StepRegistration(
    EQuestionaryStepId Id,
    Type StepIdType,
    Func<IWizardStep> StepFactory,
    Type ComponentType);
```

**Key Features:**
- ✅ Uses reusable library steps: `FormStepLogic<TModel>`, `ResultStepLogic<TResultModel>`
- ✅ Static registry (no DI needed)
- ✅ Simple factory functions
- ❌ No service injection in steps
- ✅ Enum validation at startup

---

## 3. Service Injection Examples

### Person Wizard: Full DI Support

**Step Logic with Injected Services:**
```csharp
public sealed class PersonInfoStepLogic : GeneralStepLogic<PersonInfoModel>
{
    private readonly IWizardAnimationService _animationService;

    public PersonInfoStepLogic(IWizardAnimationService animationService)
    {
        _animationService = animationService;
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        // Use injected service
        _animationService.Warn("Too young for wizard mission");
        
        // Access services from WizardData
        var toaster = data.GetService<IToasterService>();
        toaster.ShowWarning("Age must be at least 16 to proceed.");
        
        return new StepResult { NextStepId = typeof(AddressStepLogic) };
    }
}
```

**ViewModel Initialization with DI:**
```csharp
public partial class PersonWizardDialog
{
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private IToasterService Toaster { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new PersonWizardViewModel(
                new PersonModelMapper(),
                new PersonWizardDefinition(ServiceProvider),  // Pass ServiceProvider
                StartupWizardDiagnostics.Create());
            
            _viewModel.Initialize(null);
            _viewModel.Data.AddService(Toaster);  // Add service to WizardData
            await _viewModel.StartAsync();
        }
    }
}
```

**Program.cs Service Registration:**
```csharp
builder.Services.AddScoped<IToasterService, ToasterService>();
builder.Services.AddScoped<IWizardAnimationService, WizardAnimationService>();
```

---

### Questionary Wizard: No Service Injection

**Step Logic (Reusable Library Steps):**
```csharp
// Uses built-in FormStepLogic<TModel> - no custom logic needed
new(
    EQuestionaryStepId.Step1,
    typeof(QuestionaryStep1Model),
    () => new FormStepLogic<QuestionaryStep1Model>(typeof(QuestionaryStep1Model)),
    typeof(QuestionaryStep1))
```

**ViewModel Initialization (No DI):**
```csharp
public partial class QuestionaryWizardDialog
{
    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new QuestionaryWizardViewModel(
                new QuestionaryResultBuilder(),
                StartupWizardDiagnostics.Create());
            
            _viewModel.Initialize(null);
            await _viewModel.StartAsync();
        }
    }
}
```

---

## 4. ViewModel Comparison

### Person Wizard ViewModel

```csharp
public class PersonWizardViewModel : ComponentWizardViewModel<PersonModel>
{
    private readonly PersonWizardDefinition _definition;

    public PersonWizardViewModel(
        IWizardModelBuilder<PersonModel> mapper,
        PersonWizardDefinition definition,
        IWizardDiagnostics? diagnostics = null)
        : base(mapper, diagnostics)
    {
        _definition = definition;
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return _definition.ResolveComponentType(step.Id);
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return _definition.CreateStepFactories();
    }

    // Custom business logic
    public override async Task<bool> NextAsync()
    {
        UpdatePensionInfoIfNeeded();
        return await base.NextAsync();
    }

    // Live validation on field change
    protected override async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (Flow != null && Steps[Flow.Index] is PersonInfoStepLogic personStep)
        {
            var validation = new ValidationResult { IsValid = await personStep.ValidateAsync(Data) };
            personStep.Evaluate(Data, validation);
        }
        await UpdateCanProceedAsync();
    }
}
```

**Features:**
- ✅ Bidirectional mapping (edit mode support)
- ✅ Custom navigation logic
- ✅ Live field validation
- ✅ Dynamic step visibility updates

---

### Questionary Wizard ViewModel

```csharp
public class QuestionaryWizardViewModel : ComponentWizardViewModel<QuestionaryModel>
{
    public QuestionaryWizardViewModel(
        IWizardResultBuilder<QuestionaryModel> resultBuilder,
        IWizardDiagnostics? diagnostics = null)
        : base(resultBuilder, diagnostics)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return QuestionaryStepRegistry.GetByStepIdType(step.Id).ComponentType;
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return QuestionaryStepRegistry.CreateStepFactories();
    }
}
```

**Features:**
- ✅ Simple linear flow
- ✅ One-way result building
- ✅ Minimal custom logic
- ✅ Uses static registry

---

## 5. Result Building Patterns

### Person Wizard: Bidirectional Mapper

```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>, IWizardModelSplitter<PersonModel>
{
    // Build result from wizard data (finish wizard)
    public PersonModel Build(IWizardData data)
    {
        data.TryGet<PersonInfoModel>(out var person);
        data.TryGet<AddressModel>(out var address);
        
        return new PersonModel
        {
            FirstName = person?.FirstName ?? string.Empty,
            LastName = person?.LastName ?? string.Empty,
            Age = person?.Age ?? 0,
            City = address?.City ?? string.Empty
        };
    }

    // Split result into wizard data (edit mode)
    public void Split(PersonModel result, IWizardData data)
    {
        data.Set(new PersonInfoModel
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email,
            Age = result.Age
        });
        data.Set(new AddressModel
        {
            Street = result.Street,
            City = result.City,
            ZipCode = result.ZipCode,
            Country = result.Country
        });
    }
}
```

**Usage in Dialog:**
```csharp
_viewModel.ModelSplitter.Split(Model, _viewModel.Data);  // Prefill wizard
await _viewModel.StartAsync();
```

---

### Questionary Wizard: One-Way Builder

```csharp
public class QuestionaryResultBuilder : IWizardResultBuilder<QuestionaryModel>
{
    public QuestionaryModel Build(IWizardData data)
    {
        var result = new QuestionaryModel();

        if (data.TryGet<QuestionaryStep1Model>(out var step1))
            result.Name = step1.Name;

        if (data.TryGet<QuestionaryStep2Model>(out var step2))
            result.Age = step2.Age;

        if (data.TryGet<QuestionaryStep3Model>(out var step3))
            result.FavoriteColor = step3.FavoriteColor;

        return result;
    }
}
```

**Note:** No `Split` method - wizard always starts empty.

---

## 6. Conditional Logic & Visibility

### Person Wizard: Dynamic Step Visibility

```csharp
public sealed class PensionInfoStepLogic : BaseStepLogic<AddressModel>
{
    private PersonInfoModel? _personInfo;

    public override bool IsVisible => _personInfo?.Age > AgeRuleConstants.MaxPensionAge;

    public void UpdatePersonInfo(PersonInfoModel personInfo)
    {
        _personInfo = personInfo;
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!IsVisible)
        {
            return new StepResult { NextStepId = typeof(SummaryStepLogic) };
        }
        return new StepResult { NextStepId = typeof(SummaryStepLogic) };
    }
}
```

**ViewModel Updates Visibility:**
```csharp
private void UpdatePensionInfoIfNeeded()
{
    var personStep = Steps.OfType<PersonInfoStepLogic>().FirstOrDefault();
    var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
    
    if (personStep != null && pensionStep != null)
    {
        if (Data.TryGet<PersonInfoModel>(out var personInfo))
        {
            pensionStep.UpdatePersonInfo(personInfo);  // Updates IsVisible
        }
    }
}
```

---

### Questionary Wizard: Linear Flow

All steps always visible. No conditional routing.

```csharp
// Simple linear progression
Step1 → Step2 → Step3 → Report
```

---

### Inline Fun Wizard: Linear Flow

All steps always visible. No conditional routing. Same simple progression as the questionary wizard, but hosted inline on a page instead of a dialog.

```csharp
// Simple linear progression
Mood -> Snacks -> Summary
```

---

## 7. When to Use Each Pattern

### Use Inline Fun Wizard Pattern When:

- you want the smallest possible working wizard example
- you prefer an inline page instead of a dialog
- radio buttons and checkboxes are enough for the first version
- no service injection or edit mode is needed
- you want to learn the core flow before adding extra abstractions

**Example Use Cases:**
- onboarding teaser flow
- playful preference picker
- very small setup wizards
- prototype screens

### Use Person Wizard Pattern When:

- ✅ Steps need injected services (logging, API calls, business services)
- ✅ Complex business rules determine step flow
- ✅ Steps need to show/hide dynamically
- ✅ Edit mode required (prefill wizard from existing data)
- ✅ Live validation with custom logic
- ✅ Steps need to communicate with each other

**Example Use Cases:**
- Multi-step registration with age-based rules
- Insurance quote wizard with conditional coverage options
- Onboarding flow with role-based steps
- Configuration wizard with feature toggles

---

### Use Questionary Wizard Pattern When:

- ✅ Simple linear form flow
- ✅ No service injection needed
- ✅ All steps always visible
- ✅ Standard form validation sufficient
- ✅ No edit mode required
- ✅ Minimal custom logic

**Example Use Cases:**
- Simple surveys
- Basic data collection forms
- Linear questionnaires
- Step-by-step tutorials

---

## 8. Adding a New Step

### Person Wizard (DI Pattern)

1. **Add enum value:**
```csharp
public enum EPersonStepId
{
    PersonInfo,
    Address,
    PensionInfo,
    YourNewStep,  // Add here
    Summary
}
```

2. **Create step logic with DI:**
```csharp
public sealed class YourNewStepLogic : GeneralStepLogic<YourModel>
{
    private readonly IYourService _service;

    public YourNewStepLogic(IYourService service)
    {
        _service = service;
    }

    public override Type Id => typeof(YourNewStepLogic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = typeof(SummaryStepLogic) };
    }
}
```

3. **Create Razor component:**
```razor
<EditForm Model="@Model">
    <!-- Your form fields -->
</EditForm>
```

4. **Register in PersonWizardDefinition:**
```csharp
new(
    EPersonStepId.YourNewStep,
    typeof(YourNewStepLogic),
    typeof(YourNewStepComponent),
    sp => ActivatorUtilities.CreateInstance<YourNewStepLogic>(sp))
```

5. **Register service in Program.cs:**
```csharp
builder.Services.AddScoped<IYourService, YourService>();
```

---

### Questionary Wizard (Static Pattern)

1. **Add enum value:**
```csharp
public enum EQuestionaryStepId
{
    Step1,
    Step2,
    Step3,
    YourNewStep,  // Add here
    Report
}
```

2. **Create model:**
```csharp
public class YourStepModel
{
    public string SomeProperty { get; set; } = string.Empty;
}
```

3. **Create Razor component:**
```razor
<EditForm Model="@Model">
    <!-- Your form fields -->
</EditForm>
```

4. **Register in QuestionaryStepRegistry:**
```csharp
new(
    EQuestionaryStepId.YourNewStep,
    typeof(YourStepModel),
    () => new FormStepLogic<YourStepModel>(typeof(YourStepModel)),
    typeof(YourNewStepComponent))
```

5. **Update result builder:**
```csharp
if (data.TryGet<YourStepModel>(out var yourStep))
    result.YourProperty = yourStep.SomeProperty;
```

---

## Summary

| Aspect | Inline Fun Wizard | Questionary Wizard | Person Wizard |
|--------|-------------------|-------------------|--------------|
| **Registration** | Inline ViewModel factories | Static Registry | DI-based Definition |
| **Service Injection** | No service injection | No service injection | Full support |
| **Step Factories** | `Func<IWizardStepViewModel>` | `Func<IWizardStep>` | `Func<IServiceProvider, IWizardStep>` |
| **Custom Logic** | Minimal reusable steps | Reusable library steps | Custom step classes |
| **Complexity** | Very low (starter flow) | Low (simple forms) | High (business rules) |
| **Edit Mode** | No | No | Bidirectional mapper |
| **Dynamic Visibility** | Not used | Not used | Supported |
| **Live Validation** | Standard only | Standard only | Custom logic |
| **Best For** | Tiny starter flows | Simple forms | Complex workflows |
---


## Recommendations

1. **Start with Inline Fun pattern** when you want the absolute smallest example.
2. **Move to Questionary pattern** when you want a reusable simple-form registry structure.
3. **Upgrade to Person pattern** when you need:
   - Service injection in steps
   - Complex conditional logic
   - Dynamic step visibility
   - Edit mode support
4. **All patterns** support:
   - Enum validation at startup
   - DynamicComponent rendering
   - Type-safe step identification
   - Shared WizardData state

---

**Choose the pattern that matches your complexity needs. Don't over-engineer simple wizards, but don't under-engineer complex ones.**

