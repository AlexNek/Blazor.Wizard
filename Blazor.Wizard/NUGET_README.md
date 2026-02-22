# Blazor.Wizard

**Version:** 1.0.0  
**Author:** Alex Nek  
**License:** MIT

---

## Overview

Blazor.Wizard is an wizard library for Blazor applications that eliminates the complexity of building multi-step workflows. Handle navigation, validation, state management, and conditional branching with minimal code.

---

##  What Makes It Special

Unlike simple stepper components that just display UI, Blazor.Wizard provides:

- **Complete Workflow Management** - Not just navigation, but validation gates, lifecycle hooks, and state persistence
- **Type-Safe Architecture** - Strongly-typed step identifiers and data containers
- **UI-Agnostic** - Works with Bootstrap, MudBlazor, DevExpress, or any component library
- **Production-Ready** - Built-in error handling, async support, and extensibility points
- **Developer-Friendly** - Clean API, minimal boilerplate, easy testing

---

##  Quick Install

```shell
dotnet add package Blazor.Wizard
```

**Requirements:** .NET 8.0+

---

##  30-Second Example

```csharp
// 1. Define step logic
public class ContactStepLogic : GeneralStepLogic<ContactModel>
{
    public override Type Id => typeof(ContactStepLogic);
    
    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        // Navigate to next step
        return new StepResult { NextStepId = typeof(AddressStepLogic) };
    }
}

// 2. Configure flow
var flow = new WizardFlow<Type>(new WizardData());
flow.Add(new ContactStepLogic());
flow.Add(new AddressStepLogic());
flow.Add(new SummaryStepLogic());

// 3. Navigate
await flow.NextAsync(); // Validates and moves forward
await flow.PrevAsync(); // Moves backward
```

---

##  Key Features

### Core Capabilities
-  **Smart Navigation** - Next/Back with automatic validation
-  **State Management** - Share data across steps with `WizardData`
-  **Conditional Branching** - Dynamic routing based on user input
-  **Validation Integration** - Works with DataAnnotations and EditContext
-  **Lifecycle Hooks** - `EnterAsync`, `ValidateAsync`, `Evaluate`, `LeaveAsync`
-  **Result Aggregation** - Build final models with `IWizardResultBuilder`

### Advanced Features
- 🔧 **Step Adapters** - Override behavior via `IFlowStepAdapter`
- 🔧 **Visibility Control** - Show/hide steps dynamically
- 🔧 **Field-Level Validation** - Granular error messages
- 🔧 **Async-First** - Full async/await support
- 🔧 **Testable** - Logic isolated from UI

---

## Common Use Cases

| Scenario | Description |
|----------|-------------|
| **Multi-Step Forms** | Registration, applications, surveys |
| **E-Commerce** | Checkout, quote builders, product configurators |
| **Onboarding** | User setup, account activation, tutorials |
| **Workflows** | Approval processes, document workflows |
| **Wizards** | Configuration assistants, setup wizards |

---

##  Architecture

```
┌─────────────────────────────────────┐
│   Blazor Component (UI)             │
│   - Renders current step            │
│   - Binds to EditContext            │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   WizardViewModel                   │
│   - Manages Flow and Steps          │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   WizardFlow<TStep>                 │
│   - Navigation logic                │
│   - Validation coordination         │
│   - State change events             │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Step Logic (GeneralStepLogic)     │
│   - Business rules                  │
│   - Validation logic                │
│   - Conditional routing             │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   WizardData                        │
│   - Type-safe storage               │
│   - Shared state container          │
└─────────────────────────────────────┘
```

---

##  Code Examples

### Conditional Branching
```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    data.TryGet<PersonModel>(out var person);
    
    // Route based on age
    if (person.Age >= 65)
        return new StepResult { NextStepId = typeof(RetirementStepLogic) };
    else
        return new StepResult { NextStepId = typeof(EmploymentStepLogic) };
}
```

### Custom Validation
```csharp
public override StepResult Evaluate(IWizardData data, ValidationResult validation)
{
    data.TryGet<PaymentModel>(out var payment);
    
    if (payment.Amount > 10000 && string.IsNullOrEmpty(payment.AuthorizationCode))
    {
        validation.IsValid = false;
        validation.ErrorMessage = "Authorization required for amounts over $10,000";
        AddValidationError(GetEditContext(), nameof(PaymentModel.AuthorizationCode), 
                          validation.ErrorMessage);
        return new StepResult { StayOnStep = true, CanContinue = false };
    }
    
    return new StepResult { NextStepId = typeof(ConfirmationStepLogic) };
}
```

### Result Aggregation
```csharp
public class OrderResultBuilder : IWizardResultBuilder<Order>
{
    public Order Build(IWizardData data)
    {
        data.TryGet<CustomerModel>(out var customer);
        data.TryGet<ShippingModel>(out var shipping);
        data.TryGet<PaymentModel>(out var payment);
        
        return new Order
        {
            Customer = customer,
            ShippingAddress = shipping?.Address,
            PaymentMethod = payment?.Method,
            Total = CalculateTotal(data)
        };
    }
}
```

---

##  Package Contents

- **Core Libraries** - All `.cs` source files
- **Interfaces** - Extensibility points for customization
- **Base Classes** - `BaseStepLogic`, `GeneralStepLogic`
- **README** - Complete documentation
- **LICENSE** - MIT license

---

##  Resources

- **GitHub**: https://github.com/alexnek/blazor.wizard
- **Full Documentation**: See README.md in package
- **Demo Examples**: See DEMO.md for walkthrough
- **Changelog**: See CHANGELOG.md for version history

---

##  Support

- **Issues**: Report bugs on GitHub
- **Questions**: Open a discussion on GitHub

---

## License

MIT License - Free for personal and commercial use.

---

##  Why Choose Blazor.Wizard?

| Feature | Blazor.Wizard | Basic Stepper Components |
|---------|---------------|--------------------------|
| Navigation Logic | ✅ Built-in | ❌ Manual |
| Validation | ✅ Integrated | ❌ Custom code |
| State Management | ✅ Type-safe | ❌ Props/cascading |
| Conditional Routing | ✅ Built-in | ❌ Manual logic |
| Lifecycle Hooks | ✅ 4 hooks | ❌ None |
| Result Building | ✅ Built-in | ❌ Manual |
| Testability | ✅ Isolated logic | ❌ Coupled to UI |
| Extensibility | ✅ Adapters & factories | ❌ Limited |

---

**Get started in minutes. Scale to enterprise complexity.**

```shell
dotnet add package Blazor.Wizard
```
