# Blazor.Wizard

[![NuGet](https://img.shields.io/nuget/v/Blazor.Wizard.svg)](https://www.nuget.org/packages/Blazor.Wizard/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A robust, wizard framework for Blazor applications that simplifies complex multi-step workflows with built-in navigation, validation, state management, and conditional branching.

---

## Key Features

### Core Capabilities
- **Intelligent Navigation** – Automatically handles Next/Back/Finish buttons, including validation gates that prevent invalid progress.
- **Seamless Validation** – Built-in support for Blazor's `EditContext` and `DataAnnotations`, plus custom validation hooks.
- **Type-Safe State** – Share data between steps using a strongly-typed `WizardData` container.
- **Dynamic Flow** – Support for conditional branching. Skip steps or inject new ones based on user input (e.g., flow `1 → 2 → 4` vs `1 → 2 → 3 → 4`).
- **Lifecycle Hooks** – Full control over step behavior with `EnterAsync`, `Evaluate`, `ValidateAsync`, and `LeaveAsync`.
- **UI Agnostic** – Bring your own CSS. Works perfectly with Bootstrap, MudBlazor, DevExpress, or custom components.
- **Testable Architecture** – Business logic is decoupled from the UI, making unit testing straightforward.

### Advanced Control
- **Custom Adapters** – Override default step behavior via `IFlowStepAdapter`.
- **Async First** – Full `async/await` support throughout the entire workflow.
- **Granular Validation** – Field-level error messages powered by `ValidationMessageStore`.
- **Compile-Time Safety** – Use `Type` as step identifiers to catch errors before runtime.

### Architecture Overview
- **Standalone Steps** – Every wizard step is an independent component with its own data model.
- **Guarded Navigation** – Users cannot proceed to the next step until the current model passes validation (DataAnnotations + custom rules).
- **Flexible UI** – While the logic follows a clear separation of concerns (MVVM-friendly), the UI components are entirely up to you.
- **Result Aggregation** – Individual step models are automatically merged into a single final wizard model upon completion via `IWizardResultBuilder`.

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

##  Installation

### NuGet Package
```shell
# .NET CLI
dotnet add package Blazor.Wizard

# Package Manager Console
Install-Package Blazor.Wizard
```

### Build from Source
```shell
cd BlazorStepper
dotnet build
nuget pack BlazorWizard.nuspec
```

**Requirements:** .NET 8.0 or higher

---

## Quick Start

### 1. Define Your Models
```csharp
public class PersonInfoModel
{
    [Required]
    public string FirstName { get; set; } = "";
    
    [Required]
    [Range(1, 120)]
    public int Age { get; set; }
}

public class AddressModel
{
    [Required]
    public string Street { get; set; } = "";
    
    [Required]
    public string City { get; set; } = "";
}
```

### 2. Create Step Logic
```csharp
using Blazor.Wizard;

public class PersonInfoStepLogic : GeneralStepLogic<PersonInfoModel>
{
    public override Type Id => typeof(PersonInfoStepLogic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!data.TryGet<PersonInfoModel>(out var person) || person == null)
            return new StepResult { StayOnStep = true };

        // Addition custom validation, simple example
        if (person.Age < 18)
        {
            validation.IsValid = false;
            validation.ErrorMessage = "Must be 18 or older";
            AddValidationError(GetEditContext(), nameof(PersonInfoModel.Age), 
                              validation.ErrorMessage);
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        // Navigate to next step
        return new StepResult { NextStepId = typeof(AddressStepLogic) };
    }
}

public class AddressStepLogic : GeneralStepLogic<AddressModel>
{
    public override Type Id => typeof(AddressStepLogic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = typeof(SummaryStepLogic) };
    }
}
```

### 3. Configure the Wizard Flow
```csharp
public class PersonWizardViewModel : WizardViewModel<Type>
{
    public PersonWizardViewModel()
    {
        Data = new WizardData();
        Flow = new WizardFlow<Type>(Data);

        // Register steps
        var personStep = new PersonInfoStepLogic();
        var addressStep = new AddressStepLogic();
        var summaryStep = new SummaryStepLogic();

        Flow.Add(personStep);
        Flow.Add(addressStep);
        Flow.Add(summaryStep);

        Steps = new List<IWizardStep> { personStep, addressStep, summaryStep };
        
        // Initialize to first step
        Flow.Index = 0;
        Flow.Current = typeof(PersonInfoStepLogic);
    }
}
```

### 4. Create the UI Component
```razor
@using Blazor.Wizard
@using YourApp.Models

<div class="wizard-container">
    @if (_viewModel?.Steps != null && _viewModel.Flow.Index >= 0)
    {
        var currentStep = _viewModel.Steps[_viewModel.Flow.Index];
        
        @if (currentStep is PersonInfoStepLogic personStep)
        {
            <PersonInfoForm Model="@personStep.GetModel()" 
                          EditContext="@personStep.GetEditContext()"/>
        }
        else if (currentStep is AddressStepLogic addressStep)
        {
            <AddressForm Model="@addressStep.GetModel()" 
                       EditContext="@addressStep.GetEditContext()"/>
        }
        else if (currentStep is SummaryStepLogic)
        {
            <SummaryView Data="@_viewModel.Data"/>
        }
    }

    <div class="wizard-buttons">
        <button @onclick="OnBack" 
                disabled="@(!CanGoBack)">Back</button>
        <button @onclick="OnNext" 
                disabled="@(!CanGoNext)">Next</button>
        <button @onclick="OnFinish" 
                disabled="@(!IsLastStep)">Finish</button>
    </div>
</div>

@code {
    private PersonWizardViewModel? _viewModel;

    protected override void OnInitialized()
    {
        _viewModel = new PersonWizardViewModel();
        _viewModel.Flow.StateChanged += StateHasChanged;
    }

    private async Task OnNext() => await _viewModel!.Flow.NextAsync();
    private async Task OnBack() => await _viewModel!.Flow.PrevAsync();
    
    private async Task OnFinish()
    {
        var result = new PersonModelResultBuilder().Build(_viewModel!.Data);
        // Handle completion
    }

    private bool CanGoBack => _viewModel?.Flow.Index > 0;
    private bool CanGoNext => _viewModel?.Flow.Index < _viewModel?.Steps.Count - 1;
    private bool IsLastStep => _viewModel?.Flow.Index == _viewModel?.Steps.Count - 1;
}
```

---

## Core Concepts

### Step Lifecycle
Each step goes through a predictable lifecycle:
1. **EnterAsync** - Called when navigating *to* the step (load data, initialize)
2. **ValidateAsync** - Called before leaving to validate the current step
3. **Evaluate** - Determines next step, handles conditional branching
4. **LeaveAsync** - Called when navigating *away* (cleanup, save state)

### Wizard Data Container
`WizardData` stores and shares data between steps:
```csharp
var data = new WizardData();
data.Set(new PersonInfoModel { FirstName = "John" });
data.Set(new AddressModel { City = "Seattle" });

if (data.TryGet<PersonInfoModel>(out var person))
{
    Console.WriteLine(person.FirstName); // "John"
}
```

### Conditional Navigation
Control step visibility and routing:
```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    data.TryGet<PersonInfoModel>(out var person);

    // Skip pension step if under 65
    if (person.Age < 65)
        return new StepResult { NextStepId = typeof(SummaryStepLogic) };
    
    return new StepResult { NextStepId = typeof(PensionInfoStepLogic) };
}
```

### Result Aggregation
Build final models from wizard data:
```csharp
public class PersonModelResultBuilder : IWizardResultBuilder<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        data.TryGet<PersonInfoModel>(out var person);
        data.TryGet<AddressModel>(out var address);

        return new PersonModel
        {
            FirstName = person?.FirstName ?? "",
            Age = person?.Age ?? 0,
            Street = address?.Street ?? "",
            City = address?.City ?? ""
        };
    }
}
```

---

## 🎯 Use Cases

-  Multi-step registration forms
-  E-commerce checkout flows
-  Onboarding wizards
-  Configuration assistants
-  Survey/questionnaire systems
-  Document approval workflows
-  Insurance quote applications
-  Any guided, step-based user interaction

---

##  Documentation

- **[DEMO.md](DEMO.md)** - Detailed walkthrough of included demo examples
- **[CHANGELOG.md](CHANGELOG.md)** - Version history and release notes
- **[NUGET_README.md](NUGET_README.md)** - Package-specific documentation
- **[CREATE_NUGET_PACKAGE.md](CREATE_NUGET_PACKAGE.md)** - Build and publish guide

---

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