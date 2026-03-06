# Project Reorganization Summary

This document records the current repository layout at a high level.

## Core Package Layout

```text
Blazor.Wizard/
├── Core/
├── Extensions/
├── Interfaces/
├── Obsolete/
├── ViewModels/
├── wwwroot/
├── Blazor.Wizard.csproj
├── BlazorWizard.nuspec
├── NUGET_README.md
└── PROJECT_STRUCTURE.md
```

## Current Notes

- `FormStepLogic<TModel>` and `ResultStepLogic<TResultModel>` are active APIs.
- `WizardData` service helper methods live in `Extensions/WizardDataServiceExtensions.cs`.
- `Obsolete/` still contains legacy compatibility artifacts, including `IWizardResultBuilder.cs`.
- UI hosts and step components live in app/demo projects, not in the core package.

## Demo Projects

- `Blazor.Wizard.Demo` - active native Blazor sample
- `Blazor.Wizard.DemoDexEx` - older DevExpress-oriented reference sample

## Compatibility

- `WizardViewModel<TStep, TData, TResult>` remains supported.
- `ComponentWizardViewModel<TResult>` is the recommended base for `DynamicComponent` hosts.
## Related Docs

- [Library Structure](Blazor.Wizard/PROJECT_STRUCTURE.md)
- [Main README](readme.md)
- [Changelog](CHANGELOG.md)
