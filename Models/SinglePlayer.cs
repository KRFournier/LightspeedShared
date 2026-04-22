using MessagePack;

namespace Lightspeed;

/// <summary>
/// Represents an individual fencer in a tournament.
/// </summary>
[MessagePackObject]
public sealed class SinglePlayer : Participant
{
    [Key(3)]
    public Guid Competitor { get; set; } = new();
}
