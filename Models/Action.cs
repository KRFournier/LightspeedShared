namespace Lightspeed;

/// <summary>
/// An action performed in a match.
/// </summary>
public sealed class Action
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public SideReference Actor { get; set; } = SideReference.Neither;
    public SideReference Scorer { get; set; } = SideReference.Neither;
    public int Points { get; set; } = 0;
    public ActionType Type { get; set; } = ActionType.Unknown;
    public string? SubType { get; set; }
}
