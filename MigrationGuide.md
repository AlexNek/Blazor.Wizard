# Migration Guide: IWizardResultBuilder → IWizardModelBuilder/IWizardModelSplitter

## Overview

Starting from version 2.0, `IWizardResultBuilder<TResult>` is **obsolete** and replaced with two focused interfaces following the Interface Segregation Principle:

- **`IWizardModelBuilder<TResult>`** - Builds result model from wizard data (merge operation)
- **`IWizardModelSplitter<TResult>`** - Splits result model into wizard data (decompose operation)

## Why the Change?

### Before (v1.x - Single Method)
```csharp
public interface IWizardResultBuilder<TResult>
{
    TResult Build(IWizardData data);
}
```

**Limitations:**
- ❌ No way to prefill wizard from existing data
- ❌ Edit scenarios required manual data splitting
- ❌ No bidirectional mapping support

### After (v2.0 - Segregated Interfaces)
```csharp
public interface IWizardModelBuilder<TResult>
{
    TResult Build(IWizardData data);
}

public interface IWizardModelSplitter<TResult>
{
    void Split(TResult result, IWizardData data);
}
```

**Benefits:**
- ✅ Single Responsibility - each interface has one purpose
- ✅ Implement only what you need
- ✅ NEW: Bidirectional mapping for edit scenarios
- ✅ Clear naming - "Builder" builds, "Splitter" splits
- ✅ Better testability and maintainability

## Migration Steps

### Step 1: Rename Your Class

**Old Code (v1.x):**
```csharp
public class PersonModelResultBuilder : IWizardResultBuilder<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        // Build logic
    }
}
```

**New Code (v2.0):**
```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        // Build logic (unchanged)
    }
}
```

### Step 2: Add Split Method (Optional - NEW in v2.0)

If you need to prefill wizard from existing data (edit scenarios):

```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>, IWizardModelSplitter<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        // Build logic (unchanged)
    }
    
    // NEW: Split method for prefilling wizard
    public void Split(PersonModel result, IWizardData data)
    {
        data.Set(new PersonInfoModel
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email,
            Age = result.Age
        });
        
        data.Set(new AddressModel
        {
            Street = result.Street,
            City = result.City,
            ZipCode = result.ZipCode,
            Country = result.Country
        });
    }
}
```

### Step 3: Update ViewModel Constructor

**Old Code:**
```csharp
public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    public PersonWizardViewModel() 
        : base(new PersonModelResultBuilder())
    {
    }
}
```

**New Code:**
```csharp
public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    public PersonWizardViewModel() 
        : base(new PersonModelMapper())
    {
    }
}
```

### Step 4: Update Property Access (Only if using Split)

**Old Code:**
```csharp

var result = _viewModel.ResultBuilder.Build(_viewModel.Data);
```

**New Code:**
```csharp
_viewModel.ModelSplitter.Split(Model, _viewModel.Data);
var result = _viewModel.ModelBuilder.Build(_viewModel.Data);
```

## Use Cases

### Use Case 1: Build Only (Most Common)

If you only need to aggregate data from wizard steps into a result model:

```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        data.TryGet<PersonInfoModel>(out var person);
        data.TryGet<AddressModel>(out var address);
        
        return new PersonModel
        {
            FirstName = person?.FirstName ?? string.Empty,
            LastName = person?.LastName ?? string.Empty,
            Street = address?.Street ?? string.Empty,
            City = address?.City ?? string.Empty
        };
    }
}
```

### Use Case 2: Split Only (Prefill Wizard) - NEW in v2.0

If you only need to prefill wizard from existing data:

```csharp
public class PersonModelSplitter : IWizardModelSplitter<PersonModel>
{
    public void Split(PersonModel result, IWizardData data)
    {
        data.Set(new PersonInfoModel
        {
            FirstName = result.FirstName,
            LastName = result.LastName
        });
        
        data.Set(new AddressModel
        {
            Street = result.Street,
            City = result.City
        });
    }
}
```

### Use Case 3: Both (Bidirectional Mapping)

If you need both operations (e.g., edit existing data):

```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>, IWizardModelSplitter<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        // Aggregate step data into result
    }
    
    public void Split(PersonModel result, IWizardData data)
    {
        // Decompose result into step data
    }
}
```

## Backward Compatibility

The old `IWizardResultBuilder<TResult>` interface is marked `[Obsolete]` but still works. You'll see compiler warnings:

```
Warning CS0618: 'IWizardResultBuilder<TResult>' is obsolete: 
'Use IWizardModelBuilder<TResult> instead. IWizardResultBuilder only supports Build, 
while IWizardModelSplitter adds optional prefill support.'
```

**Recommendation:** Migrate to new interfaces to avoid future breaking changes.

## Complete Example

### Before
```csharp
public class PersonModelResultBuilder : IWizardResultBuilder<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        data.TryGet<PersonInfoModel>(out var person);
        data.TryGet<AddressModel>(out var address);
        return new PersonModel
        {
            FirstName = person?.FirstName ?? string.Empty,
            Street = address?.Street ?? string.Empty
        };
    }
}

public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    public PersonWizardViewModel() : base(new PersonModelResultBuilder()) { }
}
```

### After
```csharp
public class PersonModelMapper : IWizardModelBuilder<PersonModel>, IWizardModelSplitter<PersonModel>
{
    public PersonModel Build(IWizardData data)
    {
        data.TryGet<PersonInfoModel>(out var person);
        data.TryGet<AddressModel>(out var address);
        return new PersonModel
        {
            FirstName = person?.FirstName ?? string.Empty,
            Street = address?.Street ?? string.Empty
        };
    }
    
    public void Split(PersonModel result, IWizardData data)
    {
        data.Set(new PersonInfoModel { FirstName = result.FirstName });
        data.Set(new AddressModel { Street = result.Street });
    }
}

public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    public PersonWizardViewModel() : base(new PersonModelMapper()) { }
}
```

## FAQ

**Q: Do I need to implement both interfaces?**  
A: No! Implement only what you need. Most wizards only need `IWizardModelBuilder`.

**Q: What if I only need Build?**  
A: Just implement `IWizardModelBuilder<TResult>`.

**Q: Can I still use IWizardResultBuilder?**  
A: Yes, but it's obsolete. Migrate to avoid future breaking changes.

**Q: What's the performance impact?**  
A: None. The new interfaces are more efficient as you only implement what you need.

## Summary

| Aspect | Old | New |
|--------|-----|-----|
| **Interface** | `IWizardResultBuilder<TResult>` | `IWizardModelBuilder<TResult>` + `IWizardModelSplitter<TResult>` |
| **Class Name** | `*ResultBuilder` | `*ModelMapper` or `*ModelBuilder` |
| **Constructor** | `base(new PersonModelResultBuilder())` | `base(new PersonModelMapper())` |
| **Property** | `ResultBuilder` | `ModelBuilder` / `ModelSplitter` |
| **Status** | Obsolete | Current |

---

**Need Help?** Open an issue on GitHub or check the demo projects for complete examples.
