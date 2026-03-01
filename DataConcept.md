# Blazor.Wizard Data Concept

This guide explains how data should be organized in a wizard and exactly when validation runs.

## 1. Data ownership model

Rule of thumb:
- Each page (step) owns exactly one step model (`TModel`).
- The wizard owns shared storage (`WizardData`) that contains all step models.

`BaseStepLogic<TModel>` already follows this pattern:
- On `EnterAsync`, it tries to load existing `TModel` from `WizardData`.
- If missing, it creates/keeps its local model and stores it in `WizardData`.
- On `BeforeLeaveAsync`, it writes the current step model back to `WizardData`.

This gives each step isolated state while still allowing cross-step reads when needed.

## 2. Common wizard data (`WizardData`)

`WizardData` is a type-keyed container:
- `Set<T>(value)` stores by model type.
- `TryGet<T>(out value)` reads safely.

Practical effect:
- `PersonInfoModel` and `AddressModel` can coexist.
- Any later step can read previous step data for routing or rules.
- The final result can be merged from all stored step models.

## 3. Split and merge responsibilities

Use mapper interfaces for boundary transformations:
- `IWizardModelSplitter<TResult>.Split(result, data)`
- `IWizardModelBuilder<TResult>.Build(data)`

### When to split

Call `ModelSplitter.Split(...)` before `StartAsync()` when opening wizard in edit mode or with prefilled data.

Example:
```csharp
_viewModel.Initialize(null);
_viewModel.ModelSplitter.Split(existingModel, _viewModel.Data);
await _viewModel.StartAsync();
```

### When to merge

`Build(...)` is used at completion (`FinishAsync`) to assemble final result from all step models in `WizardData`.

## 4. Validation timing: exactly when each layer runs

There are two validation layers:

### A. DataAnnotations / EditContext validation

This is `ValidateAsync(...)` (default implementation calls `EditContext.Validate()`).

It runs:
- On field changes (through `UpdateCanProceedAsync`) to update `CanProceed`.
- On `NextAsync` and `FinishAsync` before `Evaluate(...)`.

### B. Custom/business validation

This is your step `Evaluate(IWizardData data, ValidationResult validation)` logic.

It runs:
- On `NextAsync` and `FinishAsync` by default.
- Optionally on field changes if you override `OnFieldChanged` in your ViewModel.

Typical pattern for custom field errors:
- Derive from `GeneralStepLogic<TModel>`.
- Use `EnsureValidationMessageStore(...)`.
- `ClearValidation(...)` old message(s).
- `AddValidationError(...)` for custom rule failures.
- `NotifyValidation(...)` so UI updates immediately.

## 5. DataAnnotations vs custom validation: when to use which

Use DataAnnotations for:
- Required fields
- Length/range/format
- Simple per-property rules

Use custom validation (`Evaluate`) for:
- Business rules (for example `Age >= 16`)
- Cross-field rules
- Cross-step dependencies (needs data from another step)
- Conditional navigation decisions tied to rule outcomes

Recommended flow:
1. Let `ValidateAsync` enforce DataAnnotations.
2. In `Evaluate`, enforce business rules and decide `StepResult`.

## 6. Immediate custom validation right after page is shown

By default, custom `Evaluate` runs on Next/Finish. If you want immediate custom validation after step view loads, use one of these approaches.

### Option A (recommended): trigger from step `EnterAsync`

```csharp
public override ValueTask EnterAsync(IWizardData data)
{
    base.EnterAsync(data);

    var validation = new ValidationResult { IsValid = true };
    Evaluate(data, validation); // executes business rules immediately

    return ValueTask.CompletedTask;
}
```

If you add custom field messages, make sure `Evaluate` calls `EnsureValidationMessageStore(...)` and `NotifyValidation(...)`.

### Option B: trigger from ViewModel after navigation

After entering a step (or in an overridden `OnFieldChanged`), call:
- `await step.ValidateAsync(Data)`
- `step.Evaluate(Data, validation)`

This is useful when you want live business-rule feedback while user types.

## 7. Recommended architecture checklist

- One model class per page.
- Store/retrieve through `WizardData` only.
- Use `ModelSplitter` before `StartAsync` for edit/prefill.
- Use `ModelBuilder` for final merge.
- Keep DataAnnotations in model attributes.
- Keep business rules in `Evaluate`.
- Add custom UI errors via `GeneralStepLogic` message helpers.
- If required, run custom validation on `EnterAsync` for immediate feedback.

## 8. Common errors (and how to avoid them)

- Sharing one model type across multiple pages.
Consequence: both pages write to the same `WizardData` key (type), so data and validation state can overwrite each other.
Fix: use one dedicated model class per page/step.

- Adding validation attributes but missing `<DataAnnotationsValidator />` in the form.
Consequence: attribute-based validation does not run in `EditContext.Validate()`.
Fix: include `<DataAnnotationsValidator />` inside every step `EditForm` that relies on attributes.

- Writing rules only in `Evaluate(...)` and expecting immediate feedback while typing.
Consequence: users see business-rule errors only on Next/Finish.
Fix: run business validation from overridden `OnFieldChanged` and/or on `EnterAsync`.

- Forgetting to clear old custom errors before adding new ones.
Consequence: stale messages stay visible even after values are fixed.
Fix: call `ClearValidation(...)` before `AddValidationError(...)` for the same field.

- Calling `ModelSplitter.Split(...)` after `StartAsync()`.
Consequence: first rendered step may use empty/default data and then jump unexpectedly.
Fix: call `Initialize(...)`, then `ModelSplitter.Split(...)`, then `StartAsync()`.

- Building final result from UI component state instead of `WizardData`.
Consequence: result can miss changes from non-current steps.
Fix: always aggregate in `IWizardModelBuilder<TResult>.Build(IWizardData data)`.

- Using the same step `Id` for multiple steps.
Consequence: `NextStepId` routing can target the wrong step.
Fix: keep unique `Id` per step type.

## 9. Small end-to-end shape

```csharp
public class PersonMapper : IWizardModelBuilder<PersonResult>, IWizardModelSplitter<PersonResult>
{
    public PersonResult Build(IWizardData data) { /* merge step models */ }
    public void Split(PersonResult result, IWizardData data) { /* prefill step models */ }
}

public sealed class PersonInfoStep : GeneralStepLogic<PersonInfoModel>
{
    public override Type Id => typeof(PersonInfoStep);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        var ec = GetEditContext();
        EnsureValidationMessageStore(ec);
        ClearValidation(ec, nameof(PersonInfoModel.Age));

        if (data.TryGet<PersonInfoModel>(out var model) && model != null && model.Age < 16)
        {
            validation.IsValid = false;
            AddValidationError(ec, nameof(PersonInfoModel.Age), "Age must be at least 16.");
            NotifyValidation(ec);
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        NotifyValidation(ec);
        return new StepResult { NextStepId = typeof(AddressStep), CanContinue = true };
    }
}
```

This keeps page data isolated, wizard data centralized, and validation predictable.

