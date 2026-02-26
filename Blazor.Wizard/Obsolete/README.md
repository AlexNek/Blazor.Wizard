# Obsolete Components

This folder contains legacy artifacts kept only for backward compatibility.

## Current Obsolete Files

- `FlowStep.razor`
- `IIdentifiableStep.cs`
- `IWizardStepLogic.cs`
- `IWizardResultBuilder.cs` - **Deprecated in v2.0** - Use `IWizardModelBuilder<TResult>` and `IWizardModelSplitter<TResult>` instead

## Important

`FormStepLogic<TModel>` and `ResultStepLogic<TResultModel>` are **not** obsolete.
They are active APIs in `Blazor.Wizard/Core`.

## Guidance

- Do not use files from this folder for new development.
- Use `IWizardStep`, `GeneralStepLogic<TModel>`, `FormStepLogic<TModel>`, `ResultStepLogic<TResultModel>`, and `WizardViewModel`/`ComponentWizardViewModel` instead.
- For model mapping, use `IWizardModelBuilder<TResult>` and `IWizardModelSplitter<TResult>` instead of `IWizardResultBuilder<TResult>`.

## Migration Guide

See [MIGRATION_GUIDE.md](../../MIGRATION_GUIDE.md) for detailed instructions on migrating from `IWizardResultBuilder` to the new interfaces.

## Removal Plan

Legacy files in this folder may be removed in a future major version.
