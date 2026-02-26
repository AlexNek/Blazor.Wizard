# Obsolete Components

This folder contains legacy artifacts kept only for backward compatibility.

## Current Obsolete Files

- `FlowStep.razor`
- `IIdentifiableStep.cs`
- `IWizardStepLogic.cs`

## Important

`FormStepLogic<TModel>` and `ResultStepLogic<TResultModel>` are **not** obsolete.
They are active APIs in `Blazor.Wizard/Core`.

## Guidance

- Do not use files from this folder for new development.
- Use `IWizardStep`, `GeneralStepLogic<TModel>`, `FormStepLogic<TModel>`, `ResultStepLogic<TResultModel>`, and `WizardViewModel`/`ComponentWizardViewModel` instead.

## Removal Plan

Legacy files in this folder may be removed in a future major version.
