# Demo Examples With Developer Express Library

This document explains the three demo applications included in the BlazorWizardDemo project, showcasing different integration patterns and UI approaches for the Blazor.Wizard framework.

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
**Route:** `/wizard-demo-2`

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

**File:** [Components/Pages/WizardDemo3.razor](../BlazorWizardDemo/Components/Pages/WizardDemo3.razor)  
**Route:** `/wizard-demo-3`

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

## 🏗️ Demo 3 Architecture (Recommended Pattern)

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
   cd BlazorWizardDemo
   ```

2. **Restore packages**
   ```shell
   dotnet restore
   ```

3. **Run the application**
   ```shell
   dotnet run --project BlazorWizardDemo
   ```

4. **Navigate to demos**
   - Demo 1: `https://localhost:5001/wizard-demo`
   - Demo 2: `https://localhost:5001/wizard-demo-2`
   - Demo 3: `https://localhost:5001/wizard-demo-3`

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

1. **Start with Demo 1** - Understand basic wizard lifecycle
2. **Review Demo 2** - See alternative UI pattern
3. **Study Demo 3** - Learn production patterns
4. **Build Your Own** - Apply patterns to your use case

---

## 📚 Additional Resources

- **[Main README](README.md)** - Framework documentation
- **[Architecture Guide](../BlazorWizardDemo/WIZARD_ARCHITECTURE.md)** - Design patterns
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
