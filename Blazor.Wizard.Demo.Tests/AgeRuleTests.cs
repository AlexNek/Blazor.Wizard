using Blazor.Wizard.Demo.Models;
using FluentAssertions;
using Xunit;

namespace Blazor.Wizard.Demo.Tests
{
    public class AgeRuleTests
    {
        [Theory]
        [InlineData(15, false, "Under 16: Should not allow adult pages")]
        [InlineData(16, true, "16: Should allow adult pages")]
        [InlineData(67, true, "Over 66: Should allow pension pages")]
        public void AgeVerification_ShouldReturnExpectedResult(int age, bool expected, string reason)
        {
            // Arrange
            var model = new PersonInfoModel { Age = age };

            // Act
            var isAdult = IsAdult(model);
            var isPensioner = IsPensioner(model);

            // Assert
            if (age < 16)
            {
                isAdult.Should().BeFalse(reason);
                isPensioner.Should().BeFalse("Under 16: Not pensioner");
            }
            else if (age > 66)
            {
                isAdult.Should().BeTrue(reason);
                isPensioner.Should().BeTrue("Over 66: Pensioner");
            }
            else
            {
                isAdult.Should().BeTrue(reason);
                isPensioner.Should().BeFalse("16-66: Not pensioner");
            }
        }

        [Theory]
        [InlineData(15, "ChildView")]
        [InlineData(16, "AdultView")]
        [InlineData(67, "PensionView")]
        public void DynamicPageView_ShouldReturnCorrectView(int age, string expectedView)
        {
            // Arrange
            var model = new PersonInfoModel { Age = age };

            // Act
            var view = GetPageView(model);

            // Assert
            view.Should().Be(expectedView);
        }

        private static bool IsAdult(PersonInfoModel model)
        {
            return model.Age >= 16;
        }

        private static bool IsPensioner(PersonInfoModel model)
        {
            return model.Age > 66;
        }

        private static string GetPageView(PersonInfoModel model)
        {
            if (model.Age < 16)
                return "ChildView";
            if (model.Age > 66)
                return "PensionView";
            return "AdultView";
        }
    }
}
