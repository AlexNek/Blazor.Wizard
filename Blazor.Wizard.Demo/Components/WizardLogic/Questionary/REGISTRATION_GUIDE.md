# Questionary Wizard - Single Point of Registration

## ✅ Adding a New Step (ONE place only)

Edit: `QuestionaryStepRegistry.cs`

```csharp
private static readonly List<StepRegistration> _steps = new()
{
    new(EQuestionaryStepId.Step1, new QuestionaryStep1Model(), typeof(QuestionaryStep1)),
    new(EQuestionaryStepId.Step2, new QuestionaryStep2Model(), typeof(QuestionaryStep2)),
    new(EQuestionaryStepId.Step3, new QuestionaryStep3Model(), typeof(QuestionaryStep3)),
    // 👇 ADD NEW STEP HERE
    new(EQuestionaryStepId.Step4, new QuestionaryStep4Model(), typeof(QuestionaryStep4)),
    // Report step uses null model (uses main QuestionaryModel)
    new(EQuestionaryStepId.Report, null, typeof(QuestionaryReportStep))
};
```

That's it! The registry automatically:
- ✅ Creates the model instance
- ✅ Registers it in WizardData
- ✅ Maps it to the component type
- ✅ Adds it to WizardEngine steps
- ✅ **Validates at startup** - throws clear error if you forget to register a step

## 🛡️ Built-in Validation

The registry validates itself at startup. If you add a new enum value but forget to register it:

```csharp
public enum EQuestionaryStepId { Step1, Step2, Step3, Step4, Report } // Added Step4
```

But forget to add it to the registry, you'll get a **clear error message at startup**:

```
InvalidOperationException: Missing step registrations in QuestionaryStepRegistry: Step4. 
Add them to the _steps list in QuestionaryStepRegistry.cs
```

**No more runtime surprises!** ✅

## Architecture

```
QuestionaryStepRegistry (SINGLE SOURCE OF TRUTH)
    ↓
    ├─→ QuestionaryWizardViewModel (reads from registry)
    │       └─→ Creates WizardEngine with registered steps
    │
    └─→ QuestionaryWizardDialog (reads from registry)
            └─→ Renders DynamicComponent with registered type
```

## Benefits

1. **DRY**: No duplicate registration
2. **SOLID**: Open/Closed Principle compliant
3. **Maintainable**: Add/remove steps in one place
4. **Type-safe**: Compile-time checking
5. **Clear**: Single source of truth
6. **Fail-fast**: Startup validation catches missing registrations

## Example: Adding Step4

### Step 1: Add enum value
```csharp
public enum EQuestionaryStepId { Step1, Step2, Step3, Step4, Report }
```

### Step 2: Create model
```csharp
public class QuestionaryStep4Model { /* properties */ }
```

### Step 3: Create component
```razor
@* QuestionaryStep4.razor *@
<EditForm EditContext="EditContext">
    <!-- UI -->
</EditForm>
@code {
    [Parameter] public QuestionaryStep4Model Model { get; set; }
    [Parameter] public EditContext EditContext { get; set; }
}
```

### Step 4: Register in ONE place
```csharp
// QuestionaryStepRegistry.cs
new(EQuestionaryStepId.Step4, new QuestionaryStep4Model(), typeof(QuestionaryStep4))
```

Done! ✅

**If you forget Step 4, the app will fail at startup with a clear error message telling you exactly what's missing.**

## Special Case: Report Step

The Report step is special - it doesn't have its own model, it uses the main `QuestionaryModel`:

```csharp
new(EQuestionaryStepId.Report, null, typeof(QuestionaryReportStep))
```

The ViewModel handles this by using `_mainModel` when `Model` is null.
