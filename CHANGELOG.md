# Changelog

All notable changes to Blazor.Wizard will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.0.0] - 2026-02-23

### 🚀 Major Release

Significant architectural improvements and new features for enhanced wizard management.

#### Added
- **WizardEngine** - New centralized engine for wizard orchestration (118 lines)
- **ComponentWizardViewModel** - Enhanced view model with component-specific features (82 lines)
- **IWizardContext** - New context interface for wizard state management
- **Step Registry Pattern** - Centralized step registration with `PersonStepRegistry` and `QuestionaryStepRegistry`
- **Enum-based Step IDs** - Type-safe step identification with `EPersonStepId` and `EQuestionaryStepId`
- **Questionary Wizard Example** - Complete multi-step questionnaire implementation
- **Serilog Integration** - Built-in diagnostics with `SerilogWizardDiagnostics`
- **Enhanced Testing** - 7 new test files with 2,000+ lines of comprehensive tests
  - `BaseStepLogicTests` (324 lines)
  - `GeneralStepLogicTests` (289 lines)
  - `StepResultTests` (206 lines)
  - `ValidationResultTests` (148 lines)
  - `WizardDataTests` (273 lines)
  - `WizardFlowGenericTests` (500 lines)
  - `WizardStepFactoryTests` (284 lines)

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

## [Unreleased]

### Planned Features
- 🔄 Step progress indicators
- 🔄 Save/resume wizard state (LocalStorage/DB)
- 🔄 Multi-level undo/redo
- 🔄 Wizard templates library
- 🔄 Enhanced async validation with debouncing
- 🔄 Built-in analytics hooks
- 🔄 Full ARIA support and keyboard navigation

---

## Version History Summary

| Version | Date       | Description                                      |
|---------|------------|--------------------------------------------------|
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
public class PersonWizardViewModel : WizardViewModel<Type>
{
    public PersonWizardViewModel()
    {
        var personStep = new PersonInfoStepLogic();
        Flow.Add(personStep);
    }
}

// New approach (2.0.0)
public class PersonWizardViewModel : ComponentWizardViewModel<EPersonStepId>
{
    public PersonWizardViewModel(PersonStepRegistry registry)
        : base(registry.CreateSteps(), new WizardData())
    {
    }
}
```

#### Step 3: Use Enum-based Step IDs
```csharp
public enum EPersonStepId
{
    PersonInfo,
    Address,
    Summary
}
```

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
