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

* **Age < 16:** Observe how the flow restricts or modifies "Minor" specific steps.
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

## Minimal Example

```csharp
public class PersonStep : GeneralStepLogic<PersonModel>
{
    public override Type Id => typeof(PersonStep);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = typeof(AddressStep) };
    }
}

public class AddressStep : GeneralStepLogic<AddressModel>
{
    public override Type Id => typeof(AddressStep);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { CanContinue = true };
    }
}

// Build result from wizard data
public class PersonModelMapper : IWizardModelBuilder<PersonResult>
{
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
}

public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonResult>
{
    public PersonWizardViewModel() : base(new PersonModelMapper())
    {
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        base.Initialize(new List<Func<IWizardStep>>
        {
            () => new PersonStep(),
            () => new AddressStep()
        });
    }
}
```

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

- [Data Concept & Validation](DataConcept.md)
- [Library Structure](Blazor.Wizard/PROJECT_STRUCTURE.md)
- [NuGet README](Blazor.Wizard/NUGET_README.md)
- [Demo Walkthrough](demo.md)
- [Changelog](CHANGELOG.md)
--

##  Architecture

### Design Principles
- **Separation of Concerns** - UI renders, logic controls behavior
- **Extensibility** - Override any part of the workflow
- **Composability** - Mix and match reusable steps
- **Testability** - Business logic isolated from Blazor components

### Class Hierarchy
```
IWizardStep
  ├─ BaseStepLogic<TModel>
  │    └─ GeneralStepLogic<TModel> (adds validation helpers)
  │
  ├─ WizardFlow<TStep>
  ├─ WizardViewModel<TStep>
  ├─ WizardData : IWizardData
  └─ IWizardResultBuilder<TResult>
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
[See change log](CHANGELOG.md)
