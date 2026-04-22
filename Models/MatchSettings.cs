using MessagePack;

namespace Lightspeed;

/// <summary>
/// The settings for a set of matches
/// </summary>
[Union(0, typeof(StandardMatchSettings))]
[MessagePackObject]
public class MatchSettings
{
    [Key(0)]
    public bool IsLocked { get; set; } = false;
}