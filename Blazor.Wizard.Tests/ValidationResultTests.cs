using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class ValidationResultTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var result = new ValidationResult();

        // Assert
        result.IsValid.Should().BeFalse(); // default(bool) is false
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void IsValid_CanBeSet()
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.IsValid = true;

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ErrorMessage_CanBeSet()
    {
        // Arrange
        var result = new ValidationResult();
        var errorMessage = "Validation failed";

        // Act
        result.ErrorMessage = errorMessage;

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void IsValid_CanBeSetToFalse()
    {
        // Arrange
        var result = new ValidationResult { IsValid = true };

        // Act
        result.IsValid = false;

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ErrorMessage_CanBeSetToNull()
    {
        // Arrange
        var result = new ValidationResult { ErrorMessage = "Error" };

        // Act
        result.ErrorMessage = null;

        // Assert
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ErrorMessage_CanBeSetToEmptyString()
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.ErrorMessage = string.Empty;

        // Assert
        result.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void MultiplePropertyUpdates_ShouldMaintainIndependentState()
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.IsValid = true;
        result.ErrorMessage = "Warning message";
        
        // Change again
        result.IsValid = false;
        result.ErrorMessage = "Error message";

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error message");
    }

    [Fact]
    public void TypicalValidationScenario_Success()
    {
        // Arrange & Act
        var result = new ValidationResult
        {
            IsValid = true,
            ErrorMessage = null
        };

        // Assert
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void TypicalValidationScenario_Failure()
    {
        // Arrange & Act
        var result = new ValidationResult
        {
            IsValid = false,
            ErrorMessage = "Field is required"
        };

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Field is required");
    }

    [Theory]
    [InlineData("Required field missing")]
    [InlineData("Invalid format")]
    [InlineData("Value out of range")]
    public void ErrorMessage_SupportsDifferentMessages(string errorMessage)
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.ErrorMessage = errorMessage;

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }
}
