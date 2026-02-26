# Questionary Wizard Registration Guide

## Single Source Of Truth

Edit only `QuestionaryStepRegistry.cs` to wire a step.

Each entry contains:
- `Id` (`EQuestionaryStepId`)
- `StepType` (old engine runtime step logic)
- `StepFactory` (step construction)
- `ComponentType` (UI component rendered by `DynamicComponent`)

## Add A New Step

1. Add enum value in `EQuestionaryStepId`.
2. Create step logic class implementing `IWizardStep` (or `BaseStepLogic<TModel>`).
3. Create Razor component.
4. Add one registration entry in `QuestionaryStepRegistry`.

The registry validates enum coverage at startup and throws if a step is missing.

## Hybrid Architecture

- Runtime flow: `WizardViewModel<IWizardStep, WizardData, QuestionaryModel>`
- Rendering: `DynamicComponent` from registry mapping
- Logging/diagnostics: `IWizardDiagnostics` events from `WizardViewModel`

This keeps old-engine flexibility and new-engine maintainability.
