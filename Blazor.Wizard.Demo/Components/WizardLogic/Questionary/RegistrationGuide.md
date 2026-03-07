# Questionary Wizard Registration Guide

## Single Source Of Truth

Edit only `QuestionaryStepRegistry.cs` to wire a step.

Each entry contains:
- `Id` (`EQuestionaryStepId`)
- `StepIdType` (unique runtime step identifier type)
- `StepFactory` (step construction)
- `ComponentType` (UI component rendered by `DynamicComponent`)

## Add A New Step

1. Add enum value in `EQuestionaryStepId`.
2. Create model (for form steps) and Razor component.
3. Choose reusable step kind (`FormStepLogic<TModel>` or `ResultStepLogic<TResultModel>`).
4. Add one registration entry in `QuestionaryStepRegistry` using reusable library steps:
   `FormStepLogic<TModel>` or `ResultStepLogic<TResultModel>`.

The registry validates enum coverage at startup and throws if a step is missing.

## Hybrid Architecture

- Runtime flow: `WizardViewModel<IWizardStep, WizardData, QuestionaryModel>`
- Rendering: `DynamicComponent` from registry mapping
- Logging/diagnostics: `IWizardDiagnostics` events from `WizardViewModel`

This keeps old-engine flexibility and new-engine maintainability.
