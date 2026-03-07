# Blazor.Wizard.Demo

Native Blazor demo application for `Blazor.Wizard`.

## Summary

- UI: inline page and Bootstrap modal hosts with `DynamicComponent`
- App type: ASP.NET Core Blazor Web App
- Routes: `/` and `/inline-fun-wizard`
- Samples: inline fun wizard, person wizard dialog, and questionary wizard dialog

## Run

```powershell
dotnet run --project Blazor.Wizard.Demo
```

Open `/inline-fun-wizard` for the smallest example, or `/` for the dialog-based demos.

## Key Folders

- `Components/Common` - shared dialog host
- `Components/InlineFun` - simple inline wizard step components
- `Components/Person` - person wizard UI
- `Components/Questionary` - questionary wizard UI
- `Components/WizardLogic` - step/viewmodel registration and logic
- `Models` - demo models for all examples: small step models for inline/questionary flows, and aggregate plus split models for the person flow
- `Pages` - app pages and root demo entry point

## Notes

- The inline fun wizard demonstrates the smallest practical `Blazor.Wizard` setup.
- The person wizard demonstrates DI-heavy, business-rule-driven flow.
- The questionary wizard demonstrates the lighter reusable-step pattern.
- Model comparison: inline fun wizard uses tiny per-step models plus one result model; questionary follows the same simple step-model pattern; person wizard adds richer domain models and mapper/splitter-based prefill support.
