# Blazor Wizard Demo Examples

This document explains the demo applications showcasing different integration patterns and UI approaches for the Blazor.Wizard framework.

---

# Native Blazor Demo (No Third-Party Dependencies)

**Project:** `Blazor.Wizard.Demo`  
**Location:** [Blazor.Wizard.Demo/](Blazor.Wizard.Demo/)  
**UI Framework:** Bootstrap (native Blazor components)  
**Routes:** `/` (dialog-based demos) and `/inline-fun-wizard` (smallest inline example)

## Overview

A production-ready wizard implementation using **native Blazor components** and Bootstrap styling - no third-party UI libraries required. This demo showcases the complete Blazor.Wizard framework architecture with clean MVVM patterns, conditional step logic, real-time validation, and state management.

The native demo currently exposes three examples with increasing complexity:

- `Inline Fun Wizard` at `/inline-fun-wizard` - the smallest possible inline setup
- `Questionary Wizard` at `/` - a simple dialog-based reusable-step flow
- `Person Wizard` at `/` - the advanced business-rule-heavy example

### Why This Demo?

- ✅ **Zero third-party UI dependencies** - Uses only native Blazor and Bootstrap
- ✅ **Production-ready architecture** - Clean separation of concerns
- ✅ **Complete feature set** - Conditional steps, validation, state management
- ✅ **Easy to understand** - No DevExpress-specific code
- ✅ **Ready to copy** - Take this code and adapt it to your needs

---

## Architecture Overview

### Component Structure

```
WizardDemo.razor (Host Page)
    └─ PersonWizardDialog.razor (Modal Dialog)
          ├─ PersonWizardDialog.razor.cs (Code-behind)
          │     └─ PersonWizardViewModel.cs (Flow Controller)
          │           ├─ WizardFlow<Type>
          │           ├─ WizardData (State Container)
          │           ├─ WizardStepFactory (Step Registration)
          │           └─ PersonModelResultBuilder (Result Aggregation)
          │
          └─ Step Components
                ├─ PersonInfoForm.razor (Step 1)
                ├─ AddressForm.razor (Step 2)
                ├─ PensionInfoForm.razor (Step 3 - Conditional)
                └─ SummaryView.razor (Step 4 - Final)
          
          └─ Step Logic Classes
                ├─ PersonInfoStepLogic.cs (Validation + Age Rules)
                ├─ AddressStepLogic.cs (Dynamic Routing)
                ├─ PensionInfoStepLogic.cs (Conditional Visibility)
                └─ SummaryStepLogic.cs (Read-only Display)
```

Current person demo note: the active implementation uses `PersonWizardDefinition` for step registration and `PersonModelMapper` for result mapping.

---

## Features Demonstrated

| Feature | Implementation | Description |
|---------|---------------|-------------|
| **Multi-Step Flow** | `WizardFlow<Type>` | Type-safe navigation between steps |
| **Conditional Steps** | `PensionInfoStepLogic` | Pension step appears only for age 66+ |
| **Dynamic Routing** | `AddressStepLogic.Evaluate()` | Next step determined at runtime |
| **Field Validation** | `DataAnnotationsValidator` | Real-time validation with error messages |
| **Custom Business Rules** | `PersonInfoStepLogic.Evaluate()` | Age must be 16+ to proceed |
| **State Management** | `WizardData` | Shared data container across steps |
| **Result Aggregation** | `PersonModelMapper` | Builds final model from step data and supports prefilling |
| **Modal Dialog** | Bootstrap modal | Native implementation without libraries |
| **Responsive UI** | Bootstrap classes | Mobile-friendly layout |

---

## User Interface

### Modal Dialog (Bootstrap Native)

The wizard appears in a **Bootstrap modal dialog** with:
- **Header**: Title + Close button
- **Body**: Current step form (dynamically rendered)
- **Footer**: Cancel/Back/Next/OK buttons with smart visibility logic

```razor
@if (Visible)
{
  <div class="modal fade show d-block" tabindex="-1" style="background:rgba(0,0,0,0.5);">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Person Information Wizard</h5>
          <button type="button" class="btn-close" @onclick="OnCancel"></button>
        </div>
        <div class="modal-body">
          <!-- Dynamic step content rendered here -->
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @onclick="OnCancel">Cancel</button>
          <button class="btn btn-outline-primary" @onclick="OnBack">Back</button>
          <button class="btn btn-primary" @onclick="OnNext">Next</button>
        </div>
      </div>
    </div>
  </div>
}
```

### Button Logic

- **Cancel**: Always visible, closes dialog without saving
- **Back**: Disabled on first step or if no previous visible steps
- **Next**: Shows when there are more visible steps ahead
- **OK**: Replaces "Next" on the last step, triggers completion

---

##  Step-by-Step Walkthrough

### Step 1: Person Information

**Component:** `PersonInfoForm.razor`  
**Logic:** `PersonInfoStepLogic.cs`  
**Model:** `PersonInfoModel`

#### Fields
- First Name (required, 2-50 chars)
- Last Name (required, 2-50 chars)
- Email (required, valid email format)
- Age (required, 6-120 range)

#### Business Rules
```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    if (!data.TryGet<PersonInfoModel>(out var person))
        return new StepResult { StayOnStep = true };

    // Age must be at least 16
    if (person.Age < AgeRuleConstants.MinAllowedAge)
    {
        validation.IsValid = false;
        validation.ErrorMessage = "Age must be at least 16 to proceed.";
        AddValidationError(editContext, nameof(PersonInfoModel.Age), 
                          validation.ErrorMessage);
        return new StepResult { StayOnStep = true, CanContinue = false };
    }

    return new StepResult { NextStepId = typeof(AddressStepLogic) };
}
```

#### Key Concepts
- **Custom validation** beyond DataAnnotations
- **Field-level error messages** using `ValidationMessageStore`
- **Business rule enforcement** (age >= 16)
- **State storage** via `WizardData.Set<PersonInfoModel>()`

---

### Step 2: Address Information

**Component:** `AddressForm.razor`  
**Logic:** `AddressStepLogic.cs`  
**Model:** `AddressModel`

#### Fields
- Street (required, 5-100 chars)
- City (required, 2-50 chars)
- Zip Code (required, 2-10 chars)
- Country (required, 2-50 chars)

#### Dynamic Routing
```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    if (!validation.IsValid)
        return new StepResult { StayOnStep = true };

    // Decide next step at RUNTIME based on age
    if (data.TryGet<PersonInfoModel>(out var person) && 
        person.Age > AgeRuleConstants.MaxPensionAge)
    {
        // Show pension step for age 66+
        return new StepResult { NextStepId = typeof(PensionInfoStepLogic) };
    }

    // Skip pension step for younger users
    return new StepResult { NextStepId = typeof(SummaryStepLogic) };
}
```

#### Key Concepts
- **Runtime navigation** - Next step determined by data
- **Cross-step data access** - Reads `PersonInfoModel` from previous step
- **Conditional branching** - Shows/skips steps based on business logic

---

### Step 3: Pension Information (Conditional)

**Component:** `PensionInfoForm.razor`  
**Logic:** `PensionInfoStepLogic.cs`  
**Model:** `AddressModel` (reused)

#### Visibility Logic
```csharp
public sealed class PensionInfoStepLogic : BaseStepLogic<AddressModel>
{
    private PersonInfoModel? _cachedPersonInfo;
    
    public override bool IsVisible => ShouldShowPension(_cachedPersonInfo);

    public void UpdatePersonInfo(PersonInfoModel? personInfo)
    {
        _cachedPersonInfo = personInfo;
    }

    private static bool ShouldShowPension(PersonInfoModel? personInfo)
    {
        // Show pension step ONLY if age > 66
        return personInfo != null && 
               personInfo.Age > AgeRuleConstants.MaxPensionAge;
    }
}
```

#### Key Concepts
- **Conditional visibility** - Step hidden for users under 66
- **Framework auto-skip** - Wizard automatically skips hidden steps
- **Dynamic updates** - Visibility recalculated when data changes

---

### Step 4: Summary

**Component:** `SummaryView.razor`  
**Logic:** `SummaryStepLogic.cs`  
**Purpose:** Display all collected information for review

#### Display Logic
```razor
<div class="card">
    <div class="card-header">Review Your Information</div>
    <div class="card-body">
        <h5>Personal Information</h5>
        <p><strong>Name:</strong> @Model.FirstName @Model.LastName</p>
        <p><strong>Email:</strong> @Model.Email</p>
        <p><strong>Age:</strong> @Model.Age</p>
        
        <h5 class="mt-3">Address</h5>
        <p><strong>Street:</strong> @Model.Street</p>
        <p><strong>City:</strong> @Model.City, @Model.ZipCode</p>
        <p><strong>Country:</strong> @Model.Country</p>
    </div>
</div>
```

#### Key Concepts
- **Result aggregation** - `PersonModelMapper` combines all step data
- **Read-only display** - No form inputs, just review
- **Final validation** - User confirms before completion

---

## Core Components Explained

### Inline Fun Wizard

The inline fun wizard is the baseline example for this repository. It is intentionally small:

- inline page host instead of dialog wrapper
- radio-button and checkbox steps
- `FormStepLogic<TModel>` plus `ResultStepLogic<TResultModel>`
- one `FunWizardViewModel` with direct default step factories
- one `FunWizardModelMapper` for final result creation

Use it first if you want to understand the minimum moving parts before reading the questionary or person implementations.

---

### Person Wizard

### 1. Step Registration (PersonWizardDefinition.cs)

The actual source of truth for creating person wizard steps is `PersonWizardDefinition`.

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
    }
}
```

What each registration entry means:

- `EPersonStepId.PersonInfo` is the logical step key used for registration coverage checks.
- `typeof(PersonInfoStepLogic)` is the runtime step id used by the flow and `NextStepId`.
- `typeof(PersonInfoForm)` is the Razor component rendered by `DynamicComponent`.
- `ActivatorUtilities.CreateInstance<...>(sp)` lets the step class use constructor DI.

Why this matters for a reader:

- to add a new wizard page, add one registration entry here
- the ViewModel does not manually create steps anymore
- component mapping and step factories now come from this definition

Why the person wizard is more complex than the questionary wizard:

- it uses custom step classes with business rules, not only reusable form steps
- it has conditional routing based on age
- it has a conditional pension step with dynamic visibility
- it uses constructor DI plus runtime service registration
- it supports prefilling through `PersonModelMapper : IWizardModelSplitter<PersonModel>`

### 2. ViewModel (PersonWizardViewModel.cs)

The orchestrator that manages wizard flow, steps, and state.

```csharp
public class PersonWizardViewModel : ComponentWizardViewModel<PersonModel>
{
    private readonly PersonWizardDefinition _definition;

    public PersonWizardViewModel(
        IWizardModelBuilder<PersonModel> mapper,
        PersonWizardDefinition definition,
        IWizardDiagnostics? diagnostics = null) : base(mapper, diagnostics)
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

    protected override async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        // Run business logic validation in real-time
        if (Flow?.Index >= 0 && Flow.Index < Steps.Count)
        {
            var step = Steps[Flow.Index];
            if (step is PersonInfoStepLogic personStep)
            {
                var validation = new ValidationResult { 
                    IsValid = await personStep.ValidateAsync(Data) 
                };
                personStep.Evaluate(Data, validation);
            }
        }
        
        await UpdateCanProceedAsync();
    }
}
```

Creation note: in the current implementation, step creation is provided by `PersonWizardDefinition`. `PersonWizardViewModel` resolves component types through `ResolveComponentType(...)` and supplies default step factories through `GetDefaultStepFactories()`.

**Responsibilities:**
- ✅ Step registration and lifecycle management
- ✅ Real-time validation on field changes
- ✅ State coordination across steps
- ✅ Navigation flow control

---

### 3. Dialog Component (PersonWizardDialog.razor)

The modal dialog that hosts the wizard UI.

```csharp
// Code-behind (PersonWizardDialog.razor.cs)
public partial class PersonWizardDialog
{
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private IToasterService Toaster { get; set; } = default!;

    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public PersonModel? Model { get; set; }
    [Parameter] public EventCallback<PersonModel> OnFinished { get; set; }

    private PersonWizardViewModel? _viewModel;

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new PersonWizardViewModel(
                new PersonModelMapper(),
                new PersonWizardDefinition(ServiceProvider),
                StartupWizardDiagnostics.Create());
            _viewModel.StateChanged += OnViewModelStateChanged;
            _viewModel.Initialize(null);
            _viewModel.Data.AddService(Toaster);
            _viewModel.ModelSplitter.Split(Model ?? new PersonModel(), _viewModel.Data);
            await _viewModel.StartAsync();
        }
        else if (!Visible && _viewModel != null)
        {
            _viewModel.StateChanged -= OnViewModelStateChanged;
            _viewModel.Reset();
            _viewModel = null;
        }
    }

    private void OnViewModelStateChanged()
    {
        StateHasChanged();
    }

    private async Task OnOkClick()
    {
        if (_viewModel == null) return;

        var result = await _viewModel.FinishAsync();
        if (result != null)
        {
            await OnFinished.InvokeAsync(result);
            Visible = false;
            await VisibleChanged.InvokeAsync(false);
        }
    }
}
```

Startup note: the current dialog setup creates `PersonWizardViewModel` with `PersonModelMapper`, `PersonWizardDefinition`, and diagnostics, then calls `Initialize(null)`, registers runtime services with `AddService(...)`, prefills state through `ModelSplitter.Split(...)`, and only then calls `StartAsync()`.

**Responsibilities:**
- ✅ Lifecycle management (initialize/cleanup)
- ✅ Dynamic step rendering
- ✅ Button state management
- ✅ Event propagation to parent

---

### 4. Result Mapper (PersonModelMapper.cs)

Aggregates data from multiple steps into the final model.

```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>, IWizardModelSplitter<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        data.TryGet<PersonInfoModel>(out var person);
        data.TryGet<AddressModel>(out var address);

        return new PersonModel
        {
            FirstName = person?.FirstName ?? string.Empty,
            LastName = person?.LastName ?? string.Empty,
            Email = person?.Email ?? string.Empty,
            Age = person?.Age ?? 0,
            Street = address?.Street ?? string.Empty,
            City = address?.City ?? string.Empty,
            ZipCode = address?.ZipCode ?? string.Empty,
            Country = address?.Country ?? string.Empty
        };
    }

    public void Split(PersonModel result, IWizardData data)
    {
        if (!string.IsNullOrWhiteSpace(result.Email) || !string.IsNullOrWhiteSpace(result.FirstName))
        {
            data.Set(new PersonInfoModel
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Age = result.Age
            });
        }

        if (!string.IsNullOrWhiteSpace(result.Street) || !string.IsNullOrWhiteSpace(result.City))
        {
            data.Set(new AddressModel
            {
                Street = result.Street,
                City = result.City,
                ZipCode = result.ZipCode,
                Country = result.Country
            });
        }
    }
}
```

This mapper also implements `Split(...)`, so the person wizard can prefill `PersonInfoModel` and `AddressModel` from an existing `PersonModel` before the wizard starts.

**Responsibilities:**
- ✅ Data transformation from step models to final model
- ✅ Null-safe data extraction
- ✅ Centralized aggregation logic

---

### Questionary Wizard

### 1. Step Registration (QuestionaryStepRegistry.cs)

The questionary wizard uses a simpler static registry because its flow is linear and its steps are mostly plain form pages.

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
}
```

Why this one is simpler:

- no constructor DI is needed for the registered form steps
- no conditional step visibility is involved
- no edit-mode splitter is used
- most steps can reuse `FormStepLogic<TModel>` directly
- compared with the inline fun wizard, it adds a reusable static registry and dialog host
- compared with the person wizard, it does not need custom business-rule-heavy step classes, dynamic pension-step visibility, or bidirectional model mapping

---

## Key Design Patterns

### 1. MVVM (Model-View-ViewModel)
- **Model**: `PersonModel`, `PersonInfoModel`, `AddressModel`
- **View**: Razor components (`PersonInfoForm.razor`, etc.)
- **ViewModel**: `PersonWizardViewModel` + Step Logic classes

### 2. Factory Pattern

Current person demo note: step registration now goes through `PersonWizardDefinition` instead of manual `WizardStepFactory` registration.
```csharp
var factory = new WizardStepFactory();
factory.Register(typeof(PersonInfoStepLogic), () => new PersonInfoStepLogic());
var step = factory.CreateStep(typeof(PersonInfoStepLogic));
```

### 3. Strategy Pattern
Each step implements `IWizardStep` with custom `Evaluate()` logic.

### 4. Builder Pattern
`PersonModelMapper` constructs the final result from partial data and can also prefill wizard state for edit scenarios.

### 5. State Container
`WizardData` acts as a type-safe dictionary for cross-step data sharing.

---

## Running the Demo

### Prerequisites
- .NET 8.0 SDK or higher
- No third-party UI libraries needed!

### Steps

1. **Navigate to the demo project**
   ```powershell
   cd Blazor.Wizard.Demo
   ```

2. **Restore dependencies**
   ```powershell
   dotnet restore
   ```

3. **Run the application**
   ```powershell
   dotnet run
   ```

4. **Open browser**
   ```
  https://localhost:7111
   ```

5. **Try the examples**
   - open `/inline-fun-wizard` for the smallest inline example
   - open `/` for the dialog-based questionary and person examples

---

##  Best Practices Demonstrated

### 1. Clean Architecture
- **Separation of concerns** - Logic, UI, and state are decoupled
- **Testability** - Each component can be unit tested independently
- **Maintainability** - Easy to add/remove/modify steps

### 2. Type Safety
- **Generic types** - `WizardViewModel<TStep, TData, TResult>`
- **Type-based step IDs** - `typeof(PersonInfoStepLogic)` instead of strings
- **Compile-time checking** - Errors caught before runtime

### 3. Validation Strategy
- **Multi-layer validation**:
  1. DataAnnotations (field-level)
  2. Custom business rules (step-level)
  3. Cross-step dependencies (workflow-level)

### 4. Error Handling
- **Graceful degradation** - Handles null data safely
- **User feedback** - Clear error messages at field level
- **Defensive programming** - Null checks and safe navigation

### 5. State Management
- **Centralized state** - `WizardData` container
- **Type-safe access** - `data.TryGet<T>(out var result)`
- **Immutability awareness** - Steps don't modify other steps' data directly

---

## Code Highlights

### Real-Time Validation

The wizard validates as you type, not just on submit:

```csharp
protected override async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
{
    try
    {
        // Validate current step
        if (Flow?.Index >= 0 && Flow.Index < Steps.Count)
        {
            var step = Steps[Flow.Index];
            if (step is PersonInfoStepLogic personStep)
            {
                var validation = new ValidationResult { 
                    IsValid = await personStep.ValidateAsync(Data) 
                };
                personStep.Evaluate(Data, validation);
            }
        }
        
        // Update button states
        await UpdateCanProceedAsync();
    }
    catch (Exception ex)
    {
        // Log errors to prevent crashes
        Trace.TraceError($"Exception in OnFieldChanged: {ex}");
    }
}
```

### Smart Button Visibility

The "Next" button automatically becomes "OK" on the final step:

```razor
@if (_viewModel?.Flow != null && _viewModel.Flow.Index < _viewModel.Steps.Count - 1)
{
    <button class="btn btn-primary" @onclick="OnNext" 
            disabled="@(!_viewModel.CanProceed)">
        Next
    </button>
}
else
{
    <button class="btn btn-primary" @onclick="OnOkClick" 
            disabled="@(!_viewModel.CanProceed)">
        OK
    </button>
}
```

### Conditional Step Visibility

Framework automatically skips hidden steps:

```csharp
// In PensionInfoStepLogic
public override bool IsVisible => 
    _cachedPersonInfo != null && 
    _cachedPersonInfo.Age > AgeRuleConstants.MaxPensionAge;
```

---

## Comparison: Native vs DevExpress Demo

| Aspect | Native Demo | DevExpress Demo |
|--------|-------------|-----------------|
| **UI Library** | Bootstrap (native) | DevExpress Blazor |
| **Dependencies** | Minimal | Requires DevExpress license |
| **Learning Curve** | Lower | Higher (DevExpress APIs) |
| **Customization** | Full control | Component limitations |
| **Performance** | Lightweight | Heavier (more features) |
| **Styling** | CSS/Bootstrap | DevExpress themes |
| **Components** | Native Blazor | DxPopup, DxButton, DxFormLayout |
| **Portability** | High | Locked to DevExpress |
| **Cost** | Free | Commercial license |

**Recommendation:** Start with the **native demo** for learning and prototyping. Consider DevExpress for enterprise apps with complex UI requirements.

---

## Testing Support

The demo includes comprehensive unit tests in `Blazor.Wizard.Demo.Tests`:

- **Step Logic Tests**: `AddressStepLogicTests.cs`, `PensionInfoStepLogicVisibilityTests.cs`
- **Validation Tests**: `AgeRuleTests.cs`, `ValidationTests.cs`
- **Flow Tests**: `WizardFlowSequenceTests.cs`, `WizardEdgeCasesTests.cs`
- **Integration Tests**: `PersonWizardDialogTests.cs`

Run tests:
```powershell
cd Blazor.Wizard.Demo.Tests
dotnet test
```

---

## Additional Resources

- **[Main README](Readme.md)** - Framework documentation
- **[Changelog](Changelog.md)** - Version history
- **[NuGet Package](https://www.nuget.org/packages/Blazor.Wizard)** - Install in your project

---

## Learning Path

1. **Review the architecture** - Understand the component structure
2. **Run the demo** - See it in action
3. **Read the code** - Start with `PersonWizardDialog.razor`
4. **Modify a step** - Try adding a new field
5. **Add a step** - Create a new step with custom logic
6. **Run tests** - Verify your changes
7. **Build your own** - Apply patterns to your use case

---

# Demo Examples With Developer Express Library

This section describes an older DevExpress-oriented sample kept for reference. The active demo project in this repository is `Blazor.Wizard.Demo` at route `/`.

---

## 📁 Demo Overview

The project includes three progressively sophisticated wizard implementations:

| Demo | Description | UI Library | Complexity |
|------|-------------|------------|------------|
| **WizardDemo** | Custom circular navigation buttons | Bootstrap | Basic |
| **WizardDemo2** | Standard button layout at bottom | Bootstrap | Basic |
| **WizardDemo3** | Production-ready dialog wizard | DevExpress | Advanced |

---

## Demo 1: WizardDemo (Circular Buttons)

**File:** [Components/Pages/WizardDemo.razor](../BlazorWizardDemo/Components/Pages/WizardDemo.razor)  
**Route:** `/wizard-demo`

### Overview
A simple 3-step wizard with custom circular arrow buttons positioned on the sides of the wizard container. Demonstrates basic wizard lifecycle and conditional navigation.

### Features
- ✅ Custom circular navigation buttons with Bootstrap Icons
- ✅ Conditional step completion (checkbox control)
- ✅ Step initialization and validation callbacks
- ✅ Event logging to display area

### Key Implementation Details

#### Navigation Buttons
```css
.wiz-btn {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
}

.wiz-btn.prev { left: 4px; }
.wiz-btn.next { right: 4px; }
```

#### Step Flow Control
```csharp
public void TryCompleteStep(WizardStepAttemptedCompleteEventArgs args, int step)
{
    StepData += $"TryCompleteStep step {step}\n";
    args.IsCancelled = !GoNext; // Block navigation if checkbox unchecked
}
```

### Steps
1. **Step 1** - Simple introduction step
2. **Step 2** - Contains a checkbox to control whether user can proceed
3. **Step 3** - Summary step

### Usage
```csharp
<Wizard @ref="Wizard"
        OnStart="Start"
        OnStop="Stop"
        OnFinish="Finish"
        AllowCancel="true"
        AllowPrevious="true">
  <Steps>
    <WizardStep OnInitialize="..." OnTryComplete="..." Title="The first step">
      Content here
    </WizardStep>
  </Steps>
</Wizard>
```

---

## Demo 2: WizardDemo2 (Bottom Buttons)

**File:** [Components/Pages/WizardDemo2.razor](../BlazorWizardDemo/Components/Pages/WizardDemo2.razor)  
**Route:** Historical sample

### Overview
Similar to Demo 1, but with standard rectangular buttons positioned at the bottom center of the wizard. More conventional UI pattern suitable for most applications.

### Features
- ✅ Standard button layout (Previous/Next/Finish)
- ✅ Bottom-centered navigation buttons
- ✅ Text labels with icon accents
- ✅ Same validation logic as Demo 1

### Key Implementation Details

#### Button Layout
```css
.wizard-buttons {
    display: flex;
    justify-content: center;
    gap: 16px;
    position: absolute;
    bottom: 24px;
}

.wiz-btn {
    min-width: 100px;
    height: 40px;
    border-radius: 20px;
    padding: 0 24px;
}
```

#### Button Text with Icons
```css
.wiz-btn.prev::before { 
    content: "\F284"; /* Bootstrap icon */
    font-family: bootstrap-icons; 
    margin-right: 8px; 
}
```

### When to Use
- Traditional form wizards
- Desktop applications
- When users expect conventional button placement

---

## Demo 3: WizardDemo3 (Production-Ready)

> Historical note: the DevExpress examples below are older reference material. The active sample in this repository is `Blazor.Wizard.Demo` on route `/`.

**File:** [Components/Pages/WizardDemo3.razor](../BlazorWizardDemo/Components/Pages/WizardDemo3.razor)  
**Route:** Historical sample

### Overview
A production-ready implementation using the Blazor.Wizard framework with DevExpress popup dialog. Demonstrates the **recommended architecture** for real-world applications.

### Features
- ✅ **Clean Architecture** - Separated concerns (ViewModel, Logic, UI)
- ✅ **Type-Safe Flow** - Uses Blazor.Wizard framework
- ✅ **Conditional Branching** - Pension step appears only for users 65+
- ✅ **Field-Level Validation** - Real-time validation with error messages
- ✅ **State Management** - Data shared across steps via `WizardData`
- ✅ **Result Aggregation** - Builds final `PersonModel` via `IWizardResultBuilder`
- ✅ **Dialog UI** - Professional popup with DevExpress components

---

## Demo 3 Architecture (Recommended Pattern)

### Component Structure

```
WizardDemo3.razor (Host Page)
    └─ PersonWizardDialog.razor (Dialog Container)
          ├─ PersonWizardViewModel (Flow Controller)
          │     ├─ WizardFlow<Type>
          │     ├─ WizardData
          │     └─ PersonModelResultBuilder
          │
          └─ Step Components
                ├─ PersonInfoForm.razor
                ├─ AddressForm.razor
                ├─ PensionInfoForm.razor (conditional)
                └─ SummaryView.razor
```

---

## Step-by-Step Breakdown

### 1. Host Page (WizardDemo3.razor)

Simple page that triggers the wizard dialog and handles completion.

```csharp
@page "/wizard-demo-3"

<button @onclick="OpenWizard">Open Wizard Dialog</button>

<PersonWizardDialog @bind-Visible="_showDialog" 
                    Model="_model" 
                    OnFinished="OnWizardFinished" />

@code {
    private bool _showDialog = false;
    private PersonModel _model = new();
    
    private void OpenWizard()
    {
        _model = new PersonModel();
        _showDialog = true;
    }
    
    private void OnWizardFinished(PersonModel model)
    {
        // Handle completed wizard data
        _showDialog = false;
    }
}
```

---

### 2. Wizard Dialog (PersonWizardDialog.razor)

The dialog container that manages the wizard lifecycle and renders current step.

#### Key Features
- **ViewModel Pattern** - Uses `PersonWizardViewModel` for flow control
- **Dynamic Step Rendering** - Renders appropriate form based on current step
- **Smart Button Logic** - Automatically shows/hides Next/Finish buttons
- **Lifecycle Management** - Initializes wizard when dialog opens

#### Code Structure
```csharp
<DxPopup Visible="@Visible" HeaderText="Person Information Wizard">
  <BodyContentTemplate>
    @if (_viewModel?.Flow != null)
    {
        var step = _viewModel.Steps[_viewModel.Flow.Index];
        
        @if (step is PersonInfoStepLogic personStep)
        {
            <PersonInfoForm Model="@personStep.GetModel()" 
                          EditContext="@personStep.GetEditContext()"/>
        }
        else if (step is AddressStepLogic addressStep)
        {
            <AddressForm Model="@addressStep.GetModel()" 
                       EditContext="@addressStep.GetEditContext()"/>
        }
        // ... other steps
    }
  </BodyContentTemplate>
  
  <FooterContentTemplate>
    <DxButton Text="Back" Click="@OnBack" 
              Enabled="@(canGoBack)"/>
    <DxButton Text="Next" Click="@OnNext" 
              Enabled="@_viewModel.CanProceed"/>
  </FooterContentTemplate>
</DxPopup>

@code {
    private PersonWizardViewModel? _viewModel;
    
    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _viewModel == null)
        {
            _viewModel = new PersonWizardViewModel();
            _viewModel.StateChanged += StateHasChanged;
            await _viewModel.StartAsync();
        }
    }
}
```

---

### 3. View Model (PersonWizardViewModel.cs)

The brain of the wizard - manages flow, steps, validation, and state.

```csharp
public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    public PersonWizardViewModel() : base(new PersonModelResultBuilder())
    {
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        var factory = new WizardStepFactory();
        factory.Register(typeof(PersonInfoStepLogic), 
                        () => new PersonInfoStepLogic("Demo value"));
        factory.Register(typeof(AddressStepLogic), 
                        () => new AddressStepLogic());
        factory.Register(typeof(PensionInfoStepLogic), 
                        () => new PensionInfoStepLogic());
        factory.Register(typeof(SummaryStepLogic), 
                        () => new SummaryStepLogic());

        base.Initialize(new List<Func<IWizardStep>>
        {
            () => factory.CreateStep(typeof(PersonInfoStepLogic)),
            () => factory.CreateStep(typeof(AddressStepLogic)),
            () => factory.CreateStep(typeof(PensionInfoStepLogic)),
            () => factory.CreateStep(typeof(SummaryStepLogic))
        });
    }
}
```

---

### 4. Step Logic Classes

Each step has dedicated logic handling validation, routing, and state.

#### PersonInfoStepLogic.cs
```csharp
public class PersonInfoStepLogic : GeneralStepLogic<PersonInfoModel>
{
    public override Type Id => typeof(PersonInfoStepLogic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!data.TryGet<PersonInfoModel>(out var person))
            return new StepResult { StayOnStep = true };

        // Custom validation rule
        if (person.Age < AgeRuleConstants.MinAllowedAge)
        {
            validation.IsValid = false;
            validation.ErrorMessage = "Age must be at least 16 to proceed.";
            AddValidationError(GetEditContext(), 
                              nameof(PersonInfoModel.Age), 
                              validation.ErrorMessage);
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        // Navigate to next step
        return new StepResult { NextStepId = typeof(AddressStepLogic) };
    }
}
```

#### PensionInfoStepLogic.cs (Conditional Step)
```csharp
public class PensionInfoStepLogic : GeneralStepLogic<AddressModel>
{
    private PersonInfoModel? _personInfo;

    public void UpdatePersonInfo(PersonInfoModel? personInfo)
    {
        _personInfo = personInfo;
        // Show step ONLY if age >= 65
        IsVisible = _personInfo != null && _personInfo.Age >= 65;
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = typeof(SummaryStepLogic) };
    }
}
```

---

### 5. Result Builder (PersonModelResultBuilder.cs)

Note: the current native sample uses `PersonModelMapper` and `ZipCode`; this snippet reflects the older DevExpress sample.

Aggregates data from all steps into final model.

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
            LastName = person?.LastName ?? "",
            Age = person?.Age ?? 0,
            Street = address?.Street ?? "",
            City = address?.City ?? "",
            Country = address?.Country ?? "",
            PostalCode = address?.PostalCode ?? ""
        };
    }
}
```

---

### 6. Form Components

#### PersonInfoForm.razor
```razor
@using BlazorWizardDemo.Models
@using DevExpress.Blazor

<EditForm Model="@Model" Context="editContext">
    <DataAnnotationsValidator />
    
    <DxFormLayout>
        <DxFormLayoutItem Caption="First Name">
            <DxTextBox @bind-Text="@Model.FirstName" />
        </DxFormLayoutItem>
        
        <DxFormLayoutItem Caption="Last Name">
            <DxTextBox @bind-Text="@Model.LastName" />
        </DxFormLayoutItem>
        
        <DxFormLayoutItem Caption="Age">
            <DxSpinEdit @bind-Value="@Model.Age" MinValue="0" MaxValue="120" />
        </DxFormLayoutItem>
    </DxFormLayout>
    
    <ValidationSummary />
</EditForm>

@code {
    [Parameter] public PersonInfoModel Model { get; set; } = new();
    [Parameter] public EditContext? EditContext { get; set; }
}
```

---

## 🎯 Key Concepts Demonstrated

### 1. Conditional Step Visibility
The pension information step only appears for users aged 65 or older:
```csharp
// In PensionInfoStepLogic
IsVisible = personInfo != null && personInfo.Age >= 65;
```

### 2. Real-Time Validation
Validation runs as user types, with immediate feedback:
```csharp
protected override async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
{
    await UpdateCanProceedAsync();
    
    // Run business validation live
    if (step is PersonInfoStepLogic personStep)
    {
        var validation = new ValidationResult { IsValid = await personStep.ValidateAsync(Data) };
        personStep.Evaluate(Data, validation);
    }
}
```

### 3. State Management
Data flows through `WizardData` container:
```csharp
// Step stores data
Data.Set(new PersonInfoModel { FirstName = "John", Age = 70 });

// Later step retrieves it
if (Data.TryGet<PersonInfoModel>(out var person))
{
    Console.WriteLine(person.FirstName); // "John"
}
```

### 4. Result Aggregation
Final model built from multiple step models:
```csharp
var result = new PersonModelResultBuilder().Build(_viewModel.Data);
// result contains aggregated data from all steps
```

---

## 📊 Demo Comparison Matrix

| Feature | Demo 1 | Demo 2 | Demo 3 |
|---------|--------|--------|--------|
| UI Library | Bootstrap | Bootstrap | DevExpress |
| Architecture | Simple | Simple | MVVM |
| Validation | Basic | Basic | Advanced |
| Conditional Steps | ❌ | ❌ | ✅ |
| Real-time Validation | ❌ | ❌ | ✅ |
| Result Builder | ❌ | ❌ | ✅ |
| Dialog/Modal | ❌ | ❌ | ✅ |
| Production Ready | ❌ | ❌ | ✅ |
| Complexity | Low | Low | Medium |

---

## 🚀 Running the Demos

### Prerequisites
- .NET 8.0 SDK or higher
- Visual Studio 2022 or VS Code
- DevExpress Blazor components (for Demo 3)

### Steps
1. **Clone the repository**
   ```shell
   git clone https://github.com/alexnek/blazor.wizard.git
   cd BlazorWizardDemo   // historical sample project
   ```

2. **Restore packages**
   ```shell
   dotnet restore
   ```

3. **Run the application**
   ```shell
   dotnet run --project BlazorWizardDemo   // historical sample project
   ```

4. **Navigate to demos**
   - Demo 1: `https://localhost:5001/wizard-demo`
   - Demo 2: historical sample route
   - Demo 3: historical sample route

---

## 💡 Best Practices (Learned from Demo 3)

### 1. Separation of Concerns
- **View Models** - Flow control and state
- **Step Logic** - Business rules and validation
- **UI Components** - Rendering only

### 2. Type Safety
- Use `Type` as step identifiers
- Strongly-typed models for each step
- Compile-time checking via generics

### 3. Validation Strategy
- **DataAnnotations** - Basic field validation
- **Custom Logic** - Business rules in `Evaluate()`
- **Real-time Feedback** - Update on field change

### 4. State Management
- Store step data in `WizardData`
- Retrieve in later steps or result builder
- Type-safe with `TryGet<T>()`

### 5. Conditional Navigation
- Set `IsVisible` in step logic
- Update based on previous step data
- Framework handles skipping hidden steps

---

## 🎓 Learning Path

1. **Start with Inline Fun Wizard** - Learn the minimum working setup
2. **Study the Questionary Wizard** - Add a reusable registry-based pattern
3. **Study the Person Wizard** - Learn DI, visibility rules, and richer mapping
4. **Review the historical demos below** - Only if you need legacy reference material
5. **Build your own** - Apply the matching pattern to your use case

---

## 📚 Additional Resources

- **[Main README](Readme.md)** - Framework documentation
- **[Architecture Guide](Blazor.Wizard.DemoDexEx/WizardArchitecture.md)** - Design patterns for the older DevExpress-oriented sample
- **[Refactoring Notes](../BlazorWizardDemo/refactoring.md)** - Evolution history

---

## 🤝 Contributing Examples

Have a great wizard implementation? Consider contributing:
1. Fork the repository
2. Add your demo to `Components/Pages/`
3. Update this documentation
4. Submit a pull request

---

**Questions?** Open an issue on GitHub or start a discussion!
