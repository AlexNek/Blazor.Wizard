using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;

using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class WizardDataTests
{
    [Fact]
    public void Set_ShouldStoreValue()
    {
        // Arrange
        var wizardData = new WizardData();
        var model = new TestModel { Value = 42 };

        // Act
        wizardData.Set(model);
        var retrieved = wizardData.TryGet<TestModel>(out var value);

        // Assert
        retrieved.Should().BeTrue();
        value.Should().BeSameAs(model);
        value!.Value.Should().Be(42);
    }

    [Fact]
    public void TryGet_WithNonExistentType_ShouldReturnFalse()
    {
        // Arrange
        var wizardData = new WizardData();

        // Act
        var retrieved = wizardData.TryGet<TestModel>(out var value);

        // Assert
        retrieved.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGet_WithDefaultValue_ShouldReturnDefault()
    {
        // Arrange
        var wizardData = new WizardData();

        // Act
        var retrieved = wizardData.TryGet<int>(out var value);

        // Assert
        retrieved.Should().BeFalse();
        value.Should().Be(0); // default(int)
    }

    [Fact]
    public void Set_ShouldOverwriteExistingValue()
    {
        // Arrange
        var wizardData = new WizardData();
        var model1 = new TestModel { Value = 10 };
        var model2 = new TestModel { Value = 20 };

        // Act
        wizardData.Set(model1);
        wizardData.Set(model2);
        wizardData.TryGet<TestModel>(out var retrieved);

        // Assert
        retrieved.Should().BeSameAs(model2);
        retrieved!.Value.Should().Be(20);
    }

    [Fact]
    public void Set_WithMultipleTypes_ShouldStoreIndependently()
    {
        // Arrange
        var wizardData = new WizardData();
        var testModel = new TestModel { Value = 42 };
        var anotherModel = new AnotherModel { Name = "Test" };

        // Act
        wizardData.Set(testModel);
        wizardData.Set(anotherModel);
        var retrieved1 = wizardData.TryGet<TestModel>(out var value1);
        var retrieved2 = wizardData.TryGet<AnotherModel>(out var value2);

        // Assert
        retrieved1.Should().BeTrue();
        value1.Should().BeSameAs(testModel);
        value1!.Value.Should().Be(42);

        retrieved2.Should().BeTrue();
        value2.Should().BeSameAs(anotherModel);
        value2!.Name.Should().Be("Test");
    }

    [Fact]
    public void Set_WithValueType_ShouldStoreCorrectly()
    {
        // Arrange
        var wizardData = new WizardData();
        var intValue = 123;
        var doubleValue = 45.67;

        // Act
        wizardData.Set(intValue);
        wizardData.Set(doubleValue);
        
        var retrievedInt = wizardData.TryGet<int>(out var valueInt);
        var retrievedDouble = wizardData.TryGet<double>(out var valueDouble);

        // Assert
        retrievedInt.Should().BeTrue();
        valueInt.Should().Be(123);
        
        retrievedDouble.Should().BeTrue();
        valueDouble.Should().Be(45.67);
    }

    [Fact]
    public void Set_WithString_ShouldStoreCorrectly()
    {
        // Arrange
        var wizardData = new WizardData();
        var stringValue = "Hello, World!";

        // Act
        wizardData.Set(stringValue);
        var retrieved = wizardData.TryGet<string>(out var value);

        // Assert
        retrieved.Should().BeTrue();
        value.Should().Be(stringValue);
    }

    [Fact]
    public void ImplementsIWizardData_Interface()
    {
        // Arrange & Act
        var wizardData = new WizardData();

        // Assert
        wizardData.Should().BeAssignableTo<IWizardData>();
    }

    [Fact]
    public void MultipleOperations_ShouldMaintainCorrectState()
    {
        // Arrange
        var wizardData = new WizardData();
        var model1 = new TestModel { Value = 1 };
        var model2 = new TestModel { Value = 2 };
        var another = new AnotherModel { Name = "Test" };

        // Act - Complex sequence of operations
        wizardData.Set(model1);
        wizardData.TryGet<TestModel>(out var step1);
        
        wizardData.Set(another);
        wizardData.TryGet<AnotherModel>(out var step2);
        
        wizardData.Set(model2); // Overwrite TestModel
        wizardData.TryGet<TestModel>(out var step3);
        wizardData.TryGet<AnotherModel>(out var step4);

        // Assert
        step1!.Value.Should().Be(1);
        step2!.Name.Should().Be("Test");
        step3!.Value.Should().Be(2); // Updated value
        step4!.Name.Should().Be("Test"); // Unchanged
    }

    [Fact]
    public void Set_WithComplexObject_ShouldPreserveReferences()
    {
        // Arrange
        var wizardData = new WizardData();
        var innerModel = new TestModel { Value = 100 };
        var complexModel = new ComplexModel 
        { 
            Id = 1,
            Inner = innerModel
        };

        // Act
        wizardData.Set(complexModel);
        wizardData.TryGet<ComplexModel>(out var retrieved);

        // Assert
        retrieved.Should().BeSameAs(complexModel);
        retrieved!.Inner.Should().BeSameAs(innerModel);
        retrieved.Inner.Value.Should().Be(100);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Set_WithDifferentIntValues_ShouldStoreCorrectly(int value)
    {
        // Arrange
        var wizardData = new WizardData();

        // Act
        wizardData.Set(value);
        wizardData.TryGet<int>(out var retrieved);

        // Assert
        retrieved.Should().Be(value);
    }

    [Fact]
    public void StoreMultipleStepModels_TypicalWizardScenario()
    {
        // Arrange
        var wizardData = new WizardData();
        var step1Model = new TestModel { Value = 1 };
        var step2Model = new AnotherModel { Name = "Step2" };
        var step3Model = new ComplexModel { Id = 3 };

        // Act - Simulate wizard storing data from each step
        wizardData.Set(step1Model);
        wizardData.Set(step2Model);
        wizardData.Set(step3Model);

        // Retrieve all
        var hasStep1 = wizardData.TryGet<TestModel>(out var retrievedStep1);
        var hasStep2 = wizardData.TryGet<AnotherModel>(out var retrievedStep2);
        var hasStep3 = wizardData.TryGet<ComplexModel>(out var retrievedStep3);

        // Assert
        hasStep1.Should().BeTrue();
        retrievedStep1!.Value.Should().Be(1);
        
        hasStep2.Should().BeTrue();
        retrievedStep2!.Name.Should().Be("Step2");
        
        hasStep3.Should().BeTrue();
        retrievedStep3!.Id.Should().Be(3);
    }

    // Helper classes for testing
    private class TestModel
    {
        public int Value { get; set; }
    }

    private class AnotherModel
    {
        public string Name { get; set; } = string.Empty;
    }

    private class ComplexModel
    {
        public int Id { get; set; }
        public TestModel? Inner { get; set; }
    }
}
