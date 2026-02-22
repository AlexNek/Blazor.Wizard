namespace Blazor.Wizard;

public interface IIdentifiableStep<TStep>
{
    TStep Id { get; }
}
