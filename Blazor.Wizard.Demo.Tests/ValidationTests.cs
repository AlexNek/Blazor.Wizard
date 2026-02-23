using Blazor.Wizard.Demo.Components.Wizard;
using Blazor.Wizard.Demo.Models;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Tests
{
    public class ValidationTests
    {
        // Minimal mock for IWizardData
        private class MockWizardData : IWizardData
        {
            private readonly object _model;
            public MockWizardData(object model) => _model = model;
            public bool TryGet<T>(out T? value)
            {
                if (_model is T t)
                {
                    value = t;
                    return true;
                }
                value = default;
                return false;
            }
            public void Set<T>(T value)
            {
                // No-op for test
            }
        }

        [Fact]
        public void AttributeValidation_ShouldFailForMissingRequiredFields()
        {
            // Arrange
            var model = new PersonInfoModel(); // Assume FirstName, LastName required
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains("FirstName"));
            results.Should().Contain(r => r.MemberNames.Contains("LastName"));
        }

        [Fact]
        public void AttributeValidation_ShouldPassForValidFields()
        {
            // Arrange
            var model = new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 30, Email = "john.doe@example.com" };
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void CustomLogicValidation_ShouldFailForInvalidAge()
        {
            // Arrange
            var model = new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 15 };
            var logic = new PersonInfoStepLogic("test");
            var validation = new ValidationResult();
            var data = new MockWizardData(model);

            // Ensure EditContext is set up for the test model
            logic.EnterAsync(data).GetAwaiter().GetResult();

            // Act
            logic.Evaluate(data, validation);

            // Assert
            validation.IsValid.Should().BeFalse();
            validation.ErrorMessage.Should().Contain("Age must be at least 16");
        }

        [Fact]
        public async Task CustomLogicValidation_ShouldPassForValidAge()
        {
            // Arrange
            var model = new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 30 };
            var logic = new PersonInfoStepLogic("test");
            var validation = new ValidationResult { IsValid = true };
            var data = new MockWizardData(model);

            // Ensure EditContext is set up for the test model
            await logic.EnterAsync(data);

            // Act
            logic.Evaluate(data, validation);

            // Assert
            validation.IsValid.Should().BeTrue();
        }
    }
}
