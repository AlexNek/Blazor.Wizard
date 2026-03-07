using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations; // Added for validation
using System.Collections.Generic;
using Blazor.Wizard.Demo.Components.WizardLogic;
using Blazor.Wizard.Demo.Components.WizardLogic.Person;
using Blazor.Wizard.Demo.Models.Person;

namespace Blazor.Wizard.Demo.Tests
{
    public class AddressStepLogicTests
    {
        [Fact]
        public void AddressStepLogic_ShouldInitializeModel()
        {
            // Arrange
            var logic = new AddressStepLogic();

            // Act
            var model = logic.GetModel();

            // Assert
            model.Should().NotBeNull();
            model.Should().BeOfType<AddressModel>();
        }

        [Fact]
        public void AddressStepLogic_ShouldValidateAddress()
        {
            // Arrange
            var model = new AddressModel { Street = "Main St", City = "Berlin", ZipCode = "12345", Country = "Germany" };
            var context = new ValidationContext(model);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeTrue();
        }
    }
}
