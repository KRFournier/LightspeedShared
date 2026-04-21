namespace Lightspeed;

/// <summary>
/// Adds priority tracking to a match
/// </summary>
public sealed class Priority
{
    public Side PrioritySide { get; set; } = Side.Neither;
    public int PriorityPoints { get; set; } = 3;
    public bool InPriority { get; set; } = false;
}