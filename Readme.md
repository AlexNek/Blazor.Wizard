# Blazor.Wizard

[![NuGet](https://img.shields.io/nuget/v/Blazor.Wizard.svg)](https://www.nuget.org/packages/Blazor.Wizard/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A wizard framework for Blazor focused on step orchestration, validation, shared state, and conditional navigation.

## Key Features

- UI-agnostic core (works with Bootstrap, DevExpress, MudBlazor, custom UI)
- Step lifecycle (`EnterAsync`, `ValidateAsync`, `Evaluate`, `BeforeLeaveAsync`, `LeaveAsync`)
- Type-safe shared state with `WizardData`
- Conditional routing with `StepResult.NextStepId`
- Reusable simple-step helpers: `FormStepLogic<TModel>`, `ResultStepLogic<TResultModel>`
- Diagnostics hook via `IWizardDiagnostics`

---

## Interactive Demo

Experience the wizard's logic and flexibility in real-time through our hosted sandbox.

**[Explore the Live Demo](https://blazorwizarddemo202602.azurewebsites.net/)**

### What to Try

The wizard features dynamic conditional logic based on user input. Try these scenarios in the **Age** field to see the UI adapt:

* **Age < 16:** The person wizard blocks progress with validation.
* **Age 16–66:** The standard adult workflow.
* **Age > 66:** Triggers specific senior-tier options or validation.

## UI & Customization

The component is designed with a **headless-first** philosophy. You have total control over the aesthetic:

* **Complete UI Freedom:** Design any wrapper or layout you need.
* **Simple Button Logic:** Step navigation is handled via standard `onclick` events and `disabled` states, ensuring compatibility with any CSS framework (Bootstrap, Tailwind, etc.).

---
## Installation

```bash
dotnet add package Blazor.Wizard
```

Requirements: .NET 8+

## Core Namespaces

```csharp
using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;
```

## Simple Wizard Example

The smallest example in the demo is the inline fun wizard at `/inline-fun-wizard`.
It uses two reusable `FormStepLogic<TModel>` steps, one `ResultStepLogic<TResultModel>` summary step,
and renders everything inline with `DynamicComponent`.

```csharp
public class FunMoodStepModel
{
    [Required(ErrorMessage = "Choose a wizard mood.")]
    public string Mood { get; set; } = string.Empty;
}

public class FunSnackStepModel : IValidatableObject
{
    public bool Tacos { get; set; }
    public bool Donuts { get; set; }
    public bool Popcorn { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Tacos && !Donuts && !Popcorn)
        {
            yield return new ValidationResult(
                "Pick at least one snack for the quest.",
                new[] { nameof(Tacos) });
        }
    }
}

public class FunWizardModelMapper : IWizardModelBuilder<FunWizardResult>
{
    public FunWizardResult Build(IWizardData data)
    {
        data.TryGet<FunMoodStepModel>(out var mood);
        data.TryGet<FunSnackStepModel>(out var snacks);

        return new FunWizardResult
        {
            Mood = mood?.Mood ?? string.Empty,
            Tacos = snacks?.Tacos ?? false,
            Donuts = snacks?.Donuts ?? false,
            Popcorn = snacks?.Popcorn ?? false
        };
    }
}

public class FunWizardViewModel : ComponentWizardViewModel<FunWizardResult>
{
    public FunWizardViewModel(IWizardModelBuilder<FunWizardResult> mapper)
        : base(mapper)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return step.Id switch
        {
            var id when id == typeof(FunMoodStepModel) => typeof(FunMoodStep),
            var id when id == typeof(FunSnackStepModel) => typeof(FunSnackStep),
            var id when id == typeof(FunWizardResult) => typeof(FunSummaryStep),
            _ => throw new InvalidOperationException()
        };
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return
        [
            () => new FormStepLogic<FunMoodStepModel>(typeof(FunMoodStepModel)),
            () => new FormStepLogic<FunSnackStepModel>(typeof(FunSnackStepModel)),
            () => new ResultStepLogic<FunWizardResult>(typeof(FunWizardResult), data => ModelBuilder.Build(data))
        ];
    }
}
```

That pattern is the simplest starting point:

- no dialog wrapper
- no DI-based step creation
- radio buttons and checkboxes only
- one mapper to build the final result
- one inline page hosting the wizard

If you need injected services, conditional visibility, or richer business rules, move to the person wizard pattern used in the demo project.
For the full source and walkthrough, see [Demo Walkthrough](Demo.md), [Wizard Comparison](WizardComparison.md), and [Service Injection Patterns](ServiceInjectionPatterns.md).

## Person Wizard Example

The person wizard is the next step after the inline fun wizard.
It keeps the same wizard engine, but adds DI-backed step creation, custom step logic, dynamic step visibility, and richer model mapping for real application workflows.
See the full implementation in `Blazor.Wizard.Demo/Components/Person` and `Blazor.Wizard.Demo/Components/WizardLogic/Person`, with a broader explanation in [Demo Walkthrough](Demo.md) and [Wizard Comparison](WizardComparison.md).

## Bidirectional Mapping Example (Edit Scenario)

If you need to prefill wizard from existing data (e.g., edit mode), implement both interfaces:

```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonResult>, IWizardModelSplitter<PersonResult>
{
    // Build result from wizard data
    public PersonResult Build(IWizardData data)
    {
        data.TryGet<PersonModel>(out var person);
        data.TryGet<AddressModel>(out var address);
        return new PersonResult
        {
            Name = person?.Name ?? string.Empty,
            City = address?.City ?? string.Empty
        };
    }

    // Split result into wizard data (for prefilling)
    public void Split(PersonResult result, IWizardData data)
    {
        data.Set(new PersonModel { Name = result.Name });
        data.Set(new AddressModel { City = result.City });
    }
}

// Usage: Prefill wizard with existing data
_viewModel.ModelSplitter.Split(existingModel, _viewModel.Data);
await _viewModel.StartAsync();
```

## Component-Oriented ViewModel

For DynamicComponent-based hosts, inherit `ComponentWizardViewModel<TResult>` and provide:

- default step factories
- mapping from current step to component type

---

## Use Cases

-  Multi-step registration forms
-  E-commerce checkout flows
-  Onboarding wizards
-  Configuration assistants
-  Survey/questionnaire systems
-  Document approval workflows
-  Insurance quote applications
-  Any guided, step-based user interaction

---

## Documentation

### Getting Started
- [Demo Walkthrough](Demo.md) - Interactive examples and live demo
- [Data Concept & Validation](DataConcept.md) - Understanding WizardData and validation

### Advanced Topics
- [IsVisible Guide](IsVisibleGuide.md) - Building complex conditional wizards with dynamic step visibility
- [Service Injection Patterns](ServiceInjectionPatterns.md) - Constructor DI, reusable steps, and runtime service access
- [Wizard Comparison](WizardComparison.md) - Compare the inline fun, questionary, and person wizard patterns
- [Migration Guide](MigrationGuide.md) - Move from `IWizardResultBuilder` to builder/splitter interfaces
- [Reorganization Summary](ReorganizationSummary.md) - High-level repository and package layout notes

### Reference
- [Library Structure](Blazor.Wizard/ProjectStructure.md) - Project organization and architecture
- [NuGet README](Blazor.Wizard/NugetReadme.md) - Package documentation
- [Changelog](Changelog.md) - Version history and updates
---

##  Architecture

### Design Principles
- **Separation of Concerns** - UI renders, logic controls behavior
- **Extensibility** - Override any part of the workflow
- **Composability** - Mix and match reusable steps
- **Testability** - Business logic isolated from Blazor components

### Core Types
```
IWizardStep
  ├─ BaseStepLogic<TModel>
  │    └─ GeneralStepLogic<TModel> (adds validation helpers)

WizardViewModel<TStep, TData, TResult>
  ├─ owns WizardFlow<int>
  ├─ uses TData / WizardData via IWizardData
  ├─ builds results with IWizardModelBuilder<TResult>
  └─ can prefill data with IWizardModelSplitter<TResult>
```

### Design Principles
- **Separation of Concerns** - UI renders, logic controls behavior
- **Extensibility** - Override any part of the workflow
- **Composability** - Mix and match reusable steps
- **Testability** - Business logic isolated from Blazor components


---

## Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request with tests

---

## License

MIT License - see [LICENSE](LICENSE) file for details

---

## Support

- **Issues**: Report bugs or request features on GitHub
- **Questions**: Open a discussion on GitHub

---

**Built with ❤️ for the Blazor community**

BlazorWizard adapts to different architectural styles and application sizes:
- Customize step instantiation
- Control navigation rules
- Intercept entry/exit events
- Share data between steps
- Build custom result models

---

##  Roadmap

This library was optimized for a rapid **.NET 8** integration. While the core is stable, the following enhancements are planned for future versions:

### Priorities
- [x] **Unit Testing** – Expanding test coverage for core logic and validators.
- [x] **Live Demo** – A hosted Blazor WebApp showcasing real-world usage.
- [ ] **Documentation** – Enhanced guides with architecture diagrams and visuals.

### Under Consideration
- [ ] **State Persistence** – Resume wizards after page refresh (LocalStorage/DB).
- [ ] **Accessibility** – Full ARIA support and keyboard navigation.
- [ ] **Step Templates** – Pre-built components for common patterns (Login, Payment, etc.).
- [ ] **Easy using** - 

**Contributing**  
Feel free to open an issue to discuss these features or submit a PR if you'd like to help implement them!

## History
[See change log](Changelog.md)
