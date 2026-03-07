namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public static class DetectiveCaseFacts
{
    private record QuestionData(string Question, string Answer, bool IsKey = false);
    public record OptionData(string Value, bool IsCorrect = false);

    private static readonly List<QuestionData> InterviewData = new()
    {
        new("Ivy: Where were you at 21:12?", "I was in the garden at 21:12."),
        new("Ivy: Why is your sleeve wet?", "Rain made my sleeve wet."),
        new("Ivy: Did you fight with Elias?", "Yes. We argued about the will."),
        new("Ivy: Were you in debt?", "My concert career failed. I owe creditors £50,000."),
        new("Nora: Did you change the will?", "No. The will was unchanged."),
        new("Nora: Who knows poison plants?", "Ivy studies plants and herbal extracts.", IsKey: true),
        new("Nora: Where were you at 21:10?", "I was in the library with the clerk."),
        new("Nora: Who inherits if Elias dies?", "Ivy inherits the entire estate worth £200,000."),
        new("Marcos: Who touched the tea tray?", "Ivy asked for the tea tray at 21:05, before death at 21:15.", IsKey: true),
        new("Marcos: Was the window forced open?", "No. The latch marks are from inside."),
        new("Marcos: Did you see a stranger?", "No stranger entered the estate.")
    };

    private static readonly List<QuestionData> LabData = new()
    {
        new("Poison: What was inside the tea cup?", "Tea had foxglove poison.", IsKey: true),
        new("Poison: What was in the victim's blood?", "The same poison from the tea was in blood."),
        new("Window: What was on the window latch?", "Black piano glove fiber was on the latch.", IsKey: true),
        new("Window: Was there mud from outside?", "No outside mud in the room.")
    };

    public static readonly List<OptionData> SuspectOptions = new()
    {
        new("Ivy Marlowe (concert pianist)", IsCorrect: true),
        new("Marcos Vale (groundskeeper)"),
        new("Nora Flint (estate lawyer)")
    };

    public static readonly List<OptionData> MethodOptions = new()
    {
        new("Poison in tea", IsCorrect: true),
        new("Electric shock with floor lamp"),
        new("Hit with metronome")
    };

    public static readonly List<OptionData> MotiveOptions = new()
    {
        new("Money and inheritance", IsCorrect: true),
        new("Professional jealousy"),
        new("Revenge for old family conflict")
    };

    public static IEnumerable<string> GetIvyQuestions() => InterviewData.Where(q => q.Question.StartsWith("Ivy:")).Select(q => q.Question);
    public static IEnumerable<string> GetNoraQuestions() => InterviewData.Where(q => q.Question.StartsWith("Nora:")).Select(q => q.Question);
    public static IEnumerable<string> GetMarcosQuestions() => InterviewData.Where(q => q.Question.StartsWith("Marcos:")).Select(q => q.Question);
    public static IEnumerable<string> GetPoisonLabQuestions() => LabData.Where(q => q.Question.StartsWith("Poison:")).Select(q => q.Question);
    public static IEnumerable<string> GetWindowLabQuestions() => LabData.Where(q => q.Question.StartsWith("Window:")).Select(q => q.Question);

    public static string GetInterviewAnswer(string question) => InterviewData.FirstOrDefault(q => q.Question == question)?.Answer ?? "No answer.";
    public static string GetLabAnswer(string question) => LabData.FirstOrDefault(q => q.Question == question)?.Answer ?? "No result.";

    public static bool IsKeyWitnessQuestion(string question) => InterviewData.Any(q => q.Question == question && q.IsKey);
    public static bool IsKeyLabQuestion(string question) => LabData.Any(q => q.Question == question && q.IsKey);

    public static bool IsCorrectSuspect(string suspect) => SuspectOptions.Any(o => o.Value == suspect && o.IsCorrect);
    public static bool IsCorrectMethod(string method) => MethodOptions.Any(o => o.Value == method && o.IsCorrect);
    public static bool IsCorrectMotive(string motive) => MotiveOptions.Any(o => o.Value == motive && o.IsCorrect);
}
