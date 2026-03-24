namespace Lightspeed;

/// <summary>
/// Specifies the type of scoring actions possible in a match.
/// </summary>
public enum ActionType
{
    Unknown = -1,
    Card,
    Clean,
    Conceded,
    Disarm,
    Ejected,
    FirstContact,
    Headshot,
    HeadshotOverride,
    Indirect,
    OutOfBounds,
    Overtime,
    Penalty,
    Priority,
    Return,
}
