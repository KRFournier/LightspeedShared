namespace Lightspeed;

/// <summary>
/// A team, consisting of multiple members
/// </summary>
public sealed class Team : IParticipant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Player[] Members { get; set; } = [];
}