using System.ComponentModel.DataAnnotations;
using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.WizardLogic;
using Blazor.Wizard.Demo.Components.WizardLogic.Person;
using Blazor.Wizard.Demo.Models;
using Blazor.Wizard.Demo.Models.Person;
using Blazor.Wizard.Demo.Services.Animation;
using Blazor.Wizard.Demo.Services.Toaster;
using Blazor.Wizard.Extensions;
using Blazor.Wizard.Interfaces;

using FluentAssertions;
using Moq;

using ValidationResult = Blazor.Wizard.Core.ValidationResult;

namespace Blazor.Wizard.Demo.Tests;

public class ValidationTests
{
    [Fact]
    public void AttributeValidation_ShouldFailForMissingRequiredFields()
    {
        // Arrange
        var model = new PersonInfoModel(); // Assume FirstName, LastName required
        var context = new ValidationContext(model);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

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
        var model = new PersonInfoModel
            { FirstName = "John", LastName = "Doe", Age = 30, Email = "john.doe@example.com" };
        var context = new ValidationContext(model);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task CustomLogicValidation_ShouldFailForInvalidAge()
    {
        // Arrange
        var model = new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 15 };
        var animation = new Mock<IWizardAnimationService>();
        var logic = new PersonInfoStepLogic(animation.Object);
        var validation = new ValidationResult();
        var data = new WizardData();
        data.Set(model);
        data.AddService(Mock.Of<IToasterService>());

        // Ensure EditContext is set up for the test model
        await logic.EnterAsync(data);

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
        var animation = new Mock<IWizardAnimationService>();
        var logic = new PersonInfoStepLogic(animation.Object);
        var validation = new ValidationResult { IsValid = true };
        var data = new WizardData();
        data.Set(model);
        data.AddService(Mock.Of<IToasterService>());

        // Ensure EditContext is set up for the test model
        await logic.EnterAsync(data);

        // Act
        logic.Evaluate(data, validation);

        // Assert
        validation.IsValid.Should().BeTrue();
    }
}
