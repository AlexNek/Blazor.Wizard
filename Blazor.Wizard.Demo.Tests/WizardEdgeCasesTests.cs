using Blazor.Wizard;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;

using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;

using Xunit;

namespace Blazor.Wizard.Demo.Tests
{
    public class WizardEdgeCasesTests
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
        public async Task Next_ShouldNotMovePastLastStep()
        {
            // Arrange
            var data = Mock.Of<IWizardData>();
            var flow = new WizardFlow<string>(data);
            var step1 = new Step { IsVisible = true };
            var step2 = new Step { IsVisible = true };
            flow.Add(step1);
            flow.Add(step2);
            await flow.StartAsync();

            // Act
            await flow.NextAsync();
            await flow.NextAsync(); // Should stay at last step

            // Assert
            flow.Index.Should().Be(1);
        }

        [Fact]
        public async Task Prev_ShouldNotMoveBeforeFirstStep()
        {
            // Arrange
            var data = Mock.Of<IWizardData>();
            var flow = new WizardFlow<string>(data);
            var step1 = new Step { IsVisible = true };
            var step2 = new Step { IsVisible = true };
            flow.Add(step1);
            flow.Add(step2);
            await flow.StartAsync();

            // Act
            await flow.PrevAsync(); // Should stay at first step

            // Assert
            flow.Index.Should().Be(0);
        }

        [Fact]
        public async Task EmptyWizard_ShouldHandleGracefully()
        {
            // Arrange
            var data = Mock.Of<IWizardData>();
            var flow = new WizardFlow<string>(data);

            // Act
            await flow.StartAsync();
            await flow.NextAsync();
            await flow.PrevAsync();

            // Assert
            flow.Index.Should().Be(0);
        }
    }
}
