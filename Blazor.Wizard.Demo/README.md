# Blazor.Wizard.Demo

Native Blazor demo application for `Blazor.Wizard`.

## Summary

- UI: Bootstrap modal + `DynamicComponent`
- App type: ASP.NET Core Blazor Web App
- Route: `/`
- Sample dialogs: person wizard and questionary wizard

## Run

```powershell
dotnet run --project Blazor.Wizard.Demo
```

Open the root page and start either wizard from the home screen.

## Key Folders

- `Components/Common` - shared dialog host
- `Components/Person` - person wizard UI
- `Components/Questionary` - questionary wizard UI
- `Components/WizardLogic` - step/viewmodel registration and logic
- `Models` - demo models
- `Pages` - app pages and root demo entry point

## Notes

- The person wizard demonstrates DI-heavy, business-rule-driven flow.
- The questionary wizard demonstrates the lighter reusable-step pattern.
