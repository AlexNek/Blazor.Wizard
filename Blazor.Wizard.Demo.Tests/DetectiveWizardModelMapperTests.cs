using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.WizardLogic.Detective;
using Blazor.Wizard.Demo.Models.Detective;
using FluentAssertions;
using Xunit;

namespace Blazor.Wizard.Demo.Tests;

public class DetectiveWizardModelMapperTests
{
    [Fact]
    public void Build_ShouldReturnCorrectVerdict_ForMatchingCluesAndAccusation()
    {
        var mapper = new DetectiveWizardModelMapper();
        var data = new WizardData();
        data.Set(new InvestigationPlanStepModel { Strategy = InvestigationStrategy.FullSweep });
        data.Set(new WitnessInterviewStepModel { StatementChoice = DetectiveCaseFacts.CorrectWitnessStatement });
        data.Set(new ForensicsEvidenceStepModel { EvidenceChoice = DetectiveCaseFacts.CorrectForensicEvidence });
        data.Set(new DetectiveAccusationStepModel
        {
            Suspect = DetectiveCaseFacts.CorrectSuspect,
            MurderMethod = DetectiveCaseFacts.CorrectMethod
        });

        var result = mapper.Build(data);

        result.IsCorrect.Should().BeTrue();
        result.ConfidenceScore.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Build_ShouldReturnWrongVerdict_ForWrongAccusation()
    {
        var mapper = new DetectiveWizardModelMapper();
        var data = new WizardData();
        data.Set(new InvestigationPlanStepModel { Strategy = InvestigationStrategy.WitnessOnly });
        data.Set(new WitnessInterviewStepModel { StatementChoice = DetectiveCaseFacts.CorrectWitnessStatement });
        data.Set(new DetectiveAccusationStepModel
        {
            Suspect = "Nora Flint (estate lawyer)",
            MurderMethod = "Staged electrocution using a floor lamp"
        });

        var result = mapper.Build(data);

        result.IsCorrect.Should().BeFalse();
    }

    [Fact]
    public void Build_ShouldThrow_WhenRequiredBranchDataIsMissing()
    {
        var mapper = new DetectiveWizardModelMapper();
        var data = new WizardData();
        data.Set(new InvestigationPlanStepModel { Strategy = InvestigationStrategy.ForensicsOnly });
        data.Set(new DetectiveAccusationStepModel
        {
            Suspect = DetectiveCaseFacts.CorrectSuspect,
            MurderMethod = DetectiveCaseFacts.CorrectMethod
        });

        var act = () => mapper.Build(data);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Missing ForensicsEvidenceStepModel data*");
    }
}
