using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Tests;

public class GeneralStepLogicTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithoutValidationMessageStore()
    {
        // Arrange & Act
        var step = new TestGeneralStepLogic();

        // Assert
        step.GetValidationMessageStore().Should().BeNull();
    }

    [Fact]
    public void EnsureValidationMessageStore_WithEditContext_ShouldCreateStore()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();

        // Act
        step.EnsureValidationMessageStorePublic(editContext);

        // Assert
        step.GetValidationMessageStore().Should().NotBeNull();
    }

    [Fact]
    public void EnsureValidationMessageStore_WithNullEditContext_ShouldNotCreateStore()
    {
        // Arrange
        var step = new TestGeneralStepLogic();

        // Act
        step.EnsureValidationMessageStorePublic(null);

        // Assert
        step.GetValidationMessageStore().Should().BeNull();
    }

    [Fact]
    public void EnsureValidationMessageStore_CalledTwiceWithSameContext_ShouldUseSameStore()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();

        // Act
        step.EnsureValidationMessageStorePublic(editContext);
        var firstStore = step.GetValidationMessageStore();
        step.EnsureValidationMessageStorePublic(editContext);
        var secondStore = step.GetValidationMessageStore();

        // Assert
        firstStore.Should().BeSameAs(secondStore);
    }

    [Fact]
    public void EnsureValidationMessageStore_WithDifferentEditContext_ShouldCreateNewStore()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var model1 = new TestModel();
        var model2 = new TestModel();
        var editContext1 = new Microsoft.AspNetCore.Components.Forms.EditContext(model1);
        var editContext2 = new Microsoft.AspNetCore.Components.Forms.EditContext(model2);

        // Act
        step.EnsureValidationMessageStorePublic(editContext1);
        var firstStore = step.GetValidationMessageStore();
        step.EnsureValidationMessageStorePublic(editContext2);
        var secondStore = step.GetValidationMessageStore();

        // Assert
        firstStore.Should().NotBeNull();
        secondStore.Should().NotBeNull();
        firstStore.Should().NotBeSameAs(secondStore);
    }

    [Fact]
    public void AddValidationError_WithValidParameters_ShouldAddError()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();
        step.EnsureValidationMessageStorePublic(editContext);
        var fieldName = nameof(TestModel.Value);
        var errorMessage = "Test error message";

        // Act
        step.AddValidationErrorPublic(editContext, fieldName, errorMessage);
        var messages = editContext.GetValidationMessages(editContext.Field(fieldName));

        // Assert
        messages.Should().Contain(errorMessage);
    }

    [Fact]
    public void AddValidationError_WithNullEditContext_ShouldNotThrow()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        step.EnsureValidationMessageStorePublic(step.GetEditContext());

        // Act
        Action act = () => step.AddValidationErrorPublic(null, "FieldName", "Error");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AddValidationError_WithoutEnsureMessageStore_ShouldNotThrow()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();

        // Act
        Action act = () => step.AddValidationErrorPublic(editContext, "FieldName", "Error");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ClearValidation_WithExistingError_ShouldRemoveError()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();
        step.EnsureValidationMessageStorePublic(editContext);
        var fieldName = nameof(TestModel.Value);
        var errorMessage = "Test error";
        step.AddValidationErrorPublic(editContext, fieldName, errorMessage);

        // Act
        step.ClearValidationPublic(editContext, fieldName);
        var messages = editContext.GetValidationMessages(editContext.Field(fieldName));

        // Assert
        messages.Should().BeEmpty();
    }

    [Fact]
    public void ClearValidation_WithNullEditContext_ShouldNotThrow()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        step.EnsureValidationMessageStorePublic(step.GetEditContext());

        // Act
        Action act = () => step.ClearValidationPublic(null, "FieldName");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NotifyValidation_WithValidEditContext_ShouldNotThrow()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();
        var validationStateChangedCalled = false;
        editContext.OnValidationStateChanged += (sender, args) => validationStateChangedCalled = true;

        // Act
        step.NotifyValidationPublic(editContext);

        // Assert
        validationStateChangedCalled.Should().BeTrue();
    }

    [Fact]
    public void NotifyValidation_WithNullEditContext_ShouldNotThrow()
    {
        // Arrange
        var step = new TestGeneralStepLogic();

        // Act
        Action act = () => step.NotifyValidationPublic(null);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AddMultipleValidationErrors_ShouldAccumulateErrors()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();
        step.EnsureValidationMessageStorePublic(editContext);
        var fieldName = nameof(TestModel.Value);

        // Act
        step.AddValidationErrorPublic(editContext, fieldName, "Error 1");
        step.AddValidationErrorPublic(editContext, fieldName, "Error 2");
        var messages = editContext.GetValidationMessages(editContext.Field(fieldName)).ToList();

        // Assert
        messages.Should().HaveCount(2);
        messages.Should().Contain("Error 1");
        messages.Should().Contain("Error 2");
    }

    [Fact]
    public async Task ValidationWorkflow_AddErrorAndClear_ShouldWorkCorrectly()
    {
        // Arrange
        var step = new TestGeneralStepLogic();
        var editContext = step.GetEditContext();
        step.EnsureValidationMessageStorePublic(editContext);
        var fieldName = nameof(TestModel.Value);
        var wizardData = new WizardData();

        // Act - Add error
        step.AddValidationErrorPublic(editContext, fieldName, "Validation failed");
        var isValidWithError = await step.ValidateAsync(wizardData);
        
        // Clear error
        step.ClearValidationPublic(editContext, fieldName);
        var isValidAfterClear = await step.ValidateAsync(wizardData);

        // Assert
        isValidWithError.Should().BeFalse();
        isValidAfterClear.Should().BeTrue();
    }

    [Fact]
    public void InheritsFromBaseStepLogic_ShouldHaveAllBaseFunctionality()
    {
        // Arrange & Act
        var step = new TestGeneralStepLogic();

        // Assert
        step.Should().BeAssignableTo<BaseStepLogic<TestModel>>();
        step.GetModel().Should().NotBeNull();
        step.GetEditContext().Should().NotBeNull();
        step.IsVisible.Should().BeTrue();
    }

    // Helper classes for testing
    private class TestModel
    {
        public int Value { get; set; }
    }

    private class TestGeneralStepLogic : GeneralStepLogic<TestModel>
    {
        public override Type Id => typeof(TestGeneralStepLogic);

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }

        // Expose protected members for testing
        public void EnsureValidationMessageStorePublic(Microsoft.AspNetCore.Components.Forms.EditContext? editContext)
        {
            EnsureValidationMessageStore(editContext);
        }

        public void AddValidationErrorPublic(Microsoft.AspNetCore.Components.Forms.EditContext? editContext, string fieldName, string errorMessage)
        {
            AddValidationError(editContext, fieldName, errorMessage);
        }

        public void ClearValidationPublic(Microsoft.AspNetCore.Components.Forms.EditContext? editContext, string fieldName)
        {
            ClearValidation(editContext, fieldName);
        }

        public void NotifyValidationPublic(Microsoft.AspNetCore.Components.Forms.EditContext? editContext)
        {
            NotifyValidation(editContext);
        }

        public Microsoft.AspNetCore.Components.Forms.ValidationMessageStore? GetValidationMessageStore()
        {
            return ValidationMessageStore;
        }
    }
}
