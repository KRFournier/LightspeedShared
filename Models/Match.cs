namespace Lightspeed;

/// <summary>
/// Base class for all matches
/// </summary>
public abstract class Match : CollectionObject
{
    public int? Number { get; set; }
    public Score First { get; set; } = new();
    public Score Second { get; set; } = new();
    public Side Winner { get; set; } = Side.Neither;
    public bool IsMatchStarted { get; set; } = false;
}