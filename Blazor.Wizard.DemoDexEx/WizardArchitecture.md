# Clean Wizard Architecture - WizardDemo3

> Historical note: this file describes an older DevExpress-oriented sample kept for reference. 
> The current primary sample in this repository is `Blazor.Wizard.Demo`.

## Overview

This implementation demonstrates a **workflow host** pattern where UI components participate in a wizard flow without inheriting base classes or coupling to wizard infrastructure.

## Architecture Benefits

? **Independent UI Components** - Forms remain pure UI components  
? **No Base Class Inheritance** - Components don't need to inherit from wizard base classes  
? **Optional Validation** - Validation logic is attached, not inherited  
? **External Dialog Buttons** - Works seamlessly with DevExpress popup controls  
? **SOLID Principles** - Clean separation of concerns  
? **Tiny Footprint** - Minimal code with maximum flexibility  

## Core Components

### 1. `IWizardStepLogic` (Contract)
```csharp
public interface IWizardStepLogic
{
    Task OnEnterAsync();         // Called when step becomes active
    Task<bool> CanLeaveAsync();  // Validates before leaving step
    Task<bool> OnFinishAsync();  // Validates before wizard completion
}
```

### 2. `WizardFlow<TStep>` (Engine)
The workflow engine that manages step navigation and state:
- Registers steps dynamically
- Tracks current step
- Handles navigation with validation
- Notifies components of state changes

### 3. `FlowStep<TStep>` (Adapter)
An invisible component that wraps any UI content and registers it as a wizard step:
- Displays content only when step is active
- Optionally attaches validation logic
- No inheritance required for wrapped components

### 4. Form Components
Pure UI components without wizard dependencies:
- `GeneralForm.razor` - Personal information form
- `AddressForm.razor` - Address information form
- `SummaryView.razor` - Read-only summary display

### 5. Step Logic Validators
Optional classes that implement `IWizardStepLogic`:
- `GeneralStepLogic` - Validates general information
- `AddressStepLogic` - Validates address information

## Usage Pattern

```razor
<DxPopup @bind-Visible="@_popupVisible">
    <BodyContentTemplate>
        <EditForm Model="@_model">
            <CascadingValue Value="_flow">
                <FlowStep TStep="Steps" Id="Steps.General" Logic="@_generalLogic">
                    <GeneralForm Model="@_model" />
                </FlowStep>
                
                <FlowStep TStep="Steps" Id="Steps.Address" Logic="@_addressLogic">
                    <AddressForm Model="@_model" />
                </FlowStep>
                
                <FlowStep TStep="Steps" Id="Steps.Summary">
                    <SummaryView Model="@_model" />
                </FlowStep>
            </CascadingValue>
        </EditForm>
    </BodyContentTemplate>
    
    <FooterContentTemplate>
        <DxButton Text="Back" Click="@(async () => await _flow.MoveAsync(-1))" />
        <DxButton Text="Next" Click="@(async () => await _flow.MoveAsync(1))" />
        <DxButton Text="Finish" Click="@HandleFinish" />
    </FooterContentTemplate>
</DxPopup>
```

## Key Design Patterns

### Adapter Pattern
`FlowStep` adapts any component into the wizard workflow without modification.

### Observer Pattern
`WizardFlow` notifies registered steps of state changes.

### Strategy Pattern
`IWizardStepLogic` implementations define step-specific behavior.

## Extensibility

The same form components can be reused in:
- Different wizard flows
- Standalone forms
- Different dialog systems (DevExpress, Bootstrap Modal, etc.)
- Mobile layouts
- Background automation UI

## Navigation Flow

1. **Start** ? `WizardFlow.StartAsync()` activates first step
2. **Navigation** ? `MoveAsync(+1)` or `MoveAsync(-1)` with validation
3. **Validation** ? `CanLeaveAsync()` checked before moving
4. **Finish** ? `OnFinishAsync()` validates before completion

## State Management

- Step enumeration defines available steps
- `WizardFlow` tracks current step
- State changes trigger UI updates via events
- No global state pollution
