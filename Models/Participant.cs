using MessagePack;

namespace Lightspeed;

[Union(0, typeof(SinglePlayer))]
[MessagePackObject]
public class Participant
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public int Honor { get; set; } = 0;

    [Key(2)]
    public Penalties Penalties { get; set; } = new();
}
