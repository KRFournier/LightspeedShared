using MessagePack;

namespace Lightspeed;

/// <summary>
/// Represents one side of a two-sided match. This tracks the participant, their score, and any minor violations they have received.
/// </summary>
[MessagePackObject]
public class Side
{
    [Key(0)]
    public Guid Participant { get; set; }

    [Key(1)]
    public int Points { get; set; } = 0;

    [Key(2)]
    public int MinorViolations { get; set; } = 0;
}
