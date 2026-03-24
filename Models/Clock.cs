namespace Lightspeed;

/// <summary>
/// A clock used in a match. A clock can have multiple timers, but
/// one overtime counter.
/// </summary>
public sealed class Clock
{
    public TimeSpan Timer { get; set; } = TimeSpan.FromSeconds(90);
    public int Overtime { get; set; } = 0;
}