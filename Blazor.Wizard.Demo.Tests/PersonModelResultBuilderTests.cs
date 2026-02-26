using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.WizardLogic;
using Blazor.Wizard.Demo.Components.WizardLogic.Person;
using Blazor.Wizard.Demo.Models;
using FluentAssertions;
using Xunit;

namespace Blazor.Wizard.Demo.Tests
{
    public class PersonModelResultBuilderTests
    {
        [Fact]
        public void Build_ShouldThrowIfPersonInfoMissing()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();

            // Act
            var act = () => builder.Build(data);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*Missing PersonInfoModel data*");
        }

        [Fact]
        public void Build_ShouldThrowIfAddressMissing()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();
            data.Set(new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 30 });

            // Act
            var act = () => builder.Build(data);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*Missing AddressModel data*");
        }

        [Fact]
        public void Build_ShouldReturnPersonModelEvenIfPersonInfoIncomplete()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();
            data.Set(new PersonInfoModel());
            data.Set(new AddressModel());

            // Act
            var result = builder.Build(data);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().BeEmpty();
            result.LastName.Should().BeEmpty();
        }

        [Fact]
        public void Build_ShouldReturnPersonModelEvenIfAddressIncomplete()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();
            data.Set(new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 30 });
            data.Set(new AddressModel());

            // Act
            var result = builder.Build(data);

            // Assert
            result.Should().NotBeNull();
            result.Street.Should().BeEmpty();
            result.City.Should().BeEmpty();
        }

        [Fact]
        public void Build_ShouldReturnPersonModelForValidData()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();
            data.Set(new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 30 });
            data.Set(new AddressModel { Street = "Main St", City = "Berlin", ZipCode = "12345" });

            // Act
            var result = builder.Build(data);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PersonModel>();
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.Age.Should().Be(30);
            result.Street.Should().Be("Main St");
        }
    }
}
