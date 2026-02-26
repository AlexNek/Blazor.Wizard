# Blazor.Wizard Project Structure

Clean, organized structure for the Blazor.Wizard library (v2.0.0).

## 📁 Directory Organization

```
Blazor.Wizard/
├── Core/                          # Core wizard logic and base classes
│   ├── BaseStepLogic.cs          # Abstract base class for step logic
│   ├── GeneralStepLogic.cs       # Recommended base class with validation helpers
│   ├── StepResult.cs             # Navigation result model
│   ├── ValidationResult.cs       # Validation result model
│   ├── WizardData.cs             # Type-safe data container
│   ├── WizardFlow.cs             # Flow controller and navigation
│   └── WizardStepFactory.cs      # Factory for creating steps
│
├── Interfaces/                    # Public interfaces
│   ├── IFlowStepAdapter.cs       # Adapter for custom step behavior
│   ├── IWizardContext.cs         # Context interface (v2.0)
│   ├── IWizardData.cs            # Data container interface
│   ├── IWizardDiagnostics.cs     # Diagnostics interface
│   ├── IWizardModelBuilder.cs    # Model builder interface (v2.0)
│   ├── IWizardModelSplitter.cs   # Model splitter interface (v2.0)
│   ├── IWizardStep.cs            # Main step interface
│   └── IWizardStepFactory.cs     # Step factory interface
│
├── ViewModels/                    # View model classes
│   ├── ComponentWizardViewModel.cs  # Enhanced view model (v2.0)
│   └── WizardViewModel.cs           # Base view model
│
├── Obsolete/                      # ⚠️ Deprecated/experimental code
│   ├── README.md                 # Migration guide for obsolete code
│   ├── FlowStep.razor            # [Obsolete] Old component approach
│   ├── IIdentifiableStep.cs      # [Obsolete] Unused interface
│   ├── IWizardResultBuilder.cs   # [Obsolete] Use IWizardModelBuilder + IWizardModelSplitter
│   └── IWizardStepLogic.cs       # [Obsolete] Unused interface
│
├── wwwroot/                       # Static assets
│   ├── background.png
│   └── exampleJsInterop.js
│
├── Blazor.Wizard.csproj          # Project file
├── BlazorWizard.nuspec           # NuGet package specification
└── NUGET_README.md               # NuGet package documentation
```

## 🎯 Usage by Directory

### Core/ - Start Here
The `Core/` directory contains the essential classes you'll use in every wizard:

```csharp
using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;

// Create a step
public class MyStepLogic : GeneralStepLogic<MyModel>
{
    public override Type Id => typeof(MyStepLogic);
    
    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = typeof(NextStep) };
    }
}

// Create a flow
var flow = new WizardFlow<Type>(new WizardData());
flow.Add(new MyStepLogic());
```

### Interfaces/ - For Advanced Scenarios
Implement these interfaces when you need custom behavior:

```csharp
// Custom model mapper (v2.0+)
public class MyModelMapper : IWizardModelBuilder<MyResult>, IWizardModelSplitter<MyResult>
{
    public MyResult Build(IWizardData data) { ... }
    public void Split(MyResult result, IWizardData data) { ... }
}

// Custom diagnostics
public class MyDiagnostics : IWizardDiagnostics
{
    public void StepEntered(string stepName) { ... }
}
```

### ViewModels/ - For UI Integration
Use view models to connect your wizard logic to Blazor components:

```csharp
// Basic view model
public class MyWizardViewModel : WizardViewModel<IWizardStep, WizardData, MyResult> { ... }

// Enhanced view model (v2.0)
public class MyWizardViewModel : ComponentWizardViewModel<MyResult> { ... }
```

### Obsolete/ - ⚠️ Do Not Use
Contains deprecated code marked with `[Obsolete]` attribute. See `Obsolete/README.md` for migration guides.

## 📦 NuGet Package Structure

When published to NuGet, the package includes:
- All classes from `Core/`, `Interfaces/`, and `ViewModels/`
- All classes from `Obsolete/` (for backward compatibility)
- `NUGET_README.md` as package documentation
- Static assets from `wwwroot/`

## 🔄 Namespace Convention

Namespaces are split by layer:

```csharp
namespace Blazor.Wizard.Core;        // Core classes
namespace Blazor.Wizard.Interfaces;  // Interface classes
namespace Blazor.Wizard.ViewModels;  // ViewModel classes
namespace Blazor.Wizard.Obsolete;    // Legacy artifacts
```

Typical imports:
```csharp
using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;
```

## 🏗️ Architecture Layers

```
┌─────────────────────────────────────┐
│   Blazor Components (Your UI)      │
│   - Uses ViewModels/               │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   ViewModels/                       │
│   - WizardViewModel                 │
│   - ComponentWizardViewModel        │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Core/                             │
│   - WizardFlow (navigation)         │
│   - BaseStepLogic (step base)       │
│   - WizardData (state)              │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Interfaces/                       │
│   - IWizardStep                     │
│   - IWizardData                     │
│   - IWizardModelBuilder             │
│   - IWizardModelSplitter            │
└─────────────────────────────────────┘
```

## 📝 Best Practices

1. **Start with Core/** - Use `GeneralStepLogic<TModel>` for your steps
2. **Use ViewModels/** - Choose `WizardViewModel<IWizardStep, WizardData, TResult>` or `ComponentWizardViewModel<TResult>`
3. **Implement Interfaces/** - Only when you need custom behavior
4. **Avoid Obsolete/** - Never use code from this directory in new projects

## 🔍 Finding What You Need

| I want to... | Look in... | Use... |
|-------------|-----------|--------|
| Create a wizard step | `Core/` | `GeneralStepLogic<TModel>` |
| Control navigation | `Core/` | `WizardFlow<TStep>` |
| Store shared data | `Core/` | `WizardData` |
| Build final result | `Interfaces/` | `IWizardModelBuilder<T>` |
| Prefill wizard data | `Interfaces/` | `IWizardModelSplitter<T>` |
| Connect to UI | `ViewModels/` | `WizardViewModel<TStep>` |
| Add diagnostics | `Interfaces/` | `IWizardDiagnostics` |

## 📚 Related Documentation

- [README.md](../readme.md) - Main documentation
- [CHANGELOG.md](../CHANGELOG.md) - Version history
- [Obsolete/README.md](Obsolete/README.md) - Migration guide for deprecated code

## 🚀 Version 2.0.0

This structure was introduced in v2.0.0 to improve code organization and maintainability.
