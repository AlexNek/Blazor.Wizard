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

public class PersonResultBuilder : IWizardResultBuilder<PersonResult>
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
    public PersonWizardViewModel() : base(new PersonResultBuilder())
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
