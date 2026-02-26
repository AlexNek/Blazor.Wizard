using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;

using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class WizardStepFactoryTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmptyFactory()
    {
        // Arrange & Act
        var factory = new WizardStepFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeAssignableTo<IWizardStepFactory>();
    }

    [Fact]
    public void Register_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);
        Func<IWizardStep> creator = () => new TestStep();

        // Act
        Action act = () => factory.Register(stepType, creator);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Register_WithNullStepType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var factory = new WizardStepFactory();
        Func<IWizardStep> creator = () => new TestStep();

        // Act
        Action act = () => factory.Register(null!, creator);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("stepType");
    }

    [Fact]
    public void Register_WithNullCreator_ShouldThrowArgumentNullException()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);

        // Act
        Action act = () => factory.Register(stepType, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("creator");
    }

    [Fact]
    public void CreateStep_WithRegisteredType_ShouldReturnInstance()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);
        factory.Register(stepType, () => new TestStep());

        // Act
        var step = factory.CreateStep(stepType);

        // Assert
        step.Should().NotBeNull();
        step.Should().BeOfType<TestStep>();
    }

    [Fact]
    public void CreateStep_WithUnregisteredType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);

        // Act
        Action act = () => factory.CreateStep(stepType);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"No step registered for type '{stepType.Name}'.");
    }

    [Fact]
    public void CreateStep_ShouldCreateNewInstanceEachTime()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);
        factory.Register(stepType, () => new TestStep());

        // Act
        var step1 = factory.CreateStep(stepType);
        var step2 = factory.CreateStep(stepType);

        // Assert
        step1.Should().NotBeNull();
        step2.Should().NotBeNull();
        step1.Should().NotBeSameAs(step2);
    }

    [Fact]
    public void Register_OverwriteExistingType_ShouldReplaceCreator()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);
        var firstStep = new TestStep { Value = 1 };
        var secondStep = new TestStep { Value = 2 };
        
        factory.Register(stepType, () => firstStep);
        factory.Register(stepType, () => secondStep); // Overwrite

        // Act
        var step = factory.CreateStep(stepType);

        // Assert
        step.Should().BeSameAs(secondStep);
        ((TestStep)step).Value.Should().Be(2);
    }

    [Fact]
    public void RegisterMultipleTypes_ShouldStoreIndependently()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var type1 = typeof(TestStep);
        var type2 = typeof(AnotherTestStep);
        
        factory.Register(type1, () => new TestStep());
        factory.Register(type2, () => new AnotherTestStep());

        // Act
        var step1 = factory.CreateStep(type1);
        var step2 = factory.CreateStep(type2);

        // Assert
        step1.Should().BeOfType<TestStep>();
        step2.Should().BeOfType<AnotherTestStep>();
    }

    [Fact]
    public void CreateStep_WithDifferentGenericSteps_ShouldWorkCorrectly()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var type1 = typeof(GenericTestStep<int>);
        var type2 = typeof(GenericTestStep<string>);
        
        factory.Register(type1, () => new GenericTestStep<int>());
        factory.Register(type2, () => new GenericTestStep<string>());

        // Act
        var step1 = factory.CreateStep(type1);
        var step2 = factory.CreateStep(type2);

        // Assert
        step1.Should().BeOfType<GenericTestStep<int>>();
        step2.Should().BeOfType<GenericTestStep<string>>();
        step1.Should().NotBeSameAs(step2);
    }

    [Fact]
    public void TypicalScenario_RegisterAndCreateMultipleSteps()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var step1Type = typeof(TestStep);
        var step2Type = typeof(AnotherTestStep);
        var step3Type = typeof(GenericTestStep<string>);

        factory.Register(step1Type, () => new TestStep());
        factory.Register(step2Type, () => new AnotherTestStep());
        factory.Register(step3Type, () => new GenericTestStep<string>());

        // Act
        var step1 = factory.CreateStep(step1Type);
        var step2 = factory.CreateStep(step2Type);
        var step3 = factory.CreateStep(step3Type);

        // Assert
        step1.Should().BeOfType<TestStep>();
        step2.Should().BeOfType<AnotherTestStep>();
        step3.Should().BeOfType<GenericTestStep<string>>();
    }

    [Fact]
    public void Creator_WithComplexInitialization_ShouldExecuteCorrectly()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);
        var counter = 0;
        
        factory.Register(stepType, () =>
        {
            counter++;
            return new TestStep { Value = counter * 10 };
        });

        // Act
        var step1 = factory.CreateStep(stepType) as TestStep;
        var step2 = factory.CreateStep(stepType) as TestStep;

        // Assert
        counter.Should().Be(2);
        step1!.Value.Should().Be(10);
        step2!.Value.Should().Be(20);
    }

    [Fact]
    public void CreateStep_CalledMultipleTimes_ShouldInvokeCreatorEachTime()
    {
        // Arrange
        var factory = new WizardStepFactory();
        var stepType = typeof(TestStep);
        var invocationCount = 0;
        
        factory.Register(stepType, () =>
        {
            invocationCount++;
            return new TestStep();
        });

        // Act
        factory.CreateStep(stepType);
        factory.CreateStep(stepType);
        factory.CreateStep(stepType);

        // Assert
        invocationCount.Should().Be(3);
    }

    // Helper classes for testing
    private class TestStep : IWizardStep
    {
        public int Value { get; set; }
        public Type Id => typeof(TestStep);
        public bool IsVisible => true;

        public ValueTask EnterAsync(IWizardData data) => ValueTask.CompletedTask;
        public StepResult Evaluate(IWizardData data, ValidationResult validation) => 
            new StepResult { CanContinue = true };
        public ValueTask BeforeLeaveAsync(IWizardData data) => ValueTask.CompletedTask;
        public ValueTask LeaveAsync(IWizardData data) => ValueTask.CompletedTask;
        public ValueTask<bool> ValidateAsync(IWizardData data) => ValueTask.FromResult(true);
    }

    private class AnotherTestStep : IWizardStep
    {
        public Type Id => typeof(AnotherTestStep);
        public bool IsVisible => true;

        public ValueTask EnterAsync(IWizardData data) => ValueTask.CompletedTask;
        public StepResult Evaluate(IWizardData data, ValidationResult validation) => 
            new StepResult { CanContinue = true };
        public ValueTask BeforeLeaveAsync(IWizardData data) => ValueTask.CompletedTask;
        public ValueTask LeaveAsync(IWizardData data) => ValueTask.CompletedTask;
        public ValueTask<bool> ValidateAsync(IWizardData data) => ValueTask.FromResult(true);
    }

    private class GenericTestStep<T> : IWizardStep
    {
        public Type Id => typeof(GenericTestStep<T>);
        public bool IsVisible => true;

        public ValueTask EnterAsync(IWizardData data) => ValueTask.CompletedTask;
        public StepResult Evaluate(IWizardData data, ValidationResult validation) => 
            new StepResult { CanContinue = true };
        public ValueTask BeforeLeaveAsync(IWizardData data) => ValueTask.CompletedTask;
        public ValueTask LeaveAsync(IWizardData data) => ValueTask.CompletedTask;
        public ValueTask<bool> ValidateAsync(IWizardData data) => ValueTask.FromResult(true);
    }
}
