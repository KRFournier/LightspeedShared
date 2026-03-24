using Lightspeed.Network;

namespace Lightspeed;

public static class Extensions
{
    #region Actions

    public static string? ToSaberScore(this ActionType type) => type switch
    {
        ActionType.Card => "T",
        ActionType.Clean => "C",
        ActionType.Conceded => "H",
        ActionType.Disarm => "D",
        ActionType.FirstContact => "F",
        ActionType.Headshot => "A",
        ActionType.Indirect => "I",
        ActionType.OutOfBounds => "O",
        ActionType.Penalty => "S",
        ActionType.Priority => "P",
        ActionType.HeadshotOverride => "V",
        _ => null,
    };

    public static IEnumerable<string> ToSaberScore(this IEnumerable<Action> actions) => actions
            .Select(a => a.Type.ToSaberScore())
            .Where(s => s != null)!;

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
