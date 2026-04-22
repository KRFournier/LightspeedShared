using Lightspeed.Network;
using Lightspeed.MatchComponents;

namespace Lightspeed;

public static class Extensions
{
    #region Actions

    public static ActionState ToState(this Action action) => new()
    {
        Id = action.Id,
        Actor = action.Actor,
        Scorer = action.Scorer,
        Points = action.Points,
        Type = action.Type,
        SubType = action.SubType
    };

    public static Action ToModel(this ActionState state) => new()
    {
        Id = state.Id,
        Actor = state.Actor,
        Scorer = state.Scorer,
        Points = state.Points,
        Type = state.Type,
        SubType = state.SubType
    };

    #endregion

    #region Sides

    /// <summary>
    /// Gets the side reference (Left, Right, or Neither) for a given SideViewModel within a LeftRightViewModel.
    /// </summary>
    public static SideReference ToReference(this LeftRightViewModel sides, SideViewModel? side)
    {
        if (side == sides.Left)
            return SideReference.Left;
        if (side == sides.Right)
            return SideReference.Right;
        return SideReference.Neither;
    }

    /// <summary>
    /// Returns the other side of a reference. If the reference is Left, it returns Right; if it's Right, it returns Left; if it's Neither, it returns Neither.
    /// </summary>
    public static SideReference ToOtherSide(this SideReference reference) => reference switch
    {
        SideReference.Left => SideReference.Right,
        SideReference.Right => SideReference.Left,
        _ => SideReference.Neither
    };

    /// <summary>
    /// Gets the side for a given reference
    /// </summary>
    public static SideViewModel? ToSide(this LeftRightViewModel sides, SideReference reference) =>
        reference switch
        {
            SideReference.Left => sides.Left,
            SideReference.Right => sides.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(reference), "Invalid side reference")
        };

    #endregion

    #region Match Phases

    public static string ToDisplayString(this MatchPhase phase) => phase switch
    {
        MatchPhase.Overtime => "Overtime",
        MatchPhase.PriorityOvertime => "Priority Overtime",
        MatchPhase.WinnerDeclared => "Winner Declared",
        _ => "Main"
    };

    public static MatchPhase NextPhase(this MatchPhase phase) => phase switch
    {
        MatchPhase.Main => MatchPhase.Overtime,
        MatchPhase.Overtime => MatchPhase.PriorityOvertime,
        _ => MatchPhase.WinnerDeclared
    };

    public static bool InOvertime(this MatchPhase phase) =>
        phase == MatchPhase.Overtime || phase == MatchPhase.PriorityOvertime;

    public static int ToCount(this MatchPhase phase) => (int)phase;

    public static MatchPhase ToPhase(this int count) => count switch
    {
        0 => MatchPhase.Main,
        1 => MatchPhase.Overtime,
        2 => MatchPhase.PriorityOvertime,
        _ => MatchPhase.WinnerDeclared
    };

    #endregion
}
