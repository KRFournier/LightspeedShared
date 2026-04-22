using MessagePack;

namespace Lightspeed;

/// <summary>
/// Represents an individual fencer in a tournament.
/// </summary>
[MessagePackObject]
public sealed class StandardPlayer : Participant
{
    [Key(4)]
    public Competitor Competitor { get; set; } = new();
}
