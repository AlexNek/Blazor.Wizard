# Service Injection Patterns in Blazor.Wizard

Two architectural approaches for handling dependencies in wizard steps.

---

## Pattern 1: Constructor Injection (Person Wizard)

**When to use:** Steps need services (logging, API calls, business logic)

### Step Registration with DI

```csharp
public sealed class PersonWizardDefinition
{
    private readonly IServiceProvider _serviceProvider;

    public PersonWizardDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _steps =
            [
                new(
                    EPersonStepId.PersonInfo,
                    typeof(PersonInfoStepLogic),
                    typeof(PersonInfoForm),
                    sp => ActivatorUtilities.CreateInstance<PersonInfoStepLogic>(sp))
            ];
    }

    public IReadOnlyList<Func<IWizardStep>> CreateStepFactories()
    {
        return _steps
            .Select(step => new Func<IWizardStep>(() => step.StepFactory(_serviceProvider)))
            .ToList();
    }
}
```

### Step Logic with Injected Services

```csharp
public sealed class PersonInfoStepLogic : GeneralStepLogic<PersonInfoModel>
{
    private readonly IWizardAnimationService _animationService;

    // Constructor injection
    public PersonInfoStepLogic(IWizardAnimationService animationService)
    {
        _animationService = animationService;
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        _animationService.Celebrate("Step validated!");
        return new StepResult { NextStepId = typeof(AddressStepLogic) };
    }
}
```

### Dialog Setup

```csharp
public partial class PersonWizardDialog
{
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        _viewModel = new PersonWizardViewModel(
            new PersonModelMapper(),
            new PersonWizardDefinition(ServiceProvider),  // Pass IServiceProvider
            StartupWizardDiagnostics.Create());
        
        _viewModel.Initialize(null);
        await _viewModel.StartAsync();
    }
}
```

### Service Registration (Program.cs)

```csharp
builder.Services.AddScoped<IWizardAnimationService, WizardAnimationService>();
builder.Services.AddScoped<IToasterService, ToasterService>();
```

**Key Points:**
- ✅ Services injected via constructor
- ✅ `ActivatorUtilities.CreateInstance` resolves dependencies
- ✅ Type-safe, compile-time checked
- ✅ Testable (mock services in unit tests)

---

## Pattern 2: No Injection (Questionary Wizard)

**When to use:** Simple forms with no external dependencies

### Step Registration (Static)

```csharp
public static class QuestionaryStepRegistry
{
    private static readonly List<StepRegistration> _steps = new()
    {
        new(
            EQuestionaryStepId.Step1,
            typeof(QuestionaryStep1Model),
            () => new FormStepLogic<QuestionaryStep1Model>(typeof(QuestionaryStep1Model)),
            typeof(QuestionaryStep1))
    };

    public static IReadOnlyList<Func<IWizardStep>> CreateStepFactories()
    {
        return _steps.Select(step => step.StepFactory).ToList();
    }
}
```

### Step Logic (Reusable Library Steps)

```csharp
// No custom step class needed - uses built-in FormStepLogic<TModel>
// Factory: () => new FormStepLogic<QuestionaryStep1Model>(typeof(QuestionaryStep1Model))
```

### Dialog Setup

```csharp
public partial class QuestionaryWizardDialog
{
    protected override async Task OnParametersSetAsync()
    {
        _viewModel = new QuestionaryWizardViewModel(
            new QuestionaryResultBuilder(),
            StartupWizardDiagnostics.Create());
        
        _viewModel.Initialize(null);
        await _viewModel.StartAsync();
    }
}
```

**Key Points:**
- ✅ No DI infrastructure needed
- ✅ Static registry, simple factories
- ✅ Uses library's reusable steps
- ✅ Minimal boilerplate

---

## Pattern 3: WizardData Service Locator (Both Wizards)

**When to use:** Pass services to steps at runtime (not compile-time)

### Adding Services to WizardData

```csharp
public partial class PersonWizardDialog
{
    [Inject]
    private IToasterService Toaster { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        _viewModel = new PersonWizardViewModel(...);
        _viewModel.Initialize(null);
        
        // Add service to WizardData
        _viewModel.Data.AddService(Toaster);
        
        await _viewModel.StartAsync();
    }
}
```

### Accessing Services in Step Logic

```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    // Retrieve service from WizardData
    var toaster = data.GetService<IToasterService>();
    toaster.ShowSuccess("Validation passed!");
    
    return new StepResult { NextStepId = typeof(NextStep) };
}
```

**Key Points:**
- ✅ Runtime service resolution
- ✅ No constructor changes needed
- ✅ Useful for cross-cutting concerns
- ⚠️ Not type-safe (runtime errors if service missing)

---

## Comparison Matrix

| Aspect | Constructor Injection | No Injection | WizardData Locator |
|--------|----------------------|--------------|-------------------|
| **Type Safety** | ✅ Compile-time | ✅ N/A | ❌ Runtime |
| **Testability** | ✅ Easy mocking | ✅ No deps | ⚠️ Manual setup |
| **Boilerplate** | ⚠️ Medium | ✅ Minimal | ✅ Low |
| **Flexibility** | ✅ High | ❌ None | ✅ High |
| **Use Case** | Business logic | Simple forms | Cross-cutting |

---

## Decision Guide

### Choose Constructor Injection when:
- Steps need external services (API, logging, validation)
- Business logic requires testable dependencies
- Type safety is important
- Example: `PersonInfoStepLogic` needs `IWizardAnimationService`

### Choose No Injection when:
- Simple data collection forms
- No external dependencies
- Using library's built-in steps (`FormStepLogic`, `ResultStepLogic`)
- Example: Basic questionnaire with standard validation

### Choose WizardData Locator when:
- Services needed across multiple steps
- Runtime service resolution acceptable
- Avoiding constructor changes
- Example: Toaster notifications, shared state

---

## Migration Path

**From No Injection → Constructor Injection:**

1. Create `WizardDefinition` class accepting `IServiceProvider`
2. Change factory: `() => new Step()` → `sp => ActivatorUtilities.CreateInstance<Step>(sp)`
3. Add constructor parameters to step logic
4. Register services in `Program.cs`
5. Pass `ServiceProvider` to definition in dialog

**From WizardData Locator → Constructor Injection:**

1. Move `data.GetService<T>()` calls to constructor parameters
2. Update step factories to use `ActivatorUtilities.CreateInstance`
3. Remove `data.AddService()` calls from dialog

---

## Example: Hybrid Approach

Combine patterns for maximum flexibility:

```csharp
public sealed class HybridStepLogic : GeneralStepLogic<Model>
{
    private readonly ILogger _logger;  // Constructor injection

    public HybridStepLogic(ILogger logger)
    {
        _logger = logger;
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        _logger.LogInformation("Step evaluated");
        
        // WizardData locator for runtime services
        var toaster = data.GetService<IToasterService>();
        toaster?.ShowSuccess("Step completed");
        
        return new StepResult { NextStepId = typeof(NextStep) };
    }
}
```

**Best Practice:** Use constructor injection for required dependencies, WizardData locator for optional cross-cutting concerns.
