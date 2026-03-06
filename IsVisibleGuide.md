# IsVisible Guide: Building Complex Conditional Wizards

This guide explains how to build wizards with dynamic step visibility based on:
- **Preconditions** - Initial state from a global model (e.g., edit mode)
- **Runtime conditions** - User input changes during wizard navigation

---

## The Problem

You need a wizard where:
1. Some steps only appear if certain conditions are met
2. Conditions can come from initial data (editing existing record)
3. Conditions can change as user fills out the wizard
4. Navigation must skip invisible steps automatically

**Example:** A pension step that only appears if user's age > 66.

---

## How It Works

### Navigation Pattern Overview

The framework supports two navigation approaches:

| Pattern | Invisible Steps | Best For |
|---------|----------------|----------|
| **WizardFlow** | Auto-skipped | Simple linear wizards |
| **WizardViewModel** | Manual routing via `Evaluate()` | Complex conditional wizards |

**This guide focuses on WizardViewModel pattern** for complex conditional wizards where:
- Multiple steps may be conditionally visible
- Navigation depends on business rules
- You manually control routing via `StepResult.NextStepId` in `Evaluate()`

**Key difference:** 
- **WizardFlow**: Auto-skips invisible steps (loops until it finds visible step)
- **WizardViewModel**: Uses `NextStepId` for explicit routing and otherwise advances to the next visible step

```csharp
// WizardViewModel.NextAsync() - explicit route or next visible step
var stepResult = step.Evaluate(_data, validation);
var nextStepIndex = FindNextStepIndex(stepResult.NextStepId);
if (nextStepIndex >= 0)
    Flow.Index = nextStepIndex; // Jump to explicitly specified step
else
    Flow.Index = FindNextVisibleStepIndex(Flow.Index + 1); // Auto-skip hidden steps
```

Use `NextStepId` when you need branching. If `NextStepId` is not set, the framework advances to the next visible step automatically.

### Visibility Control: Two Sources

**1. Preconditions (Global Model)**
- Initial state from existing data (e.g., edit mode, prefilled forms)
- Set via `ModelSplitter.Split()` before wizard starts
- Refreshed in `Initialize()`
- Example: Insurance application wizard where user's country = "USA" (loaded from profile) → shows USA-specific tax forms from start
- **Important:** Even with preconditions, user can change values during wizard, triggering dynamic visibility updates

**2. Dynamic Conditions (User Input)**
- Runtime state changes as user fills wizard
- Updated in `WizardData` as user progresses through steps
- Refreshed in `NextAsync()` before each navigation
- Example: User selects "Business" account type in step 1 → business verification steps appear in step 3
- Example: User changes age from 65 to 70 in step 1 → pension step becomes visible when navigating to step 3

**Key Insight:** Both sources use the same refresh mechanism. The difference is WHEN the refresh happens:
- Preconditions: Refresh once in `Initialize()` before wizard starts
- Dynamic conditions: Refresh in `NextAsync()` before each navigation (handles both initial values AND user changes)

### Three-Part Pattern

1. **Step** - Declares visibility logic based on cached data
2. **ViewModel** - Refreshes step caches from `WizardData` (both preconditions and dynamic conditions)
3. **Evaluate()** - Returns `NextStepId` to route around invisible steps

---

### 1. Step Declares Visibility Logic

Each step owns its visibility predicate:

```csharp
public sealed class PensionInfoStepLogic : BaseStepLogic<AddressModel>
{
    private PersonInfoModel? _cachedPersonInfo;
    
    public override bool IsVisible => _cachedPersonInfo?.Age > 66;
    
    public void UpdatePersonInfo(PersonInfoModel? personInfo)
    {
        _cachedPersonInfo = personInfo;
    }
}
```

**Key points:**
- `IsVisible` is computed from cached data, not from `WizardData` directly
- Step exposes an `Update...()` method to refresh the cache
- Visibility logic is centralized in one place

### 2. Navigation Routes Around Invisible Steps

**IMPORTANT:** When you provide `NextStepId`, `WizardViewModel` does not validate visibility for that explicit target. If `NextStepId` is `null`, it advances to the next visible step automatically.

You must ensure your routing logic only targets visible steps:

```csharp
public sealed class AddressStepLogic : BaseStepLogic<AddressModel>
{
    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
            return new StepResult { StayOnStep = true };
        
        // Route based on age - MUST specify NextStepId
        if (data.TryGet<PersonInfoModel>(out var person) && person.Age > 66)
            return new StepResult { NextStepId = typeof(PensionInfoStepLogic) };
        
        // Skip pension step by routing directly to summary
        return new StepResult { NextStepId = typeof(SummaryStepLogic) };
    }
}
```

**Key points:**
- `Evaluate()` reads from `WizardData` to decide next step
- Return `NextStepId` to jump to a specific step
- Framework does not validate visibility for an explicit target step
- Always ensure your explicit routing logic only targets the correct step

### 3. ViewModel Refreshes Visibility

ViewModel updates step caches before navigation:

```csharp
public class PersonWizardViewModel : ComponentWizardViewModel<PersonModel>
{
    public override async Task<bool> NextAsync()
    {
        UpdatePensionInfoIfNeeded();
        return await base.NextAsync();
    }
    
    private void UpdatePensionInfoIfNeeded()
    {
        var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
        if (pensionStep != null && Data.TryGet<PersonInfoModel>(out var personInfo))
        {
            pensionStep.UpdatePersonInfo(personInfo);
        }
    }
}
```

**Key points:**
- Refresh happens before `base.NextAsync()`
- Reads from `WizardData` and pushes to step cache
- Can also refresh in `OnFieldChanged` for immediate UI updates

---

## Complete Pattern: Two Scenarios

### Scenario A: Precondition (Edit Mode)

User edits existing record with age = 70. Pension step should be visible from start.

```csharp
// 1. Load existing model
var existingPerson = await _repository.GetPersonAsync(id);

// 2. Split into wizard data
_viewModel.ModelSplitter.Split(existingPerson, _viewModel.Data);

// 3. Refresh visibility from initial data
_viewModel.Initialize(); // Calls UpdatePensionInfoIfNeeded internally

// 4. Start wizard
await _viewModel.StartAsync();
```

**Flow:**
1. `ModelSplitter.Split()` → Puts `PersonInfoModel` into `WizardData`
2. `Initialize()` → Reads from `WizardData`, updates pension step cache
3. `PensionInfoStepLogic.IsVisible` → Returns `true` (age 70 > 66)

### Scenario B: Runtime Condition (New Record)

User creates new record, enters age = 70 during wizard. Pension step appears dynamically.

```csharp
public override async Task<bool> NextAsync()
{
    UpdatePensionInfoIfNeeded(); // Refresh before navigation
    return await base.NextAsync();
}

public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    // Route based on current age in WizardData
    if (data.TryGet<PersonInfoModel>(out var person) && person.Age > 66)
        return new StepResult { NextStepId = typeof(PensionInfoStepLogic) };
    
    return new StepResult { NextStepId = typeof(SummaryStepLogic) };
}
```

**Flow:**
1. User enters age = 70 in PersonInfo step
2. Clicks "Next"
3. `NextAsync()` → Calls `UpdatePensionInfoIfNeeded()`
4. Pension step cache updated → `IsVisible` becomes `true`
5. `AddressStepLogic.Evaluate()` → Routes to `PensionInfoStepLogic`

---

## Implementation Checklist

### Step Logic
- [ ] Declare `IsVisible` property based on cached data
- [ ] Add `Update...()` method to refresh cache
- [ ] Never read `WizardData` directly in `IsVisible` getter
- [ ] Use `Evaluate()` to route around invisible steps

### ViewModel
- [ ] Create `UpdateVisibility()` method that reads `WizardData`
- [ ] Call `UpdateVisibility()` in `Initialize()` (for preconditions)
- [ ] Call `UpdateVisibility()` in `NextAsync()` (for runtime conditions)
- [ ] Optional: Call in `OnFieldChanged()` for immediate UI feedback

### Navigation
- [ ] `Evaluate()` returns `NextStepId` that matches visibility state
- [ ] Never route to a step that will be invisible
- [ ] Test both visible and invisible paths

---

## Common Mistakes

❌ **Reading WizardData in IsVisible getter**
```csharp
// DON'T: Creates tight coupling and timing issues
public override bool IsVisible => 
    Data.TryGet<PersonInfoModel>(out var p) && p.Age > 66;
```

✅ **Use cached data instead**
```csharp
public override bool IsVisible => _cachedPersonInfo?.Age > 66;
```

---

❌ **Forgetting to refresh before navigation**
```csharp
// DON'T: Visibility will be stale
public override async Task<bool> NextAsync()
{
    return await base.NextAsync(); // Missing refresh!
}
```

✅ **Always refresh first**
```csharp
public override async Task<bool> NextAsync()
{
    UpdatePensionInfoIfNeeded();
    return await base.NextAsync();
}
```

---

❌ **Routing to invisible steps**
```csharp
// DON'T: Doesn't check if pension step is visible
return new StepResult { NextStepId = typeof(PensionInfoStepLogic) };
```

✅ **Route based on condition**
```csharp
if (person.Age > 66)
    return new StepResult { NextStepId = typeof(PensionInfoStepLogic) };
return new StepResult { NextStepId = typeof(SummaryStepLogic) };
```

---

## Advanced: Multiple Dependent Steps

If multiple steps depend on the same condition:

```csharp
private void UpdateVisibilityFromAge()
{
    if (!Data.TryGet<PersonInfoModel>(out var person)) return;
    
    var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
    var retirementStep = Steps.OfType<RetirementStepLogic>().FirstOrDefault();
    var healthStep = Steps.OfType<HealthInsuranceStepLogic>().FirstOrDefault();
    
    pensionStep?.UpdatePersonInfo(person);
    retirementStep?.UpdatePersonInfo(person);
    healthStep?.UpdatePersonInfo(person);
}
```

Call once in `NextAsync()` to update all dependent steps.

---

## Summary

**Three-part pattern:**
1. **Step** - Owns visibility logic, exposes update method
2. **ViewModel** - Refreshes step caches from `WizardData`
3. **Navigation** - Routes around invisible steps in `Evaluate()`

**Two refresh points:**
- `Initialize()` - For preconditions (edit mode)
- `NextAsync()` - For runtime conditions (user input)

**Golden rule:** Always return `NextStepId` pointing to visible steps in `Evaluate()`.

---

## Understanding Navigation Patterns

> Correction: in the current implementation, `WizardViewModel` auto-skips hidden steps when `NextStepId` is `null`. Explicit `NextStepId` values are still trusted as-is, so explicit branching must target the correct step.

### Are Invisible Steps Automatically Skipped?

**It depends on which navigation approach you use:**

| Approach | Auto-Skip? | How It Works |
|----------|-----------|-------------|
| `WizardFlow` directly | ✅ YES | `while (nextIndex < _steps.Count && !_steps[nextIndex].IsVisible) nextIndex++;` |
| `WizardViewModel` | ❌ NO | Uses `Evaluate()` to return `NextStepId` - YOU control routing |

### WizardFlow Pattern (Auto-Skip)
```csharp
// Simple linear wizard - framework skips invisible steps
var nextIndex = Index + 1;
while (nextIndex < _steps.Count && !_steps[nextIndex].IsVisible) nextIndex++;
```
**Use when:** Simple linear wizard (1→2→3) with one or two conditional steps.

### WizardViewModel Pattern (Manual Routing)

**Use when:** Complex branching logic, multiple conditional steps, non-linear navigation.

**How it works:** 
- You return `NextStepId` from `Evaluate()` to control which step comes next
- Framework jumps to that step without checking if it's visible
- If you return null, framework finds next visible step automatically

**Your responsibility:** Only return `NextStepId` pointing to visible steps.

**Example:**
```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    if (data.TryGet<PersonInfoModel>(out var person) && person.Age > 66)
        return new StepResult { NextStepId = typeof(PensionStep) }; // Go to pension
    
    return new StepResult { NextStepId = typeof(SummaryStep) }; // Skip pension
}
```es update method
2. **ViewModel** - Refreshes step caches from `WizardData`
3. **Navigation** - Routes around invisible steps in `Evaluate()`

**Two refresh points:**
- `Initialize()` - For preconditions (edit mode)
- `NextAsync()` - For runtime conditions (user input)

**Golden rule:** Never route to an invisible step. Always check conditions in `Evaluate()` and return the correct `NextStepId`.
