using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.Obsolete;

using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class WizardFlowGenericTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithWizardData()
    {
        // Arrange
        var wizardData = new WizardData();

        // Act
        var flow = new WizardFlow<int>(wizardData);

        // Assert
        flow.Should().NotBeNull();
        flow.Data.Should().BeSameAs(wizardData);
        flow.Index.Should().Be(0);
    }

    [Fact]
    public void Add_ShouldAddStepToFlow()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step = new TestStep();

        // Act
        flow.Add(step);

        // Assert - Flow should have the step added (verified through StartAsync)
        flow.Should().NotBeNull();
    }

    [Fact]
    public async Task StartAsync_WithNoSteps_ShouldNotThrow()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);

        // Act
        Func<Task> act = async () => await flow.StartAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task StartAsync_WithSteps_ShouldEnterFirstStep()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step = new TestStep();
        flow.Add(step);

        // Act
        await flow.StartAsync();

        // Assert
        step.EnterCount.Should().Be(1);
        flow.Index.Should().Be(0);
    }

    [Fact]
    public async Task StartAsync_ShouldRaiseStateChangedEvent()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step = new TestStep();
        flow.Add(step);
        var eventRaised = false;
        flow.StateChanged += () => eventRaised = true;

        // Act
        await flow.StartAsync();

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public async Task NextAsync_WithValidStep_ShouldMoveToNextStep()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        await flow.StartAsync();

        // Act
        await flow.NextAsync();

        // Assert
        flow.Index.Should().Be(1);
        step1.BeforeLeaveCount.Should().Be(1);
        step2.EnterCount.Should().Be(1);
    }

    [Fact]
    public async Task NextAsync_WithInvalidValidation_ShouldStayOnCurrentStep()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = false }; // Validation fails
        var step2 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        await flow.StartAsync();

        // Act
        await flow.NextAsync();

        // Assert
        flow.Index.Should().Be(0); // Should not advance
        step2.EnterCount.Should().Be(0);
    }

    [Fact]
    public async Task NextAsync_ShouldSkipInvisibleSteps()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep { StepIsVisible = false }; // Invisible
        var step3 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        flow.Add(step3);
        await flow.StartAsync();

        // Act
        await flow.NextAsync();

        // Assert
        flow.Index.Should().Be(2); // Should skip step2
        step2.EnterCount.Should().Be(0);
        step3.EnterCount.Should().Be(1);
    }

    [Fact]
    public async Task NextAsync_AtLastStep_ShouldNotAdvance()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep { ValidateResult = true };
        flow.Add(step1);
        flow.Add(step2);
        await flow.StartAsync();
        await flow.NextAsync(); // Move to last step

        // Act
        await flow.NextAsync(); // Try to advance beyond last step

        // Assert
        flow.Index.Should().Be(1); // Should stay at last step
    }

    [Fact]
    public async Task PrevAsync_ShouldMoveToPreviousStep()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        await flow.StartAsync();
        await flow.NextAsync(); // Move to step2

        // Act
        await flow.PrevAsync();

        // Assert
        flow.Index.Should().Be(0);
        step1.EnterCount.Should().Be(2); // Entered initially and when going back
    }

    [Fact]
    public async Task PrevAsync_AtFirstStep_ShouldNotGoBack()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep();
        flow.Add(step1);
        await flow.StartAsync();

        // Act
        await flow.PrevAsync();

        // Assert
        flow.Index.Should().Be(0); // Should stay at first step
    }

    [Fact]
    public async Task PrevAsync_ShouldSkipInvisibleSteps()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep { StepIsVisible = false, ValidateResult = true }; // Invisible
        var step3 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        flow.Add(step3);
        await flow.StartAsync();
        await flow.NextAsync(); // Should skip to step3

        // Act
        await flow.PrevAsync();

        // Assert
        flow.Index.Should().Be(0); // Should skip back to step1
        step1.EnterCount.Should().Be(2); // Initial + back navigation
    }

    [Fact]
    public async Task StateChanged_ShouldBeRaisedOnNextAsync()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        await flow.StartAsync();
        var eventCount = 0;
        flow.StateChanged += () => eventCount++;

        // Act
        await flow.NextAsync();

        // Assert
        eventCount.Should().Be(1);
    }

    [Fact]
    public async Task StateChanged_ShouldBeRaisedOnPrevAsync()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep();
        flow.Add(step1);
        flow.Add(step2);
        await flow.StartAsync();
        await flow.NextAsync();
        var eventCount = 0;
        flow.StateChanged += () => eventCount++;

        // Act
        await flow.PrevAsync();

        // Assert
        eventCount.Should().Be(1);
    }

    [Fact]
    public void Register_ShouldStoreAdapter()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var adapter = new TestFlowStepAdapter();

        // Act
        Action act = () => flow.Register(1, adapter);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task NextAsync_WithAdapter_ShouldCallOnEnter()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new IdentifiableTestStep(1) { ValidateResult = true };
        var step2 = new IdentifiableTestStep(2);
        var adapter = new TestFlowStepAdapter();
        flow.Add(step1);
        flow.Add(step2);
        flow.Register(2, adapter);
        await flow.StartAsync();

        // Act
        await flow.NextAsync();

        // Assert
        adapter.OnEnterCount.Should().Be(1);
    }

    [Fact]
    public async Task NextAsync_WithAdapterDeniesLeave_ShouldStayOnStep()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new IdentifiableTestStep(1) { ValidateResult = true };
        var step2 = new IdentifiableTestStep(2) { ValidateResult = true };
        var step3 = new IdentifiableTestStep(3);
        var adapter = new TestFlowStepAdapter { CanLeaveResult = false }; // Deny leaving
        flow.Add(step1);
        flow.Add(step2);
        flow.Add(step3);
        flow.Register(2, adapter); // Register adapter for step 2
        await flow.StartAsync();
        await flow.NextAsync(); // Move to step2 (which has the adapter that denies leaving)

        // Act
        await flow.NextAsync(); // Try to leave step2

        // Assert
        flow.Index.Should().Be(1); // Should stay on step2 (index 1)
        step3.EnterCount.Should().Be(0); // Should not enter step3
        adapter.CanLeaveCallCount.Should().Be(1);
    }

    [Fact]
    public async Task PrevAsync_WithAdapterDeniesLeave_ShouldStayOnStep()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new IdentifiableTestStep(1) { ValidateResult = true };
        var step2 = new IdentifiableTestStep(2) { ValidateResult = true };
        var step3 = new IdentifiableTestStep(3);
        var adapter = new TestFlowStepAdapter { CanLeaveResult = false }; // Deny leaving
        flow.Add(step1);
        flow.Add(step2);
        flow.Add(step3);
        flow.Register(3, adapter); // Register adapter for step 3
        await flow.StartAsync();
        await flow.NextAsync(); // Move to step2
        await flow.NextAsync(); // Move to step3 (which has the adapter that denies leaving)

        // Act
        await flow.PrevAsync(); // Try to go back from step3

        // Assert
        flow.Index.Should().Be(2); // Should stay on step3 (index 2)
        adapter.CanLeaveCallCount.Should().Be(1);
    }

    [Fact]
    public async Task CompleteFlow_ShouldProceedThroughAllSteps()
    {
        // Arrange
        var wizardData = new WizardData();
        var flow = new WizardFlow<int>(wizardData);
        var step1 = new TestStep { ValidateResult = true };
        var step2 = new TestStep { ValidateResult = true };
        var step3 = new TestStep { ValidateResult = true };
        flow.Add(step1);
        flow.Add(step2);
        flow.Add(step3);

        // Act
        await flow.StartAsync();
        await flow.NextAsync();
        await flow.NextAsync();

        // Assert
        flow.Index.Should().Be(2);
        step1.EnterCount.Should().Be(1);
        step1.BeforeLeaveCount.Should().Be(1);
        step2.EnterCount.Should().Be(1);
        step2.BeforeLeaveCount.Should().Be(1);
        step3.EnterCount.Should().Be(1);
    }

    // Helper classes for testing
    private class TestStep : IWizardStep
    {
        public int EnterCount { get; private set; }
        public int BeforeLeaveCount { get; private set; }
        public int LeaveCount { get; private set; }
        public int ValidateCount { get; private set; }
        public bool ValidateResult { get; set; } = true;
        public bool StepIsVisible { get; set; } = true;

        public Type Id => typeof(TestStep);
        public bool IsVisible => StepIsVisible;

        public ValueTask EnterAsync(IWizardData data)
        {
            EnterCount++;
            return ValueTask.CompletedTask;
        }

        public StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }

        public ValueTask BeforeLeaveAsync(IWizardData data)
        {
            BeforeLeaveCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask LeaveAsync(IWizardData data)
        {
            LeaveCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> ValidateAsync(IWizardData data)
        {
            ValidateCount++;
            return ValueTask.FromResult(ValidateResult);
        }
    }

    private class IdentifiableTestStep : IWizardStep, IIdentifiableStep<int>
    {
        private readonly int _id;
        public int EnterCount { get; private set; }
        public int BeforeLeaveCount { get; private set; }
        public bool ValidateResult { get; set; } = true;
        public bool StepIsVisible { get; set; } = true;

        public IdentifiableTestStep(int id)
        {
            _id = id;
        }

        int IIdentifiableStep<int>.Id => _id;
        Type IWizardStep.Id => typeof(IdentifiableTestStep);
        public bool IsVisible => StepIsVisible;

        public ValueTask EnterAsync(IWizardData data)
        {
            EnterCount++;
            return ValueTask.CompletedTask;
        }

        public StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }

        public ValueTask BeforeLeaveAsync(IWizardData data)
        {
            BeforeLeaveCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask LeaveAsync(IWizardData data)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> ValidateAsync(IWizardData data)
        {
            return ValueTask.FromResult(ValidateResult);
        }
    }

    private class TestFlowStepAdapter : IFlowStepAdapter
    {
        public int OnEnterCount { get; private set; }
        public int CanLeaveCallCount { get; private set; }
        public bool CanLeaveResult { get; set; } = true;

        public Task OnEnterAsync()
        {
            OnEnterCount++;
            return Task.CompletedTask;
        }

        public Task<bool> CanLeaveAsync()
        {
            CanLeaveCallCount++;
            return Task.FromResult(CanLeaveResult);
        }

        public Task<bool> OnFinishAsync()
        {
            return Task.FromResult(true);
        }
    }
}
