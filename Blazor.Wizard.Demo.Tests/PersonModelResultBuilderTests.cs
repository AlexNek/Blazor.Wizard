using Blazor.Wizard.Demo.Components.WizardLogic;
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
        public void Build_ShouldThrowIfPersonInfoIncomplete()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();
            data.Set(new PersonInfoModel());
            data.Set(new AddressModel());

            // Act
            var act = () => builder.Build(data);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*PersonInfoModel is incomplete*");
        }

        [Fact]
        public void Build_ShouldThrowIfAddressIncomplete()
        {
            // Arrange
            var builder = new PersonModelResultBuilder();
            var data = new WizardData();
            data.Set(new PersonInfoModel { FirstName = "John", LastName = "Doe", Age = 30 });
            data.Set(new AddressModel());

            // Act
            var act = () => builder.Build(data);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*AddressModel is incomplete*");
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
