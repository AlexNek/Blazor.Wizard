# Person Wizard Registration Guide

## Single Source Of Truth

Edit only `PersonWizardDefinition.cs` to wire a step.

Each entry contains:
- `Id` (`EPersonStepId`)
- `StepIdType` (unique runtime step identifier type)
- `StepFactory` (step construction with DI support)
- `ComponentType` (UI component rendered by `DynamicComponent`)

## Add A New Step

1. Add enum value in `EPersonStepId`.
2. Create step logic class implementing `IWizardStep` or extending `BaseStepLogic<TModel>`.
3. Create Razor component for UI.
4. Add one registration entry in `PersonWizardDefinition`:

```csharp
new(
    EPersonStepId.YourStep,
    typeof(YourStepLogic),
    typeof(YourStepComponent),
    sp => ActivatorUtilities.CreateInstance<YourStepLogic>(sp))
```

The registry validates enum coverage at startup and throws if a step is missing.

## Hybrid Architecture Benefits

- **Async lifecycle**: `EnterAsync`, `ValidateAsync`, `LeaveAsync`, `BeforeLeaveAsync`
- **Conditional routing**: `Evaluate()` controls dynamic step flow (e.g., skip PensionInfo if age < 66)
- **Dynamic visibility**: `IsVisible` property hides/shows steps at runtime
- **DynamicComponent rendering**: No hardcoded if/else chains in UI
- **Factory pattern**: DI-ready step instantiation
- **Single registration point**: Add steps without touching UI code

## Key Differences from Questionary

- **Business rules**: Person wizard has age-based conditional logic
- **Step visibility**: PensionInfoStepLogic dynamically shows/hides
- **Live validation**: OnFieldChanged triggers real-time business validation
- **Custom parameters**: Steps can pass extra data via `GetComponentParameters()`
