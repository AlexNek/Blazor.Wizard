namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public static class DetectiveCaseFacts
{
    public const string CorrectSuspect = "Ivy Marlowe (concert pianist)";

    public const string CorrectMethod = "Poison in tea";

    public const string CorrectMotive = "Money and inheritance";

    public const string IvyQuestionTime = "Ivy: Where were you at 21:12?";
    public const string IvyQuestionSleeve = "Ivy: Why is your sleeve wet?";
    public const string IvyQuestionFight = "Ivy: Did you fight with Elias?";

    public const string NoraQuestionWill = "Nora: Did you change the will?";
    public const string NoraQuestionPoison = "Nora: Who knows poison plants?";
    public const string NoraQuestionAlibi = "Nora: Where were you at 21:10?";

    public const string MarcosQuestionTea = "Marcos: Who touched the tea tray?";
    public const string MarcosQuestionWindow = "Marcos: Was the window forced open?";
    public const string MarcosQuestionStranger = "Marcos: Did you see a stranger?";

    public const string LabQuestionTea = "Lab: What was inside the tea cup?";
    public const string LabQuestionBlood = "Lab: Was poison found in blood?";
    public const string LabQuestionWindow = "Lab: What was on the window latch?";
    public const string LabQuestionMud = "Lab: Was there mud from outside?";

    public static string GetInterviewAnswer(string question)
    {
        return question switch
        {
            IvyQuestionTime => "I was in the garden at 21:12.",
            IvyQuestionSleeve => "Rain made my sleeve wet.",
            IvyQuestionFight => "Yes. We argued about the will.",
            NoraQuestionWill => "No. The will was unchanged.",
            NoraQuestionPoison => "Ivy studies plants and herbal extracts.",
            NoraQuestionAlibi => "I was in the library with the clerk.",
            MarcosQuestionTea => "Ivy asked for the tea tray at 21:05, before death at 21:15.",
            MarcosQuestionWindow => "No. The latch marks are from inside.",
            MarcosQuestionStranger => "No stranger entered the estate.",
            _ => "No answer."
        };
    }

    public static string GetLabAnswer(string question)
    {
        return question switch
        {
            LabQuestionTea => "Tea had foxglove poison.",
            LabQuestionBlood => "Yes. The same poison from the tea was in blood.",
            LabQuestionWindow => "Black piano glove fiber was on the latch.",
            LabQuestionMud => "No outside mud in the room.",
            _ => "No result."
        };
    }
}
