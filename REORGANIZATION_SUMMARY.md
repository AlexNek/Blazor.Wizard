# Project Reorganization Summary - v2.x

This document records the current repository organization after the refactor.

## Current Library Layout

```text
Blazor.Wizard/
├── Core/
├── Interfaces/
├── ViewModels/
├── Obsolete/
├── wwwroot/
├── Blazor.Wizard.csproj
├── BlazorWizard.nuspec
├── NUGET_README.md
└── PROJECT_STRUCTURE.md
```

## Important Clarifications

- `FormStepLogic<TModel>` and `ResultStepLogic<TResultModel>` are active APIs under `Core/`.
- `Blazor.Wizard` package is UI-agnostic; UI dialog components belong to app/demo projects.
- `Obsolete/` currently contains legacy artifacts only (`FlowStep.razor`, `IIdentifiableStep.cs`, `IWizardStepLogic.cs`).

## Compatibility

- Existing `WizardViewModel`-based flows remain supported.
- `ComponentWizardViewModel<TResult>` is available for DynamicComponent-based UI hosts.

## Related Docs

- [Library Structure](Blazor.Wizard/PROJECT_STRUCTURE.md)
- [Main README](readme.md)
- [Changelog](CHANGELOG.md)
