# Blazor.Wizard

Blazor.Wizard is a UI-agnostic wizard framework for Blazor.

## What You Get

- Step lifecycle and navigation orchestration
- Validation gating before transitions
- Shared typed state via `WizardData`
- Conditional routing with `StepResult.NextStepId`
- Reusable helpers (`FormStepLogic<TModel>`, `ResultStepLogic<TResultModel>`)
- State persistence (SSR-safe, resume after page refresh)
- Diagnostics extension point (`IWizardDiagnostics`)

## Requirements

- .NET 8+

## Quick Start

```csharp
using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

public sealed class MyStep : FormStepLogic<MyModel>
{
    public MyStep() : base(typeof(MyStep)) { }
}

public sealed class MyModelMapper : IWizardModelBuilder<MyResult>
{
    public MyResult Build(IWizardData data) => new();
}

public sealed class MyWizardVm : WizardViewModel<IWizardStep, WizardData, MyResult>
{
    public MyWizardVm() : base(new MyModelMapper()) { }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        base.Initialize(new List<Func<IWizardStep>>
        {
            () => new MyStep(),
            () => new ResultStepLogic<MyResult>(typeof(MyResult), d => new MyResult())
        });
    }
}
```

## Layered Namespaces

```csharp
using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;
```

## UI Guidance

This package is headless by design. Build dialogs/wrappers in your app layer
(Bootstrap, DevExpress, MudBlazor, etc.).

## Links

- Source: https://github.com/alexnek/blazor.wizard
- Main docs: https://github.com/alexnek/blazor.wizard/blob/main/Readme.md
