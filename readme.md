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
```

## Component-Oriented ViewModel

For DynamicComponent-based hosts, inherit `ComponentWizardViewModel<TResult>` and provide:

- default step factories
- mapping from current step to component type

## Documentation

- [Library Structure](Blazor.Wizard/PROJECT_STRUCTURE.md)
- [NuGet README](Blazor.Wizard/NUGET_README.md)
- [Demo Walkthrough](demo.md)
- [Changelog](CHANGELOG.md)

## Notes

- Keep dialog components in app/UI projects.
- Keep `Blazor.Wizard` package headless and UI-agnostic.
