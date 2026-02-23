using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary
{
    public class QuestionaryWizardViewModel : WizardViewModel<IWizardStep, WizardData, QuestionaryModel>
    {
        public QuestionaryWizardViewModel() : base(new QuestionaryResultBuilder())
        {
        }
        public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
        {
            var factories = stepFactories ?? new List<Func<IWizardStep>>
            {
                () => new QuestionaryStep1Logic(),
                () => new QuestionaryStep2Logic(),
                () => new QuestionaryStep3Logic(),
                () => new QuestionaryReportStepLogic()
            };
            base.Initialize(factories);
        }
    }

    public class QuestionaryResultBuilder : IWizardResultBuilder<QuestionaryModel>
    {
        public QuestionaryModel Build(IWizardData data)
        {
            data.TryGet<QuestionaryModel>(out var model);
            return model ?? new QuestionaryModel();
        }
    }
}