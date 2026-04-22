using MessagePack;

namespace Lightspeed;

/// <summary>
/// Common interface for participants in a tournament, either individual contestants or teams.
/// </summary>
[MessagePackObject]
public sealed class Penalties
{
    [Key(0)]
    public Card Card { get; set; }

    [Key(1)]
    public bool IsEjected { get; set; }
}