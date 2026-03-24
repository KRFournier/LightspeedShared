namespace Lightspeed;

/// <summary>
/// Placeholder participant used when a participant is expected but not yet assigned
/// </summary>
public sealed class EmptyParticipant : IParticipant
{
    public Guid Id => Guid.Empty;
    public string Name => "EMPTY";
}
