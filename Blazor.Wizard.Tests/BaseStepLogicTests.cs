using FluentAssertions;
using System.ComponentModel.DataAnnotations;

using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;

using ValidationResult = Blazor.Wizard.Core.ValidationResult;

namespace Blazor.Wizard.Tests;

public class BaseStepLogicTests
{
    [Fact]
    public void Constructor_ShouldInitializeModelAndEditContext()
    {
        var step = new TestStepLogic();
        var model = step.GetModel();
        var context = step.GetEditContext();

        model.Should().NotBeNull();
        context.Should().NotBeNull();
        context.Model.Should().BeSameAs(model);
    }

    [Fact]
    public void Constructor_WithCustomFactory_ShouldUseFactoryToCreateModel()
    {
        // Arrange
        var customModel = new TestModel { Value = 42 };
        
        // Act
        var step = new TestStepLogic(() => customModel);
        var model = step.GetModel();
        
        // Assert
        model.Should().BeSameAs(customModel);
        model.Value.Should().Be(42);
    }

    [Fact]
    public void Constructor_WithTypeWithoutParameterlessConstructor_ShouldThrowException()
    {
        // Act
        Action act = () => new InvalidStepLogic();
        
        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*must have a parameterless constructor or provide a factory*");
    }

    [Fact]
    public async Task EnterAsync_WithExistingDataInWizardData_ShouldReplaceModel()
    {
        // Arrange
        var step = new TestStepLogic();
        var originalModel = step.GetModel();
        var wizardData = new WizardData();
        var existingModel = new TestModel { Value = 99 };
        wizardData.Set(existingModel);

        // Act
        await step.EnterAsync(wizardData);
        var currentModel = step.GetModel();

        // Assert
        currentModel.Should().BeSameAs(existingModel);
        currentModel.Should().NotBeSameAs(originalModel);
        currentModel.Value.Should().Be(99);
    }

    [Fact]
    public async Task EnterAsync_WithoutExistingData_ShouldKeepOriginalModel()
    {
        // Arrange
        var step = new TestStepLogic();
        var originalModel = step.GetModel();
        originalModel.Value = 10;
        var wizardData = new WizardData();

        // Act
        await step.EnterAsync(wizardData);
        var currentModel = step.GetModel();

        // Assert
        currentModel.Should().BeSameAs(originalModel);
        currentModel.Value.Should().Be(10);
    }

    [Fact]
    public async Task EnterAsync_ShouldUpdateEditContext()
    {
        // Arrange
        var step = new TestStepLogic();
        var originalContext = step.GetEditContext();
        var wizardData = new WizardData();
        var existingModel = new TestModel { Value = 77 };
        wizardData.Set(existingModel);

        // Act
        await step.EnterAsync(wizardData);
        var newContext = step.GetEditContext();

        // Assert
        newContext.Should().NotBeSameAs(originalContext);
        newContext.Model.Should().BeSameAs(existingModel);
    }

    [Fact]
    public async Task BeforeLeaveAsync_ShouldSaveModelToWizardData()
    {
        // Arrange
        var step = new TestStepLogic();
        var model = step.GetModel();
        model.Value = 123;
        var wizardData = new WizardData();

        // Act
        await step.BeforeLeaveAsync(wizardData);

        // Assert
        wizardData.TryGet<TestModel>(out var savedModel).Should().BeTrue();
        savedModel.Should().BeSameAs(model);
        savedModel!.Value.Should().Be(123);
    }

    [Fact]
    public async Task LeaveAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var step = new TestStepLogic();
        var wizardData = new WizardData();

        // Act
        Func<Task> act = async () => await step.LeaveAsync(wizardData);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateAsync_WithValidModel_ShouldReturnTrue()
    {
        // Arrange
        var step = new ValidatedStepLogic();
        var model = step.GetModel();
        model.RequiredField = "Valid Value";
        model.RangeField = 5;
        var wizardData = new WizardData();

        // Act
        var result = await step.ValidateAsync(wizardData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WithEditContextErrors_ShouldReturnFalse()
    {
        // Arrange
        var step = new ValidatedStepLogic();
        var context = step.GetEditContext();
        var model = step.GetModel();
        
        // Manually add validation errors to EditContext
        var messageStore = new Microsoft.AspNetCore.Components.Forms.ValidationMessageStore(context);
        var fieldIdentifier = context.Field(nameof(model.RequiredField));
        messageStore.Add(fieldIdentifier, "This field is required");
        context.NotifyValidationStateChanged();
        
        var wizardData = new WizardData();

        // Act
        var result = await step.ValidateAsync(wizardData);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsVisible_DefaultValue_ShouldBeTrue()
    {
        // Arrange & Act
        var step = new TestStepLogic();

        // Assert
        step.IsVisible.Should().BeTrue();
    }

    [Fact]
    public void IsVisible_CanBeSetToFalse()
    {
        // Arrange & Act
        var step = new TestStepLogicWithVisibility(false);

        // Assert
        step.IsVisible.Should().BeFalse();
    }

    [Fact]
    public async Task CompleteWorkflow_ShouldMaintainStateConsistency()
    {
        // Arrange
        var step = new TestStepLogic();
        var model = step.GetModel();
        model.Value = 50;
        var wizardData = new WizardData();

        // Act - Simulate complete workflow
        await step.BeforeLeaveAsync(wizardData); // Save to wizard data
        await step.LeaveAsync(wizardData);

        var newStep = new TestStepLogic();
        await newStep.EnterAsync(wizardData); // Load from wizard data
        var loadedModel = newStep.GetModel();

        // Assert
        loadedModel.Value.Should().Be(50);
        loadedModel.Should().BeSameAs(model); // WizardData stores references, so it's the same instance
    }

    [Fact]
    public void Evaluate_ShouldReturnExpectedResult()
    {
        // Arrange
        var step = new TestStepLogic();
        var wizardData = new WizardData();
        var validation = new ValidationResult();

        // Act
        var result = step.Evaluate(wizardData, validation);

        // Assert
        result.Should().NotBeNull();
        result.CanContinue.Should().BeTrue();
    }

    [Fact]
    public void Id_ShouldReturnCorrectType()
    {
        // Arrange & Act
        var step = new TestStepLogic();

        // Assert
        step.Id.Should().Be(typeof(TestStepLogic));
    }

    [Fact]
    public async Task MultipleEnterAsyncCalls_ShouldHandleCorrectly()
    {
        // Arrange
        var step = new TestStepLogic();
        var wizardData = new WizardData();
        
        var model1 = new TestModel { Value = 10 };
        wizardData.Set(model1);
        await step.EnterAsync(wizardData);
        
        var model2 = new TestModel { Value = 20 };
        wizardData.Set(model2);

        // Act
        await step.EnterAsync(wizardData);
        var currentModel = step.GetModel();

        // Assert
        currentModel.Should().BeSameAs(model2);
        currentModel.Value.Should().Be(20);
    }

    [Fact]
    public async Task EditContext_ShouldReflectCurrentModel()
    {
        // Arrange
        var step = new TestStepLogic();
        var wizardData = new WizardData();
        var newModel = new TestModel { Value = 555 };
        wizardData.Set(newModel);

        // Act
        await step.EnterAsync(wizardData);
        var context = step.GetEditContext();

        // Assert
        context.Model.Should().BeSameAs(newModel);
        ((TestModel)context.Model).Value.Should().Be(555);
    }

    // Helper classes for testing
    private class TestModel
    {
        public int Value { get; set; }
    }

    private class ValidatedModel
    {
        [Required]
        public string? RequiredField { get; set; }

        [Range(1, 100)]
        public int RangeField { get; set; }
    }

    private class ModelWithoutParameterlessConstructor
    {
        public ModelWithoutParameterlessConstructor(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }

    private class TestStepLogic : BaseStepLogic<TestModel>
    {
        public TestStepLogic(Func<TestModel>? modelFactory = null) : base(modelFactory)
        {
        }

        public override Type Id => typeof(TestStepLogic);

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }
    }

    private class ValidatedStepLogic : BaseStepLogic<ValidatedModel>
    {
        public override Type Id => typeof(ValidatedStepLogic);

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }
    }

    private class InvalidStepLogic : BaseStepLogic<ModelWithoutParameterlessConstructor>
    {
        public override Type Id => typeof(InvalidStepLogic);

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }
    }

    private class TestStepLogicWithVisibility : BaseStepLogic<TestModel>
    {
        public TestStepLogicWithVisibility(bool isVisible)
        {
            IsVisible = isVisible;
        }

        public override Type Id => typeof(TestStepLogicWithVisibility);

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }
    }
}