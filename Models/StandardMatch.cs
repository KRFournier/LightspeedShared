namespace Lightspeed;

/// <summary>
/// A standard match
/// </summary>
public sealed class StandardMatch : Match
{
    public Clock Clock { get; set; } = new();
    public LeftRightSide Scores { get; set; } = new();
    public Priority Priority { get; set; } = new();
    public Action[] Actions { get; set; } = [];
}