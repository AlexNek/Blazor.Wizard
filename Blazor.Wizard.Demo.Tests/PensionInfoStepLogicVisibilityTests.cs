using Blazor.Wizard.Demo.Components.WizardLogic;
using Blazor.Wizard.Demo.Components.WizardLogic.Person;
using Blazor.Wizard.Demo.Models;
using FluentAssertions;
using Xunit;

namespace Blazor.Wizard.Demo.Tests
{
    public class PensionInfoStepLogicVisibilityTests
    {
        [Theory]
        [InlineData(null, false, "Null person info should not show pension step")]
        [InlineData(15, false, "Age < MaxPensionAge should not show pension step")]
        [InlineData(66, false, "Age < MaxPensionAge should not show pension step")]
        [InlineData(67, true, "Age >= MaxPensionAge should show pension step")]
        [InlineData(70, true, "Age >= MaxPensionAge should show pension step")]
        public void IsVisible_ShouldReflectPensionVisibility(int? age, bool expectedVisible, string reason)
        {
            // Arrange
            var logic = new PensionInfoStepLogic();
            PersonInfoModel? personInfo = age.HasValue ? new PersonInfoModel { Age = age.Value } : null;
            logic.UpdatePersonInfo(personInfo);

            // Act
            var isVisible = logic.IsVisible;

            // Assert
            isVisible.Should().Be(expectedVisible, reason);
        }
    }
}
