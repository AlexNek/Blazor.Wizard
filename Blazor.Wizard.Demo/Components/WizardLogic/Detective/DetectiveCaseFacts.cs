namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public static class DetectiveCaseFacts
{
    public const string CorrectSuspect = "Ivy Marlowe (concert pianist)";
    public const string CorrectMethod = "Poison in tea";
    public const string CorrectMotive = "Money and inheritance";

    private static readonly Dictionary<string, string> InterviewQuestions = new()
    {
        ["Ivy: Where were you at 21:12?"] = "I was in the garden at 21:12.",
        ["Ivy: Why is your sleeve wet?"] = "Rain made my sleeve wet.",
        ["Ivy: Did you fight with Elias?"] = "Yes. We argued about the will.",
        ["Ivy: Were you in debt?"] = "My concert career failed. I owe creditors £50,000.",
        ["Nora: Did you change the will?"] = "No. The will was unchanged.",
        ["Nora: Who knows poison plants?"] = "Ivy studies plants and herbal extracts.",
        ["Nora: Where were you at 21:10?"] = "I was in the library with the clerk.",
        ["Nora: Who inherits if Elias dies?"] = "Ivy inherits the entire estate worth £200,000.",
        ["Marcos: Who touched the tea tray?"] = "Ivy asked for the tea tray at 21:05, before death at 21:15.",
        ["Marcos: Was the window forced open?"] = "No. The latch marks are from inside.",
        ["Marcos: Did you see a stranger?"] = "No stranger entered the estate."
    };

    private static readonly Dictionary<string, string> LabQuestions = new()
    {
        ["Lab: What was inside the tea cup?"] = "Tea had foxglove poison.",
        ["Lab: Was poison found in blood?"] = "Yes. The same poison from the tea was in blood.",
        ["Lab: What was on the window latch?"] = "Black piano glove fiber was on the latch.",
        ["Lab: Was there mud from outside?"] = "No outside mud in the room."
    };

    public static IEnumerable<string> GetIvyQuestions() => InterviewQuestions.Keys.Where(q => q.StartsWith("Ivy:"));
    public static IEnumerable<string> GetNoraQuestions() => InterviewQuestions.Keys.Where(q => q.StartsWith("Nora:"));
    public static IEnumerable<string> GetMarcosQuestions() => InterviewQuestions.Keys.Where(q => q.StartsWith("Marcos:"));
    public static IEnumerable<string> GetLabQuestions() => LabQuestions.Keys;

    public static string GetInterviewAnswer(string question) => InterviewQuestions.GetValueOrDefault(question, "No answer.");
    public static string GetLabAnswer(string question) => LabQuestions.GetValueOrDefault(question, "No result.");
}
