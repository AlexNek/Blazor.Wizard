# Changelog

All notable changes to Blazor.Wizard will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
- 🔄 Save/resume wizard state
- 🔄 Multi-level undo/redo
- 🔄 Wizard templates library
- 🔄 Enhanced async validation with debouncing
- 🔄 Built-in analytics hooks

---

## Version History Summary

| Version | Date       | Description                    |
|---------|------------|--------------------------------|
| 1.0.0   | 2026-02-22 | Initial stable release         |

---

## Breaking Changes

None yet - this is the initial release.

---

## Migration Guide

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
