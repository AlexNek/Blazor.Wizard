# Contributing Guide

Thank you for considering contributing to Blazor.Wizard! This guide will help you get started.

---

## Quick Start

1. **Discuss first** - Open an issue to discuss your idea, approach, and implementation plan before writing code
2. **Fork the repository** - Click the "Fork" button on the [Blazor.Wizard GitHub page](https://github.com/alexnek/Blazor.Wizard) to create your own copy
3. **Create a feature branch** - Use your own repository and branch from `master` with a descriptive name (e.g., `fix-validation-bug` or `add-async-step`). Feature branches isolate your changes from the main codebase
4. **Submit a pull request** - Push your feature branch to your fork, then open a PR from your fork's branch to the original repository's `master` branch

---

## Development Setup

### Prerequisites for actual version
- .NET 8 SDK  for library now; .NET 10.0 for demo project
- IDE: Visual Studio 2026, Rider, or VS Code with C# extension
- Git

### Clone and Build

**Using Command Line:**
```bash
git clone https://github.com/YourUsername/Blazor.Wizard.git
cd Blazor.Wizard
dotnet restore
dotnet build
```

**Using Git GUI (Visual Studio, Rider, VS Code):**
1. Open your IDE's Git interface
2. Select "Clone Repository"
3. Enter your fork's URL: `https://github.com/YourUsername/Blazor.Wizard.git`
4. Choose a local directory
5. After cloning, open the solution file and build using IDE's build command

### Run Tests
```bash
dotnet test
```

### Run Demo Project
```bash
cd Blazor.Wizard.Demo
dotnet run
```
Navigate to the URL shown in the console (`https://localhost:7111`)

---

## Code Standards

### Style Guidelines
- Follow standard C# conventions ([Common C# code conventions:Style guidelines](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions#style-guidelines)   )
- Use meaningful, descriptive names (avoid abbreviations)
- Keep methods focused and small (Single Responsibility Principle)
- Prefer composition over inheritance
- Use nullable reference types appropriately

### Documentation
- Add XML documentation comments for all public APIs
- Include `<summary>`, `<param>`, and `<returns>` tags
- Document non-obvious behavior or design decisions
- Update relevant markdown files when adding features

### Testing Requirements
- Write unit tests for all new logic
- Maintain or improve existing test coverage
- Test edge cases and validation scenarios
- Follow existing test patterns (Arrange-Act-Assert)
- Use descriptive test method names

---

## Contribution Types

### Bug Fixes
- Reference the issue number in your PR
- Include a test that reproduces the bug
- Explain the root cause in the PR description

### New Features
- **Discuss first** - Open an issue to propose the feature, explain the use case, and outline your implementation approach
- Wait for maintainer feedback before starting development
- Ensure it aligns with the library's design principles
- Include comprehensive tests
- Update documentation and examples

### Documentation
- Fix typos, improve clarity, add examples
- Keep documentation in sync with code changes
- Follow the existing documentation style

### Tests
- Expand coverage for existing features
- Add integration tests for wizard flows
- Test validation and error scenarios

---

## Pull Request Guidelines

### Before Submitting
- [ ] All tests pass locally
- [ ] Code follows project style guidelines
- [ ] New code has appropriate test coverage
- [ ] Documentation is updated
- [ ] Commits are atomic and well-described
- [ ] Branch is up to date with main

### PR Description Template
```
## Description
Brief summary of changes

## Related Issue
Fixes #123

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Test improvement

## Testing
Describe how you tested the changes

## Checklist
- [ ] Tests pass
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
```

### Review Process
- Maintainers will review within 3-5 business days
- Address feedback constructively
- Keep discussions focused and professional
- Be patient—quality takes time

### Merging Criteria
- All tests pass
- Code review approved
- No merge conflicts
- Documentation complete

---

## Getting Help

### Questions
- Open a [GitHub Discussion](https://github.com/YourRepo/Blazor.Wizard/discussions) for general questions
- Check existing issues and documentation first

### Issues
- Use issue templates when available
- Provide minimal reproducible examples
- Include environment details (.NET version, OS)

### Community
- Be respectful and inclusive
- Help others when you can
- Share your wizard implementations and patterns

---

## Design Principles

When contributing, keep these principles in mind:

- **Separation of Concerns** - UI renders, logic controls behavior
- **Flexibility** - Support simple and complex scenarios
- **Extensibility** - Allow customization without modification
- **Composability** - Enable mixing reusable components
- **Testability** - Keep business logic isolated

---

## Code Examples

### Adding a New Step Helper
```csharp
/// <summary>
/// Provides logic for steps that require async data loading.
/// </summary>
public class AsyncDataStepLogic<TModel> : BaseStepLogic<TModel>
{
    public AsyncDataStepLogic(object id) : base(id) { }

    public override async Task EnterAsync(IWizardData data, CancellationToken ct)
    {
        // Load data asynchronously
        await base.EnterAsync(data, ct);
    }
}
```

### Adding Tests
```csharp
[Fact]
public async Task EnterAsync_LoadsData_Successfully()
{
    // Arrange
    var step = new AsyncDataStepLogic<TestModel>(typeof(TestModel));
    var data = new WizardData();

    // Act
    await step.EnterAsync(data, CancellationToken.None);

    // Assert
    Assert.True(data.TryGet<TestModel>(out var model));
    Assert.NotNull(model);
}
```

---

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for helping make Blazor.Wizard better!** 🎉
