using Lightspeed.MatchComponents;

namespace LightspeedShared.Services;

/// <summary>
/// Instead of instantiating this, this service is written as an extension class since all the methods
/// follow a simple formula and all want access to the sides.
/// </summary>
public static class ActionFactory
{
    /// <summary>
    /// Creates a new clean action
    /// </summary>
    public static ActionViewModel<T> NewClean<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Clean,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Clean
    };

    /// <summary>
    /// Creates a new concession
    /// </summary>
    public static ActionViewModel<T> NewConcession<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Conceded,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor.ToOtherSide()),
        Points = PointValues.Conceded
    };

    /// <summary>
    /// Creates a new disarm action
    /// </summary>
    public static ActionViewModel<T> NewDisarm<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Disarm,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor.ToOtherSide()),
        Points = PointValues.Disarm
    };

    /// <summary>
    /// Creates a new first contact action
    /// </summary>
    public static ActionViewModel<T> NewFirstContact<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.FirstContact,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.FirstContact
    };

    /// <summary>
    /// Creates a new headshot action
    /// </summary>
    public static ActionViewModel<T> NewHeadshot<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Headshot,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Headshot
    };

    /// <summary>
    /// Creates a new headshot override action
    /// </summary>
    public static ActionViewModel<T> NewHeadshotOverride<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.HeadshotOverride,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.HeadshotOverride
    };

    /// <summary>
    /// Creates a new indirect action
    /// </summary>
    public static ActionViewModel<T> NewIndirect<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Indirect,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Indirect
    };

    /// <summary>
    /// Creates a new Out of Bounds action
    /// </summary>
    public static ActionViewModel<T> NewOutOfBounds<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.OutOfBounds,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor.ToOtherSide()),
        Points = PointValues.OutOfBounds
    };

    /// <summary>
    /// Creates a new Overtime action
    /// </summary>
    public static ActionViewModel<T> NewOvertime<T>(this LeftRightViewModel<T> sides) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Overtime
    };

    /// <summary>
    /// Creates a new priority action
    /// </summary>
    public static ActionViewModel<T> NewPriority<T>(this LeftRightViewModel<T> sides, SideReference actor, int points) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Priority,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = points
    };

    /// <summary>
    /// Creates a new return action
    /// </summary>
    public static ActionViewModel<T> NewReturn<T>(this LeftRightViewModel<T> sides, SideReference actor) where T : ParticipantViewModel => new()
    {
        Type = ActionType.Return,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Return
    };
}
