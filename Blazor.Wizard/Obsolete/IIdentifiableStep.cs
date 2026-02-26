namespace Blazor.Wizard.Obsolete;

[Obsolete("IIdentifiableStep is deprecated and not used in the current architecture.", false)]
public interface IIdentifiableStep<TStep>
{
    TStep Id { get; }
}
