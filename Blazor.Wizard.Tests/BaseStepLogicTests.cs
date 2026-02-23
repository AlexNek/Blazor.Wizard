using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class BaseStepLogicTests
{
    [Fact]
    public void Constructor_ShouldInitializeModelAndEditContext()
    {
        var step = new TestStepLogic();
        var model = step.GetModel();
        var context = step.GetEditContext();

        model.Should().NotBeNull();
        context.Should().NotBeNull();
        context.Model.Should().BeSameAs(model);
    }

    private class TestModel
    {
        public int Value { get; set; }
    }

    private class TestStepLogic : BaseStepLogic<TestModel>
    {
        public override Type Id => typeof(TestStepLogic);

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            return new StepResult { CanContinue = true };
        }
    }
}