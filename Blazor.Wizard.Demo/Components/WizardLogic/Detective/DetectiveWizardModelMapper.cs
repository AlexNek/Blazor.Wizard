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

        WitnessInterviewStepModel? witness = null;
        if (InvestigationStrategy.IncludesWitness(plan.Strategy))
        {
            if (!data.TryGet<WitnessInterviewStepModel>(out witness) || witness == null)
            {
                throw new InvalidOperationException("Missing WitnessInterviewStepModel data for witness strategy.");
            }
        }

        ForensicsEvidenceStepModel? forensics = null;
        if (InvestigationStrategy.IncludesForensics(plan.Strategy))
        {
            if (!data.TryGet<ForensicsEvidenceStepModel>(out forensics) || forensics == null)
            {
                throw new InvalidOperationException("Missing ForensicsEvidenceStepModel data for forensics strategy.");
            }
        }

        var witnessCorrect = witness?.StatementChoice == DetectiveCaseFacts.CorrectWitnessStatement;
        var forensicsCorrect = forensics?.EvidenceChoice == DetectiveCaseFacts.CorrectForensicEvidence;
        var suspectCorrect = accusation.Suspect == DetectiveCaseFacts.CorrectSuspect;
        var methodCorrect = accusation.MurderMethod == DetectiveCaseFacts.CorrectMethod;

        var score = 0;
        if (witness != null)
        {
            score += witnessCorrect ? 1 : -1;
        }

        if (forensics != null)
        {
            score += forensicsCorrect ? 1 : -1;
        }

        if (suspectCorrect)
        {
            score += 2;
        }

        if (methodCorrect)
        {
            score += 2;
        }

        var isCorrect = suspectCorrect && methodCorrect && score >= 3;
        var verdictMessage = isCorrect
            ? "Correct: Ivy Marlowe poisoned Elias Thorn. The timeline and forensics match your accusation."
            : "Wrong: Your accusation does not align with the strongest clues in the case file.";

        return new DetectiveCaseVerdict
        {
            IsCorrect = isCorrect,
            Strategy = plan.Strategy,
            StatementChoice = witness?.StatementChoice ?? "Not investigated",
            EvidenceChoice = forensics?.EvidenceChoice ?? "Not investigated",
            Suspect = accusation.Suspect,
            MurderMethod = accusation.MurderMethod,
            ConfidenceScore = score,
            VerdictMessage = verdictMessage
        };
    }
}
