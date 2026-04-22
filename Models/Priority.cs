namespace Lightspeed;

/// <summary>
/// Adds priority tracking to a match
/// </summary>
public sealed class Priority
{
    public SideReference PrioritySide { get; set; } = SideReference.Neither;
    public int PriorityPoints { get; set; } = 3;
    public bool InPriority { get; set; } = false;
}