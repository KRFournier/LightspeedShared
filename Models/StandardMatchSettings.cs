using MessagePack;

namespace Lightspeed;

/// <summary>
/// The settings for a set of matches
/// </summary>
[MessagePackObject]
public class StandardMatchSettings : MatchSettings
{
    [Key(1)]
    public int WinningScore { get; set; } = 12;

    [Key(2)]
    public TimeSpan TimeLimit { get; set; } = TimeSpan.FromSeconds(90);

    [Key(3)]
    public int Rounds { get; set; } = 1;
}