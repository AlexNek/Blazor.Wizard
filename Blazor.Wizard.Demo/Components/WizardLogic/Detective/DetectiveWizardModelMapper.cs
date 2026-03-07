using Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;
using Blazor.Wizard.Demo.Models.Detective;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public class DetectiveWizardModelMapper : IWizardModelBuilder<DetectiveCaseVerdict>
{
    public DetectiveCaseVerdict Build(IWizardData data)
    {
        if (!data.TryGet<InvestigationPlanStepModel>(out var plan) || plan == null)
        {
            throw new InvalidOperationException("Missing InvestigationPlanStepModel data.");
        }

        if (!data.TryGet<DetectiveAccusationStepModel>(out var accusation) || accusation == null)
        {
            throw new InvalidOperationException("Missing DetectiveAccusationStepModel data.");
        }

        var strategy = InvestigationStrategyFactory.Create(plan.Strategy);

        WitnessInterviewStepModel? witness = null;
        if (strategy.IsWitnessStepVisible())
        {
            if (!data.TryGet<WitnessInterviewStepModel>(out witness) || witness == null)
            {
                throw new InvalidOperationException("Missing WitnessInterviewStepModel data for witness strategy.");
            }
        }

        ForensicsEvidenceStepModel? forensics = null;
        if (strategy.IsForensicsStepVisible())
        {
            if (!data.TryGet<ForensicsEvidenceStepModel>(out forensics) || forensics == null)
            {
                throw new InvalidOperationException("Missing ForensicsEvidenceStepModel data for forensics strategy.");
            }
        }

        var askedKeyWitnessQuestion = witness != null &&
                                      (DetectiveCaseFacts.IsKeyWitnessQuestion(witness.NoraQuestion) ||
                                       DetectiveCaseFacts.IsKeyWitnessQuestion(witness.MarcosQuestion));
        var askedKeyLabQuestion = forensics != null &&
                                  DetectiveCaseFacts.IsKeyLabQuestion(forensics.TeaQuestion) &&
                                  DetectiveCaseFacts.IsKeyLabQuestion(forensics.WindowQuestion);
        var suspectCorrect = DetectiveCaseFacts.IsCorrectSuspect(accusation.Suspect);
        var methodCorrect = DetectiveCaseFacts.IsCorrectMethod(accusation.MurderMethod);
        var motiveCorrect = DetectiveCaseFacts.IsCorrectMotive(accusation.Motive);

        var score = 0;
        if (askedKeyWitnessQuestion)
        {
            score += 1;
        }

        if (askedKeyLabQuestion)
        {
            score += 2;
        }

        if (suspectCorrect)
        {
            score += 2;
        }

        if (methodCorrect)
        {
            score += 2;
        }

        if (motiveCorrect)
        {
            score += 2;
        }

        var isCorrect = suspectCorrect && methodCorrect && motiveCorrect && score >= 5;
        var verdictMessage = isCorrect
            ? "Correct. Ivy poisoned Elias for inheritance money."
            : "Wrong. Ask better questions and try again.";

        var witnessSummary = witness == null
            ? "Not asked"
            : $"Ivy: {witness.IvyQuestion} | Nora: {witness.NoraQuestion} | Marcos: {witness.MarcosQuestion}";

        var labSummary = forensics == null
            ? "Not asked"
            : $"Tea: {forensics.TeaQuestion} | Window: {forensics.WindowQuestion}";

        return new DetectiveCaseVerdict
        {
            IsCorrect = isCorrect,
            Strategy = plan.Strategy,
            WitnessSummary = witnessSummary,
            LabSummary = labSummary,
            Suspect = accusation.Suspect,
            MurderMethod = accusation.MurderMethod,
            Motive = accusation.Motive,
            ConfidenceScore = score,
            VerdictMessage = verdictMessage
        };
    }
}
