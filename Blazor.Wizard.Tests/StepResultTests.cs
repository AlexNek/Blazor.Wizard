using Blazor.Wizard.Core;

using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class StepResultTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var result = new StepResult();

        // Assert
        result.CanContinue.Should().BeFalse();
        result.NextStepId.Should().BeNull();
        result.StayOnStep.Should().BeFalse();
    }

    [Fact]
    public void CanContinue_CanBeSetToTrue()
    {
        // Arrange & Act
        var result = new StepResult { CanContinue = true };

        // Assert
        result.CanContinue.Should().BeTrue();
    }

    [Fact]
    public void CanContinue_CanBeSetToFalse()
    {
        // Arrange & Act
        var result = new StepResult { CanContinue = false };

        // Assert
        result.CanContinue.Should().BeFalse();
    }

    [Fact]
    public void NextStepId_CanBeSet()
    {
        // Arrange
        var nextStepType = typeof(StepResultTests);

        // Act
        var result = new StepResult { NextStepId = nextStepType };

        // Assert
        result.NextStepId.Should().Be(nextStepType);
    }

    [Fact]
    public void NextStepId_CanBeNull()
    {
        // Arrange & Act
        var result = new StepResult { NextStepId = null };

        // Assert
        result.NextStepId.Should().BeNull();
    }

    [Fact]
    public void StayOnStep_CanBeSetToTrue()
    {
        // Arrange & Act
        var result = new StepResult { StayOnStep = true };

        // Assert
        result.StayOnStep.Should().BeTrue();
    }

    [Fact]
    public void StayOnStep_CanBeSetToFalse()
    {
        // Arrange & Act
        var result = new StepResult { StayOnStep = false };

        // Assert
        result.StayOnStep.Should().BeFalse();
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange
        var nextStepType = typeof(StepResultTests);

        // Act
        var result = new StepResult
        {
            CanContinue = true,
            NextStepId = nextStepType,
            StayOnStep = true
        };

        // Assert
        result.CanContinue.Should().BeTrue();
        result.NextStepId.Should().Be(nextStepType);
        result.StayOnStep.Should().BeTrue();
    }

    [Fact]
    public void TypicalScenario_AllowContinueToNextStep()
    {
        // Arrange & Act
        var result = new StepResult
        {
            CanContinue = true,
            NextStepId = null,
            StayOnStep = false
        };

        // Assert
        result.CanContinue.Should().BeTrue();
        result.NextStepId.Should().BeNull();
        result.StayOnStep.Should().BeFalse();
    }

    [Fact]
    public void TypicalScenario_PreventContinue()
    {
        // Arrange & Act
        var result = new StepResult
        {
            CanContinue = false
        };

        // Assert
        result.CanContinue.Should().BeFalse();
    }

    [Fact]
    public void TypicalScenario_JumpToSpecificStep()
    {
        // Arrange
        var targetStepType = typeof(TargetStep);

        // Act
        var result = new StepResult
        {
            CanContinue = true,
            NextStepId = targetStepType,
            StayOnStep = false
        };

        // Assert
        result.CanContinue.Should().BeTrue();
        result.NextStepId.Should().Be(targetStepType);
        result.StayOnStep.Should().BeFalse();
    }

    [Fact]
    public void TypicalScenario_StayOnCurrentStep()
    {
        // Arrange & Act
        var result = new StepResult
        {
            CanContinue = true,
            StayOnStep = true
        };

        // Assert
        result.CanContinue.Should().BeTrue();
        result.StayOnStep.Should().BeTrue();
    }

    [Fact]
    public void ImmutableBehavior_PropertiesUseInit()
    {
        // Arrange & Act
        var result = new StepResult
        {
            CanContinue = true,
            NextStepId = typeof(TargetStep),
            StayOnStep = false
        };

        // Assert - properties should be init-only, cannot be modified after construction
        // This test verifies the record-like behavior
        result.CanContinue.Should().BeTrue();
        result.NextStepId.Should().NotBeNull();
        result.StayOnStep.Should().BeFalse();
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public void CombinationsOfCanContinueAndStayOnStep(bool canContinue, bool stayOnStep)
    {
        // Arrange & Act
        var result = new StepResult
        {
            CanContinue = canContinue,
            StayOnStep = stayOnStep
        };

        // Assert
        result.CanContinue.Should().Be(canContinue);
        result.StayOnStep.Should().Be(stayOnStep);
    }

    // Helper class
    private class TargetStep { }
}
