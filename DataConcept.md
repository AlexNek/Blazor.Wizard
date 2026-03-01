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

## 9. Validation and Next button behavior

Use this simple model:

- `Flow.Index` = "Where am I now?" (current step position in the list)
- `StepResult.NextStepId` = "Where should I go next?" (optional jump target)
- `CanContinue` + `StayOnStep` = "Am I allowed to move at all?"

### `Flow.Index` vs `StepResult.NextStepId` (why both exist)

They solve different jobs:

- `Flow.Index` tracks current position and is used for back/forward mechanics.
- `NextStepId` is a routing decision returned by current step logic.

Transition algorithm:
1. Wizard is currently on `Steps[Flow.Index]`.
2. Current step runs validation + `Evaluate(...)`.
3. If `NextStepId` is set, wizard jumps to that step.
4. If `NextStepId` is null, wizard moves to next visible step by index.

Example from your code:
- `AddressStepLogic` chooses `NextStepId = PensionInfoStepLogic` for older users.
- Otherwise it chooses `NextStepId = SummaryStepLogic`.
- `Flow.Index` is still needed to know what current step is and to support Back.

### `CanContinue` vs `StayOnStep` (why both exist)

These are two separate intents:

- `CanContinue`: business permission ("is moving allowed?")
- `StayOnStep`: navigation command ("stay here even if allowed")

When `Next`/`OK` is clicked, wizard moves forward only when:
- form validation passed
- step logic says "continue allowed"
- step logic does not explicitly request "stay on this step"

Meaning of combinations:
- `CanContinue=true`, `StayOnStep=false`: normal success, move forward.
- `CanContinue=false`, `StayOnStep=false`: blocked by rule.
- `CanContinue=true`, `StayOnStep=true`: explicitly stay on current step.
- `CanContinue=false`, `StayOnStep=true`: blocked and forced stay.

Why not one flag only?
- With one flag, you lose intent clarity.
- Two flags let you distinguish "not allowed" from "allowed but intentionally stay".

Rule of thumb for day-to-day use:
- Most steps should return only two patterns:
- success: `CanContinue=true`, `StayOnStep=false`
- fail: `CanContinue=false`, `StayOnStep=true`

When to use the other 2 combinations:

- `CanContinue=false`, `StayOnStep=false`
Use when you only want to block progress, but do not care about "force stay" semantics.
In practice this behaves like a normal fail and user remains on current step.

Important in current implementation:
- There is no navigation difference between `CanContinue=false, StayOnStep=false` and `CanContinue=false, StayOnStep=true`, because `CanContinue=false` already blocks transition.
- The difference is semantic intent only: explicit stay vs implicit stay.

- `CanContinue=true`, `StayOnStep=true`
Use when validation passed, but you intentionally keep user on the same step for UX flow.
Example: show a confirmation/help panel first, then user clicks `Next` again to continue.

If you don't have a clear special UX case, do not use these two extra combinations.

### Why does DataAnnotations validation depend on data change notification?

`CanProceed` is refreshed from `OnFieldChanged` -> `UpdateCanProceedAsync` -> `ValidateAsync`.
So the button state updates when `EditContext` receives field-change notifications.

- With standard Blazor inputs (`InputText`, `InputNumber`, `@bind-Value`) this is automatic.
- If you change model values programmatically, call `EditContext.NotifyFieldChanged(...)` (or at least `NotifyValidationStateChanged`) so UI/validation state is recalculated immediately.

Without notification, validation can still run on `NextAsync`, but the live UI state (`CanProceed`, messages timing) may look stale.

### How to keep `Next` enabled but still run full validation?

You have two patterns:

1. Keep current behavior (recommended for strict UX):
- Disable `Next` when `CanProceed == false`.
- Validation still runs again on click (`NextAsync`/`FinishAsync`) for safety.

2. Always enable `Next`, validate on click only:
- Remove `disabled="@(!ViewModel.CanProceed)"` from `Next`/`OK` button.
- Keep `NextAsync`/`FinishAsync` unchanged.
- Result: user can click `Next` anytime, but cannot move forward unless validation + `Evaluate` pass.

This gives full validation while avoiding disabled buttons.

### How to show common errors not related to a specific field?

Use model-level validation messages:

- In step logic (`GeneralStepLogic`), add message to `ValidationMessageStore` using a model-level `FieldIdentifier` (empty field name).
- Call `NotifyValidation(editContext)`.
- In step UI, include `<ValidationSummary />` to render non-field/common messages.

Typical usage cases:
- "Server is temporarily unavailable."
- "Combination of entered data is invalid."
- Any cross-field or cross-step error that should appear once at the top.

If you only use `<ValidationMessage For="...">`, you will not see these common errors.

## 10. Small end-to-end shape

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

