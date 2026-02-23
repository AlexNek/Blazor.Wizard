using Blazor.Wizard;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;

namespace Blazor.Wizard.Demo.Tests
{
    public class WizardFlowTests
    {
        private class DummyStep : IWizardStep
        {
            public DummyStep(string id) { Id = id.GetType(); }
            public Type Id { get; }
            public bool IsVisible => true;
            public ValueTask EnterAsync(IWizardData data) => ValueTask.CompletedTask;
            public StepResult Evaluate(IWizardData data, ValidationResult validation) => new StepResult { CanContinue = true };
            public ValueTask BeforeLeaveAsync(IWizardData data) => ValueTask.CompletedTask;
            public ValueTask LeaveAsync(IWizardData data) => ValueTask.CompletedTask;
            public ValueTask<bool> ValidateAsync(IWizardData data) => ValueTask.FromResult(true);
        }

        [Fact]
        public async Task WizardFlow_ShouldInitializeWithSteps()
        {
            // Arrange
            var steps = new[] { "Step1", "Step2", "Step3" };
            var data = new WizardData();
            var flow = new WizardFlow<string>(data);
            foreach (var step in steps)
            {
                flow.Add(new DummyStep(step));
            }

            await flow.StartAsync();

            // Act & Assert
            flow.Should().NotBeNull();
            flow.Index.Should().Be(0);
            // flow.Current is default(string) because DummyStep.Id is not string, so we can't assert Current here
        }

        [Fact]
        public async Task WizardFlow_ShouldMoveToNextStep()
        {
            // Arrange
            var steps = new[] { "Step1", "Step2" };
            var data = new WizardData();
            var flow = new WizardFlow<string>(data);
            foreach (var step in steps)
            {
                flow.Add(new DummyStep(step));
            }

            await flow.StartAsync();

            // Act
            await flow.NextAsync();

            // Assert
            flow.Index.Should().Be(1);
        }

        [Fact]
        public async Task WizardFlow_ShouldNotMovePastLastStep()
        {
            // Arrange
            var steps = new[] { "Step1", "Step2" };
            var data = new WizardData();
            var flow = new WizardFlow<string>(data);
            foreach (var step in steps)
            {
                flow.Add(new DummyStep(step));
            }

            await flow.StartAsync();

            // Act
            await flow.NextAsync();
            await flow.NextAsync(); // Should stay at last step

            // Assert
            flow.Index.Should().Be(1);
        }
    }
}
