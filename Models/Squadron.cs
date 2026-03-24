namespace Lightspeed;

/// <summary>
/// A pool of players in a tournament.
/// </summary>
public sealed class Squadron
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public Guid[] Participants { get; set; } = [];
    public int Weight { get; set; } = 0;
    public MatchSettings MatchSettings { get; set; } = new();
}