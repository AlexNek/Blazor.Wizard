using Blazor.Wizard;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Blazor.Wizard.Demo.Tests
{
    public class WizardFlowSequenceTests
    {
        private class Step : IWizardStep
        {
            public bool IsVisible { get; set; } = true;
            public Type Id => typeof(Step);
            public ValueTask<bool> ValidateAsync(IWizardData data) => ValueTask.FromResult(true);
            public ValueTask BeforeLeaveAsync(IWizardData data) => ValueTask.CompletedTask;
            public ValueTask EnterAsync(IWizardData data) => ValueTask.CompletedTask;
            public ValueTask LeaveAsync(IWizardData data) => ValueTask.CompletedTask;
            public StepResult Evaluate(IWizardData data, ValidationResult validation) => new StepResult { CanContinue = true };
        }

        [Fact]
        public async Task Sequence_NextPrev_ShouldSkipInvisibleSteps()
        {
            // Arrange
            var data = Mock.Of<IWizardData>();
            var flow = new WizardFlow<string>(data);
            var step1 = new Step { IsVisible = true };
            var step2 = new Step { IsVisible = false };
            var step3 = new Step { IsVisible = true };
            flow.Add(step1);
            flow.Add(step2);
            flow.Add(step3);
            await flow.StartAsync();

            // Act: Next should skip step2
            await flow.NextAsync();

            // Assert
            flow.Index.Should().Be(2); // step3

            // Act: Prev should skip step2
            await flow.PrevAsync();

            // Assert
            flow.Index.Should().Be(0); // step1
        }

        [Fact]
        public async Task Sequence_NextPrev_ShouldNavigateVisibleSteps()
        {
            // Arrange
            var data = Mock.Of<IWizardData>();
            var flow = new WizardFlow<string>(data);
            var step1 = new Step { IsVisible = true };
            var step2 = new Step { IsVisible = true };
            var step3 = new Step { IsVisible = true };
            flow.Add(step1);
            flow.Add(step2);
            flow.Add(step3);
            await flow.StartAsync();

            // Act & Assert
            flow.Index.Should().Be(0);
            await flow.NextAsync();
            flow.Index.Should().Be(1);
            await flow.NextAsync();
            flow.Index.Should().Be(2);
            await flow.PrevAsync();
            flow.Index.Should().Be(1);
            await flow.PrevAsync();
            flow.Index.Should().Be(0);
        }
    }
}
