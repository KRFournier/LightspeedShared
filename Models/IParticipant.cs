namespace Lightspeed;

/// <summary>
/// Common interface for participants in a tournament, either individual contestants or teams.
/// </summary>
public interface IParticipant
{
    public Guid Id { get; }
    public string Name { get; }
}