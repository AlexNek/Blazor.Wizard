# Changelog

All notable changes to Blazor.Wizard will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.0.1] - 2026-02-24

### Fixed
- **WizardViewModel.NextAsync()** - Now auto-skips invisible steps when `NextStepId` is null, matching `BackAsync()` behavior
- **Code Quality** - Eliminated magic values by splitting `FindNextVisibleStepIndex()` into explicit `FindNextVisibleStepIndex()` and `FindPreviousVisibleStepIndex()` methods

### Changed
- **Navigation Consistency** - Both forward and backward navigation now consistently skip invisible steps

---

## [2.0.0] - 

###  Major Release

Significant architectural improvements and new features for enhanced wizard management.

#### Added
- **ComponentWizardViewModel<TResult>** - Component-oriented base view model
- **IWizardContext** - New context interface for wizard state management
- **Step Registry Pattern** - Centralized step registration with `PersonStepRegistry` and `QuestionaryStepRegistry`
- **FormStepLogic<TModel>** - Reusable simple form-step implementation
- **ResultStepLogic<TResultModel>** - Reusable summary/result step implementation
- **Questionary Wizard Example** - Complete multi-step questionnaire implementation
- **Serilog Integration** - Built-in diagnostics with `SerilogWizardDiagnostics`
- **Enhanced Testing** - 7 new test files 
  - `BaseStepLogicTests` 
  - `GeneralStepLogicTests` 
  - `StepResultTests`
  - `ValidationResultTests` 
  - `WizardDataTests`
  - `WizardFlowGenericTests` 
  - `WizardStepFactoryTests` 

#### Changed
- **WizardViewModel** - Major refactoring 
- **BaseStepLogic** - Enhanced s of new functionality
- **WizardData** - Improved data management 
- **WizardFlow** - Optimized flow control
- **Project Structure** - Reorganized Components folder to root level
- **Step Registration** - New registration approach with `StepRegistration` classes

#### Breaking Changes
- Folder structure reorganization (Components moved to root)
- Modified `BaseStepLogic` interface
- Updated `WizardData` API
- Changed step registration pattern
- New `IWizardContext` requirement for advanced scenarios

#### Documentation
- Added `REGISTRATION_GUIDE.md` for Person wizard
- Added `REGISTRATION_GUIDE.md` for Questionary wizard
- Enhanced `demo.md` 
- Updated `readme.md`

#### Migration Notes
See migration guide below for upgrading from 1.0.0 to 2.0.0.

---

## [1.0.0] - 2026-02-22

### 🎉 Initial Release

The first stable release of Blazor.Wizard, a robust wizard framework for Blazor applications.

#### Added
- **Core Framework**
  - `WizardFlow<TStep>` - Main flow controller with navigation logic
  - `WizardViewModel<TStep>` - Base view model for wizard implementations
  - `WizardData` - Type-safe container for sharing data between steps
  - `IWizardStep` - Core interface for step implementations

- **Step Logic Classes**
  - `BaseStepLogic<TModel>` - Abstract base class with model and EditContext management
  - `GeneralStepLogic<TModel>` - Extended base with ValidationMessageStore helpers
  - Step lifecycle hooks: `EnterAsync`, `ValidateAsync`, `Evaluate`, `LeaveAsync`

- **Navigation Features**
  - Async navigation with `NextAsync()` and `PrevAsync()`
  - Automatic validation before step transitions
  - Conditional step visibility with `IsVisible` property
  - Skip hidden steps during navigation

- **Validation System**
  - Integration with Blazor's `EditContext` and `DataAnnotations`
  - `ValidationResult` for custom validation messages
  - `ValidationMessageStore` for field-level error management
  - Helper methods: `AddValidationError`, `ClearValidation`, `NotifyValidation`

- **Advanced Features**
  - `IFlowStepAdapter` for custom step behavior overrides
  - `IWizardResultBuilder<TResult>` for result aggregation
  - `IWizardStepFactory` for dynamic step creation
  - `StepResult` for conditional branching and routing
  - State change notifications via `StateChanged` event

- **Type Safety**
  - Generic type parameters throughout the API
  - Type-based step identifiers via `Type Id` property
  - Compile-time checking for step navigation

#### Features
- ✅ Multi-step form workflows
- ✅ Conditional step branching
- ✅ Async/await support throughout
- ✅ UI-agnostic core (works with any component library)
- ✅ Testable business logic
- ✅ Extensible architecture

#### Documentation
- Comprehensive README with quick start guide
- Code examples for common scenarios
- Architecture documentation
- NuGet package specification







---

## Version History Summary

| Version | Date       | Description                                      |
|---------|------------|--------------------------------------------------|
| 2.0.1   | 2026-02-24 | Bug fix: Auto-skip invisible steps in NextAsync |
| 2.0.0   | 2026-02-23 | Major architectural improvements & new features  |
| 1.0.0   | 2026-02-22 | Initial stable release                           |

---

## Breaking Changes

### Version 2.0.0
- **Folder Structure**: Components moved from `Components/` to root level
- **Step Registration**: New registry pattern replaces manual step instantiation
- **BaseStepLogic**: Modified interface with new virtual methods
- **WizardData**: Updated API for data management
- **IWizardContext**: New interface for advanced wizard scenarios

---

## Migration Guide

### From 1.0.0 to 2.0.0

#### Step 1: Update Package Reference
```xml
<PackageReference Include="Blazor.Wizard" Version="2.0.0" />
```

#### Step 2: Adopt Step Registry Pattern (Recommended)
```csharp
// Old approach (1.0.0)
public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    public PersonWizardViewModel() : base(new PersonModelResultBuilder())
    {
        Initialize(new List<Func<IWizardStep>>
        {
            () => new PersonInfoStepLogic("demo"),
            () => new AddressStepLogic(),
            () => new SummaryStepLogic()
        });
    }
}

// New approach (2.0.0)
public class PersonWizardViewModel : ComponentWizardViewModel<PersonModel>
{
    public PersonWizardViewModel() : base(new PersonModelResultBuilder())
    {
    }

    protected override Type ResolveComponentType(IWizardStep step) => ...;
    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories() => ...;
}
```

#### Step 3: Keep UI layer separate from library core
Use app-layer dialogs/components (Bootstrap, DevExpress, etc.) while keeping the library headless.

#### Step 4: Update Folder References
If you reference demo components, update paths:
- `Components/Person/` → `Person/`
- `Components/Layout/` → `Layout/`
- `Components/Pages/` → `Pages/`

#### Step 5: Test Your Wizard Flows
Run comprehensive tests to ensure compatibility.

### From Pre-release to 1.0.0
This is the first official release. If you were using internal builds:
- Update package reference to `Blazor.Wizard 1.0.0`
- Review API changes in interfaces (consider them now stable)
- Test your wizard flows with the official release

---

## Support

- **Issues**: https://github.com/alexnek/blazor.wizard/issues
- **Discussions**: https://github.com/alexnek/blazor.wizard/discussions

---

**Legend**:
- Added - New features
- Changed - Changes in existing functionality
- Deprecated - Soon-to-be removed features
- Removed - Removed features
- Fixed - Bug fixes
- Security - Security fixes
